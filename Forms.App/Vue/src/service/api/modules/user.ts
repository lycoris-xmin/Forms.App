import { baseResp, flatRequest } from '../../request';

export function fetchGetSystemUserList(input: Api.User.UserSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.email) {
    data.email = input.email;
  }

  input.roleId = Number.parseInt(input.roleId, 10);
  if (!Number.isNaN(input.roleId)) {
    data.roleId = input.roleId;
  }

  input.status = Number.parseInt(input.status, 10);
  if (!Number.isNaN(input.status)) {
    data.status = input.status;
  }

  return flatRequest({
    url: '/api/user/list',
    params: { ...data }
  });
}

export function fetchCreateSystemUser(input: Api.User.UserCreate) {
  const { email, password, nickName, roleId } = input;
  const data = {
    email,
    password,
    nickName,
    roleId
  };

  return flatRequest({
    url: '/api/user',
    method: 'post',
    data: { ...data }
  });
}

export function fetchUpdateSystemUser(input: Api.User.UserUpdate) {
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

  input.roleId = Number.parseInt(input.roleId, 10);
  if (!Number.isNaN(input.roleId)) {
    data.roleId = input.roleId;
  }

  input.status = Number.parseInt(input.status, 10);
  if (!Number.isNaN(input.status)) {
    data.status = input.status;
  }

  if (Object.keys(data).length === 1) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/user',
    method: 'put',
    data: { ...data }
  });
}

export function fetchDeleteSystemUser(input: Array<string>) {
  if (!input || !input.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/user',
    method: 'delete',
    params: { ids: input }
  });
}
