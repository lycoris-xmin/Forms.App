<script setup lang="ts">
  import { computed } from 'vue';
  import type { Component } from 'vue';
  import { getColorPalette, mixColor } from '@sa/utils';
  import { $t } from '@/locales';
  import { useAppStore } from '@/store/modules/app';
  import { useThemeStore } from '@/store/modules/theme';
  import PhonePwdLogin from './modules/phone-pwd-login.vue';
  import EmailPwdLogin from './modules/email-pwd-login.vue';
  import CodeLogin from './modules/code-login.vue';
  import EmailRegister from './modules/email-register.vue';
  import CodeRegister from './modules/code-register.vue';
  import ResetPwd from './modules/reset-pwd.vue';
  import BindWechat from './modules/bind-wechat.vue';

  interface Props {
    /** The login module */
    module?: UnionKey.LoginModule;
  }

  const props = defineProps<Props>();

  const appStore = useAppStore();
  const themeStore = useThemeStore();

  interface LoginModule {
    label: string;
    component: Component;
  }

  const moduleMap: Record<UnionKey.LoginModule, LoginModule> = {
    'phone-pwd-login': { label: '密码登录', component: PhonePwdLogin },
    'email-pwd-login': { label: '密码登录', component: EmailPwdLogin },
    'code-login': { label: '验证码登录', component: CodeLogin },
    'email-register': { label: '邮箱注册', component: EmailRegister },
    'code-register': { label: '手机号注册', component: CodeRegister },
    'reset-pwd': { label: '重置密码', component: ResetPwd },
    'bind-wechat': { label: '微信登录', component: BindWechat }
  };

  if (import.meta.env.VITE_LOGIN_TYPE === 'phone') {
    delete moduleMap['email-pwd-login'];
    delete moduleMap['email-register'];
  } else {
    delete moduleMap['phone-pwd-login'];
    delete moduleMap['code-login'];
    delete moduleMap['code-register'];
  }

  const activeModule = computed(() => moduleMap[props.module || 'phone-pwd-login']);

  const bgThemeColor = computed(() => (themeStore.darkMode ? getColorPalette(themeStore.themeColor, 7) : themeStore.themeColor));

  const bgColor = computed(() => {
    const COLOR_WHITE = '#ffffff';

    const ratio = themeStore.darkMode ? 0.5 : 0.2;

    return mixColor(COLOR_WHITE, themeStore.themeColor, ratio);
  });
</script>

<template>
  <div class="relative size-full flex-center" :style="{ backgroundColor: bgColor }">
    <WaveBg :theme-color="bgThemeColor" />
    <ACard class="relative z-4">
      <div class="w-400px lt-sm:w-300px">
        <header class="flex-y-center justify-between">
          <SystemLogo class="text-64px text-primary lt-sm:text-48px" />
          <h3 class="text-28px text-primary font-500 lt-sm:text-22px">{{ $t('system.title') }}</h3>
          <div class="i-flex-col">
            <ThemeSchemaSwitch :theme-schema="themeStore.themeScheme" :show-tooltip="false" class="text-20px lt-sm:text-18px" @switch="themeStore.toggleThemeScheme" />
            <LangSwitch :lang="appStore.locale" :lang-options="appStore.localeOptions" :show-tooltip="false" @change-lang="appStore.changeLocale" />
          </div>
        </header>
        <main class="pt-24px">
          <h3 class="text-18px text-primary font-medium">{{ activeModule.label }}</h3>
          <div class="animation-slide-in-left pt-24px">
            <Transition :name="themeStore.page.animateMode" mode="out-in" appear>
              <component :is="activeModule.component" />
            </Transition>
          </div>
        </main>
      </div>
    </ACard>
  </div>
</template>

<style scoped></style>
