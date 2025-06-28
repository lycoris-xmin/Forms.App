import { baseResp, flatRequest } from '../../request';

export function getRoleListApi(input: Api.Common.PageFilter): App.Service.PageResponse<Api.Role.Data> {
  const { pageIndex, pageSize } = input;
  return flatRequest({
    url: '/api/role/list',
    params: {
      pageIndex,
      pageSize
    }
  });
}

export function createRoleApi(roleName: string) {
  return flatRequest({
    url: '/api/role',
    method: 'post',
    data: {
      roleName
    }
  });
}

export function updateRoleApi(input: Api.Role.UpdateRoe) {
  const { id, roleName } = input;
  return flatRequest({
    url: `/api/role/${id}`,
    method: 'put',
    data: {
      roleName
    }
  });
}

export function deleteRoleApi(id: string) {
  if (!ids || !ids.length) {
    return baseResp();
  }

  return flatRequest({
    url: `/api/role/${id}`,
    method: 'delete'
  });
}

export function getRolePermissionApi(id: string) {
  return flatRequest({
    url: `/api/role/permission/${id}`
  });
}

export function setRolePermissionApi(id: string, permissions: Array<string>) {
  return flatRequest({
    url: `/api/role/permission/${id}`,
    method: 'put',
    data: {
      permissions
    }
  });
}
