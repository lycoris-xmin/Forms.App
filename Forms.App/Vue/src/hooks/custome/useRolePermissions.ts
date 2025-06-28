import { useAuthStore } from '@/store/modules/auth';

export const AppPermissions = {
  Server: {
    Redis: {
      VIEW: 'server.redis.view',
      CREATE: 'server.redis.create',
      UPDATE: 'server.redis.update',
      DELETE: 'server.redis.delete'
    }
  },
  Logger: {
    System: {
      VIEW: 'logger.system.view'
    },
    Request: {
      VIEW: 'logger.request.view'
    }
  },
  System: {
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
  }
};

export function useRolePermission(): { hasOne: () => boolean; hasAll: () => boolean } {
  const authStore = useAuthStore();

  function hasOne(permissions: Array<string> | string): boolean {
    if (authStore.userInfo.isAdmin) {
      return true;
    }

    const permissionInput = Array.isArray(permissions) ? [...permissions] : [permissions];

    return permissionInput.some(x => authStore.userPermission.includes(x));
  }

  function hasAll(permissions: Array<string> | string): boolean {
    if (authStore.userInfo.isAdmin) {
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
