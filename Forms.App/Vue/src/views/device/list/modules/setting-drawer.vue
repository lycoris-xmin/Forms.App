<script setup lang="ts">
import { computed, nextTick, reactive, ref, watch } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchUpdateDevice } from '@/service/api';
import LoadingWrap from '@/components/loading/index.vue';
import VNodes from '@/components/v-nodes/index.vue';
import { platform as PLATFORM } from '@/views/shared/enums';

defineOptions({
  name: 'DeviceSettingDrawer'
});

type Model = Api.Device.UpdateDevice & {
  status: number;
  loading: boolean;
  submitLoading: boolean;
  areaEnum: Array;
  newAreaName: string;
  groupEnum: Array;
  newGroupName: string;
  sectionEnum: Array;
  newSectionName: string;
};

type Props = {
  rowData: Api.Device.DeviceData;
  areaName: Array;
  groupName: Array;
  sectionName: Array;
  devieType: Array;
};

type Emits = {
  (e: 'submitted'): void;
};

const inputRef = ref<HTMLElement | null>(null);

const emit = defineEmits<Emits>();

const visible = defineModel<boolean>('visible', { default: false });

const props = defineProps<Props>();
const model = reactive<Model>({
  id: '',
  type: 0,
  name: '',
  supportedPlatforms: [],
  remark: '',
  status: 0,
  canUse: 1,
  areaName: '',
  groupName: '',
  sectionName: '',
  loading: true,
  submitLoading: false,
  areaEnum: [],
  groupEnum: [],
  sectionEnum: [],
  newAreaName: '',
  newGroupName: '',
  newSectionName: ''
});

const areaEnum = computed(() => {
  const enums = [...props.areaName, ...model.areaEnum];
  return Array.from(new Map(enums.map(item => [item.value, item])).values());
});

const groupEnum = computed(() => {
  const enums = [...props.groupName, ...model.groupEnum];
  return Array.from(new Map(enums.map(item => [item.value, item])).values());
});

const sectionEnum = computed(() => {
  const enums = [...props.sectionName, ...model.sectionEnum];
  return Array.from(new Map(enums.map(item => [item.value, item])).values());
});

const { formRef, validate, resetFields } = useAntdForm();
const { defaultRequiredRule, createRequiredRule } = useFormRules();

const rules = {
  name: defaultRequiredRule,
  type: defaultRequiredRule,
  canUse: defaultRequiredRule,
  supportedPlatforms: createRequiredRule('请至少选择一个平台')
};

watch(visible, value => {
  if (value) {
    resetFields();
    nextTick(async () => {
      initModelHandler();
    });
  }
});

