import type { DirectiveBinding } from 'vue';
import { useAuthStore } from '@/store/modules/auth';

/** 检查是否具备某一个权限 */
function checkPermission(permission: string | string[]): boolean {
  const authStore = useAuthStore();

  if (authStore.userInfo.isAdmin) {
    return true;
  }

  const list = Array.isArray(permission) ? permission : [permission];
  return list.some(p => authStore.userPermission.includes(p));
}

/** 权限指令 v-permission 用法： v-permission="'system.user.view'" v-permission="['system.user.view', 'system.user.update']" */
const permissionDirective = {
  mounted(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    if (!checkPermission(binding.value)) {
      el.parentNode?.removeChild(el);
    }
  }
};

export default permissionDirective;
