import { flatRequest } from '../../request';

/** get user routes */
export function getUserPermissionsApi() {
  return flatRequest({
    url: '/api/authorize/user/permissions'
  });
}

/**
 * whether the route is exist
 *
 * @param permission route name
 */
export function checkIsRouteExistApi(permission: string) {
  return flatRequest({
    url: '/api/authorize/route/exist',
    params: { permission }
  });
}
