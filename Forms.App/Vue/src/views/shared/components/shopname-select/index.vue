<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { fetchDouyinShopName, fetchGetALIShopName, fetchGetTaobaoShopName } from '@/service/api';

type Props = {
  platform?: number;
  allowClear?: boolean;
  placeholder?: string;
  showSearch?: boolean;
  options?: Array<any>;
  filterOption?: (inputValue: string, option: any) => boolean;
  remote?: boolean;
};

const value = defineModel<string>('value', { default: '', required: true });

const selectOptions = ref<Array>([]);

const props = withDefaults(defineProps<Props>(), {
  platform: 0,
  allowClear: true,
  placeholder: '- 全部 -',
  showSearch: false,
  options: () => [],
  filterOption: () => void 0,
  remote: false
});

const searchHandler = async (val: string) => {
  if (!props.showSearch) {
    return;
  }

  let res;
  let error;
  if (props.platform === 0) {
    ({ data: res, error } = await fetchGetTaobaoShopName(val));
  } else if (props.platform === 1) {
    ({ data: res, error } = await fetchGetALIShopName(val));
  } else if (props.platform === 2) {
    ({ data: res, error } = await fetchDouyinShopName(val));
  }

  if (!error && res && res.code === 0) {
    selectOptions.value = res.data.list;
  }
};

onMounted(() => {
  if (props.remote) {
    searchHandler('');
  }
});

function KeywordFilterOption(input: string, option: any) {
  return (
    props.options
      .filter(x => x.text.toLocaleLowerCase().includes(input.toLocaleLowerCase()))
      .filter(x => x.value === option.value).length > 0
  );
}
</script>

<template>
  <ASelect
    v-if="!props.remote"
    v-model:value="value"
    :placeholder="props.placeholder"
    :show-search="props.showSearch"
    :allow-clear="props.allowClear"
    :filter-option="props.filterOption || KeywordFilterOption"
  >
    <ASelectOption v-for="item in props.options" :key="item.value" :value="item.value">
      {{ item.text }}
    </ASelectOption>
  </ASelect>
  <ASelect
    v-else
    v-model:value="value"
    :placeholder="props.placeholder"
    :show-search="props.showSearch"
    :allow-clear="props.allowClear"
    :filter-option="!props.remote"
    @search="searchHandler"
  >
    <ASelectOption v-for="item in selectOptions" :key="item.value" :value="item.value">
      {{ item.text }}
    </ASelectOption>
  </ASelect>
</template>

<style scoped></style>
