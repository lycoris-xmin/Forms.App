<script setup lang="ts">
import { computed, reactive, ref, watch, withDefaults } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchCreateTenant, fetchUpdateTenant } from '@/service/api';
import { REG_PWD } from '@/constants/reg';
import type { EnumMap } from '../../shared/shared';

defineOptions({
  name: 'TenantOperateDrawer'
});

interface Props {
  operateType?: AntDesign.TableOperateType;
  rowData?: Api.Product.TaobaoData | null;
  status?: Array<EnumMap>;
  userInfo: object;
  payType: Array;
  deviceType: Array;
  tenantType: Array;
  platformType: Array;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  roleRefresh: boolean;
  submit: boolean;
};

type Model = Pick<
  Api.Tenant.TenantData,
  'nickName' | 'password' | 'email' | 'status' | 'payType' | 'deviceType' | 'tenantType' | 'platformType'
> & {
  payType: Array;
  days: number;
  platformType: Array;
};

type RuleKey = Extract<
  keyof Model,
  'nickName' | 'password' | 'email' | 'status' | 'payType' | 'deviceType' | 'platformType'
>;

const { formRef, validate, resetFields } = useAntdForm();

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  rowData: null,
  status: () => []
});

const payType = computed(() => {
  const opt = [
    {
      label: 'TB联盟',
      children: props.payType.filter(x => x.value < 10)
    },
    {
      label: 'JD联盟',
      children: props.payType.filter(x => x.value >= 10 && x.value < 20)
    },
    {
      label: 'PDD联盟',
      children: props.payType.filter(x => x.value >= 20 && x.value < 30)
    },
    {
      label: 'DY联盟',
      children: props.payType.filter(x => x.value >= 30 && x.value < 40)
    },
    {
      label: '1688联盟',
      children: props.payType.filter(x => x.value >= 40 && x.value < 50)
    }
  ];

  return opt;
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增商户',
    edit: '编辑商户'
  };
  return titles[props.operateType];
});

const isEdit = computed<boolean>(() => {
  return props.operateType === 'edit';
});

const model = ref<Model>(createDefaultModel());

const rules = computed<Record<RuleKey, App.Global.FormRule[]>>(() => {
  const { formRules, defaultRequiredRule } = useFormRules();

  return {
    email: formRules.email,
    nickName: formRules.userName,
    password: isEdit.value
      ? [
          {
            validator(_, value) {
              if (value && value.trim() && !REG_PWD.test(value)) {
                return Promise.reject(new Error('密码格式不正确，6-18位字符，英文字母、数字及@#_!'));
              }

              return Promise.resolve();
            },
            trigger: 'blur'
          }
        ]
      : formRules.pwd,
    status: defaultRequiredRule,
    payType: defaultRequiredRule,
    deviceType: defaultRequiredRule,
    platfromType: defaultRequiredRule
  };
});

watch(visible, async () => {
  if (visible.value) {
    resetFields();
    initModelHandler();
  }
});

function createDefaultModel(): Model {
  return {
    id: '',
    email: '',
    password: '',
    nickName: '',
    tenantType: 0,
    status: 0,
    days: 0,
    payType: [0],
    deviceType: [10],
    platformType: [0]
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (isEdit.value) {
    if (props.rowData) {
      Object.keys(model.value).forEach(key => {
        if (props.rowData[key]) {
          model.value[key] = props.rowData[key];
        }
      });
    }
  }
}

function emailChangeHandler() {
  if (model.value.email) {
    model.value.email = model.value.email?.toLowerCase();
  }
}
function tenantTypeChangeHandler() {
  if (model.value.tenantType === 10 && model.value.days === 0) {
    model.value.days = 99999999;
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  loadingMap.submit = true;
  try {
    const { data: res, error } = isEdit.value
      ? await fetchUpdateTenant({ ...model.value })
      : await fetchCreateTenant({ ...model.value });

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
  <ADrawer v-model:open="visible" :title="title" :width="550">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="邮箱" name="email">
          <AInput v-model:value="model.email" autocomplete="off" placeholder="请输入邮箱" @blur="emailChangeHandler" />
        </AFormItem>

        <AFormItem label="密码" name="password">
          <AInput v-model:value="model.password" autocomplete="off" placeholder="请输入密码" type="password" />
        </AFormItem>

        <AFormItem label="昵称" name="nickName">
          <AInput v-model:value="model.nickName" autocomplete="off" placeholder="请输入昵称" />
        </AFormItem>

        <AFormItem v-if="!isEdit" label="商户类型" name="tenantType">
          <ASelect
            v-model:value="model.tenantType"
            :options="props.tenantType"
            @change="tenantTypeChangeHandler"
          ></ASelect>
        </AFormItem>

        <AFormItem v-else label="状态" name="status">
          <ASelect v-model:value="model.status" :options="props.status"></ASelect>
        </AFormItem>

        <AFormItem label="支付配置" name="payType">
          <ASelect v-model:value="model.payType" mode="multiple">
            <ASelectOptGroup v-for="item in payType" :key="item.label">
              <template #label>
                <ATag :color="item.children[0].color">{{ item.label }}</ATag>
              </template>
              <ASelectOption v-for="child in item.children" :key="child.value" :value="child.value">
                {{ child.label }}
              </ASelectOption>
            </ASelectOptGroup>
          </ASelect>
        </AFormItem>

        <AFormItem label="平台权限" name="platformType">
          <ASelect v-model:value="model.platformType" mode="multiple">
            <ASelectOption v-for="item in props.platformType" :key="item.value" :value="item.value">
              {{ item.label }}联盟
            </ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem label="设备配置" name="deviceType">
          <ASelect v-model:value="model.deviceType" mode="multiple">
            <ASelectOption v-for="item in props.deviceType" :key="item.value" :value="item.value">
              {{ item.label }}
            </ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem v-if="!props.userInfo.isTenant" label="预充天数" name="days">
          <AInputNumber
            v-model:value="model.days"
            class="input-number"
            addon-before="+"
            addon-after="天"
            :controls="false"
            :min="0"
            :max="999999999"
            :step="1"
            :precision="0"
          ></AInputNumber>
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

<style scoped lang="scss">
.input-number {
  width: 100%;
}
</style>
