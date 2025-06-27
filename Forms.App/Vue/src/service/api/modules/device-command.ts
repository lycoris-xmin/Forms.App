import { flatRequest } from '../../request';

export function fetchGetDeviceCommandList(input: Api.DeviceCommand.SearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.id) {
    data.id = input.id;
  }

  if (input.deviceId) {
    data.deviceId = input.deviceId;
  }

  if (input.taskId) {
    data.taskId = input.taskId;
  }

  if (typeof input.command === 'number') {
    data.command = input.command;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  return flatRequest({
    url: '/api/device/command/list',
    params: {
      ...data
    }
  });
}

export function fetchGetDeviceCommandDetail(id: string) {
  return flatRequest({
    url: `/api/device/command/${id}`
  });
}

export function fetchDebuggerDeviceCommand({ deviceId, command, data }) {
  return flatRequest({
    url: `/api/device/command/debugger`,
    method: 'post',
    data: {
      deviceId,
      command,
      data
    }
  });
}

export function fetchDeviceCommandUploadLog(id: string) {
  return flatRequest({
    url: `/api/device/command/log/upload/${id}`
  });
}
export function fetchGetDeviceScriptList(id: string) {
  return flatRequest({
    url: `/api/device/command/scriptLog/${id}`
  });
}
