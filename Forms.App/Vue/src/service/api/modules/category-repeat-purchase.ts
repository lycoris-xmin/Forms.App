import { baseResp, flatRequest } from '../../request';

export function fetchGetCategoryRepeatPurchaseList(input: Api.CategoryRepeatPurchase.SearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.name) {
    data.name = input.name;
  }

  return flatRequest({
    url: '/api/categoryrepeatpurchase/list',
    params: {
      ...data
    }
  });
}

export function fetchCreateCategoryRepeatPurchase(input: Api.CategoryRepeatPurchase.Create) {
  return flatRequest({
    url: '/api/categoryrepeatpurchase',
    method: 'post',
    data: {
      name: input.name,
      keyword: input.keyword,
      limitDays: input.limitDays
    }
  });
}

export function fetchUpdateCategoryRepeatPurchase(input: Api.CategoryRepeatPurchase.Update) {
  const { id } = input;
  const data = {
    id
  };

  if (input.name) {
    data.name = input.name;
  }

  if (input.keyword) {
    data.keyword = input.keyword;
  }

  input.limitDays = Number.parseInt(input.limitDays, 10);
  if (!Number.isNaN(input.limitDays)) {
    data.limitDays = input.limitDays;
  }

  if (Object.keys(data).length === 1) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/categoryrepeatpurchase',
    method: 'put',
    data: {
      ...data
    }
  });
}

export function fetchDeleteCategoryRepeatPurchase(ids: Array<string>) {
  if (!ids || !ids.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/categoryrepeatpurchase',
    method: 'delete',
    params: {
      ids
    }
  });
}
