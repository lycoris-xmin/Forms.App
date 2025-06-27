<script setup lang="ts">
import { reactive, ref, watch, withDefaults } from 'vue';
import { TreeSelect } from 'ant-design-vue';
import dayjs from 'dayjs';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { useChinAreaTree } from '@/hooks/custome/china-area-map';
import { fetchGetTaobaoProductSkuMap, fetchUpdatePlanTask } from '@/service/api';
import CheckAgreement from './check-agreement.vue';
import PlantTaskActions from './actions.vue';

defineOptions({
  name: 'PlanTaskEditDrawer'
});

interface Model extends Api.PlanTask.UpdatePlanTask {
  skuChangeVisible: boolean;
  skuMap: Array<Api.Product.ApiSkuMap>;
}

interface Props {
  rowData?: Api.Product.TaobaoData | null;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  submit: boolean;
};

const SHOW_PARENT = TreeSelect.SHOW_PARENT;

const { chinaArea } = useChinAreaTree();

const { formRef, validate, resetFields } = useAntdForm();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false,
  skuMap: []
});

const props = withDefaults(defineProps<Props>(), {
  rowData: () => {}
});

const emit = defineEmits<Emits>();

const model = ref<Model>(createDefaultModel());

const { defaultRequiredRule } = useFormRules();

const rules = {
  roleName: defaultRequiredRule
};

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();
  }
});

function createDefaultModel(): Model {
  return {
    id: '',
    skuList: [],
    count: 0,
    scope: 50,
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
    endTime: '',
    actions: [],
    skuChangeVisible: false
  };
}

async function initModelHandler() {
  model.value = createDefaultModel();

  if (props.rowData) {
    const rowData = {};
    Object.assign(rowData, {}, props.rowData);

    if (!rowData.actions) {
      rowData.actions = {
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
      };
    }

    Object.assign(model.value, rowData);

    model.value.beginTime = dayjs(model.value.beginTime);
    model.value.endTime = dayjs(model.value.endTime);

    getProductSkuMap();
  }
}

async function getProductSkuMap() {
  if (props.rowData.platform === 0) {
    const { data: res, error } = await fetchGetTaobaoProductSkuMap(model.value.productId);
    if (!error && res && res.code === 0) {
      model.value.skuMap = res.data.list;
    }
  }
}

function closeDrawer() {
  visible.value = false;
}

function changeSkuHandler() {
  model.value.skuChangeVisible = true;
}

function changeSkuClickHandler({ sku, price }) {
  model.value.skuList = [...sku];
  model.value.price = price;
  model.value.skuChangeVisible = false;
}

async function submitHandler() {
  await validate();

  const json: Api.PlanTask.UpdatePlanTask = {
    id: model.value.id,
    keyword: model.value.keyword,
    skuList: [...model.value.skuList],
    count: model.value.count,
    scope: model.value.scope,
    actions: model.value.actions,
    excludeArea: [...model.value.excludeArea],
    beginTime: dayjs(model.value.beginTime).format('YYYY-MM-DD HH:mm:ss'),
    endTime: dayjs(model.value.endTime).format('YYYY-MM-DD HH:mm:ss')
  };

  loadingMap.submit = true;
  try {
    const { data: res, error } = await fetchUpdatePlanTask(json);
    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}

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

  const beginTime = model.value && model.value.beginTime ? dayjs(model.value.beginTime) : null;

  if (currentDate.isBefore(today)) {
    return currentDate.isBefore(today);
  }

  if (beginTime !== null) {
    const maxDate = beginTime.add(3, 'days');
    if (currentDate.isBefore(beginTime)) {
      return true;
    } else if (currentDate.isAfter(maxDate)) {
      return true;
    }
  }

  return false;
}

