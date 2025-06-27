<script setup lang="tsx">
import { onMounted, reactive } from 'vue';
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeleteSystemUser, fetchGetRoleEnum, fetchGetSystemUserList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { userStatus as USER_STATUS } from '@/views/shared/enums';
import SearchFilter from './modules/search-filter.vue';
import OperateDrawer from './modules/operate-drawer.vue';

const rolePermission = useRolePermission();

const { tableWrapperRef, scrollConfig } = useTableScroll();

const enums = reactive({
  role: []
});

onMounted(async () => {
  await getRoleEnum();
});

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
  apiFn: fetchGetSystemUserList,
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
    const { data: res, error } = await fetchDeleteSystemUser(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteSystemUser([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

function getColumns() {
  const cols = [
    {
      key: 'email',
      dataIndex: 'email',
      title: '邮箱'
    },
    {
      key: 'nickName',
      dataIndex: 'nickName',
      title: '昵称'
    },
    {
      key: 'roleName',
      dataIndex: 'roleName',
      title: '角色',
      align: 'center'
    },
    {
      key: 'status',
      dataIndex: 'status',
      title: '状态',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        const map = USER_STATUS.filter(x => x.value === record.status)[0];
        return <ATag color={map.color}>{map.label}</ATag>;
      }
    },
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
      width: 130,
      customRender: ({ record }) => {
        const edit = editButton(record);
        const del = deleteButton(record);

        return (
          <div class="flex-center gap-4px">
            {edit}
            {del}
            {edit === '' && del === '' ? <ATag>无可用操作</ATag> : ''}
          </div>
        );
      }
    }
  ];

  return cols;
}

async function getRoleEnum() {
  const { data: res, error } = await fetchGetRoleEnum();
  if (!error && res && res.code === 0) {
    enums.role = (res.data.list || []).map(x => {
      return {
        value: x.value,
        label: x.text
      };
    });
  }
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.System.User.UPDATE)) {
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
  if (!rolePermission.hasOne(AppPermissions.System.User.DELETE)) {
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
      :role="enums.role"
      :status="USER_STATUS"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="用户列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.System.User.CREATE)"
          :delete-btn="rolePermission.hasOne(AppPermissions.System.User.DELETE)"
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
        :role="enums.role"
        :status="USER_STATUS"
        :refresh-role="getRoleEnum"
        @submitted="getDataByPage"
      ></OperateDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped></style>
