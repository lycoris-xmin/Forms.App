<script setup lang="ts">
  import { computed, withDefaults } from 'vue';
  import { $t } from '@/locales';

  defineOptions({
    name: 'LangSwitch'
  });

  interface Props {
    /** Current language */
    lang: App.I18n.LangType;
    /** Language options */
    langOptions: App.I18n.LangOption[];
    /** Show tooltip */
    showTooltip?: boolean;
  }

  const props = withDefaults(defineProps<Props>(), {
    showTooltip: true
  });

  type Emits = {
    (e: 'changeLang', lang: App.I18n.LangType): void;
  };

  const emit = defineEmits<Emits>();

  const tooltipContent = computed(() => {
    if (!props.showTooltip) return '';

    return $t('icon.lang');
  });

  function changeLang(lang: App.I18n.LangType) {
    emit('changeLang', lang);
  }
</script>

<template>
  <ADropdown placement="bottom">
    <ButtonIcon :tooltip-content="tooltipContent" tooltip-placement="left">
      <SvgIcon icon="heroicons:language" />
    </ButtonIcon>
    <template #overlay>
      <AMenu :selected-keys="[lang]">
        <AMenuItem v-for="option in langOptions" :key="option.key" @click="changeLang(option.key)">
          {{ option.label }}
        </AMenuItem>
      </AMenu>
    </template>
  </ADropdown>
</template>

<style scoped></style>
