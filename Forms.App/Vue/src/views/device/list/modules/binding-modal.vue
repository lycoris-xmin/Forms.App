<script setup lang="ts">
import { nextTick, ref, watch } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { useAntdForm } from '@/hooks/common/form';
import {
  fetchAutomaticNetworking,
  fetchGetDeviceAreaName,
  fetchGetDeviceGroup,
  fetchGetDeviceSection
} from '@/service/api';
import VNodes from '@/components/v-nodes/index.vue';

type Model = {
  dataFlow: string;
  newGroupName: string;
  groupName: string;
  areaName: string;
  newAreaName: string;
  sectionName: string;
  newSectionName: string;
  count: number;
  current: number;
  areaEnum: Array;
  groupEnum: Array;
  sectionEnum: Array;
};

type Emits = {
  (e: 'submitted', payload: { areaName: string; groupName: string; sectionName: string }): void;
};

const areaInputRef = ref<HTMLElement | null>(null);
const groupInputRef = ref<HTMLElement | null>(null);
const inputRef = ref<HTMLElement | null>(null);

const visible = defineModel<boolean>('visible', {
  default: false
});

const steps = [
  {
    title: '设备信息',
    key: '设备信息'
  },
  {
    title: '设备注册条件',
    key: '设备注册条件'
  },
  {
    title: '绑定数量',
    key: '绑定数量'
  }
];

const dataFlowOptions = [
  {
    value: 1,
    label: '已连接WIFI或有手机流量'
  },
  {
    value: 0,
    label: '未连接WIFI，无手机流量'
  }
];

const model = ref(createDefaultModel());

const { formRef, resetFields, validate } = useAntdForm();

const emit = defineEmits<Emits>();

const rules = {
  dataFlow: [{ required: true, message: '请选择设备信息' }],
  areaName: [{ required: true, message: '请选择设备区域' }],
  groupName: [{ required: true, message: '请选择设备分组' }],
  sectionName: [{ required: true, message: '请选择设备小组' }],
  count: [{ required: true, message: '请输入本次绑定子机数量' }]
};

watch(visible, async value => {
  if (value) {
    resetFields();
    model.value = createDefaultModel();

    const { data: resgroup, errorgroup } = await fetchGetDeviceGroup();
    if (!errorgroup && resgroup && resgroup.code === 0) {
      if (resgroup.data.list && resgroup.data.list.length) {
        model.value.groupEnum = resgroup.data.list.map(x => {
          return {
            label: x,
            value: x
          };
        });
        if (!model.value.groupEnum) {
          model.value.groupEnum = [];
        }
      }
    }

    const { data: resarea, errorarea } = await fetchGetDeviceAreaName();
    if (!errorarea && resarea && resarea.code === 0) {
      if (resarea.data.list && resarea.data.list.length) {
        model.value.areaEnum = resarea.data.list.map(x => {
          return {
            label: x,
            value: x
          };
        });
        if (!model.value.areaEnum) {
          model.value.areaEnum = [];
        }
      }
    }

    const { data: ressection, errorsection } = await fetchGetDeviceSection();
    if (!errorsection && ressection && ressection.code === 0) {
      if (ressection.data.list && ressection.data.list.length) {
        model.value.sectionEnum = ressection.data.list.map(x => {
          return {
            label: x,
            value: x
          };
        });

        if (!model.value.sectionEnum) {
          model.value.sectionEnum = [];
        }
      }
    }
  }
});

function createDefaultModel(): Model {
  return {
    dataFlow: '',
    newGroupName: '',
    groupName: '',
    areaName: '',
    newAreaName: '',
    newSectionName: '',
    sectionName: '',
    count: 1,
    current: 0,
    areaEnum: [],
    groupEnum: [],
    sectionEnum: []
  };
}

const next = async () => {
  await validate();

  model.value.current += 1;
  if (model.value.current === 1) {
    if (model.value.groupEnum.length === 1) {
      model.value.groupName = model.value.groupEnum[0].value;
    }

    if (model.value.areaEnum.length === 1) {
      model.value.areaName = model.value.areaEnum[0].value;
    }
  }
};

