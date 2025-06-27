import { baseResp, flatRequest } from '../../request';

export function fetchGetRoleList(input: Api.Common.PageFilter): App.Service.PageResponse<Api.Role.RoleData> {
  const { pageIndex, pageSize } = input;
  return flatRequest({
    url: '/api/role/list',
    params: {
      pageIndex,
      pageSize
    }
  });
}

export function fetchCreateRole(roleName: string) {
  return flatRequest({
    url: '/api/role',
    method: 'post',
    data: {
      roleName
    }
  });
}

export function fetchUpdateRole(input: Api.Role.UpdateRoe) {
  const { id, roleName } = input;
  return flatRequest({
    url: '/api/role',
    method: 'put',
    data: {
      id,
      roleName
    }
  });
}

export function fetchDeleteRole(ids: Array<string>) {
  if (!ids || !ids.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/role',
    method: 'delete',
    params: {
      ids
    }
  });
}

export function fetchGetRolePermission(id: string): App.Service.ListResponse<Api.Role.RolePermission> {
  return flatRequest({
    url: `/api/role/permission/${id}`
  });
}

export function fetchSetRolePermission(id: string, permissions: Array<string>): App.Service.Response {
  return flatRequest({
    url: `/api/role/permission/${id}`,
    method: 'put',
    data: {
      permissions
    }
  });
}
