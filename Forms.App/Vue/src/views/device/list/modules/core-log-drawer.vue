<script setup lang="ts">
import { reactive, watch } from 'vue';
import dayjs from 'dayjs';
import { apiUrl, fetchGetCoreLogList, fetchUploadCoreLog } from '@/service/api';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import LoadingWrap from '@/components/loading/index.vue';
import { downloadFile } from '@/utils/helper';

defineOptions({
  name: 'DeviceCoreLogDrawer'
});

interface Props {
  deviceId?: string;
}

interface CoreLog {
  fileName: string;
  size: string;
  url: string;
  createTime: string;
}

interface Model {
  loading: boolean;
  id: stirng;
  day: string;
  list: Array<CoreLog>;
  uploadModal: boolean;
}

const { formRef, resetFields, validate } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

const visible = defineModel<boolean>('visible', { default: false });

const props = withDefaults(defineProps<Props>(), {
  deviceId: ''
});

const model: Model = reactive({
  loading: false,
  id: '',
  day: dayjs('00:00:00', 'HH:mm:ss'),
  list: [],
  uploadModal: false
});

watch(visible, value => {
  //
  if (value) {
    resetFields();
    getCoreLogList();
  }
});

const rules = {
  day: defaultRequiredRule
};

async function getCoreLogList(refresh = false) {
  if (model.id === props.deviceId) {
    if (!refresh) {
      return;
    }
  }

  model.loading = true;
  try {
    const { data: res, error } = await fetchGetCoreLogList(props.deviceId);
    if (!error && res && res.code === 0) {
      model.list = res.data.list.map(x => {
        const url = window.$isDebugger ? `${apiUrl}${x.url}` : x.url;
        return {
          fileName: x.fileName,
          size: x.size,
          url,
          createTime: x.createTime
        };
      });
    }
  } finally {
    model.loading = false;
  }
}

function disabledDate(currentDate: dayjs) {
  const today = dayjs().startOf('day');
  return !currentDate.isBefore(today);
}

async function submitHandler() {
  await validate();
  //
  const { data: res, error } = await fetchUploadCoreLog(props.deviceId, dayjs(model.day).format('YYYY-MM-DD 00:00:00'));
  if (!error && res && res.code === 0) {
    window.$message?.success('指令已下发，等待设备上传');
    model.uploadModal = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="核心日志" :width="600">
    <div class="body">
      <p class="end v-center flex gap-8px">
        <AButton size="small" type="primary" ghost @click="() => getCoreLogList(true)">刷新列表</AButton>
        <AButton size="small" @click="() => (model.uploadModal = true)">上传日志</AButton>
      </p>
      <AList item-layout="horizontal" :data-source="model.list">
        <template #renderItem="{ item }">
          <AListItem>
            <AListItemMeta :description="`文件大小：${item.size} &nbsp; | &nbsp; 上传时间：${item.createTime}`">
              <template #title>
                <ATooltip placement="top" title="点击下载日志">
                  <span class="title" @click="downloadFile(item.url, item.fileName)">{{ item.fileName }}</span>
                </ATooltip>
              </template>
            </AListItemMeta>
          </AListItem>
        </template>
      </AList>
      <LoadingWrap :loading="model.loading"></LoadingWrap>
    </div>

    <AModal v-model:open="model.uploadModal" title="核心日志时间" @ok="submitHandler">
      <AForm ref="formRef" :model="model" layout="vertical" :rules="rules">
        <AFormItem label="日志日期" name="day">
          <ADatePicker
            v-model:value="model.day"
            value-format="YYYY-MM-DD"
            :disabled-date="date => disabledDate(date, 0)"
          />
        </AFormItem>
      </AForm>
    </AModal>
  </ADrawer>
</template>

<style scoped lang="scss">
.body {
  position: relative;
  min-height: 500px;
  width: 100%;

  .title {
    cursor: pointer;
    transition: all 0.3s ease-in-out;
    font-size: 16px;
    font-weight: 600;
    letter-spacing: 1.5px;

    &:hover {
      color: var(--color-purple);
    }
  }
}

:deep(.ant-picker) {
  width: 100%;
}
</style>
