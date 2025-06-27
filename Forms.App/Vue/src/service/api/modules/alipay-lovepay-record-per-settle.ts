import { baseResp, flatRequest } from '@/service/request';
export function fetchGetAlipayLovePayPayPerSettleList(input: Api.AlipayLovePayPerSettle.ListSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  } as any;
  if (!Number.isNaN(Number.parseInt(input.platform, 10))) {
    data.platform = input.platform;
  }

  if (input.userId) {
    data.userId = input.userId;
  }
  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }
  if (input.endTime) {
    data.endTime = input.endTime;
  }
  if (!Number.isNaN(Number.parseInt(input.settlemented, 10))) {
    data.settlemented = input.settlemented === 1;
  }

  return flatRequest({
    url: '/api/alipay/lovepay/record/payPerSettle/list',
    params: { ...data }
  });
}
export function fetchSettlementAlipayLovePayPerSettle(ids: Array<string>) {
  if (!ids.length) {
    return baseResp();
  }
  return flatRequest({
    url: '/api/alipay/lovepay/record/payPerSettle/settlement',
    method: 'put',
    data: { ids }
  });
}
