import { baseResp, flatRequest } from '../../request';

export function fetchGetTenantUserList(input: Api.TenantUser.ListSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.email) {
    data.email = input.email;
  }

  if (input.status !== undefined && input.status !== null && input.status > 0) {
    data.status = input.status;
  }

  return flatRequest({
    url: '/api/tenant/user/list',
    params: { ...data }
  });
}

export function fetchCreateTenantUser(input: Api.TenantUser.TenantUserCreate) {
  const { email, password, nickName, isTenantAdmin } = input;
  const data = {
    email,
    password,
    nickName,
    isTenantAdmin: isTenantAdmin === 1
  };

  return flatRequest({
    url: '/api/tenant/user',
    method: 'post',
    data: { ...data }
  });
}

export function fetchUpdateTenantUser(input: Api.TenantUser.TenantUserUpdate) {
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

  if (input.email) {
    data.email = input.email;
  }

  if (typeof input.isTenantAdmin === 'number') {
    data.isTenantAdmin = input.isTenantAdmin === 1;
  }

  if (Object.keys(data).length === 1) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/tenant/user',
    method: 'put',
    data: { ...data }
  });
}

export function fetchDeleteTenantUser(input: Array<string>) {
  if (!input || !input.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/tenant/user',
    method: 'delete',
    params: { ids: input }
  });
}
