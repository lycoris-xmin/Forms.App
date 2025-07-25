<script setup lang="ts">
  import { computed, withDefaults } from 'vue';
  import type { TooltipPlacement } from 'ant-design-vue/es/tooltip';
  import { $t } from '@/locales';

  defineOptions({ name: 'ThemeSchemaSwitch' });

  interface Props {
    /** Theme schema */
    themeSchema: UnionKey.ThemeScheme;
    /** Show tooltip */
    showTooltip?: boolean;
    /** Tooltip placement */
    tooltipPlacement?: TooltipPlacement;
  }

  const props = withDefaults(defineProps<Props>(), {
    showTooltip: true,
    tooltipPlacement: 'bottom'
  });

  interface Emits {
    (e: 'switch'): void;
  }

  const emit = defineEmits<Emits>();

  function handleSwitch() {
    emit('switch');
  }

  const icons: Record<UnionKey.ThemeScheme, string> = {
    light: 'material-symbols:sunny',
    dark: 'material-symbols:nightlight-rounded',
    auto: 'material-symbols:hdr-auto'
  };

  const icon = computed(() => icons[props.themeSchema]);

  const tooltipContent = computed(() => {
    if (!props.showTooltip) return '';

    return $t('icon.themeSchema');
  });
</script>

<template>
  <ButtonIcon :icon="icon" :tooltip-content="tooltipContent" :tooltip-placement="tooltipPlacement" @click="handleSwitch" />
</template>

<style scoped></style>
