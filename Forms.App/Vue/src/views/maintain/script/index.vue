<script setup lang="tsx">
import { DeleteOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeleteJsScript, fetchGetJsScriptList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import OperateDrawer from './modules/operate-drawer.vue';
import SearchFilter from './modules/search-filter.vue';

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
  apiFn: fetchGetJsScriptList,
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { drawerVisible, operateType, editingData, handleAdd, checkedRowKeys, rowSelection, onBatchDeleted, onDeleted } =
  useTableOperate(data, getData);

async function batchDeleteHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    const { data: res, error } = await fetchDeleteJsScript(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteJsScript([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

function getColumns() {
  const cols = [
    {
      key: 'name',
      dataIndex: 'name',
      title: '脚本名称',
      width: 200,
      ellipsis: true
    },
    {
      key: 'version',
      dataIndex: 'version',
      title: '版本号',
      width: 150,
      align: 'center'
    },
    {
      key: 'size',
      dataIndex: 'size',
      title: '文件大小',
      align: 'center',
      width: 150
    },
    {
      key: 'remark',
      dataIndex: 'remark',
      title: '文件备注',
      minWidth: 150,
      ellipsis: true
    },
    {
      key: 'filePath',
      dataIndex: 'filePath',
      title: '文件位置',
      minWidth: 200,
      ellipsis: true
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '上传时间',
      align: 'center',
      width: 200
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center',
      width: 80,
      customRender: ({ record }) => {
        return deleteButton(record);
      }
    }
  ];

  return cols;
}

function deleteButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Maintain.AutoJsScript.DELETE)) {
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
    <SearchFilter v-model:model="searchParams" @reset="resetSearchParams" @search="getDataByPage(1)"></SearchFilter>

    <ACard
      title="脚本列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.Maintain.AutoJsScript.CREATE)"
          :delete-btn="rolePermission.hasOne(AppPermissions.Maintain.AutoJsScript.DELETE)"
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
