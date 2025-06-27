import { useAuthStore } from '@/store/modules/auth';

const Device = {
  List: {
    VIEW: 'device.list.view',
    CREATE: 'device.list.create',
    UPDATE: 'device.list.update',
    DELETE: 'device.list.delete'
  },
  Taobao: {
    VIEW: 'device.taobao.view',
    UPDATE: 'device.taobao.update'
  },
  PDD: {
    VIEW: 'device.pdd.view',
    UPDATE: 'device.pdd.update'
  },
  DY: {
    VIEW: 'device.dy.view',
    UPDATE: 'device.dy.update'
  },
  RedNote: {
    VIEW: 'device.rednote.view',
    UPDATE: 'device.rednote.update'
  },
  Ali1688: {
    VIEW: 'device.alibaba.view',
    UPDATE: 'device.alibaba.update'
  }
};

const Taobao = {
  Shop: {
    VIEW: 'taobao.shop.view',
    CREATE: 'taobao.shop.create',
    UPDATE: 'taobao.shop.update',
    DELETE: 'taobao.shop.delete'
  },
  Product: {
    VIEW: 'taobao.product.view',
    CREATE: 'taobao.product.create',
    UPDATE: 'taobao.product.update',
    DELETE: 'taobao.product.delete'
  },
  Task: {
    VIEW: 'taobao.task.view',
    CREATE: 'taobao.task.create',
    UPDATE: 'taobao.task.update',
    DELETE: 'taobao.task.delete'
  },
  Comment: {
    VIEW: 'taobao.comment.view',
    CREATE: 'taobao.comment.create',
    UPDATE: 'taobao.comment.update',
    DELETE: 'taobao.comment.delete'
  }
};
// 1688
const Ali1688 = {
  Shop: {
    VIEW: 'ali1688.shop.view',
    CREATE: 'ali1688.shop.create',
    UPDATE: 'ali1688.shop.update',
    DELETE: 'ali1688.shop.delete'
  },
  Product: {
    VIEW: 'ali1688.product.view',
    CREATE: 'ali1688.product.create',
    UPDATE: 'ali1688.product.update',
    DELETE: 'ali1688.product.delete'
  },
  Task: {
    VIEW: 'ali1688.task.view',
    CREATE: 'ali1688.task.create',
    UPDATE: 'ali1688.task.update',
    DELETE: 'ali1688.task.delete'
  },
  Comment: {
    VIEW: 'ali1688.comment.view',
    CREATE: 'ali1688.comment.create',
    UPDATE: 'ali1688.comment.update',
    DELETE: 'ali1688.comment.delete'
  }
};
const PDD = {
  Shop: {
    VIEW: 'pdd.shop.view',
    CREATE: 'pdd.shop.create',
    UPDATE: 'pdd.shop.update',
    DELETE: 'pdd.shop.delete'
  },
  Product: {
    VIEW: 'pdd.product.view',
    CREATE: 'pdd.product.create',
    UPDATE: 'pdd.product.update',
    DELETE: 'pdd.product.delete'
  },
  Task: {
    VIEW: 'pdd.task.view',
    CREATE: 'pdd.task.create',
    UPDATE: 'pdd.task.update',
    DELETE: 'pdd.task.delete'
  },
  Comment: {
    VIEW: 'pdd.comment.view',
    CREATE: 'pdd.comment.create',
    UPDATE: 'pdd.comment.update',
    DELETE: 'pdd.comment.delete'
  }
};

const DY = {
  Shop: {
    VIEW: 'dy.shop.view',
    CREATE: 'dy.shop.create',
    UPDATE: 'dy.shop.update',
    DELETE: 'dy.shop.delete'
  },
  Product: {
    VIEW: 'dy.product.view',
    CREATE: 'dy.product.create',
    UPDATE: 'dy.product.update',
    DELETE: 'dy.product.delete'
  },
  Task: {
    VIEW: 'dy.task.view',
    CREATE: 'dy.task.create',
    UPDATE: 'dy.task.update',
    DELETE: 'dy.task.delete'
  },
  Comment: {
    VIEW: 'dy.comment.view',
    CREATE: 'dy.comment.create',
    UPDATE: 'dy.comment.update',
    DELETE: 'dy.comment.delete'
  }
};

const RedNote = {
  Shop: {
    VIEW: 'rednote.shop.view',
    CREATE: 'rednote.shop.create',
    UPDATE: 'rednote.shop.update',
    DELETE: 'rednote.shop.delete'
  },
  Product: {
    VIEW: 'rednote.product.view',
    CREATE: 'rednote.product.create',
    UPDATE: 'rednote.product.update',
    DELETE: 'rednote.product.delete'
  },
  Task: {
    VIEW: 'rednote.task.view',
    CREATE: 'rednote.task.create',
    UPDATE: 'rednote.task.update',
    DELETE: 'rednote.task.delete'
  },
  Comment: {
    VIEW: 'rednote.comment.view',
    CREATE: 'rednote.comment.create',
    UPDATE: 'rednote.comment.update',
    DELETE: 'rednote.comment.delete'
  }
};

