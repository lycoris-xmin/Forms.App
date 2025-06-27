<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue';
import { debounce } from 'lodash-es';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchDebuggerDeviceCommand, fetchGetDeviceEnum } from '@/service/api';
import { commandType as COMMAND_TYPE } from '@/views/shared/enums';

defineOptions({
  name: 'AutoJsScriptOperateDrawer'
});

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  submit: boolean;
};

type Model = {
  deviceId: string;
  command: number;
  data: string;
  laseSearchFilter: string;
  deviceIdOptions: Array<any>;
};

const { formRef, validate, resetFields } = useAntdForm();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const command = computed(() => {
  return COMMAND_TYPE.filter(x => x.value !== 0);
});

const emit = defineEmits<Emits>();

const model = ref(createDefaultModel());

const { defaultRequiredRule } = useFormRules();

const rules = {
  deviceId: defaultRequiredRule,
  command: defaultRequiredRule,
  data: [
    {
      validator: (_, value) => {
        if (value.trim() !== '') {
          try {
            const obj = JSON.parse(value);
            if (!obj || !Object.keys(obj).length) {
              return Promise.reject(new Error('任务参数格式错误，不是json格式'));
            }
          } catch {
            return Promise.reject(new Error('任务参数格式错误，不是json格式'));
          }
        }

        return Promise.resolve();
      },
      trigger: 'change'
    }
  ]
};

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();

    refreshDeivceIdEnums();
  }
});

function createDefaultModel(): Model {
  return {
    deviceId: '',
    command: 1000,
    data: '',
    laseSearchFilter: '',
    deviceIdOptions: []
  };
}

function initModelHandler() {
  model.value = createDefaultModel();
}

const searchHandler = debounce(async val => {
  model.value.laseSearchFilter = val;
  const { data: res, error } = await fetchGetDeviceEnum(val);
  if (!error && res && res.code === 0) {
    if (res.data && res.data.list && res.data.list.length) {
      model.value.deviceIdOptions = res.data.list.map(x => {
        return {
          value: x.value,
          label: x.text
        };
      });

      if (model.value.deviceIdOptions.length === 1) {
        model.value.deviceId = model.value.deviceIdOptions[0].value;
      }
    }
  }
});

function refreshDeivceIdEnums() {
  searchHandler(model.value.laseSearchFilter);
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  loadingMap.submit = true;
  try {
    const { data: res, error } = await fetchDebuggerDeviceCommand({ ...model.value });
    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="调试指令" :width="500">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="执行指令" name="command">
          <ASelect v-model:value="model.command" placeholder="- 全部 -" :options="command" allow-clear></ASelect>
        </AFormItem>
        <AFormItem label="选择设备" name="command">
          <div class="v-center flex gap-5px">
            <ASelect
              v-model:value="model.deviceId"
              placeholder="- 请选择 -"
              show-search
              :default-active-first-option="false"
              :show-arrow="false"
              :filter-option="false"
              :options="model.deviceIdOptions"
              @search="searchHandler"
            >
              <template #notFoundContent>
                <p>没有找到可用设备</p>
              </template>
            </ASelect>
            <AButton @click="refreshDeivceIdEnums">刷新</AButton>
          </div>
        </AFormItem>
        <AFormItem label="任务参数" name="data">
          <ATextarea
            v-model:value="model.data"
            placeholder="请输入任务参数（json格式），若任务没有参数可不填"
            :auto-size="{ minRows: 6 }"
          />
        </AFormItem>
      </AForm>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="loadingMap.submit" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss"></style>
