import type {
  CheckServerResponse,
  CreateServerRequest,
  DashboardSummaryResponse,
  ServerResponse,
  UpdateServerRequest
} from './types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...init?.headers
    },
    ...init
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed with status ${response.status}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export interface GetServersQuery {
  search?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export const api = {
  getServers(query: GetServersQuery = {}) {
    const params = new URLSearchParams();

    if (query.search) params.set('search', query.search);
    if (query.sortBy) params.set('sortBy', query.sortBy);
    if (query.sortDirection) params.set('sortDirection', query.sortDirection);

    const suffix = params.toString() ? `?${params}` : '';

    return request<ServerResponse[]>(`/api/servers${suffix}`);
  },

  createServer(payload: CreateServerRequest) {
    return request<ServerResponse>('/api/servers', {
      method: 'POST',
      body: JSON.stringify(payload)
    });
  },

  updateServer(id: string, payload: UpdateServerRequest) {
    return request<ServerResponse>(`/api/servers/${id}`, {
      method: 'PUT',
      body: JSON.stringify(payload)
    });
  },

  deleteServer(id: string) {
    return request<void>(`/api/servers/${id}`, {
      method: 'DELETE'
    });
  },

  heartbeat(id: string) {
    return request<ServerResponse>(`/api/servers/${id}/heartbeat`, {
      method: 'POST'
    });
  },

  checkServer(id: string) {
    return request<CheckServerResponse>(`/api/servers/${id}/check`, {
      method: 'POST'
    });
  },

  getDashboardSummary() {
    return request<DashboardSummaryResponse>('/api/dashboard/summary');
  }
};
