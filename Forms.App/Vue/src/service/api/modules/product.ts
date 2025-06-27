import { baseResp, flatRequest, objectToFormData } from '../../request';

export function fetchGetTaobaoProductList(input: Api.Product.TaobaoSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.shopId) {
    data.shopId = input.shopId;
  }

  if (input.itemId) {
    data.itemId = input.itemId;
  }

  if (input.title) {
    data.title = input.title;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/product/taobao/list',
    params: {
      ...data
    }
  });
}

export function fetchCreateTaobaoProduct(input: Api.Product.TaobaoCreate) {
  const { title, skuMap, shopId, type } = input;

  const json = {
    type,
    title,
    skuMap,
    shopId
  };

  if (input.url) {
    json.url = input.url;
    json.itemId = input.itemId;
  }

  if (input.image) {
    json.image = input.image;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: '/api/product/taobao',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

export function fetchUpdateTaobaoProduct(input: Api.Product.Update) {
  const { id, title, skuMap, shopId, type } = input;

  const json = {
    type,
    title,
    skuMap,
    shopId
  };

  if (input.url) {
    json.url = input.url;
    json.itemId = input.itemId;
  }

  if (input.image) {
    json.image = input.image;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: `/api/product/taobao/${id}`,
    method: 'put',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

export function fetchDeleteTaobaoProduct(input: Array<string>) {
  if (!input.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/product/taobao',
    method: 'delete',
    params: {
      ids: input
    }
  });
}

// 抖音商品
export function fetchGetDouyinProductList(input: Api.Product.DouyinSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.shopId) {
    data.shopId = input.shopId;
  }

  if (input.itemId) {
    data.itemId = input.itemId;
  }

  if (input.title) {
    data.title = input.title;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/product/DouyinList',
    params: {
      ...data
    }
  });
}

export function fetchCreateDouyinProduct(input: Api.Product.DouyinCreate) {
  const { title, skuMap, shopId, type } = input;

  const json = {
    type,
    title,
    skuMap,
    shopId
  };

  if (input.url) {
    json.url = input.url;
    json.itemId = input.itemId;
  }

  if (input.image) {
    json.image = input.image;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: '/api/product/Douyin',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

export function fetchUpdateDouyinProduct(input: Api.Product.Update) {
  const { id, title, skuMap, shopId, type } = input;

  const json = {
    type,
    title,
    skuMap,
    shopId
  };

  if (input.url) {
    json.url = input.url;
    json.itemId = input.itemId;
  }

  if (input.image) {
    json.image = input.image;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: `/api/product/Douyin/${id}`,
    method: 'put',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

export function fetchDeleteDouyinProduct(input: Array<string>) {
  if (!input.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/product/Douyin',
    method: 'delete',
    params: {
      ids: input
    }
  });
}

// 1688商品列表
export function GetALIProductList(input: Api.Product.ALISearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.shopId) {
    data.shopId = input.shopId;
  }

  if (input.itemId) {
    data.itemId = input.itemId;
  }

  if (input.title) {
    data.title = input.title;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/product/Ali/List',
    params: {
      ...data
    }
  });
}
// 1688商品新增
export function CreateALIProduct(input: Api.Product.ALICreate) {
  const { title, skuMap, shopId, type } = input;

  const json = {
    type,
    title,
    skuMap,
    shopId
  };

  if (input.url) {
    json.url = input.url;
    json.itemId = input.itemId;
  }

  if (input.image) {
    json.image = input.image;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: '/api/product/Ali',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}
// 1688商品修改
export function UpdateALIProduct(input: Api.Product.Update) {
  const { id, title, skuMap, shopId, type } = input;

  const json = {
    type,
    title,
    skuMap,
    shopId
  };

  if (input.url) {
    json.url = input.url;
    json.itemId = input.itemId;
  }

  if (input.image) {
    json.image = input.image;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: `/api/product/Ali/${id}`,
    method: 'put',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}
// 1688商品删除
export function DeleteALIProduct(input: Array<string>) {
  if (!input.length) {
    return baseResp();
  }

  return flatRequest({
    url: '/api/product/Ali',
    method: 'delete',
    params: {
      ids: input
    }
  });
}
