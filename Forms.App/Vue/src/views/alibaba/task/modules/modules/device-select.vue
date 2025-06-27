<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { debounce } from 'lodash-es';
import { fetchGetDeviceEnum } from '@/service/api';

defineOptions({
  name: 'PlanTaskDeviceSelect'
});

const deviceId = defineModel<string>('value', {
  default: ''
});

const enums = reactive({
  lastSearchFilter: '',
  deviceIdOptions: []
});

const searchHandler = debounce(async val => {
  enums.lastSearchFilter = val;
  const { data: res, error } = await fetchGetDeviceEnum(val);
  if (!error && res && res.code === 0) {
    if (res.data && res.data.list && res.data.list.length) {
      enums.deviceIdOptions = res.data.list.map(x => {
        return {
          value: x.value,
          label: x.text
        };
      });

      if (enums.deviceIdOptions.length === 1) {
        deviceId.value = enums.deviceIdOptions[0].value;
      }
    }
  }
});
onMounted(() => {
  searchHandler('');
});
</script>

<template>
  <AFormItem name="deviceId">
    <template #label>
      <p>
        指定设备
        <small class="info">* 如未找到设备，请核对设备剩余使用数</small>
      </p>
    </template>
    <div class="v-center flex gap-8px">
      <ASelect
        v-model:value="deviceId"
        placeholder="- 请选择 -"
        show-search
        :default-active-first-option="false"
        :show-arrow="false"
        :filter-option="false"
        @search="searchHandler"
      >
        <ASelectOption v-for="item in enums.deviceIdOptions" :key="item.value" :value="item.value">
          {{ item.label }} ({{ item.value }})
        </ASelectOption>

        <template #notFoundContent>
          <p>没有可分配的设备</p>
        </template>
      </ASelect>
      <AButton @click="() => searchHandler(enums.lastSearchFilter)">刷新</AButton>
    </div>
  </AFormItem>
</template>

<style scoped>
.info {
  color: var(--color-danger);
}
</style>
