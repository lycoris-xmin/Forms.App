<script setup lang="tsx">
  import { DeleteOutlined, EditOutlined, SettingFilled } from '@ant-design/icons-vue';
  import { reactive } from 'vue';
  import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
  import { AppPermissions, useRolePermission } from '@/hooks/custome/useRolePermissions';
  import { deleteRoleApi, getRoleListApi } from '@/service/api';
  import OperateDrawer from './modules/operate-drawer.vue';
  import PermissionDrawer from './modules/permission-drawer.vue';
  import SearchFilter from './modules/search-filter.vue';

  type Model = {
    id?: string | null;
    permissionVisable?: string | null;
  };

  const rolePermission = useRolePermission();

  const { tableWrapperRef, scrollConfig } = useTableScroll();

  const { columns, columnChecks, data, getData, loading, mobilePagination, getDataByPage, searchParams, resetSearchParams } = useTable({
    apiFn: getRoleListApi,
    apiParams: {
      pageIndex: 1,
      pageSize: 10
    },
    columns: getColumns
  });

  const { drawerVisible, operateType, editingData, handleAdd, handleEdit, checkedRowKeys, rowSelection, onBatchDeleted, onDeleted } = useTableOperate(data, getData);

  const model = reactive<Model>({
    id: '',
    permissionVisable: false
  });

  async function batchDeleteHandler() {
    const ids = checkedRowKeys.value;
    if (ids && ids.length) {
      const { data: res, error } = await deleteRoleApi(ids);
      if (!error && res && res.code === 0) {
        onBatchDeleted();
      }
    }
  }

  async function deleteHandler(id: string) {
    const { data: res, error } = await deleteRoleApi([id]);

    if (!error && res && res.code === 0) {
      onDeleted();
    }
  }

  function permissionHandler(id: string) {
    model.id = id;
    model.permissionVisable = true;
  }

  function getColumns() {
    const cols = [
      {
        key: 'roleName',
        dataIndex: 'roleName',
        title: '角色名称',
        align: 'center'
      },
      {
        key: 'count',
        dataIndex: 'count',
        title: '用户数量',
        align: 'center'
      },
      {
        key: 'operate',
        title: '操作',
        align: 'left',
        width: 130,
        customRender: ({ record }) => (
          <div class="v-center flex gap-8px">
            {editButton(record)}
            {permissionButton(record)}
            {deleteButton(record)}
          </div>
        )
      }
    ];

    return cols;
  }

  function permissionButton(record) {
    if (!rolePermission.hasOne(AppPermissions.System.Role.UPDATE)) {
      return '';
    }

    return (
      <ATooltip placement="top" title="权限设置">
        <AButton type="link" size="small" onClick={() => permissionHandler(record.id)}>
          <SettingFilled />
        </AButton>
      </ATooltip>
    );
  }

  function editButton(record) {
    if (!rolePermission.hasOne(AppPermissions.System.Role.UPDATE)) {
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
    if (!rolePermission.hasOne(AppPermissions.System.Role.DELETE)) {
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
    <SearchFilter v-model:model="searchParams" @reset="resetSearchParams" @search="getDataByPage"></SearchFilter>

    <ACard title="角色列表" :bordered="false" :body-style="{ flex: 1, overflow: 'hidden' }" class="flex-col-stretch sm:flex-1-hidden card-wrapper">
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.System.Role.CREATE)"
          :delete-btn="rolePermission.hasOne(AppPermissions.System.Role.DELETE)"
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

      <OperateDrawer v-model:visible="drawerVisible" :operate-type="operateType" :row-data="editingData" @submitted="getDataByPage"></OperateDrawer>

      <PermissionDrawer :id="model.id" v-model:visible="model.permissionVisable"></PermissionDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped></style>
