<script setup lang="ts">
  import { useAppStore } from '@/store/modules/app';
  import { useThemeStore } from '@/store/modules/theme';
  import { $t } from '@/locales';
  import LayoutModeCard from '../components/layout-mode-card.vue';
  import SettingItem from '../components/setting-item.vue';

  defineOptions({
    name: 'LayoutMode'
  });

  const appStore = useAppStore();
  const themeStore = useThemeStore();

  type CheckedType = boolean | string | number;

  function handleReverseHorizontalMixChange(value: CheckedType) {
    themeStore.setLayoutReverseHorizontalMix(value as boolean);
  }
</script>

<template>
  <ADivider>{{ $t('theme.layoutMode.title') }}</ADivider>
  <LayoutModeCard v-model:mode="themeStore.layout.mode" :disabled="appStore.isMobile">
    <template #vertical>
      <div class="layout-sider h-full w-18px"></div>
      <div class="vertical-wrapper">
        <div class="layout-header"></div>
        <div class="layout-main"></div>
      </div>
    </template>
    <template #vertical-mix>
      <div class="layout-sider h-full w-8px"></div>
      <div class="layout-sider h-full w-16px"></div>
      <div class="vertical-wrapper">
        <div class="layout-header"></div>
        <div class="layout-main"></div>
      </div>
    </template>
    <template #horizontal>
      <div class="layout-header"></div>
      <div class="horizontal-wrapper">
        <div class="layout-main"></div>
      </div>
    </template>
    <template #horizontal-mix>
      <div class="layout-header"></div>
      <div class="horizontal-wrapper">
        <div class="layout-sider w-18px"></div>
        <div class="layout-main"></div>
      </div>
    </template>
  </LayoutModeCard>
  <SettingItem v-if="themeStore.layout.mode === 'horizontal-mix'" :label="$t('theme.layoutMode.reverseHorizontalMix')" class="mt-16px">
    <ASwitch :checked="themeStore.layout.reverseHorizontalMix" @update:checked="handleReverseHorizontalMixChange" />
  </SettingItem>
</template>

<style scoped>
  .layout-header {
    --uno: h-16px bg-primary rd-4px;
  }

  .layout-sider {
    --uno: bg-primary-300 rd-4px;
  }

  .layout-main {
    --uno: flex-1 bg-primary-200 rd-4px;
  }

  .vertical-wrapper {
    --uno: flex-1 flex-col gap-6px;
  }

  .horizontal-wrapper {
    --uno: flex-1 flex gap-6px;
  }
</style>
