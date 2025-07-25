<script setup lang="ts">
  import { computed } from 'vue';
  import type { Component } from 'vue';
  import { transformColorWithOpacity } from '@sa/utils';
  import { useAppStore } from '@/store/modules/app';
  import { useThemeStore } from '@/store/modules/theme';
  import VerticalMenu from './modules/vertical-menu.vue';
  import VerticalMixMenu from './modules/vertical-mix-menu.vue';
  import HorizontalMenu from './modules/horizontal-menu.vue';
  import HorizontalMixMenu from './modules/horizontal-mix-menu.vue';
  import ReversedHorizontalMixMenu from './modules/reversed-horizontal-mix-menu.vue';

  defineOptions({
    name: 'GlobalMenu'
  });

  const appStore = useAppStore();
  const themeStore = useThemeStore();

  const activeMenu = computed(() => {
    const menuMap: Record<UnionKey.ThemeLayoutMode, Component> = {
      vertical: VerticalMenu,
      'vertical-mix': VerticalMixMenu,
      horizontal: HorizontalMenu,
      'horizontal-mix': themeStore.layout.reverseHorizontalMix ? ReversedHorizontalMixMenu : HorizontalMixMenu
    };

    return menuMap[themeStore.layout.mode];
  });

  const reRenderVertical = computed(() => themeStore.layout.mode === 'vertical' && appStore.isMobile);

  const selectedBgColor = computed(() => {
    const { darkMode, themeColor } = themeStore;

    const light = transformColorWithOpacity(themeColor, 0.1, '#ffffff');
    const dark = transformColorWithOpacity(themeColor, 0.3, '#000000');

    return darkMode ? dark : light;
  });
</script>

<template>
  <component :is="activeMenu" :key="reRenderVertical" />
</template>

<style>
  @import './index.scss';

  .select-menu {
    --selected-bg-color: v-bind(selectedBgColor);
  }
</style>
