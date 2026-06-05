export type ServerStatus = 'Unknown' | 'Online' | 'Offline' | number;

export interface ServerResponse {
  id: string;
  name: string;
  host: string;
  checkPort: number;
  description?: string | null;
  createdAt: string;
  lastHeartbeatAt?: string | null;
  lastCheckAt?: string | null;
  lastCheckMessage?: string | null;
  lastResponseTimeMs: number;
  status: ServerStatus;
}

export interface CreateServerRequest {
  name: string;
  host: string;
  description?: string | null;
  checkPort: number;
}

export type UpdateServerRequest = CreateServerRequest;

export interface DashboardSummaryResponse {
  totalServers: number;
  onlineServers: number;
  offlineServers: number;
  unknownServers: number;
  lastHeartbeatAt?: string | null;
}

export interface CheckServerResponse {
  serverId: string;
  host: string;
  port: number;
  isAvailable: boolean;
  status: ServerStatus;
  checkedAt: string;
  responseTimeMs: number;
  message?: string | null;
}
