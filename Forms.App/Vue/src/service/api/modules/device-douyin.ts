import { flatRequest } from '../../request';

export function fetchGetDeviceDouyinList(params: Api.DeviceDouyin.SearchFilter) {
  const { pageIndex, pageSize } = params;

  const data = {
    pageIndex,
    pageSize
  };

  if (params.buyPayType !== void 0 && typeof params.buyPayType === 'number') {
    data.buyPayType = params.buyPayType;
  }

  if (params.init !== void 0 && typeof params.init === 'number') {
    data.init = params.init;
  }

  if (params.allowPair !== void 0 && typeof params.allowPair === 'boolean') {
    data.allowPair = params.allowPair;
  }

  if (params.account) {
    data.account = params.account;
  }

  if (params.userId) {
    data.userId = params.userId;
  }

  if (params.deviceId) {
    data.deviceId = params.deviceId;
  }

  if (params.deviceName) {
    data.deviceName = params.deviceName;
  }

  return flatRequest({
    url: '/api/device/douyin/list',
    params: { ...data }
  });
}

export function fetchUpdateDeviceDouyin(input: Api.DeviceDouyin.Update) {
  const data = {
    id: input.id,
    buyPayType: input.buyPayType,
    allowPair: input.allowPair,
    payPhone: input.payPhone || null,
    payPassword: input.payPassword || null,
    name: input.name,
    phone: input.phone,
    province: input.province,
    city: input.city,
    address: input.address
  };

  return flatRequest({
    url: '/api/device/douyin',
    method: 'put',
    data
  });
}

export function fetchGetDeviceDouyinDetail(id: string) {
  return flatRequest({
    url: `/api/device/douyin/${id}`
  });
}

export function fetchDeviceDouyinInit(ids: Array<string>) {
  return flatRequest({
    url: '/api/device/douyin/init',
    method: 'post',
    data: {
      ids
    }
  });
}

export function fetchDeviceDouyinAllowPair(input: { ids: Array<string>; allowPair: number }) {
  return flatRequest({
    url: '/api/device/douyin/allowpair',
    method: 'post',
    data: {
      ids: input.ids,
      allowPair: input.allowPair === 1
    }
  });
}
