<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue';
import Button from 'primevue/button';
import Card from 'primevue/card';
import ConfirmDialog from 'primevue/confirmdialog';
import DataTable from 'primevue/datatable';
import Dialog from 'primevue/dialog';
import Column from 'primevue/column';
import Divider from 'primevue/divider';
import InputNumber from 'primevue/inputnumber';
import InputText from 'primevue/inputtext';
import Select from 'primevue/select';
import Tag from 'primevue/tag';
import Textarea from 'primevue/textarea';
import Toast from 'primevue/toast';
import Toolbar from 'primevue/toolbar';
import { useConfirm } from 'primevue/useconfirm';
import { useToast } from 'primevue/usetoast';
import { api } from './api';
import type {
  CheckServerResponse,
  CreateServerRequest,
  DashboardSummaryResponse,
  ServerResponse,
  ServerStatus
} from './types';

type ViewName = 'servers' | 'dashboard' | 'checks';

const toast = useToast();
const confirm = useConfirm();

const currentView = ref<ViewName>('servers');
const servers = ref<ServerResponse[]>([]);
const dashboardSummary = ref<DashboardSummaryResponse | null>(null);
const loading = ref(false);
const saving = ref(false);
const checkingId = ref<string | null>(null);
const dialogVisible = ref(false);
const editingServer = ref<ServerResponse | null>(null);
const lastCheck = ref<CheckServerResponse | null>(null);

const query = reactive({
  search: '',
  sortBy: 'createdAt',
  sortDirection: 'desc' as 'asc' | 'desc'
});

const form = reactive<CreateServerRequest>({
  name: '',
  host: '',
  description: '',
  checkPort: 80
});

const sortFields = [
  { label: 'Created at', value: 'createdAt' },
  { label: 'Name', value: 'name' },
  { label: 'Host', value: 'host' },
  { label: 'Status', value: 'status' },
  { label: 'Last heartbeat', value: 'lastHeartbeatAt' }
];

const sortDirections = [
  { label: 'Descending', value: 'desc' },
  { label: 'Ascending', value: 'asc' }
];

const navigation = [
  {
    key: 'servers' as const,
    label: 'Servers',
    icon: 'pi pi-server',
    title: 'Servers',
    description: 'Inventory, heartbeat state, and TCP availability checks.'
  },
  {
    key: 'dashboard' as const,
    label: 'Dashboard',
    icon: 'pi pi-chart-line',
    title: 'Dashboard',
    description: 'Current server status summary and recent activity.'
  },
  {
    key: 'checks' as const,
    label: 'Checks',
    icon: 'pi pi-bolt',
    title: 'Checks',
    description: 'Run manual TCP checks and review last check results.'
  }
];

const localSummary = computed(() => {
  return servers.value.reduce(
    (acc, server) => {
      acc.total += 1;

      const status = normalizeStatus(server.status);
      if (status === 'Online') acc.online += 1;
      if (status === 'Offline') acc.offline += 1;
      if (status === 'Unknown') acc.unknown += 1;

      return acc;
    },
    { total: 0, online: 0, offline: 0, unknown: 0 }
  );
});

const summary = computed(() => ({
  total: dashboardSummary.value?.totalServers ?? localSummary.value.total,
  online: dashboardSummary.value?.onlineServers ?? localSummary.value.online,
  offline: dashboardSummary.value?.offlineServers ?? localSummary.value.offline,
  unknown: dashboardSummary.value?.unknownServers ?? localSummary.value.unknown,
  lastHeartbeatAt: dashboardSummary.value?.lastHeartbeatAt ?? null
}));

const viewMeta = computed(() => navigation.find((item) => item.key === currentView.value) ?? navigation[0]);

const recentlyCheckedServers = computed(() => {
  return [...servers.value]
    .filter((server) => server.lastCheckAt)
    .sort((a, b) => new Date(b.lastCheckAt ?? 0).getTime() - new Date(a.lastCheckAt ?? 0).getTime())
    .slice(0, 6);
});

onMounted(async () => {
  await refreshData();
});

