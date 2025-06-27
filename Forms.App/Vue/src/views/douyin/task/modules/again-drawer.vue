<script setup lang="ts">
import { nextTick, reactive, ref, watch } from 'vue';
import useCalculate from '@/hooks/custome/calculate';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import ShopSelect from '@/views/shared/components/shopname-select/index.vue';
import { fetchAgainDouyinPlanTask, fetchGetPlanTaskAgainDetail } from '@/service/api';
import { platform as PLATFROM } from '@/views/shared/enums';
import LoadingWrap from '@/components/loading/index.vue';
import type { TaskData } from './types';
import FriendsSelect from './modules/friends-select.vue';
import DeviceSelect from './modules/device-select.vue';
import ProductSelect from './modules/again-product-select.vue';
import ProductActions from './modules/actions.vue';
import PlanTaskTimePicker from './modules/time-picker.vue';
import PlanTaskComment from './modules/again-product-comment.vue';
import PlantaskAgreement from './modules/agreement.vue';
import PlanTaskExcludeArea from './modules/exclude-area-select.vue';

interface Props {
  rowData?: Api.PlanTask.PlanTaskData | null;
  userInfo: object;
  mode?: Array<EnumAntd> | null;
  platform?: number | null;
}

interface Emits {
  (e: 'submitted'): void;
}

defineOptions({
  name: 'PlanTaskAgainDrawer'
});

const calculate = useCalculate();

const visible = defineModel<boolean>('visible', {
  default: false
});

const productSelectRef = ref<HTMLElement | null>(null);
const planTaskCommentRef = ref<HTMLElement | null>(null);
const planTaskTimePickerRef = ref<HTMLElement | null>(null);

const { formRef, validate, resetFields, validateFields } = useAntdForm();
const { defaultRequiredRule, createRequiredRule } = useFormRules();

const loadingModel = reactive({
  loading: false,
  spinLoading: false
});

const props: Props = withDefaults(defineProps<Props>(), {
  rowData: null,
  platform: 0,
  mode: () => []
});

const model: TaskData = ref(createDefaultModel());

const emit = defineEmits<Emits>();

const rules = {
  mode: defaultRequiredRule,
  shopId: createRequiredRule('请选择店铺'),
  count: defaultRequiredRule,
  price: defaultRequiredRule,
  pairScope: defaultRequiredRule,
  pairMultiple: defaultRequiredRule
};

watch(visible, async value => {
  if (value) {
    resetFields();
    // 初始化model
    model.value = createDefaultModel();

    if (props.rowData) {
      loadingModel.spinLoading = true;
      // loadingModel.loading = true;
      try {
        // 获取商品详情
        const { data: res, error } = await fetchGetPlanTaskAgainDetail(props.rowData.id);

        if (error || !(res && res.code === 0)) {
          closeDrawer();
          return;
        }

        // 赋值处理
        if (res.data.products && res.data.products.length) {
          model.value.multiple = res.data.products.length > 1;
        }

        Object.keys(model.value).forEach(key => {
          if (Object.keys(res.data).includes(key)) {
            if (key === 'products') {
              model.value.selectedProducts = res.data[key];
              productSelectRef.value.setSelectedProducts(res.data[key]);
            } else {
              model.value[key] = res.data[key];
            }
          }
        });
      } finally {
        loadingModel.spinLoading = false;
        // loadingModel.loading = false;
      }
    }

    nextTick(() => {
      window.scrollTo(0, 0);
      planTaskTimePickerRef.value.initDateRange();
    });
  } else {
    nextTick(() => {
      planTaskCommentRef.value.initComment();
      productSelectRef.value.initSelectedProducts();
    });
  }
});

function createDefaultModel(): TaskData {
  return {
    mode: 10,
    friendId: null,
    deviceId: null,
    shopId: '',
    multiple: false,
    multiplePlatform: [props.platform],
    products: [],
    price: 0,
    count: 1,
    pairScope: 0,
    calculatePairScope: 0,
    pairMultiple: 0,
    maxPairProductCount: 0,
    actions: {
      shopAround: {
        min: 3,
        max: 5
      },
      addCart: false,
      favorite: false,
      lookComment: true,
      lookPlantingGrass: true,
      lookAskEveryone: true,
      useFirstBrowse: true,
      useFindBrowse: true,
      useWaitForBuy: true
    },
    excludeArea: [],
    beginTime: '',
    endTime: ''
  };
}

function planTaskModeChangeHandler() {
  // 检测
}

function shopSelectChangeHandler() {
  model.value.products = [];
  nextTick(() => {
    productSelectRef.value.initSelectedProducts();
  });
}

function shopValidateHandler() {
  validateFields('shopId');
}

