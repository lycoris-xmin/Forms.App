<script setup lang="ts">
import { computed, nextTick, reactive, ref, watch, withDefaults } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchCreateAlipayLovePay, fetchUpdateAlipayLovePay } from '@/service/api';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';

defineOptions({
  name: 'RoleOperateDrawer'
});

interface Props {
  operateType?: AntDesign.TableOperateType;
  alipayLovePay?: Array<object>;
  rowData?: Api.AlipayLovePay.AlipayLovePayData | null;
  userInfo?: any;
  shopNameEnum?: Array<object>;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  submit: boolean;
};

const { formRef, validate, resetFields } = useAntdForm();

const inputRef = ref<HTMLElement | null>(null);

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  alipayLovePay: () => [],
  rowData: null,
  userInfo: () => {},
  shopNameEnum: () => []
});

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增亲情卡',
    edit: '编辑亲情卡'
  };
  return titles[props.operateType];
});

const isEdit = computed<boolean>(() => {
  return props.operateType === 'edit';
});

const model = ref(createDefaultModel());

const { defaultRequiredRule } = useFormRules();

const rules = {
  alipayName: defaultRequiredRule,
  alipayAccount: defaultRequiredRule,
  shopName: [{ required: true, message: '请选择绑定的店铺' }]
};

watch(visible, () => {
  if (visible.value) {
    resetFields(); // 先重置再赋值
    initModelHandler();
  }
});

function createDefaultModel(): Api.AlipayLovePay.AlipayLovePayUpdate {
  return {
    id: '',
    alipayLovePayType: props.userInfo.isTenantUser ? 2 : 1,
    alipayAccount: '',
    alipayName: '',
    shopName: '',
    tenantUser: '',
    shopNameEnum: props.shopNameEnum,
    newShopName: ''
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (isEdit.value) {
    if (props.rowData) {
      if (props.rowData.alipayLovePayType === 1) {
        model.value.shopName = props.rowData.createUserAndShop!;
      } else {
        model.value.tenantUser = props.rowData.createId!;
      }

      Object.keys(model.value).forEach(key => {
        if (Object.keys(props.rowData).includes(key)) {
          model.value[key] = props.rowData[key];
        }
      });
    }
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
      ? await fetchUpdateAlipayLovePay({ ...model.value })
      : await fetchCreateAlipayLovePay({ ...model.value });

    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}
async function alipayLovePayTypeChangeHandler() {
  model.value.shopName = '';
  model.value.tenantUser = '';
}

function addShopNameHandler(e) {
  e.preventDefault();
  if (!model.value.newShopName) {
    return;
  }
  model.value.newShopName = model.value.newShopName.trim();

  if (!model.value.shopNameEnum.filter(x => x.value === model.value.newShopName).length) {
    model.value.shopNameEnum.push({
      text: model.value.newShopName,
      value: model.value.newShopName
    });
  }

  model.value.shopName = `${model.value.newShopName}`;

  // 等待dom渲染完毕，在执行对应的方法
  nextTick(() => {
    model.value.newShopName = '';
    inputRef.value?.focus();
  });
}
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="550">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="情亲卡类型" name="alipayLovePay">
          <ASelect
            v-model:value="model.alipayLovePayType"
            :disabled="(props.userInfo.isTenant && props.userInfo.isTenantUser) || isEdit"
            @change="alipayLovePayTypeChangeHandler"
          >
            <ASelectOption v-for="item in props.alipayLovePay" :key="item.value" :value="item.value">
              {{ item.label }}
            </ASelectOption>
          </ASelect>
        </AFormItem>
        <AFormItem label="支付宝账号" name="alipayAccount">
          <AInput v-model:value="model.alipayAccount" autocomplete="off" placeholder="支付宝账号" />
        </AFormItem>
        <AFormItem label="亲情卡备注" name="alipayName">
          <AInput v-model:value="model.alipayName" autocomplete="off" placeholder="亲情卡备注" />
        </AFormItem>
        <AFormItem v-if="model.alipayLovePayType == 1" label="绑定店铺" name="shopName">
          <ASelect v-model:value="model.shopName" placeholder="请选择绑定">
            <ASelectOption v-for="item in model.shopNameEnum" :key="item.value" :value="item.text">
              {{ item.text }}
            </ASelectOption>

            <template #dropdownRender="{ menuNode: menu }">
              <VNodes :vnodes="menu" />
              <ADivider class="mb-4px mt-4px" />
              <ASpace class="select-action">
                <AInput ref="inputRef" v-model:value="model.newShopName" placeholder="新绑定店铺" />
                <AButton ghost size="small" type="primary" @click="addShopNameHandler">
                  <template #icon>
                    <PlusOutlined />
                  </template>
                  添加新绑定店铺
                </AButton>
              </ASpace>
            </template>
          </ASelect>
        </AFormItem>

        <AFormItem
          v-if="model.alipayLovePayType == 2 && props.userInfo.isTenant && !props.userInfo.isTenantUser"
          label="绑定员工"
          name="tenantUser"
        >
          <TenantSelect v-model:value="model.tenantUser" placeholder="- 全部 -" :tenant-user="true" />
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
.role-select {
  gap: 10px;
}

.select-action {
  padding: 4px 8px;
  display: flex;
  align-items: center;
  width: 100%;

  /* 使用 :deep 选择器，深入修改 Ant Design Vue 组件的内部样式（针对 .ant-space-item 类） */
  :deep(.ant-space-item) {
    /* 第一个 .ant-space-item（通常是输入框） */
    &:first-child {
      /* 设置 flex: 1，使其占用剩余空间，自动扩展宽度 */
      flex: 1;
    }

    /* 最后一个 .ant-space-item（通常是按钮） */
    &:last-child {
      /* 设置 flex-shrink: 0，防止按钮被压缩，保持其固有宽度 */
      flex-shrink: 0;
    }

    /* 针对 .ant-space-item 内的 .ant-btn 元素（按钮） */
    .ant-btn {
      /* 使用 flex 布局，使按钮内容水平和垂直居中 */
      display: flex;
      /* 垂直居中对齐 */
      align-items: center;
      /* 水平居中对齐 */
      justify-content: center;
    }
  }
}
</style>