const prev = () => {
  model.value.current -= 1;
};

const submittedHandler = async () => {
  if (model.value.dataFlow === 1) {
    emit('submitted', {
      areaName: model.value.areaName,
      groupName: model.value.groupName,
      sectionName: model.value.sectionName
    });
    visible.value = false;
  } else {
    window.$modal?.confirm({
      title: '提示',
      content: '请前往需要绑定的设备中打开App。自动组网模式需等待空闲的主控设备，请耐心等待！',
      cancelText: '取消',
      okText: '确定',
      async onOk() {
        await automaticNetworking();
      }
    });
  }
};

async function automaticNetworking() {
  const { data: res, error } = await fetchAutomaticNetworking({ ...model.value });
  if (!error && res) {
    if (res.code === 0) {
      window.$message?.success('指令下发成功,请耐心等候');
      visible.value = false;
    }
  }
}

function addAreaNameHandler(e) {
  e.preventDefault();

  model.value.newAreaName = model.value.newAreaName.trim();

  if (!model.value.areaEnum.filter(x => x.value === model.value.newAreaName).length) {
    model.value.areaEnum.push({
      label: model.value.newAreaName,
      value: model.value.newAreaName
    });
    model.value.areaName = `${model.value.newAreaName}`;
  } else {
    window.$message?.warning('请不要重复添加');
    model.value.sectionName = '';
  }

  nextTick(() => {
    model.value.newAreaName = '';
    areaInputRef.value?.focus();
  });
}

function addGroupNameHandler(e) {
  e.preventDefault();

  model.value.newGroupName = model.value.newGroupName.trim();

  if (!model.value.groupEnum.filter(x => x.value === model.value.newGroupName).length) {
    model.value.groupEnum.push({
      label: model.value.newGroupName,
      value: model.value.newGroupName
    });
    model.value.groupName = `${model.value.newGroupName}`;
  } else {
    window.$message?.warning('请不要重复添加');
    model.value.sectionName = '';
  }

  nextTick(() => {
    model.value.newGroupName = '';
    groupInputRef.value?.focus();
  });
}

function addSectionNameHandler(e) {
  e.preventDefault();
  model.value.newSectionName = model.value.newSectionName.trim();

  if (!model.value.sectionEnum.filter(x => x.value === model.value.newSectionName).length) {
    model.value.sectionEnum.push({
      label: model.value.newSectionName,
      value: model.value.newSectionName,
      newOption: true
    });
    model.value.sectionName = `${model.value.newSectionName}`;
  } else {
    window.$message?.warning('请不要重复添加');
    model.value.sectionName = '';
  }

  nextTick(() => {
    model.value.newSectionName = '';
    inputRef.value?.focus();
  });
}
</script>

