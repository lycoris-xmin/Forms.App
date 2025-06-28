import { flatRequest } from '../../request';

export function getSchedulerListApi(input: Api.Scheduler.SearchFilter) {
  //
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };

  if (input.jobGroup) {
    data.jobGroup = input.jobGroup;
  }

  if (typeof input.triggerType === 'number') {
    data.triggerType = input.triggerType;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  return flatRequest({
    url: '/api/scheduler/list',
    params: data
  });
}

export function getSchedulerRecordListApi(input: Api.Scheduler.RecordSearchFilter) {
  //
  const { pageIndex, pageSize, jobKey } = input;

  const data = {
    pageIndex,
    pageSize,
    jobKey
  };

  if (input.jobTraceId) {
    data.jobTraceId = input.jobTraceId;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  return flatRequest({
    url: '/api/scheduler/record/list',
    params: data
  });
}

export function getSchedulerRecordLoggerApi(jobKey: stirng, traceId: string) {
  return flatRequest({
    url: `/api/scheduler/record/logger/${traceId}`,
    params: {
      jobKey
    }
  });
}
