<script setup lang="tsx">
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
import { onMounted, reactive } from 'vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { useAuthStore } from '@/store/modules/auth';
import { DeleteALIProduct, GetALIProductList, apiUrl, fetchGetALIShopName } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { productType as PRODUCT_TYPE } from '@/views/shared/enums';
import { imgFallback } from '@/data/empty.json';
import SearchFilter from './modules/search-filter.vue';
import OperateDrawer from './modules/operate-drawer.vue';

type Model = {
  shopNameEnum: Array<object>;
};

const authStore = useAuthStore();
const rolePermission = useRolePermission();

const { tableWrapperRef, scrollConfig } = useTableScroll();

const {
  columns,
  columnChecks,
  data,
  getData,
  getDataByPage,
  loading,
  mobilePagination,
  searchParams,
  resetSearchParams
} = useTable({
  apiFn: filter => {
    return GetALIProductList({ ...filter });
  },
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const {
  drawerVisible,
  operateType,
  editingData,
  handleAdd,
  handleEdit,
  checkedRowKeys,
  rowSelection,
  onBatchDeleted,
  onDeleted
} = useTableOperate(data, getData);

const model = reactive<Model>({
  shopNameEnum: []
});

onMounted(() => {
  getShopNameEnum();
});

async function getShopNameEnum(showToast = false) {
  try {
    const { data: res, error } = await fetchGetALIShopName();
    if (!error && res && res.code === 0) {
      model.shopNameEnum = res.data.list;
      if (showToast) {
        window.$message?.success('刷新列表成功');
      }
    }
  } catch {}
}

async function batchDeleteHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    const { data: res, error } = await DeleteALIProduct(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

async function deleteHandler(id: string) {
  window.$modal?.confirm({
    title: '提示',
    cancelText: '取消',
    okText: '确定',
    content: '为避免查看任务时找不到对应商品，系统将只会删除未发布过任务的商品！',
    async onOk() {
      const { data: res, error } = await DeleteALIProduct([id]);

      if (!error && res && res.code === 0) {
        onDeleted();
      }
    }
  });
}

function getColumns() {
  const cols = [
    {
      key: 'image',
      dataIndex: 'image',
      title: '商品主图',
      width: 100,
      customRender({ record }) {
        if (record.image.startsWith('http')) {
          return (
            <div style="height:64px;width:64px;border-radius:3px;overflow:hidden;">
              <AImage width={64} height={64} src={record.image} fallback={imgFallback} />
            </div>
          );
        }

        return (
          <div style="height:64px;width:64px;border-radius:3px;overflow:hidden;">
            <AImage
              width={64}
              height={64}
              src={window.$isDebugger ? `${apiUrl}${record.image}` : record.image}
              fallback={imgFallback}
            />
          </div>
        );
      }
    },
    {
      key: 'title',
      dataIndex: 'title',
      title: '商品名称',
      width: 400,
      customRender({ record }) {
        function renderItemId() {
          const productId = record.id || record.productCode || record.itemId;
          if (productId) {
            return <p>商品编号：{record.itemId}</p>;
          }
          return '';
        }

        if (record.url) {
          return (
            <div class="product-title">
              <a href={record.url} target="_blank">
                {record.title}
              </a>
              {renderItemId()}
            </div>
          );
        }

        return (
          <div class="product-title">
            <a>{record.title}</a>
            {renderItemId()}
          </div>
        );
      }
    },
    {
      key: 'shopName',
      dataIndex: 'shopName',
      title: '店铺名称',
      width: 250,
      ellipsis: true,
      customRender({ record }) {
        return <span class="shop-name">{record.shopName}</span>;
      }
    },
    {
      key: 'skuMap',
      dataIndex: 'skuMap',
      title: 'SKU组合数',
      align: 'center',
      width: 130,
      customRender({ record }) {
        return <ATag>{record.skuMap?.length || 0}个组合</ATag>;
      }
    }
  ];

  if (!authStore.userInfo.isTenant || !authStore.userInfo.isTenantUser) {
    cols.push({
      key: 'createUser',
      dataIndex: 'createUser',
      title: authStore.userInfo.isTenant ? '添加员工' : '所属商户',
      align: 'center',
      width: 130
    });
  }

  const colOther = [
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '添加时间',
      align: 'center',
      width: 200
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center',
      width: 100,
      customRender: ({ record }) => (
        <div class="flex-center gap-4px">
          {editButton(record)}
          {deleteButton(record)}
        </div>
      )
    }
  ];

  return cols.concat(colOther);
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Ali1688.Product.UPDATE)) {
    return '';
  }

  return (
    <ATooltip placement="top" title="编辑">
      <AButton size="small" type="link" disabled={record.syncStatus === 1} onClick={() => handleEdit(record.id)}>
        <EditOutlined />
      </AButton>
    </ATooltip>
  );
}

function deleteButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Ali1688.Product.DELETE)) {
    return '';
  }

  return (
    <APopconfirm title="确认删除吗？" onConfirm={() => deleteHandler(record.id)}>
      <ATooltip placement="top" title="删除">
        <AButton danger size="small" type="link">
          <DeleteOutlined />
        </AButton>
      </ATooltip>
    </APopconfirm>
  );
}
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :user-info="authStore.userInfo"
      :product-type="PRODUCT_TYPE"
      :shop-name-enum="model.shopNameEnum"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="商品列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.Ali1688.Product.CREATE)"
          :delete-btn="rolePermission.hasOne(AppPermissions.Ali1688.Product.DELETE)"
          @add="handleAdd"
          @delete="batchDeleteHandler"
        ></TableHeaderOperation>
      </template>

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

      <OperateDrawer
        v-model:visible="drawerVisible"
        :operate-type="operateType"
        :row-data="editingData"
        :product-type="PRODUCT_TYPE"
        :shop-name-enum="model.shopNameEnum"
        @refresh-shop-name-enum="showToast => getShopNameEnum(showToast)"
        @submitted="getDataByPage"
      ></OperateDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
.shop-name {
  color: #1890ff; // 蓝色
  font-weight: bold;
}

.sku-map {
  gap: 10px;

  .sku {
    &::after {
      content: ';';
      padding: 0 4px 0 0;
    }

    &:nth-last-child(2) {
      &::after {
        content: ';';
        padding-right: 10px;
      }
    }
  }

  .price {
    color: #ff6a00;

    &::before {
      content: '￥';
    }
  }
}

.h-full {
  :deep(td) {
    &:has(.product-title) {
      min-width: 150px;
    }

    .product-title {
      height: 64px;
      min-width: 150px;

      a {
        font-size: 14px;
        font-weight: 600;
      }

      p {
        font-size: 12px;
        color: var(--color-dark-light);
        padding-top: 10px;
      }
    }
  }
}
</style>
