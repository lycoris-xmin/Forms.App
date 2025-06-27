import { flatRequest } from '../../request';

export function fetchGetDeviceAliList(params: Api.DeviceAli.SearchFilter) {
  const { pageIndex, pageSize } = params;

  const data = {
    pageIndex,
    pageSize
  };

  if (params.init !== void 0 && typeof params.init === 'number') {
    data.init = params.init;
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
    url: '/api/device/alibaba/list',
    params: { ...data }
  });
}

export function fetchUpdateDeviceAli(input: Api.DeviceAli.Update) {
  const { id } = input;
  const data = {
    id
  };

  if (input.id) {
    data.id = input.id;
  }

  if (input.currentMonth) {
    data.currentMonth = input.currentMonth;
  }
  if (input.lastMonth) {
    data.lastMonth = input.lastMonth;
  }

  if (input.payPhone) {
    data.payPhone = input.payPhone;
  }

  if (input.payPassword) {
    data.payPassword = input.payPassword;
  }

  if (input.address) {
    data.address = input.address;
  }

  if (input.name) {
    data.name = input.name;
  }

  if (input.phone) {
    data.phone = input.phone;
  }

  if (input.province) {
    data.province = input.province;
  }

  if (input.city) {
    data.city = input.city;
  }

  return flatRequest({
    url: '/api/device/alibaba',
    method: 'put',
    data
  });
}

export function fetchGetDeviceAliDetail(id: string) {
  return flatRequest({
    url: `/api/device/alibaba/${id}`
  });
}

export function fetchDeviceAliInit(ids: Array<string>) {
  return flatRequest({
    url: '/api/device/alibaba/init',
    method: 'post',
    data: {
      ids
    }
  });
}
