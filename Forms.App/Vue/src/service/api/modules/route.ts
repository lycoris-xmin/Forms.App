import { flatRequest } from '../../request';

/** get user routes */
export function fetchGetUserPermissions() {
  return flatRequest<App.Service.DataResponse<Array>>({
    url: '/api/authorize/user/permissions'
  });
}

/**
 * whether the route is exist
 *
 * @param permission route name
 */
export function fetchIsRouteExist(permission: string) {
  return flatRequest<App.Service.DataResponse<boolean>>({
    url: '/api/authorize/route/exist',
    params: { permission }
  });
}
