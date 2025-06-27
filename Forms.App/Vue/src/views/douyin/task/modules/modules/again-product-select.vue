<script setup lang="tsx">
import { nextTick, onMounted, ref, watch } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { fetchDouyinShopName, fetchGetTaobaoProductListForPlanTask } from '@/service/api';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { imgFallback } from '@/data/empty.json';
import LoadingWrap from '@/components/loading/index.vue';
import type { ProductData } from '../types';

interface Props {
  shopId?: string;
}

interface Emits {
  (e: 'submitted'): void;
  (e: 'shopValidate'): void;
}

const value = defineModel<Array<ProductData>>('products', {
  default: []
});

const multiple = defineModel<boolean>('multiple', {
  default: false
});

const model = ref(createDefaultModel());

const { tableWrapperRef, scrollConfig } = useTableScroll();

const { columns, data, getData, getDataByPage, loading, mobilePagination, searchParams, resetSearchParams } = useTable({
  apiFn: getProductList,
  apiParams: {
    platform: 0,
    shopId: '',
    url: '',
    title: '',
    itemId: '',
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { checkedRowKeys, rowSelection } = useTableOperate(data, getData);

const props = withDefaults(defineProps<Props>(), {
  shopId: ''
});

const emit = defineEmits<Emits>();

watch(
  () => model.value.visible,
  _value => {
    if (!_value) {
      remoSelectedKeys();
    }
  }
);

watch(checkedRowKeys, checkedKeys => {
  if (!multiple.value && checkedKeys.length > 1) {
    const last = checkedRowKeys.value.at(-1);
    checkedRowKeys.value.splice(0, checkedRowKeys.value.length, ...(last !== void 0 ? [last] : []));
    rowSelection.value.selectedRowKeys = [last];
    rowSelection.value.selectedRows = rowSelection.value.selectedRows.filter(x => x.id === last);
  }
});

watch(
  () => model.value.products,
  _value => {
    for (const item of _value) {
      const index = value.value.findIndex(x => x.id === item.id);
      if (index > -1) {
        value.value[index].skuList = item.selectedSkuMap ? JSON.parse(item.selectedSkuMap) : [];
        value.value[index].keyword = item.keyword;
        value.value[index].count = item.count;
        value.value[index].price = item.price;
        value.value[index].scope = item.scope;
        value.value[index].main = item.main;
      } else {
        value.value.push({
          id: item.id,
          shopName: item.shopName,
          url: item.url,
          image: item.image,
          title: item.title,
          keyword: item.keyword,
          skuCount: item.skuMap?.length || 0,
          skuList: item.selectedSkuMap ? JSON.parse(item.selectedSkuMap) : [],
          count: item.count,
          price: item.price,
          scope: item.scope,
          main: item.main,
          comment: item.comment
        });
      }
    }

    const ids = _value.map(x => x.id);

    value.value = value.value.filter(x => ids.includes(x.id));

    emit('submitted');
  },
  { deep: true }
);

onMounted(() => {
  getShopNameEnum();
});

function createDefaultModel() {
  return {
    visible: false,
    products: [],
    loading: true
  };
}

async function selectHandler() {
  if (props.shopId === '' || props.shopId === '0') {
    emit('shopValidate');
    return;
  }

  model.value.visible = true;
  try {
    if (searchParams.shopId !== props.shopId) {
      model.value.loading = true;
      searchParams.shopId = props.shopId;
      await getDataByPage(1);
    }
  } finally {
    if (model.value.loading) {
      model.value.loading = false;
    }
  }
}

function getProductList(
  searchFilter: Api.PlanTask.ProductSearchFilter
): App.Service.PageResponse<Api.PlanTask.ProductData> {
  if (!model.value.visible) {
    return Promise.resolve({
      code: 0,
      msg: '',
      data: {
        count: 0,
        list: [],
        pageIndex: 1,
        pageSize: 10
      }
    });
  }

  return fetchGetTaobaoProductListForPlanTask({ ...searchFilter });
}

function getColumns() {
  const cols = [
    {
      key: 'image',
      dataIndex: 'image',
      title: '商品主图',
      width: 80,
      customRender({ record }) {
        return (
          <div style="height:64px;width:64px;border-radius:3px;overflow:hidden;">
            <AImage width={64} height={64} src={record.image} fallback={imgFallback} />
          </div>
        );
      }
    },
    {
      key: 'title',
      dataIndex: 'title',
      title: '商品'
    },
    {
      key: 'shopName',
      dataIndex: 'shopName',
      title: '店铺',
      width: 200
    }
  ];

  return cols;
}

function urlInputBlurHandler() {
  if (!searchParams.url) {
    return;
  }

  const url = new URL(searchParams.url);

  searchParams.itemId = url.searchParams.get('id') || '';

  if (!searchParams.itemId) {
    window.$message?.warning('商品链接解析失败，请核对链接或手动填写商品Id');
    return;
  }

  searchParams.url = `${url.protocol}//${url.host}${url.pathname}?id=${model.value.itemId}`;
}

async function getShopNameEnum() {
  try {
    const { data: res, error } = await fetchDouyinShopName();
    if (!error && res && res.code === 0) {
      model.value.shopNameEnum = res.data.list;
    }
  } catch {}
}

function multipleChangeHandler() {
  nextTick(() => {
    if (!multiple.value && model.value.products.length > 1) {
      window.$message?.warning('只能保留一个商品，请删除其他商品');
      multiple.value = true;
    }
  });
}

function mainProductChangeHandler(index, item) {
  if (item.main) {
    model.value.products.forEach((_value, _index) => (_value.main = _index === index));
  } else if (model.value.products.filter(x => x.main).length === 0) {
    item.main = true;
  }
}

function selectedHandler() {
  const list = rowSelection.value.selectedRows.map((x, index) => {
    return {
      id: x.id,
      shopName: x.shopName,
      url: x.url,
      image: x.image,
      title: x.title,
      keyword: '',
      skuMap: x.skuMap.map(y => {
        return {
          sku: y.sku,
          price: y.price,
          value: JSON.stringify(y.sku),
          label: y.sku.join('+')
        };
      }),
      selectedSkuMap: null,
      skuList: [],
      count: 1,
      price: 0,
      scope: 10,
      main: index === 0
    };
  });

  if (multiple.value) {
    for (const item of list) {
      if (!model.value.products.filter(x => x.id === item.id).length) {
        item.main = model.value.products.filter(x => x.main).length === 0;
        model.value.products.push(item);
      }
    }
  } else if (model.value.products.length === 0 || list[0].id !== model.value.products[0].id) {
    model.value.products = [list[0]];
  }

  model.value.visible = false;
}

function skuChangeHandler(index) {
  const selected = model.value.products[index].skuMap.filter(
    x => x.value === model.value.products[index].selectedSkuMap
  );
  if (selected && selected.length) {
    model.value.products[index].price = selected[0].price;
  } else {
    model.value.products[index].price = 0.0;
  }
}

function deleteProductHander(index) {
  model.value.products = model.value.products.filter((_, i) => i !== index);

  if (model.value.products.length && !model.value.products.filter(x => x.main).length) {
    model.value.products[0].main = true;
  }
}

function remoSelectedKeys() {
  checkedRowKeys.value = [];
  rowSelection.value.selectedRowKeys = [];
  rowSelection.value.selectedRows = [];
}

function redirectToProductPage(url) {
  if (url) {
    window.open(url);
  } else {
    window.$message?.warning('该商品没有添加商品链接，无法跳转');
  }
}

defineExpose({
  initSelectedProducts: () => {
    model.value = createDefaultModel();
    remoSelectedKeys();
  },
  setSelectedProducts: (list = []) => {
    // resetFields();
    model.value.products = list.map(x => {
      return {
        id: x.id,
        shopName: x.shopName,
        url: x.url,
        image: x.image,
        title: x.title,
        keyword: x.keyword,
        skuMap: x.skuMap.map(y => {
          return {
            sku: y.sku,
            price: y.price,
            value: JSON.stringify(y.sku),
            label: y.sku.join('+')
          };
        }),
        selectedSkuMap: x.selectedSkuMap ? JSON.parse(x.selectedSkuMap) : null,
        skuList: x.skuList,
        count: x.count,
        price: x.price,
        scope: x.scope,
        main: x.main,
        comment: x.comment
      };
    });
  }
});
</script>

<template>
  <p class="space-between v-center mb-12px flex">
    <span>商品列表(已选 {{ model.products.length }} 个)</span>
    <ACheckbox v-model:checked="multiple" @change="multipleChangeHandler">多商品</ACheckbox>
  </p>

  <ul class="products">
    <li v-for="(item, index) in model.products" :key="item.id">
      <div class="v-center mb-6px flex gap-8px">
        <ACheckbox v-if="multiple" v-model:checked="item.main" @change="() => mainProductChangeHandler(index, item)">
          主商品
        </ACheckbox>
        <p class="title" @click="redirectToProductPage(item.url)">{{ item.title }}</p>
      </div>
      <div class="v-center flex gap-8px">
        <div class="products-image">
          <AImage :width="100" :height="100" :src="item.image" :fallback="imgFallback" />
        </div>
        <div class="products-body">
          <div class="sku">
            <ASelect
              v-if="item.skuMap.length"
              v-model:value="item.selectedSkuMap"
              :options="item.skuMap"
              placeholder="请选择商品Sku"
              @change="value => skuChangeHandler(index)"
            ></ASelect>
            <ATag v-else>该商品无Sku</ATag>
          </div>
          <div class="mb-6px">
            <AInput
              v-model:value="item.keyword"
              :placeholder="item.main ? '请输入搜索关键词' : '没有输入输入关键词则进店查找'"
            ></AInput>
          </div>
          <div class="v-center flex gap-8px">
            <div class="v-center num-container flex gap-8px">
              <AInputNumber v-model:value="item.price" :min="0" :max="99999" :controls="false" :precision="2">
                <template #addonBefore>
                  <span>价格</span>
                </template>
                <template #addonAfter>
                  <span>元</span>
                </template>
              </AInputNumber>
              <AInputNumber v-model:value="item.count" :min="1" :max="99999" :controls="false" :precision="0">
                <template #addonBefore>
                  <span>购买</span>
                </template>
                <template #addonAfter>
                  <span>件</span>
                </template>
              </AInputNumber>
              <AInputNumber v-model:value="item.scope" :min="0" :max="99999" :controls="false" :precision="2">
                <template #addonBefore>
                  <ATooltip placement="top">
                    <template #title>
                      <span>下单商品时商品单价与填写价格相差符合差额范围内才下单</span>
                    </template>
                    <span>浮动差额 ±</span>
                  </ATooltip>
                </template>
                <template #addonAfter>
                  <span>元</span>
                </template>
              </AInputNumber>
            </div>
            <div class="v-center del-btn flex gap-6px">
              <APopconfirm title="确认删除吗？" placement="left" @confirm="() => deleteProductHander(index)">
                <AButton danger size="small">删除</AButton>
              </APopconfirm>
            </div>
          </div>
        </div>
      </div>
    </li>
  </ul>

  <div v-if="multiple || model.products.length === 0" class="actions mb-12px" @click="selectHandler">
    <p class="text-center">
      <PlusOutlined />
      添加商品
    </p>
  </div>

  <ADrawer v-model:open="model.visible" title="商品选择" :width="800">
    <div class="page min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
      <AForm
        :model="searchParams"
        :label-col="{
          span: 5,
          md: 7
        }"
      >
        <ARow class="row" :gutter="[16, 16]" wrap>
          <ACol :span="12">
            <AFormItem label="商品链接" name="url" class="m-0">
              <AInput v-model:value="searchParams.url" placeholder="" @blur="urlInputBlurHandler" />
            </AFormItem>
          </ACol>
          <ACol :span="12">
            <AFormItem label="商品编号" name="itemId" class="m-0">
              <AInput v-model:value="searchParams.itemId" placeholder="" />
            </AFormItem>
          </ACol>
          <ACol :span="12">
            <AFormItem label="商品名称" name="title" class="m-0">
              <AInput v-model:value="searchParams.title" placeholder="" />
            </AFormItem>
          </ACol>

          <div class="flex-1">
            <AFormItem class="m-0">
              <div class="w-full flex-y-center justify-end gap-12px">
                <AButton @click="resetSearchParams()">
                  <template #icon>
                    <icon-ic-round-refresh class="align-sub text-icon" />
                  </template>
                  <span class="ml-8px">重置</span>
                </AButton>
                <AButton type="primary" ghost @click="getDataByPage(1)">
                  <template #icon>
                    <icon-ic-round-search class="align-sub text-icon" />
                  </template>
                  <span class="ml-8px">搜索</span>
                </AButton>
              </div>
            </AFormItem>
          </div>
        </ARow>
      </AForm>

      <ATable
        ref="tableWrapperRef"
        :columns="columns"
        :data-source="data"
        size="small"
        :row-selection="rowSelection"
        :scroll="scrollConfig"
        :loading="loading"
        row-key="id"
        :pagination="mobilePagination"
        class="h-full"
      ></ATable>

      <LoadingWrap :loading="model.loading"></LoadingWrap>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="() => (model.visible = false)">取消</AButton>
        <AButton type="primary" :disabled="checkedRowKeys.length === 0" @click="selectedHandler">确定</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.products {
  list-style: none;
  min-height: 20px;

  .title {
    overflow: hidden;
    word-break: break-all;
    white-space: nowrap;
    text-overflow: ellipsis;
    cursor: pointer;
    transition: all 0.3s ease-in-out;
    font-weight: 600;

    &:hover {
      color: #646cff;
    }
  }

  :deep(.ant-form-item-label) {
    > label {
      width: 100%;
    }

    .space-between {
      width: 100%;
    }
  }

  li {
    padding: 12px 0;
    margin-bottom: 6px;
    border-bottom: 1px solid #d9d9d9;
  }

  &-image {
    height: 100px;
    width: 100px;
    overflow: hidden;
    border-radius: 5px;
    flex-shrink: 0;
  }

  &-body {
    min-height: 100px;
    width: 100%;

    .sku {
      margin-bottom: 6px;

      :deep(.ant-select) {
        width: 100%;
      }
    }

    .del-btn {
      flex-shrink: 0;
    }
  }
}

.actions {
  padding: 6px 16px;
  cursor: pointer;
  background-color: #646cff;
  color: #fff;
  border: 1px solid #d9d9d9;
  border-radius: 5px;
}

.flex-1 {
  padding-right: 10px;
}

.page {
  position: relative;
  min-height: 70vh;
}
</style>
