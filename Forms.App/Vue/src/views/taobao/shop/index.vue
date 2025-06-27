<script setup lang="tsx">
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeleteTaobaoShop, fetchGetTaobaoShopList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import SearchFilter from './modules/search-filter.vue';
import OperateDrawer from './modules/operate-drawer.vue';

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
  apiFn: fetchGetTaobaoShopList,
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

async function batchDeleteHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    const { data: res, error } = await fetchDeleteTaobaoShop(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteTaobaoShop([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

function getColumns() {
  const cols = [
    {
      key: 'shopName',
      dataIndex: 'shopName',
      title: '店铺名称',
      width: 300
    },
    {
      key: 'count',
      dataIndex: 'count',
      title: '商品数量',
      width: 150
    }
  ];

  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'tenantUser',
      dataIndex: 'tenantUser',
      title: '所属商户',
      width: 200
    });
  }

  cols.push({
    key: 'createTime',
    dataIndex: 'createTime',
    title: '添加时间',
    align: 'center',
    width: 200
  });

  cols.push({
    key: 'operate',
    title: '操作',
    align: 'center',
    width: 130,
    customRender: ({ record }) => {
      return (
        <div class="flex-center gap-4px">
          {editButton(record)}
          {deleteButton(record)}
        </div>
      );
    }
  });

  return cols;
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Taobao.Shop.UPDATE)) {
    return '';
  } else if (record.roleId === 1000) {
    return '';
  }

  return (
    <ATooltip placement="top" title="编辑">
      <AButton size="small" type="link" onClick={() => handleEdit(record.id)}>
        <EditOutlined />
      </AButton>
    </ATooltip>
  );
}

function deleteButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Taobao.Shop.DELETE)) {
    return '';
  } else if (record.roleId === 1000) {
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
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="评价列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.Taobao.Shop.CREATE)"
          :delete-btn="rolePermission.hasOne(AppPermissions.Taobao.Shop.DELETE)"
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
      />

      <OperateDrawer
        v-model:visible="drawerVisible"
        :operate-type="operateType"
        :row-data="editingData"
        @submitted="getDataByPage"
      ></OperateDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped></style>
