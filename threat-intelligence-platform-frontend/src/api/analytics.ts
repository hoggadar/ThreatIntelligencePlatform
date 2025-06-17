import axios from "../api/axios";

const API_URL = "http://localhost:8888/api";

export interface IoC {
  id: string;
  type: string;
  value: string;
  source: string;
  firstSeen: string;
  lastSeen: string;
  tags: string[];
  additionalData: Record<string, any>;
  threatLevel?: ThreatLevel;
  description?: string;
}

export type ThreatLevel = 'high' | 'medium' | 'low' | 'critical' | 'informational';

export interface IoCAnalytics {
  totalCount: number;
  countByType: Record<string, number>;
  countBySource: Record<string, number>;
  countTypesBySource: Record<string, Record<string, number>>;
}

const getAuthHeader = () => {
  const token = localStorage.getItem('token');
  return token ? { Authorization: `Bearer ${token}` } : {};
};

export const analyticsApi = {
  async getAllIoCs(limit?: number, offset?: number, search?: string): Promise<IoC[]> {
    const params = new URLSearchParams();
    if (limit) params.append('limit', limit.toString());
    if (offset) params.append('offset', offset.toString());
    if (search) params.append('search', search);
    
    const response = await axios.get(`/IoC/GetAll`, { params });
    return response.data;
  },

  async getTotalCount(): Promise<number> {
    const response = await axios.get(`/IoC/Count`);
    return response.data;
  },

  async getCountByType(): Promise<Record<string, number>> {
    const response = await axios.get(`/IoC/CountByType`);
    return response.data;
  },

  async getCountBySource(): Promise<Record<string, number>> {
    const response = await axios.get(`/IoC/CountBySource`);
    return response.data;
  },

  async getCountTypesBySource(): Promise<Record<string, Record<string, number>>> {
    const response = await axios.get(`/IoC/CountTypesBySource`);
    return response.data;
  }
}; 