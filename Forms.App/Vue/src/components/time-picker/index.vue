<script setup lang="ts">
  import dayjs from 'dayjs';
  import { ref, watch } from 'vue';
  import type { AntdDesignTimePicker, AntdDesignTimePickerProps } from './types';

  defineOptions({
    name: 'AntdTimePicker'
  });

  const props = withDefaults(defineProps<AntdDesignTimePickerProps>(), {
    defaultValue: [dayjs('00:00:00', 'HH:mm:ss'), dayjs('23:59:59', 'HH:mm:ss')],
    format: 'YYYY-MM-DD HH:mm:ss',
    placeholder: ['开始时间', '结束时间']
  });

  const model = defineModel<AntdDesignTimePicker>('value', {
    beginTime: '',
    endTime: ''
  });

  const timeValue = ref<Array>([]);

  watch(model.value, (val: AntdDesignTimePicker) => {
    if (val.beginTime === '' && val.endTime === '') {
      timeValue.value = [];
    }
  });

  function changeHandler(_, dateString: string[]) {
    model.value.beginTime = dateString[0];
    model.value.endTime = dateString[1];
  }
</script>

<template>
  <ARangePicker
    v-model:value="timeValue"
    :show-time="{
      defaultValue: props.defaultValue
    }"
    :format="props.format"
    :placeholder="props.placeholder"
    @change="changeHandler"
  />
</template>

<style lang="scss" scoped></style>
