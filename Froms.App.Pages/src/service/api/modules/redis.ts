import { flatRequest } from '../../request';

export function getRedisKeyListApi() {
  return flatRequest({
    url: '/api/server/redis/key/list'
  });
}

export function getRedisMonitorApi() {
  return flatRequest({
    url: '/api/server/redis/monitor'
  });
}

export function getRedisValueApi(keyType: string, key: string) {
  return flatRequest({
    url: `/api/server/redis/${keyType}`,
    params: {
      key
    }
  });
}

export function deleteRedisKeyApi(key: string) {
  return flatRequest({
    url: '/api/server/redis',
    method: 'delete',
    params: {
      key
    }
  });
}
