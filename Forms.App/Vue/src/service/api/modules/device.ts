import { baseResp, flatRequest } from '../../request';

export function fetchGetDeviceList(input: Api.Device.SearchFilter) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };

  if (input.type !== void 0 && typeof input.type === 'number') {
    data.type = input.type;
  }

  if (input.status !== void 0 && typeof input.status === 'number') {
    data.status = input.status;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  if (input.name) {
    data.name = input.name;
  }

  if (input.id) {
    data.id = input.id;
  }

  if (input.runCount !== void 0 && typeof input.runCount === 'number') {
    data.runCount = input.runCount;
  }

  if (input.coreRunCount !== void 0 && typeof input.coreRunCount === 'number') {
    data.coreRunCount = input.coreRunCount;
  }

  if (input.sort) {
    data.sort = input.sort;
  }

  return flatRequest({
    url: '/api/device/list',
    params: { ...data }
  });
}

export function fetchGetTaobaoDeviceDetail(id: string) {
  return flatRequest({
    url: `/api/device/tb/${id}`
  });
}

export function fetchBindingQRCode(areaName: string, groupName: string, sectionName: string) {
  return flatRequest({
    url: '/api/device/binding/qrcode',
    params: {
      areaName,
      groupName,
      sectionName
    }
  });
}

export function fetchUpdateDevice(input: Api.Device.UpdateDevice) {
  const { id } = input;
  const data = {
    id
  };

  if (input.name) {
    data.name = input.name;
  }

  if (typeof input.type === 'number') {
    data.type = input.type;
  }

  if (input.remark) {
    data.remark = input.remark;
  }

  if (input.canUse === 0) {
    data.canUse = false;
  } else if (input.canUse === 1) {
    data.canUse = true;
  }

  if (input.supportedPlatforms && input.supportedPlatforms.length) {
    data.supportedPlatforms = input.supportedPlatforms;
  }

  if (input.areaName) {
    data.areaName = input.areaName;
  }

  if (input.groupName) {
    data.groupName = input.groupName;
  }

  if (input.sectionName) {
    data.sectionName = input.sectionName;
  }

  if (Object.keys(data).length === 1) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/device',
    method: 'put',
    data
  });
}

export function fetchDeleteDevice(ids: Array) {
  return flatRequest({
    url: '/api/device',
    method: 'delete',
    params: { ids }
  });
}

export function fetchDeviceRecharge(input: Api.Device.DeviceRecharge) {
  const { id } = input;
  const data = {
    id
  };

  if (typeof input.days === 'number') {
    data.days = input.days;
  }

  if (Object.keys(data).length === 1) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/device/recharge',
    method: 'post',
    data: { ...data }
  });
}

export function fetchBatchDeviceRecharge(deviceIds: Array<string>, days: number) {
  return flatRequest({
    url: '/api/device/batch/recharge',
    method: 'post',
    data: {
      deviceIds,
      days
    }
  });
}

export function fetchResetStatus(ids: Array<string>) {
  return flatRequest({
    url: '/api/device/reset/status',
    method: 'put',
    data: {
      ids
    }
  });
}

export function fetchGetBindingList(code: string) {
  return flatRequest({
    url: '/api/device/binding/list',
    params: {
      code
    }
  });
}

export function fetchAutomaticNetworking({ groupName, count }) {
  return flatRequest({
    url: '/api/device/automatic/networking',
    method: 'post',
    data: {
      groupName,
      count
    }
  });
}

export function fetchGetDeivceCommandRecordList(input: Api.Device.CommandRecordSearchFilter) {
  const { pageIndex, pageSize, deviceId } = input;
  const data: Api.Device.CommandRecordSearchFilter = {
    pageIndex,
    pageSize,
    deviceId
  };

  if (typeof input.command === 'number') {
    data.command = input.command;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  return flatRequest({
    url: '/api/device/command/record/list',
    method: 'get',
    params: data
  });
}

export function fetchGetDeviceDetail(id: string) {
  return flatRequest({
    url: `/api/device/${id}`
  });
}

export function fetchGetCoreLogList(id: string) {
  return flatRequest({
    url: `/api/device/core/logs/${id}`
  });
}

export function fetchUploadCoreLog(id: string, day: string) {
  return flatRequest({
    url: `/api/device/core/log/${id}`,
    method: 'post',
    params: {
      day
    }
  });
}
