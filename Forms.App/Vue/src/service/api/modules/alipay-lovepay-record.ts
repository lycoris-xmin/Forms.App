import { flatRequest } from '@/service/request';

export function fetchGetAlipayLovePayRecordList(input: Api.AlipayLovePayRecord.ListSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  } as any;

  if (!Number.isNaN(Number.parseInt(input.platform, 10))) {
    data.platform = input.platform;
  }

  if (input.alipayLovePayId) {
    data.alipayLovePayId = input.alipayLovePayId;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  if (!Number.isNaN(Number.parseInt(input.platform, 10))) {
    data.settlemented = input.settlemented === 1;
  }

  return flatRequest({
    url: '/api/alipay/lovepay/record/list',
    params: { ...data }
  });
}

// export function fetchSettlementAlipayLovePayRecord(ids: Array<string>) {
//   if (!ids.length) {
//     return baseResp();
//   }

//   return flatRequest({
//     url: '/api/alipay/lovepay/record',
//     method: 'put',
//     data: { ids }
//   });
// }