async function refreshData() {
  await Promise.all([loadServers(), loadDashboardSummary()]);
}

async function loadServers() {
  loading.value = true;

  try {
    servers.value = await api.getServers({
      search: query.search.trim(),
      sortBy: query.sortBy,
      sortDirection: query.sortDirection
    });
  } catch (error) {
    showError(error, 'Failed to load servers');
  } finally {
    loading.value = false;
  }
}

async function loadDashboardSummary() {
  try {
    dashboardSummary.value = await api.getDashboardSummary();
  } catch (error) {
    dashboardSummary.value = null;
  }
}

function setView(view: ViewName) {
  currentView.value = view;
}

function openCreateDialog() {
  editingServer.value = null;
  form.name = '';
  form.host = '';
  form.description = '';
  form.checkPort = 80;
  dialogVisible.value = true;
}

function openEditDialog(server: ServerResponse) {
  editingServer.value = server;
  form.name = server.name;
  form.host = server.host;
  form.description = server.description ?? '';
  form.checkPort = server.checkPort;
  dialogVisible.value = true;
}

async function saveServer() {
  saving.value = true;

  try {
    if (editingServer.value) {
      await api.updateServer(editingServer.value.id, form);
      toast.add({ severity: 'success', summary: 'Server updated', life: 2500 });
    } else {
      await api.createServer(form);
      toast.add({ severity: 'success', summary: 'Server created', life: 2500 });
    }

    dialogVisible.value = false;
    await refreshData();
  } catch (error) {
    showError(error, 'Failed to save server');
  } finally {
    saving.value = false;
  }
}

function confirmDelete(server: ServerResponse) {
  confirm.require({
    message: `Delete server ${server.name}?`,
    header: 'Delete server',
    icon: 'pi pi-exclamation-triangle',
    rejectLabel: 'Cancel',
    acceptLabel: 'Delete',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await api.deleteServer(server.id);
        toast.add({ severity: 'success', summary: 'Server deleted', life: 2500 });
        await refreshData();
      } catch (error) {
        showError(error, 'Failed to delete server');
      }
    }
  });
}

async function sendHeartbeat(server: ServerResponse) {
  try {
    await api.heartbeat(server.id);
    toast.add({ severity: 'success', summary: 'Heartbeat updated', life: 2500 });
    await refreshData();
  } catch (error) {
    showError(error, 'Failed to update heartbeat');
  }
}

async function checkServer(server: ServerResponse) {
  checkingId.value = server.id;

  try {
    lastCheck.value = await api.checkServer(server.id);
    toast.add({
      severity: lastCheck.value.isAvailable ? 'success' : 'warn',
      summary: lastCheck.value.message ?? 'Check completed',
      life: 3000
    });
    await refreshData();
  } catch (error) {
    showError(error, 'Failed to check server');
  } finally {
    checkingId.value = null;
  }
}

function normalizeStatus(status: ServerStatus): 'Unknown' | 'Online' | 'Offline' {
  if (status === 1 || status === 'Online') return 'Online';
  if (status === 2 || status === 'Offline') return 'Offline';
  return 'Unknown';
}

function statusSeverity(status: ServerStatus) {
  const normalized = normalizeStatus(status);

  if (normalized === 'Online') return 'success';
  if (normalized === 'Offline') return 'danger';
  return 'secondary';
}

function formatDate(value?: string | null) {
  if (!value) return '-';

  return new Intl.DateTimeFormat('ru-RU', {
    dateStyle: 'short',
    timeStyle: 'short'
  }).format(new Date(value));
}

function showError(error: unknown, fallback: string) {
  const detail = error instanceof Error ? error.message : fallback;
  toast.add({ severity: 'error', summary: fallback, detail, life: 5000 });
}
</script>

