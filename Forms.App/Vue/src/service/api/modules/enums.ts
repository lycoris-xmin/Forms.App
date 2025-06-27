import { flatRequest } from '../../request';

export function fetchGetRoleEnum() {
  return flatRequest({
    url: '/api/enums/role'
  });
}

export function fetchUnBindingDeviceEnum(input: Api.Common.PageFilter) {
  const { pageIndex, pageSize } = input;

  return flatRequest({
    url: '/api/enums/device/unbinding',
    params: {
      pageIndex,
      pageSize
    }
  });
}

export function fetchDeviceAdressEnum(input: Api.Enums.DeviceAddressAssign) {
  const { pageIndex, pageSize, platform } = input;
  const data = {
    pageIndex,
    pageSize,
    platform
  };

  if (typeof input.binding === 'number') {
    data.binding = input.binding !== 0;
  }

  if (input.name) {
    data.name = input.name;
  }

  if (input.phone) {
    data.phone = input.phone;
  }

  return flatRequest({
    url: '/api/enums/device/address',
    params: {
      ...data
    }
  });
}

export function fetchGetTenantSubscriptionEnum() {
  return flatRequest({
    url: '/api/enums/tenant/subscription'
  });
}

export function fetchGetDeviceEnum(keyword: string) {
  return flatRequest({
    url: '/api/enums/device/current',
    params: {
      keyword
    }
  });
}

export function fetchGetAllDeviceEnum(keyword: string) {
  return flatRequest({
    url: '/api/enums/device',
    params: {
      keyword
    }
  });
}

export function fetchGetAlipayLovePayForTenantUserEnum() {
  return flatRequest({
    url: '/api/enums/alipaylovepay/tenant/user'
  });
}

export function fetchGetAlipayLovePayForTenantAdminEnum() {
  return flatRequest({
    url: '/api/enums/alipaylovepay/tenant/admin'
  });
}

export function fetchGetAlipayLovePayForTenantEnum(taskId: string) {
  return flatRequest({
    url: '/api/enums/alipaylovepay/tenant',
    params: {
      taskId
    }
  });
}

export function fetchGetTenantEnum(keyword?: string) {
  return flatRequest({
    url: '/api/enums/tenant',
    params: {
      keyword
    }
  });
}

export function fetchGetTenantUserEnum(keyword: string) {
  return flatRequest({
    url: '/api/enums/tenant/user',
    params: {
      keyword
    }
  });
}

export function fetchGetTenantEmployEnum() {
  return flatRequest({
    url: '/api/enums/tenant/employ'
  });
}

export function fetchGetTenantPayTypeEnum(tenantId?: string | null) {
  if (tenantId) {
    return flatRequest({
      url: `/api/enums/tenant/${tenantId}/paytype`
    });
  }
  return flatRequest({
    url: '/api/enums/tenant/paytype'
  });
}

export function fetchGetTenantDeviceTypeEnum(tenantId?: string | null) {
  if (tenantId) {
    return flatRequest({
      url: `/api/enums/tenant/${tenantId}/devicetype`
    });
  }
  return flatRequest({
    url: '/api/enums/tenant/devicetype'
  });
}

export function fetchGetTenantPlatformEnum(tenantId?: string | null) {
  if (tenantId) {
    return flatRequest({
      url: `/api/enums/tenant/${tenantId}/PlatformeType`
    });
  }
  return flatRequest({
    url: '/api/enums/tenant/PlatformeType'
  });
}

export function fetchGetTaobaoProductSkuMap(productId: string) {
  return flatRequest({
    url: `/api/enums/taobao/product/skumap/${productId}`
  });
}

export function fetchGetTaobaoProductListForPlanTask(input: Api.PlanTask.ProductSearchFilter) {
  const { pageIndex, pageSize, platform } = input;

  const data = {
    pageIndex,
    pageSize,
    platform
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

  return flatRequest({
    url: '/api/enums/product',
    params: { ...data }
  });
}

export function fetchGetDeviceAreaName() {
  return flatRequest({
    url: '/api/enums/device/area'
  });
}

export function fetchGetDeviceGroup() {
  return flatRequest({
    url: '/api/enums/device/group'
  });
}

export function fetchGetDeviceSection() {
  return flatRequest({
    url: '/api/enums/device/section'
  });
}

export function fetchGetTaobaoShopName(shopName?: string) {
  return flatRequest({
    url: '/api/enums/taobao/shopname',
    params: {
      shopName
    }
  });
}

export function fetchGetTenantFriendEnum(keyword?: string) {
  return flatRequest({
    url: '/api/enums/tenant/friend',
    params: {
      keyword
    }
  });
}

export function fetchGetTaobaoDictionaryShopName(shopName?: string) {
  return flatRequest({
    url: '/api/enums/taobao/dictionary/shopname',
    params: {
      shopName
    }
  });
}

export function fetchDouyinShopName(shopName?: string) {
  return flatRequest({
    url: '/api/enums/Douyin/ShopName',
    params: {
      shopName
    }
  });
}

export function fetchGetALIShopName(shopName?: string) {
  return flatRequest({
    url: '/api/enums/Ali/ShopName',
    params: {
      shopName
    }
  });
}

export function fetchGetNetworkpairingConfigFromEnums() {
  return flatRequest<{
    code: number;
    data: {
      TB: Array<{ level: number; dayMinLimit: number; dayMaxLimit: number; monthLimit: number }>;
      JD: Array<{ level: number; dayMinLimit: number; dayMaxLimit: number; monthLimit: number }>;
      DY: Array<{ level: number; dayMinLimit: number; dayMaxLimit: number; monthLimit: number }>;
      ALIBABA: Array<{ level: number; dayMinLimit: number; dayMaxLimit: number; monthLimit: number }>;
    };
  }>({
    url: '/api/enums/networkpairing',
    method: 'get',
    headers: {
      'Content-Type': 'application/json'
    }
  });
}