const Platform = {
  Tenant: {
    VIEW: 'platform.tenant.view',
    CREATE: 'platform.tenant.create',
    UPDATE: 'platform.tenant.update',
    DELETE: 'platform.tenant.delete'
  },
  TenantUser: {
    VIEW: 'platform.tenant.user.view',
    CREATE: 'platform.tenant.user.create',
    UPDATE: 'platform.tenant.user.update',
    DELETE: 'platform.tenant.user.delete'
  },
  AlipayLovePay: {
    VIEW: 'platform.alipay.lovepay.view',
    CREATE: 'platform.alipay.lovepay.create',
    UPDATE: 'platform.alipay.lovepay.update',
    DELETE: 'platform.alipay.lovepay.delete'
  },
  CategoryRepeatPurchase: {
    VIEW: 'platform.category.repeatpurchase.view',
    CREATE: 'platform.category.repeatpurchase.create',
    UPDATE: 'platform.category.repeatpurchase.update',
    DELETE: 'platform.category.repeatpurchase.delete'
  },
  Setting: {
    VIEW: 'platform.setting.view',
    CREATE: 'platform.setting.create',
    UPDATE: 'platform.setting.update',
    DELETE: 'platform.setting.delete'
  }
};

const Report = {
  Settlement: {
    VIEW: 'report.settlement.update',
    UPDATE: 'report.settlement.update'
  },
  AlipayLovePay: {
    VIEW: 'report.alipay.lovepay.view',
    UPDATE: 'report.alipay.lovepay.update'
  },
  AlipayLovePayEntSettle: {
    VIEW: 'report.alipay.lovepay.entSettle.view',
    UPDATE: 'report.alipay.lovepay.entSettle.update'
  },
  AlipayLovePayPerSettle: {
    VIEW: 'report.alipay.lovepay.perSettle.view',
    UPDATE: 'report.alipay.lovepay.perSettle.update'
  }
};

const Maintain = {
  AutoJsScript: {
    VIEW: 'maintain.autojs.script.view',
    CREATE: 'maintain.autojs.script.create',
    UPDATE: 'maintain.autojs.script.update',
    DELETE: 'maintain.autojs.script.delete'
  },
  Commnad: {
    VIEW: 'maintain.command.view',
    CREATE: 'maintain.command.create'
  }
};

const Logger = {
  System: {
    VIEW: 'logger.system.view'
  },
  Request: {
    VIEW: 'logger.request.view'
  }
};

const System = {
  User: {
    VIEW: 'system.user.view',
    CREATE: 'system.user.create',
    UPDATE: 'system.user.update',
    DELETE: 'system.user.delete'
  },
  Role: {
    VIEW: 'system.role.view',
    CREATE: 'system.role.create',
    UPDATE: 'system.role.update',
    DELETE: 'system.role.delete'
  },
  Setting: {
    VIEW: 'system.setting.view',
    UPDATE: 'system.setting.update'
  }
};
const Friend = {
  List: {
    VIEW: 'friend.list.view',
    CREATE: 'friend.list.create',
    UPDATE: 'friend.list.update',
    DELETE: 'friend.list.delete'
  },
  Request: {
    VIEW: 'friend.request.view',
    CREATE: 'friend.request.create',
    UPDATE: 'friend.request.update',
    DELETE: 'friend.request.delete'
  },
  Audit: {
    VIEW: 'friend.audit.view',
    CREATE: 'friend.audit.create',
    UPDATE: 'friend.audit.update',
    DELETE: 'friend.audit.delete'
  }
};
export default {
  Device,
  Taobao,
  Ali1688,
  PDD,
  DY,
  RedNote,
  Platform,
  Report,
  Maintain,
  Logger,
  System,
  Friend
};

export function useRolePermission(): { hasOne: () => boolean; hasAll: () => boolean } {
  const authStore = useAuthStore();

  function hasOne(permissions: Array<string> | string): boolean {
    // 如果是超级管理员，直接返回 true
    if (authStore.userInfo.isSuperAdmin || authStore.userInfo.roleId === '1000') {
      return true;
    }

    // 将输入转换为字符串数组
    let permissionInput: string[];

    if (Array.isArray(permissions)) {
      // 如果是数组，直接使用
      permissionInput = [...permissions];
    } else if (typeof permissions === 'string') {
      // 如果是字符串，转换为单元素数组
      permissionInput = [permissions];
    } else {
      // 如果是对象，提取所有值转换为数组
      permissionInput = Object.values(permissions);
    }

    return permissionInput.some(x => authStore.userPermission.includes(x));
  }

  function hasAll(permissions: Array<string> | string): boolean {
    // 如果是超级管理员，直接返回 true
    if (authStore.userInfo.isSuperAdmin || authStore.userInfo.roleId === '1000') {
      return true;
    }

    const permissionInput = Array.isArray(permissions) ? [...permissions] : [permissions];

    return permissionInput.every(x => authStore.userPermission.includes(x));
  }

  return {
    hasOne,
    hasAll
  };
}
