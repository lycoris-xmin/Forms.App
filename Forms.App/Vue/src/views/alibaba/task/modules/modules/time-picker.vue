<script setup lang="ts">
import dayjs from 'dayjs';

defineOptions({
  name: 'PlanTaskTimePicler'
});

const beginTime = defineModel<string>('beginTime', {
  default: ''
});

const endTime = defineModel<string>('endTime', {
  default: ''
});

function disabledDate(currentDate: dayjs, type) {
  const today = dayjs().startOf('day');
  if (type === 0) {
    const maxDate = today.add(7, 'days');
    if (currentDate.isBefore(today)) {
      return true;
    } else if (currentDate.isAfter(maxDate)) {
      return true;
    }

    return false;
  }

  if (currentDate.isBefore(today)) {
    return currentDate.isBefore(today);
  }

  const value = beginTime.value ? dayjs(beginTime) : null;

  if (value !== null) {
    const maxDate = value.add(3, 'days');
    if (currentDate.isBefore(beginTime)) {
      return true;
    } else if (currentDate.isAfter(maxDate)) {
      return true;
    }
  }

  return false;
}

function beginTimeChangeHandler(value) {
  if (!endTime.value || dayjs(endTime).isBefore(value)) {
    endTime.value = dayjs(value).endOf('day').format('YYYY-MM-DD HH:mm:ss');
  }
}

defineExpose({
  initDateRange: () => {
    const now = dayjs();
    beginTime.value = now.format('YYYY-MM-DD HH:mm:ss');
    endTime.value = now.endOf('day').format('YYYY-MM-DD HH:mm:ss');
  }
});
</script>

<template>
  <div class="grid-2 grid">
    <AFormItem label="开始时间" name="beginTime">
      <ADatePicker
        v-model:value="beginTime"
        show-time
        value-format="YYYY-MM-DD HH:mm:ss"
        placeholder=""
        :disabled-date="date => disabledDate(date, 0)"
        @change="beginTimeChangeHandler"
      />
    </AFormItem>
    <AFormItem label="结束时间" name="endTime">
      <ADatePicker
        v-model:value="endTime"
        show-time
        value-format="YYYY-MM-DD HH:mm:ss"
        placeholder=""
        :disabled-date="date => disabledDate(date, 1)"
      />
    </AFormItem>
  </div>
</template>

<style scoped>
.grid-2 {
  grid-template-columns: repeat(2, 1fr);
  gap: 15px;

  :deep(.ant-picker) {
    width: 100%;
  }
}
</style>
