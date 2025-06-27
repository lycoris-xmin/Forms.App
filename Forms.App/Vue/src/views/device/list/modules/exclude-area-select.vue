<script setup lang="ts">
import { TreeSelect } from 'ant-design-vue';
import { useChinAreaTree } from '@/hooks/custome/china-area-map';

interface Props {
  label?: string;
  name?: string;
  allowClear?: boolean;
  placeholder?: string;
}

defineOptions({
  name: 'DeviceExcludeAreaSelect'
});

const SHOW_PARENT = TreeSelect.SHOW_PARENT;
const { chinaArea } = useChinAreaTree();

const value = defineModel<Array<string>>('value', {
  default: []
});

const props = withDefaults(defineProps<Props>(), {
  label: '设备区域',
  name: 'excludeArea',
  allowClear: true,
  placeholder: '选择设备区域'
});
</script>

<template>
  <AFormItem :label="props.label" :name="props.name">
    <ATreeSelect
      v-model:value="value"
      :tree-data="chinaArea"
      tree-checkable
      :allow-clear="allowClear"
      :show-checked-strategy="SHOW_PARENT"
      :search-placeholder="props.allowClear"
      :placeholder="props.placeholder"
    />
  </AFormItem>
</template>

<style scoped></style>
