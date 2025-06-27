import { flatRequest } from '../../request';

// 淘宝店铺列表
export const fetchGetTaobaoShopList = (input: Api.Shop.BaseSearchFilter) => {
  const { pageIndex, pageSize } = input;

  const data: Api.Shop.BaseSearchFilter = {
    pageIndex,
    pageSize
  };

  if (input.shopName) {
    data.shopName = input.shopName;
  }
  if (input.tenantId) {
    data.tenantId = input.tenantId;
  }

  return flatRequest({
    url: '/api/shop/taobao/list',
    params: {
      ...data
    }
  });
};
// 淘宝店铺新增
export const fetchCreateTaobaoShop = (shopName: string) => {
  return flatRequest({
    url: '/api/shop/taobao',
    method: 'post',
    data: {
      shopName
    }
  });
};
// 淘宝店铺修改
export const fetchUpdateTaobaoShop = ({ id, shopName }) => {
  return flatRequest({
    url: '/api/shop/taobao',
    method: 'put',
    data: {
      id,
      shopName
    }
  });
};
// 淘宝店铺删除
export const fetchDeleteTaobaoShop = (ids: Array<string>) => {
  return flatRequest({
    url: '/api/shop/taobao',
    method: 'delete',
    params: {
      ids
    }
  });
};

// 1688店铺列表
export const GetALIShopList = (input: Api.ShopALI.BaseSearchFilter) => {
  const { pageIndex, pageSize } = input;

  const data: Api.ShopALI.BaseSearchFilter = {
    pageIndex,
    pageSize
  };

  if (input.shopName) {
    data.shopName = input.shopName;
  }

  if (input.tenantId) {
    data.tenantId = input.tenantId;
  }

  return flatRequest({
    url: '/api/shop/Ali/List',
    params: {
      ...data
    }
  });
};

// 1688店铺新增
export const CreateALIShop = (shopName: string) => {
  return flatRequest({
    url: '/api/shop/Ali',
    method: 'post',
    data: {
      shopName
    }
  });
};

// 1688店铺修改
export const UpdateALIShop = ({ id, shopName }) => {
  return flatRequest({
    url: '/api/shop/Ali',
    method: 'put',
    data: {
      id,
      shopName
    }
  });
};

// 1688店铺删除
export const DeleteALIShop = (ids: Array<string>) => {
  return flatRequest({
    url: '/api/shop/Ali',
    method: 'delete',
    params: {
      ids
    }
  });
};
// 抖音列表显示
export const fetchGetDouyinShopList = (input: Api.Shop.BaseSearchFilter) => {
  const { pageIndex, pageSize } = input;

  const data: Api.Shop.BaseSearchFilter = {
    pageIndex,
    pageSize
  };

  if (input.shopName) {
    data.shopName = input.shopName;
  }

  if (input.tenantId) {
    data.tenantId = input.tenantId;
  }

  return flatRequest({
    url: '/api/Shop/DouyinList',
    params: {
      ...data
    }
  });
};

// 抖音添加店铺
export const fetchCreateDouyinShop = (shopName: string) => {
  return flatRequest({
    url: '/api/Shop/DouyinCreate',
    method: 'post',
    data: {
      shopName
    }
  });
};

// 抖音店铺修改
export const fetchUpdateDouyinShop = ({ id, shopName }) => {
  return flatRequest({
    url: '/api/Shop/DouyinUpdeate',
    method: 'put',
    data: {
      id,
      shopName
    }
  });
};

// 抖音店铺删除
export const fetchDeleteDouyinShop = (ids: Array<string>) => {
  return flatRequest({
    url: '/api/Shop/DouyinDelete',
    method: 'delete',
    params: {
      ids
    }
  });
};
