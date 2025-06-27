import { flatRequest } from '../../request';

type SearchFilter = {
  pageIndex: number;
  pageSize: number;
  buyPayType?: number;
  init?: number;
  allowPair?: number;
  account?: string;
  userId?: string | number;
  deviceId?: string | number;
};

type UpdateInput = {
  id: string | number;
  buyPayType: number;
  taoQi?: number;
  age?: string;
  payPhone?: string;
  payPassword?: string;
  name?: string;
  phone?: string;
  province?: string;
  city?: string;
  address?: string;
};

export function fetchGetDeviceTaobaoList(params: SearchFilter) {
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

  if (params.allowPair !== void 0 && typeof params.allowPair === 'number') {
    data.allowPair = params.allowPair === 1;
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
    url: '/api/device/taobao/list',
    params: { ...data }
  });
}

export function fetchUpdateDeviceTaobao(input: UpdateInput) {
  const data = {
    id: input.id,
    buyPayType: input.buyPayType,
    allowPair: input.allowPair,
    taoQi: input.taoQi ?? null,
    age: input.age || null,
    payPhone: input.payPhone || null,
    payPassword: input.payPassword || null,
    name: input.name,
    phone: input.phone,
    province: input.province,
    city: input.city,
    address: input.address
  };

  return flatRequest({
    url: '/api/device/taobao',
    method: 'put',
    data
  });
}

export function fetchGetDeviceTaobaoDetail(id: string) {
  return flatRequest({
    url: `/api/device/taobao/${id}`
  });
}

export function fetchDeviceTaobaoInit(ids: Array<string>) {
  return flatRequest({
    url: '/api/device/taobao/init',
    method: 'post',
    data: {
      ids
    }
  });
}

export function fetchDeviceTaobaoAllowPair(input: object) {
  return flatRequest({
    url: '/api/device/taobao/allowPair',
    method: 'post',
    data: {
      ...input
    }
  });
}
