import { flatRequest } from '../../request';

export function fetchGetAlipayLovePayList(input: Api.AlipayLovePay.ListSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  } as any;

  if (input.alipayAccount) {
    data.alipayAccount = input.alipayAccount;
  }

  if (input.alipayName) {
    data.alipayName = input.alipayName;
  }

  if (input.shopName) {
    data.shopName = input.shopName;
  }

  if (input.tenantUser) {
    data.tenantUser = input.tenantUser;
  }

  if (input.userId) {
    data.userId = input.userId;
  }
  if (input.alipayLovePayType) {
    data.alipayLovePayType = input.alipayLovePayType;
  }

  return flatRequest({
    url: '/api/alipay/lovepay/list',
    params: { ...data }
  });
}

export function fetchCreateAlipayLovePay(input: Api.AlipayLovePay.AlipayLovePayCreate) {
  const { alipayName } = input;
  const data = {
    alipayName
  } as any;

  if (input.alipayAccount) {
    data.alipayAccount = input.alipayAccount;
  }
  if (input.tenantUser) {
    data.tenantUser = input.tenantUser;
  }

  if (input.shopName) {
    data.shopName = input.shopName;
  }
  if (input.alipayLovePayType) {
    data.alipayLovePayType = input.alipayLovePayType;
  }

  return flatRequest({
    url: '/api/alipay/lovepay',
    method: 'post',
    data: { ...data }
  });
}

export function fetchUpdateAlipayLovePay(input: Api.AlipayLovePay.AlipayLovePayUpdate) {
  const { id } = input;
  const data = {
    id
  } as any;

  if (input.alipayAccount) {
    data.alipayAccount = input.alipayAccount;
  }

  if (input.alipayName) {
    data.alipayName = input.alipayName;
  }

  if (input.tenantUser) {
    data.tenantUser = input.tenantUser;
  }

  if (input.shopName) {
    data.shopName = input.shopName;
  }

  if (Object.keys(data).length === 1) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/alipay/lovepay',
    method: 'put',
    data: { ...data }
  });
}

export function fetchDeleteAlipayLovePay(input: Array<string>) {
  if (!input || !input.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/alipay/lovepay',
    method: 'delete',
    params: { ids: input }
  });
}