<template>
  <Toast />
  <ConfirmDialog />

  <div class="app-shell">
    <aside class="sidebar">
      <div class="brand">
        <div class="brand-mark">P</div>
        <div>
          <div class="brand-name">PulsePanel</div>
          <div class="brand-subtitle">Server monitoring</div>
        </div>
      </div>

      <nav class="nav">
        <button
          v-for="item in navigation"
          :key="item.key"
          type="button"
          class="nav-item"
          :class="{ active: currentView === item.key }"
          @click="setView(item.key)"
        >
          <i :class="item.icon"></i>
          {{ item.label }}
        </button>
      </nav>
    </aside>

    <main class="content">
      <header class="page-header">
        <div>
          <h1>{{ viewMeta.title }}</h1>
          <p>{{ viewMeta.description }}</p>
        </div>
        <Button v-if="currentView === 'servers'" icon="pi pi-plus" label="Add server" @click="openCreateDialog" />
      </header>

      <section class="metric-grid">
        <Card class="metric-card">
          <template #content>
            <span>Total</span>
            <strong>{{ summary.total }}</strong>
          </template>
        </Card>
        <Card class="metric-card online">
          <template #content>
            <span>Online</span>
            <strong>{{ summary.online }}</strong>
          </template>
        </Card>
        <Card class="metric-card offline">
          <template #content>
            <span>Offline</span>
            <strong>{{ summary.offline }}</strong>
          </template>
        </Card>
        <Card class="metric-card unknown">
          <template #content>
            <span>Unknown</span>
            <strong>{{ summary.unknown }}</strong>
          </template>
        </Card>
      </section>

      <section v-if="currentView === 'servers'" class="panel">
        <Toolbar class="table-toolbar">
          <template #start>
            <div class="filters">
              <InputText
                v-model="query.search"
                placeholder="Search name, host, description"
                @keyup.enter="refreshData"
              />
              <Select v-model="query.sortBy" :options="sortFields" option-label="label" option-value="value" />
              <Select
                v-model="query.sortDirection"
                :options="sortDirections"
                option-label="label"
                option-value="value"
              />
            </div>
          </template>
          <template #end>
            <Button icon="pi pi-refresh" label="Refresh" severity="secondary" :loading="loading" @click="refreshData" />
          </template>
        </Toolbar>

        <DataTable
          :value="servers"
          :loading="loading"
          data-key="id"
          striped-rows
          paginator
          :rows="10"
          :rows-per-page-options="[10, 20, 50]"
          responsive-layout="scroll"
        >
          <Column field="name" header="Name">
            <template #body="{ data }">
              <div class="server-name">
                <strong>{{ data.name }}</strong>
                <span>{{ data.description || 'No description' }}</span>
              </div>
            </template>
          </Column>
          <Column field="host" header="Host">
            <template #body="{ data }">
              <code>{{ data.host }}:{{ data.checkPort }}</code>
            </template>
          </Column>
          <Column field="status" header="Status">
            <template #body="{ data }">
              <Tag :value="normalizeStatus(data.status)" :severity="statusSeverity(data.status)" />
            </template>
          </Column>
          <Column field="lastHeartbeatAt" header="Heartbeat">
            <template #body="{ data }">
              {{ formatDate(data.lastHeartbeatAt) }}
            </template>
          </Column>
          <Column field="lastCheckAt" header="Last check">
            <template #body="{ data }">
              <div class="check-cell">
                <span>{{ formatDate(data.lastCheckAt) }}</span>
                <small>{{ data.lastCheckMessage || '-' }}</small>
              </div>
            </template>
          </Column>
          <Column header="Actions" class="actions-column">
            <template #body="{ data }">
              <div class="row-actions">
                <Button icon="pi pi-heart" text rounded v-tooltip.top="'Heartbeat'" @click="sendHeartbeat(data)" />
                <Button
                  icon="pi pi-bolt"
                  text
                  rounded
                  v-tooltip.top="'TCP check'"
                  :loading="checkingId === data.id"
                  @click="checkServer(data)"
                />
                <Button icon="pi pi-pencil" text rounded v-tooltip.top="'Edit'" @click="openEditDialog(data)" />
                <Button
                  icon="pi pi-trash"
                  text
                  rounded
                  severity="danger"
                  v-tooltip.top="'Delete'"
                  @click="confirmDelete(data)"
                />
              </div>
            </template>
          </Column>
        </DataTable>
      </section>

      <section v-else-if="currentView === 'dashboard'" class="dashboard-grid">
        <Card class="detail-card">
          <template #title>Status overview</template>
          <template #content>
            <div class="status-list">
              <div>
                <span>Online ratio</span>
                <strong>{{ summary.total ? Math.round((summary.online / summary.total) * 100) : 0 }}%</strong>
              </div>
              <div>
                <span>Last heartbeat</span>
                <strong>{{ formatDate(summary.lastHeartbeatAt) }}</strong>
              </div>
              <div>
                <span>Servers needing attention</span>
                <strong>{{ summary.offline + summary.unknown }}</strong>
              </div>
            </div>
          </template>
        </Card>

        <Card class="detail-card">
          <template #title>Recently checked</template>
          <template #content>
            <div v-if="recentlyCheckedServers.length" class="recent-list">
              <div v-for="server in recentlyCheckedServers" :key="server.id" class="recent-row">
                <div>
                  <strong>{{ server.name }}</strong>
                  <span>{{ server.host }}:{{ server.checkPort }}</span>
                </div>
                <Tag :value="normalizeStatus(server.status)" :severity="statusSeverity(server.status)" />
              </div>
            </div>
            <p v-else class="muted">No TCP checks yet.</p>
          </template>
        </Card>
      </section>

      <section v-else class="panel">
        <Toolbar class="table-toolbar">
          <template #start>
            <strong>Manual TCP checks</strong>
          </template>
          <template #end>
            <Button icon="pi pi-refresh" label="Refresh" severity="secondary" :loading="loading" @click="refreshData" />
          </template>
        </Toolbar>

        <DataTable :value="servers" :loading="loading" data-key="id" striped-rows responsive-layout="scroll">
          <Column field="name" header="Server" />
          <Column field="host" header="Target">
            <template #body="{ data }">
              <code>{{ data.host }}:{{ data.checkPort }}</code>
            </template>
          </Column>
          <Column field="lastCheckAt" header="Last check">
            <template #body="{ data }">
              {{ formatDate(data.lastCheckAt) }}
            </template>
          </Column>
          <Column field="lastResponseTimeMs" header="Response">
            <template #body="{ data }"> {{ data.lastResponseTimeMs }} ms </template>
          </Column>
          <Column field="lastCheckMessage" header="Message">
            <template #body="{ data }">
              {{ data.lastCheckMessage || '-' }}
            </template>
          </Column>
          <Column header="Action">
            <template #body="{ data }">
              <Button
                icon="pi pi-bolt"
                label="Run check"
                size="small"
                :loading="checkingId === data.id"
                @click="checkServer(data)"
              />
            </template>
          </Column>
        </DataTable>
      </section>
    </main>
  </div>

  <Dialog
    v-model:visible="dialogVisible"
    :header="editingServer ? 'Edit server' : 'Add server'"
    modal
    class="server-dialog"
  >
    <div class="form-grid">
      <label>
        <span>Name</span>
        <InputText v-model="form.name" placeholder="Production API" />
      </label>
      <label>
        <span>Host</span>
        <InputText v-model="form.host" placeholder="api.example.com" />
      </label>
      <label>
        <span>Check port</span>
        <InputNumber v-model="form.checkPort" :min="1" :max="65535" show-buttons />
      </label>
      <label class="full">
        <span>Description</span>
        <Textarea v-model="form.description" rows="4" placeholder="Optional notes" />
      </label>
    </div>

    <template #footer>
      <Button label="Cancel" severity="secondary" text @click="dialogVisible = false" />
      <Button label="Save" icon="pi pi-check" :loading="saving" @click="saveServer" />
    </template>
  </Dialog>

  <aside v-if="lastCheck" class="check-summary">
    <div>
      <strong>{{ lastCheck.host }}:{{ lastCheck.port }}</strong>
      <span>{{ lastCheck.message }}</span>
    </div>
    <Divider layout="vertical" />
    <div>
      <strong>{{ lastCheck.responseTimeMs }} ms</strong>
      <span>{{ formatDate(lastCheck.checkedAt) }}</span>
    </div>
  </aside>
</template>
