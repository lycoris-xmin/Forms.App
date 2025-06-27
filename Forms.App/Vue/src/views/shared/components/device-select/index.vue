<script setup lang="ts">
import { ref } from 'vue';
import { fetchGetDeviceEnum } from '@/service/api';

type Props = {
  allowClear?: boolean;
  placeholder?: string;
};

const deviceId = defineModel<string>('value', { default: '', required: true });
const options = ref<Array>([]);
const props = withDefaults(defineProps<Props>(), {
  allowClear: true,
  placeholder: '- 全部 -'
});

const searchHandler = async (val: string) => {
  if (val) {
    const { data: res, error } = await fetchGetDeviceEnum(val);
    if (!error && res && res.code === 0) {
      options.value = res.data.list;
    }
  } else {
    options.value = [];
  }
};
</script>

<template>
  <ASelect
    v-model:value="deviceId"
    :placeholder="props.placeholder"
    show-search
    :filter-option="false"
    :allow-clear="props.allowClear"
    @search="searchHandler"
  >
    <ASelectOption v-for="item in options" :key="item.value" :value="item.value">
      {{ `${item.text} (${item.value})` }}
    </ASelectOption>
  </ASelect>
</template>

<style scoped></style>
