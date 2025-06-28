import { flatRequest } from '../../request';

export function fetchGetRoleEnum() {
  return flatRequest({
    url: '/api/enums/role'
  });
}

export function fetchGetSchedulerGroupEnum() {
  return flatRequest({
    url: '/api/enums/scheduler/group'
  });
}
