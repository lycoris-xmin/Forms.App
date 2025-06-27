<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { fetchGetTenantEnum, fetchGetTenantUserEnum } from '@/service/api';

type Props = {
  allowClear?: boolean;
  placeholder?: string;
  tenantUser?: boolean;
};

const userId = defineModel<string>('value', { default: '', required: true });
const options = ref<Array>([]);
const props = withDefaults(defineProps<Props>(), {
  allowClear: true,
  placeholder: '- 全部 -',
  tenantUser: false
});

const searchHandler = async (val: string) => {
  const { data: res, error } = props.tenantUser ? await fetchGetTenantUserEnum(val) : await fetchGetTenantEnum(val);
  if (!error && res && res.code === 0) {
    options.value = res.data.list;
  }
};

onMounted(() => {
  searchHandler('');
});
</script>

<template>
  <ASelect
    v-model:value="userId"
    :placeholder="props.placeholder"
    show-search
    :filter-option="false"
    :allow-clear="props.allowClear"
    @search="searchHandler"
  >
    <ASelectOption v-for="item in options" :key="item.value" :value="item.value">
      {{ `${item.text} (${item.data})` }}
    </ASelectOption>
  </ASelect>
</template>

<style scoped></style>
