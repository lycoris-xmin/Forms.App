<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { fetchGetTenantFriendEnum } from '@/service/api';

type Props = {
  allowClear?: boolean;
  placeholder?: string;
};

const id = defineModel<string>('value', { default: '', required: true });
const options = ref<Array>([]);
const props = withDefaults(defineProps<Props>(), {
  allowClear: true,
  placeholder: '- 全部 -',
  tenantUser: false
});

const searchHandler = async (val: string) => {
  const { data: res, error } = await fetchGetTenantFriendEnum(val);
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
    v-model:value="id"
    :placeholder="props.placeholder"
    show-search
    :allow-clear="props.allowClear"
    @search="searchHandler"
  >
    <ASelectOption v-for="item in options" :key="item.value" :value="item.value">
      {{ `${item.text} (${item.value})` }}
    </ASelectOption>
  </ASelect>
</template>

<style scoped></style>
