import { flatRequest } from '@/service/request';
import type { SystemLevelConfig } from '@/types/platform';

export function fetchGetNetworkPairingSetting() {
  return flatRequest<{ code: number; data: SystemLevelConfig }>({
    url: '/api/platformSettings/networkpairing',
    method: 'get',
    headers: {
      'Content-Type': 'application/json'
    }
  });
}

export function fetchSetNetworkPairingSetting(input: any) {
  return flatRequest({
    url: '/api/platformSettings/networkpairing',
    method: 'post',
    data: { ...input }
  });
}