<template>
  <AModal v-model:open="visible" title="添加设备" :mask-closable="false" width="450px" :footer="null">
    <div>
      <ASteps class="step" size="small" :current="model.current" :items="steps"></ASteps>
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <div v-if="model.current === 0" class="steps-content">
          <AFormItem label="数据流量 / 无线连接" name="dataFlow">
            <ASelect v-model:value="model.dataFlow" placeholder="请根据绑定设备情况选择">
              <ASelectOption v-for="item in dataFlowOptions" :key="item.value" :value="item.value">
                {{ item.label }}
              </ASelectOption>
            </ASelect>
          </AFormItem>
        </div>
        <div v-if="model.current === 1" class="steps-content">
          <AFormItem label="设备区域" name="areaName">
            <ASelect v-model:value="model.areaName" placeholder="请选择设备区域">
              <ASelectOption v-for="item in model.areaEnum" :key="item.value" :value="item.value">
                {{ item.label }}
              </ASelectOption>

              <template v-if="model.dataFlow === 1" #dropdownRender="{ menuNode: menu }">
                <VNodes :vnodes="menu" />
                <ADivider class="mb-4px mt-4px" />
                <ASpace class="select-action">
                  <AInput ref="areaInputRef" v-model:value="model.newAreaName" placeholder="新区域名称" />
                  <AButton ghost size="small" type="primary" @click="addAreaNameHandler">
                    <template #icon>
                      <PlusOutlined />
                    </template>
                    添加区域
                  </AButton>
                </ASpace>
              </template>
            </ASelect>
          </AFormItem>

          <AFormItem label="设备分组" name="groupName">
            <ASelect v-model:value="model.groupName" placeholder="请选择设备分组">
              <ASelectOption v-for="item in model.groupEnum" :key="item.value" :value="item.value">
                {{ item.label }}
              </ASelectOption>

              <template v-if="model.dataFlow === 1" #dropdownRender="{ menuNode: menu }">
                <VNodes :vnodes="menu" />
                <ADivider class="mb-4px mt-4px" />
                <ASpace class="select-action">
                  <AInput ref="groupInputRef" v-model:value="model.newGroupName" placeholder="新分组名称" />
                  <AButton ghost size="small" type="primary" @click="addGroupNameHandler">
                    <template #icon>
                      <PlusOutlined />
                    </template>
                    添加分组
                  </AButton>
                </ASpace>
              </template>
            </ASelect>
          </AFormItem>
          <!-- v-if="model.current == steps.length - 1" -->
          <AFormItem v-if="model.dataFlow === 1" name="sectionName">
            <template #label>
              <p>
                设备小组
                <small class="text-danger">* 每台主机必须独立一个小组</small>
              </p>
            </template>
            <ASelect v-model:value="model.sectionName" placeholder="请选择设备小组">
              <ASelectOption
                v-for="item in model.sectionEnum"
                :key="item.value"
                :value="item.value"
                :disabled="!item.newOption"
              >
                {{ item.label }}
              </ASelectOption>

              <template v-if="model.dataFlow === 1" #dropdownRender="{ menuNode: menu }">
                <VNodes :vnodes="menu" />
                <ADivider class="mb-4px mt-4px" />
                <ASpace class="select-action">
                  <AInput ref="inputRef" v-model:value="model.newSectionName" placeholder="新设备小组" />
                  <AButton ghost size="small" type="primary" @click="addSectionNameHandler">
                    <template #icon>
                      <PlusOutlined />
                    </template>
                    添加小组
                  </AButton>
                </ASpace>
              </template>
            </ASelect>
          </AFormItem>
        </div>
        <div v-if="model.current === 2" class="steps-content">
          <AFormItem v-if="model.dataFlow === 0" label="子机设备数量" name="count">
            <AInputNumber
              v-model:value="model.count"
              class="count"
              :min="1"
              :controls="false"
              :precision="0"
            ></AInputNumber>
          </AFormItem>
          <p v-else>
            <AAlert message="主机设备无须填写数量" type="info" />
          </p>
        </div>
      </AForm>
      <div class="steps-action v-center flex">
        <AButton v-if="model.current > 0" @click="prev">上一步</AButton>
        <AButton v-if="model.current < steps.length - 1" type="primary" @click="next">下一步</AButton>
        <AButton v-if="model.current == steps.length - 1" type="primary" @click="submittedHandler">确定</AButton>
      </div>
    </div>
  </AModal>
</template>

<style scoped lang="scss">
.step {
  padding: 20px 0;
}

.step-contet {
  padding: 20px 0;
}

.steps-action {
  padding: 20px 0;
  justify-content: flex-end;
  gap: 10px;
}

.count {
  width: 100%;
}

.select-action {
  padding: 4px 8px;
  display: flex;
  align-items: center;
  width: 100%;

  :deep(.ant-space-item) {
    &:first-child {
      flex: 1;
    }

    &:last-child {
      flex-shrink: 0;
    }

    .ant-btn {
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }
}
</style>
