import { flatRequest } from '../../request';

export function getLoggerSystemListApi(input: Api.LoggerSystem.ListFilter) {
  const { pageIndex, pageSize } = input;
  const data = { pageIndex, pageSize };
  input.level = Number.parseInt(input.level, 10);

  if (!Number.isNaN(input.level)) {
    data.level = input.level;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  return flatRequest({
    url: '/api/logger/system/list',
    method: 'get',
    params: { ...data }
  });
}

export function getLoggerReqeustListApi(input: Api.LoggerRequest.ListFilter) {
  const { pageIndex, pageSize } = input;
  const data = { pageIndex, pageSize };
  input.httpMethod = Number.parseInt(input.httpMethod, 10);

  if (!Number.isNaN(input.httpMethod)) {
    data.httpMethod = input.httpMethod;
  }

  if (input.traceId) {
    data.traceId = input.traceId;
  }

  if (input.url) {
    data.url = input.url;
  }

  if (typeof input.success === 'number') {
    data.success = input.success > 0;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  return flatRequest({
    url: '/api/logger/request/list',
    method: 'get',
    params: { ...data }
  });
}

export function getLoggerRequestDetailApi(id: string) {
  return flatRequest({
    url: `/api/logger/request/${id}`,
    method: 'get'
  });
}