function calculateProductsTotalPrice() {
  if (!model.value.products.length) {
    model.value.price = 0;
  }

  const temp = calculate.createCalculator(0);
  for (const item of model.value.products) {
    const result = calculate.createCalculator(item.count).multiply(item.price).getResult();
    temp.add(result);
  }

  model.value.price = temp.multiply(model.value.count).getResult();

  if (model.value.pairScope === 0 || model.value.pairScope === model.value.calculatePairScope) {
    model.value.calculatePairScope = calculate.createCalculator(model.value.price).multiply(0.1).getResult();
    if (model.value.calculatePairScope < 10) {
      model.value.calculatePairScope = 10;
    }
    model.value.pairScope = model.value.calculatePairScope;
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  if (model.value.products.length === 0) {
    window.$message?.warning('请选择要下发任务的商品');
    return;
  }

  for (const item of model.value.products) {
    if (item.skuCount > 0 && item.skuList.length === 0) {
      window.$message?.warning(`${item.title} 未选择Sku`);
      return;
    } else if (item.main && !item.keyword) {
      window.$message?.warning(`${item.title} 商品关键词未填写`);
      return;
    } else if (item.price === 0) {
      window.$message?.warning(`${item.title} 商品价格必须大于0`);
      return;
    } else if (item.count === 0) {
      window.$message?.warning(`${item.title} 件数必须大于0`);
      return;
    } else if (item.scope === 0) {
      window.$message?.warning(`${item.title} 购买差额必须大于0`);
      return;
    }
  }
  const json = {
    oldTaskId: props.rowData.id,
    mode: model.value.mode,
    shopId: model.value.shopId,
    shopName: model.value.products[0].shopName,
    friendId: model.value.friendId,
    deviceId: model.value.deviceId,
    multiplePlatform: model.value.multiplePlatform,
    products: model.value.products.map(x => {
      let hasComment = x.comment?.hasComment || false;

      if (hasComment) {
        if (
          !x.comment.text &&
          (!x.comment.images || !x.comment.images.length) &&
          (!x.comment.videos || !x.comment.videos.length) &&
          (!x.comment.videoUrls || !x.comment.videoUrls.length) &&
          (!x.comment.imageUrls || !x.comment.imageUrls.length)
        ) {
          hasComment = false;
        }
      }

      const comment = hasComment
        ? {
            mode: x.comment.mode,
            text: x.comment.text || '',
            imageUrls: x.comment.imageUrls,
            images: x.comment.images?.map(y => y.file),
            videoUrls: x.comment.videoUrls,
            videos: x.comment.videos?.map(y => y.file)
          }
        : null;

      return {
        id: x.id,
        title: x.title,
        url: x.url,
        image: x.image,
        keyword: x.keyword,
        price: x.price,
        scope: x.scope,
        count: x.count,
        skuList: x.skuList,
        comment,
        main: x.main
      };
    }),
    pairScope: model.value.pairScope,
    price: model.value.price,
    count: model.value.count,
    maxPairProductCount: model.value.maxPairProductCount,
    actions: {
      addCart: model.value.actions.addCart,
      favorite: model.value.actions.favorite,
      lookAskEveryone: model.value.actions.lookAskEveryone,
      lookComment: model.value.actions.lookComment,
      lookPlantingGrass: model.value.actions.lookPlantingGrass,
      shopAround: { ...model.value.actions.shopAround },
      useFindBrowse: model.value.actions.useFindBrowse,
      useFirstBrowse: model.value.actions.useFirstBrowse,
      useWaitForBuy: model.value.actions.useWaitForBuy
    },
    excludeArea: [...model.value.excludeArea],
    beginTime: model.value.beginTime,
    endTime: model.value.endTime
  };
  const { data: res, error } = await fetchAgainDouyinPlanTask(json);

  loadingModel.loading = true;
  try {
    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      emit('submitted');
      closeDrawer();
    }
  } finally {
    loadingModel.loading = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="快速发布" :width="750" :push="false">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem name="mode" label="任务模式">
          <ASelect v-model:value="model.mode" disabled @change="planTaskModeChangeHandler">
            <ASelectOption v-for="item in props.mode" :key="item.value" :value="item.value">
              <p class="space-between v-center flex">
                <span>{{ item.label }}</span>

                <span
                  v-if="!props.userInfo.isSettlement && (item.value === 0 || item.value === 1)"
                  class="option-disabled"
                >
                  设置结算收款码后，才可以使用联网配对
                </span>
                <span v-else>
                  <span class="info">{{ props.mode.filter(x => x.value === item.value)[0].info }}</span>
                </span>
              </p>
            </ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem v-if="model.mode === 0 || model.mode === 1" name="multiplePlatform">
          <template #label>
            <p>
              匹配平台
              <small class="text-danger ml-1">* 可选择多平台匹配</small>
            </p>
          </template>
          <ASelect v-model:value="model.multiplePlatform" mode="multiple" placeholder="请选择平台">
            <ASelectOption v-for="item in PLATFROM" :key="item.value" :value="item.value">
              {{ item.label }}联盟
            </ASelectOption>
          </ASelect>
        </AFormItem>

        <!--指定好友配对-->
        <FriendsSelect v-if="model.mode === 1" v-model:value="model.friend"></FriendsSelect>
        <!--指定设备配对-->
        <DeviceSelect v-else-if="model.mode === 20" v-model:value="model.deviceId"></DeviceSelect>

        <AFormItem name="shopId" label="任务店铺">
          <ShopSelect
            v-model:value="model.shopId"
            :platform="2"
            :remote="true"
            :allow-clear="true"
            :show-search="true"
            @change="shopSelectChangeHandler"
          ></ShopSelect>
        </AFormItem>

        <!--商品选择-->
        <ProductSelect
          ref="productSelectRef"
          v-model:products="model.products"
          v-model:multiple="model.multiple"
          :shop-id="model.shopId"
          @shop-validate="shopValidateHandler"
          @submitted="calculateProductsTotalPrice"
        ></ProductSelect>

        <div class="v-center form-inline flex">
          <AFormItem name="count" label="任务单数">
            <AInputNumber
              v-model:value="model.count"
              :min="1"
              :max="100"
              :controls="false"
              :precision="0"
              @change="calculateProductsTotalPrice"
            >
              <template #addonAfter>
                <span>单</span>
              </template>
            </AInputNumber>
          </AFormItem>
          <AFormItem name="price">
            <template #label>
              <p>
                任务总价
                <small class="text-danger ml-1">* 系统自动计算无需填写</small>
              </p>
            </template>
            <AInputNumber v-model:value="model.price" :min="0" :max="99999" :controls="false" :precision="2" readonly>
              <template #addonAfter>
                <span>元</span>
              </template>
            </AInputNumber>
          </AFormItem>
        </div>

        <div v-if="model.mode === 0 || model.mode === 1" class="v-center form-inline flex">
          <AFormItem name="pairScope">
            <template #label>
              <p>
                配对差额
                <small class="text-danger ml-1">* 与好友任务的价格相差上下浮动值</small>
              </p>
            </template>
            <AInputNumber v-model:value="model.pairScope" :min="0" :controls="false" :precision="2">
              <template #addonBefore>
                <span>±</span>
              </template>
              <template #addonAfter>
                <span>元</span>
              </template>
            </AInputNumber>
          </AFormItem>
          <AFormItem name="maxPairProductCount">
            <template #label>
              <p>
                多链接匹配
                <small class="text-danger ml-1">*范围：0-5，为 0 不限制</small>
              </p>
            </template>
            <AInputNumber v-model:value="model.maxPairProductCount" :min="0" :max="5" :controls="false" :precision="0">
              <template #addonBefore>
                <span>允许最大</span>
              </template>
              <template #addonAfter>
                <span>链接数</span>
              </template>
            </AInputNumber>
          </AFormItem>
        </div>

        <!--商品互动-->
        <ProductActions v-model:actions="model.actions"></ProductActions>

        <PlanTaskComment ref="planTaskCommentRef" v-model:products="model.products"></PlanTaskComment>

        <PlanTaskExcludeArea v-model:value="model.excludeArea"></PlanTaskExcludeArea>

        <!--任务时间-->
        <PlanTaskTimePicker
          ref="planTaskTimePickerRef"
          v-model:begin-time="model.beginTime"
          v-model:end-time="model.endTime"
        ></PlanTaskTimePicker>

        <PlantaskAgreement :platform="0"></PlantaskAgreement>
      </AForm>
      <LoadingWrap :loading="loadingModel.spinLoading"></LoadingWrap>
    </div>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton
          type="primary"
          :disabled="loadingModel.spinLoading"
          :loading="loadingModel.loading"
          @click="submitHandler"
        >
          保存
        </AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.info {
  font-size: 12px;
  color: #8888ff;
}

.option-disabled {
  padding-left: 10px;
  font-size: 14px;
  font-weight: 600;
  color: var(--color-danger);

  &::before {
    content: '*';
    padding-right: 5px;
  }
}

.form-inline {
  :deep(.ant-form-item) {
    flex-shrink: 0;
    width: 50%;

    &:first-child {
      padding-right: 4px;
    }

    &:last-child {
      padding-left: 4px;
    }

    .ant-input-number-group-wrapper {
      width: 100%;
    }
  }
}
</style>