function beginTimeChangeHandler(value) {
  if (model.value.endTime.isBefore(value)) {
    model.value.endTime = value.endOf('day');
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="编辑任务" :width="750">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <div class="product">
          <p class="title" @click="() => window.open(model.productUrl)">{{ model.title }}</p>
          <p class="shop-name">{{ model.shopName }}</p>
        </div>

        <AFormItem label="查询关键词" name="keyword">
          <AInput v-model:value="model.keyword" autocomplete="off" placeholder="查询关键词" />
        </AFormItem>

        <AFormItem label="商品SKU">
          <div v-if="model.skuList.length" class="sku">
            <div class="wrap gap v-center flex">
              <ATag v-for="sku in model.skuList" :key="sku">{{ sku }}</ATag>
            </div>
            <div class="v-center center flex">
              <span class="cny">{{ model.price }}</span>
            </div>
            <div class="v-center flex">
              <AInputNumber v-model:value="model.count" size="small" :controls="false" :min="1" :step="1">
                <template #addonAfter>
                  <span>件</span>
                </template>
              </AInputNumber>
            </div>

            <div class="v-center center flex">
              <AButton class="v-center del-btn flex" type="link" @click="changeSkuHandler">更换</AButton>
            </div>
          </div>
          <AAlert v-else message="该商品没有SKU" type="info" />
        </AFormItem>

        <AFormItem v-if="!model.skuList.length" label="商品价格" name="price">
          <AInputNumber v-model:value="model.price" :min="0" :max="99999" :controls="false" :precision="2">
            <template #addonAfter>
              <span>元</span>
            </template>
          </AInputNumber>
        </AFormItem>

        <AFormItem v-if="!model.skuList.length" label="购买件数" name="count">
          <AInputNumber v-model:value="model.count" :min="0" :max="99999" :controls="false" :precision="0">
            <template #addonAfter>
              <span>件</span>
            </template>
          </AInputNumber>
        </AFormItem>

        <AFormItem class="scope" label="价格浮动" name="scope">
          <AInputNumber
            id="inputNumber"
            v-model:value="model.scope"
            :min="0"
            :max="200"
            :controls="false"
            :precision="2"
          >
            <template #addonBefore>
              <span>±</span>
            </template>
            <template #addonAfter>
              <span>元</span>
            </template>
          </AInputNumber>
        </AFormItem>

        <PlantTaskActions v-model:actions="model.actions"></PlantTaskActions>

        <div class="grid-2 grid">
          <AFormItem label="开始时间" name="beginTime">
            <ADatePicker
              v-model:value="model.beginTime"
              show-time
              value-format="YYYY-MM-DD HH:mm:ss"
              placeholder=""
              :disabled-date="date => disabledDate(date, 0)"
              @change="beginTimeChangeHandler"
            />
          </AFormItem>
          <AFormItem label="结束时间" name="endTime">
            <ADatePicker
              v-model:value="model.endTime"
              show-time
              value-format="YYYY-MM-DD HH:mm:ss"
              placeholder=""
              :disabled-date="date => disabledDate(date, 1)"
            />
          </AFormItem>
        </div>

        <AFormItem label="排除地区" name="excludeArea">
          <ATreeSelect
            v-model:value="model.excludeArea"
            :tree-data="chinaArea"
            tree-checkable
            allow-clear
            :show-checked-strategy="SHOW_PARENT"
            search-placeholder="排除地区"
            placeholder="不排除"
          />
        </AFormItem>

        <AFormItem label="友情提示">
          <CheckAgreement :platform="props.rowData.platform"></CheckAgreement>
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

  <AModal v-model:open="model.skuChangeVisible" title="Sku选择" :footer="false" :mask-closable="false" width="650px">
    <ul class="sku-change-ul">
      <li class="header grid">
        <div class="v-center center flex">Sku组合</div>
        <div class="v-center center flex">价格</div>
        <div class="v-center center flex">选择</div>
      </li>
      <li v-for="item in model.skuMap" :key="item.skuMap" class="grid">
        <div class="v-center gap flex">
          <span v-for="map in item.sku" :key="map">{{ map }}</span>
        </div>
        <div class="v-center cny flex">{{ item.price }}</div>
        <div class="v-center center sku-change flex">
          <AButton
            size="small"
            :disabled="JSON.stringify(model.skuList) === JSON.stringify(item.sku)"
            @click="changeSkuClickHandler(item)"
          >
            更换
          </AButton>
        </div>
      </li>
    </ul>
  </AModal>
</template>

<style scoped lang="scss">
.body {
  .grid-2 {
    grid-template-columns: repeat(2, 1fr);
    gap: 15px;

    :deep(.ant-picker) {
      width: 100%;
    }
  }

  .product {
    margin-bottom: 14px;

    .title {
      font-size: 14px;
      font-weight: 600;
      letter-spacing: 1.5px;
      overflow: hidden;
      word-break: break-word;
      text-overflow: ellipsis;
      white-space: nowrap;
      transition: 0.3s ease-in-out;
      cursor: pointer;

      &:hover {
        color: var(--color-purple);
      }
    }

    .shop-name {
      font-size: 12px;
      font-weight: 500;
      color: var(--color-dark-light);
    }
  }

  .sku {
    padding: 8px;
    border: 1px solid var(--color-secondary);
    border-radius: 5px;
    display: grid;
    grid-template-columns: minmax(300px, 1fr) 100px 100px 60px;
    margin-bottom: 14px;

    > div {
      &:first-child {
        overflow: hidden;
        gap: 5px;

        :deep(.ant-tag) {
          text-overflow: ellipsis;
          overflow: hidden;
        }
      }
    }
  }

  .scope {
    .ant-input-number-group-wrapper {
      width: 100% !important;
    }
  }

  .actions {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 8px;

    li {
      list-style: none;
      margin-bottom: 14px;

      &:last-child {
        margin-bottom: 0;
      }
    }
  }
}

.sku-change-ul {
  padding: 18px 10;
  max-height: 60vh;
  overflow-x: hidden;
  overflow-y: auto;

  .grid {
    display: grid;
    grid-template-columns: minmax(300px, 1fr) 100px 100px;
    gap: 10px;
    margin-bottom: 14px;
    z-index: 1;

    &.header {
      position: sticky;
      top: 0;
      padding: 4px 0;
      background-color: var(--color-secondary);
      border-radius: 5px;
      z-index: 2;
    }

    .gap {
      gap: 10px;
    }

    .sku-change {
      .ant-btn {
        width: 80%;
      }
    }
  }
}
</style>
