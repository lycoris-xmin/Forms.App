import { baseResp, flatRequest } from '../../request';

export function fetchGetTenantList(input: Api.Tenant.ListSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.email) {
    data.email = input.email;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  return flatRequest({
    url: '/api/tenant/list',
    params: { ...data }
  });
}

export function fetchCreateTenant(input: Api.Tenant.TenantCreate) {
  const { email, password, nickName, payType, deviceType, tenantType, platformType } = input;
  const data = {
    email,
    password,
    nickName,
    payType,
    deviceType,
    tenantType,
    platformType
  };

  if (typeof input.days === 'number') {
    data.days = input.days;
  }

  return flatRequest({
    url: '/api/tenant',
    method: 'post',
    data: { ...data }
  });
}

export function fetchUpdateTenant(input: Api.Tenant.TenantUpdate) {
  const { id } = input;
  const data = {
    id
  };

  if (input.email) {
    data.email = input.email;
  }

  if (input.password) {
    data.password = input.password;
  }

  if (input.nickName) {
    data.nickName = input.nickName;
  }

  input.status = Number.parseInt(input.status, 10);
  if (!Number.isNaN(input.status)) {
    data.status = input.status;
  }

  if (input.payType && input.payType.length) {
    data.payType = input.payType;
  }

  if (input.deviceType && input.deviceType.length) {
    data.deviceType = input.deviceType;
  }

  if (input.platformType && input.platformType.length) {
    data.platformType = input.platformType;
  }

  if (typeof input.tenantType === 'number') {
    data.tenantType = input.tenantType;
  }

  if (Object.keys(data).length === 1) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/tenant',
    method: 'put',
    data: { ...data }
  });
}

export function fetchDeleteTenant(input: Array<string>) {
  if (!input || !input.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/tenant',
    method: 'delete',
    params: { ids: input }
  });
}

export function fetchGetTenantSubscription(id: string) {
  return flatRequest({
    url: '/api/tenant/subscription',
    method: 'get',
    params: { id }
  });
}

export function fetchTenantRecharge(input: Api.Tenant.TenantRecharge) {
  const { id } = input;
  const data = {
    id
  };

  if (typeof input.days === 'number') {
    data.days = input.days;
  }

  if (typeof input.months === 'number') {
    data.months = input.months;
  }

  if (typeof input.quarters === 'number') {
    data.quarters = input.quarters;
  }

  if (typeof input.years === 'number') {
    data.years = input.years;
  }

  if (Object.keys(data).length === 1) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/tenant/recharge',
    method: 'post',
    data: { ...data }
  });
}