function initModelHandler() {
  try {
    Object.keys(model).forEach(key => {
      if (Object.keys(props.rowData).includes(key)) {
        model[key] = props.rowData[key];
      }
    });

    if (model.status === 11) {
      model.canUse = 0;
    } else {
      model.canUse = 1;
    }
  } finally {
    model.loading = false;
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();
  model.submitLoading = true;
  try {
    const { data: res, error } = await fetchUpdateDevice({ ...model });
    if (!error && res && res.code === 0) {
      window.$message?.success('更新成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    model.submitLoading = false;
  }
}

function addAreaNameHandler(e) {
  e.preventDefault();

  model.newAreaName = model.newAreaName.trim();

  if (!model.areaEnum.filter(x => x.value === model.newAreaName).length) {
    model.areaEnum.push({
      label: model.newAreaName,
      value: model.newAreaName
    });
  }

  model.areaName = `${model.newAreaName}`;

  nextTick(() => {
    model.newAreaName = '';
    inputRef.value?.focus();
  });
}

function addGroupNameHandler(e) {
  e.preventDefault();

  model.newGroupName = model.newGroupName.trim();

  if (!model.groupEnum.filter(x => x.value === model.newGroupName).length) {
    model.groupEnum.push({
      label: model.newGroupName,
      value: model.newGroupName
    });
  }

  model.groupName = `${model.newGroupName}`;

  nextTick(() => {
    model.newGroupName = '';
    inputRef.value?.focus();
  });
}

function addSectionNameHandler(e) {
  e.preventDefault();

  model.newSectionName = model.newSectionName.trim();

  if (!model.sectionEnum.filter(x => x.value === model.newSectionName).length) {
    model.sectionEnum.push({
      label: model.newSectionName,
      value: model.newSectionName
    });
  }

  model.sectionName = `${model.newSectionName}`;

  nextTick(() => {
    model.newSectionName = '';
    inputRef.value?.focus();
  });
}
</script>

<template>
  <ADrawer v-model:open="visible" title="编辑设备" :width="500">
    <div class="body" :class="{ loading: model.loading }">
      <AForm
        ref="formRef"
        layout="vertical"
        :model="model"
        :rules="rules"
        :label-col="{
          span: 5,
          md: 7
        }"
      >
        <AFormItem label="设备名称" name="name">
          <AInput v-model:value="model.name" placeholder="请输入设备名称" :maxlength="30" show-count />
        </AFormItem>

        <AFormItem v-if="props.devieType.length > 1" label="设备类型" name="type">
          <ASelect v-model:value="model.type" placeholder="- 请选择 -">
            <ASelectOption v-for="item in props.devieType" :key="item.value" :value="item.value">
              {{ item.label }}
            </ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem label="支持平台" name="supportedPlatforms">
          <ASelect v-model:value="model.supportedPlatforms" mode="multiple" placeholder="- 请选择 -">
            <ASelectOption v-for="item in PLATFORM" :key="item.value" :value="item.value">
              {{ item.label }}联盟
            </ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem v-if="props.devieType.length > 1" label="设备区域" name="areaName">
          <ASelect v-model:value="model.areaName" placeholder="- 请选择 -">
            <ASelectOption v-for="item in areaEnum" :key="item.value" :value="item.value">
              {{ item.label }}
            </ASelectOption>

            <template v-if="props.rowData.type === 10" #dropdownRender="{ menuNode: menu }">
              <VNodes :vnodes="menu" />
              <ADivider class="mb-4px mt-4px" />
              <ASpace class="select-action">
                <AInput ref="inputRef" v-model:value="model.newAreaName" placeholder="新设备区域名称" />
                <AButton ghost size="small" type="primary" @click="addAreaNameHandler">
                  <template #icon>
                    <PlusOutlined />
                  </template>
                  新设备区域名称
                </AButton>
              </ASpace>
            </template>
          </ASelect>
        </AFormItem>

        <AFormItem v-if="props.devieType.length > 1" label="设备分组" name="groupName">
          <ASelect v-model:value="model.groupName" placeholder="- 请选择 -">
            <ASelectOption v-for="item in groupEnum" :key="item.value" :value="item.value">
              {{ item.label }}
            </ASelectOption>

            <template v-if="props.rowData.type === 10" #dropdownRender="{ menuNode: menu }">
              <VNodes :vnodes="menu" />
              <ADivider class="mb-4px mt-4px" />
              <ASpace class="select-action">
                <AInput ref="inputRef" v-model:value="model.newGroupName" placeholder="新分组名称" />
                <AButton ghost size="small" type="primary" @click="addGroupNameHandler">
                  <template #icon>
                    <PlusOutlined />
                  </template>
                  添加新分组
                </AButton>
              </ASpace>
            </template>
          </ASelect>
        </AFormItem>

        <AFormItem v-if="props.devieType.length > 1" label="设备小组" name="sectionName">
          <ASelect v-model:value="model.sectionName" placeholder="- 请选择 -">
            <ASelectOption v-for="item in sectionEnum" :key="item.value" :value="item.value">
              {{ item.label }}
            </ASelectOption>

            <template v-if="props.rowData.type === 10" #dropdownRender="{ menuNode: menu }">
              <VNodes :vnodes="menu" />
              <ADivider class="mb-4px mt-4px" />
              <ASpace class="select-action">
                <AInput ref="inputRef" v-model:value="model.newSectionName" placeholder="新小组名称" />
                <AButton ghost size="small" type="primary" @click="addSectionNameHandler">
                  <template #icon>
                    <PlusOutlined />
                  </template>
                  新小组名称
                </AButton>
              </ASpace>
            </template>
          </ASelect>
        </AFormItem>

        <AFormItem label="可用状态" name="canUse" :disabled="model.status === 100 || model.status === -1">
          <ASelect v-model:value="model.canUse">
            <ASelectOption :value="1">设备可用</ASelectOption>
            <ASelectOption :value="0">设备停用</ASelectOption>
          </ASelect>
        </AFormItem>
      </AForm>

      <LoadingWrap :loading="model.loading"></LoadingWrap>
    </div>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="model.submitLoading" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style lang="scss" scoped>
.form-label {
  gap: 15px;
}

.input-number {
  width: 100%;
}

.body {
  position: relative;

  &.loading {
    max-height: 70vh;
    overflow: hidden;
  }
}
</style>
