import { flatRequest } from '../../request';

export function fetchGetPlanTaskSettlementList(input: Api.PlanTaskSettlement.SearchFilter) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  if (input.pairUserId) {
    data.pairUserId = input.pairUserId;
  }

  return flatRequest({
    url: '/api/plantask/settlement/list',
    params: {
      ...data
    }
  });
}

export function fetchGetPlanTaskSettlementDetail(input: string) {
  return flatRequest({
    url: `/api/planTask/settlement/${input}`
  });
}

export function fetchAuditPlanTaskSettlement(id: string) {
  return flatRequest({
    url: `/api/planTask/settlement`,
    method: 'put',
    data: {
      id
    }
  });
}
