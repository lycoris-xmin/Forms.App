<script setup lang="tsx">
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeleteTenantUser, fetchGetTenantUserList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import { userStatus as USERSTATUS } from '@/views/shared/enums';
import SearchFilter from './modules/search-filter.vue';
import OperateDrawer from './modules/operate-drawer.vue';

const authStore = useAuthStore();

const { tableWrapperRef, scrollConfig } = useTableScroll();

const rolePermission = useRolePermission();

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
    return fetchGetTenantUserList(filter);
  },
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const {
  drawerVisible,
  editingData,
  operateType,
  checkedRowKeys,
  rowSelection,
  onBatchDeleted,
  onDeleted,
  handleAdd,
  handleEdit
} = useTableOperate(data, getData);

async function batchDeleteHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    const { data: res, error } = await fetchDeleteTenantUser(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteTenantUser([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

function getColumns() {
  const cols = [
    {
      key: 'email',
      dataIndex: 'email',
      title: '邮箱',
      align: 'center',
      width: 200
    },
    {
      key: 'nickName',
      dataIndex: 'nickName',
      title: '昵称',
      align: 'center',
      width: 200
    },
    {
      key: 'isTenantAdmin',
      dataIndex: 'isTenantAdmin',
      title: '权限',
      align: 'center',
      width: 120,
      customRender: ({ record }) => {
        return (
          <ATag color={record.isTenantAdmin ? 'purple' : 'processing'}>{record.isTenantAdmin ? '管理员' : '员工'}</ATag>
        );
      }
    }
  ];

  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'tenantName',
      dataIndex: 'tenantName',
      title: '所属商户',
      align: 'center',
      width: 200
    });
  }

  const otherCols = [
    {
      key: 'status',
      dataIndex: 'status',
      title: '状态',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        const map = USERSTATUS.filter(x => x.value === record.status)[0];
        return <ATag color={map.color}>{map.label}</ATag>;
      }
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: authStore.userInfo.isTenant ? '添加时间' : '注册时间',
      align: 'center',
      width: 200
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        return (
          <div class="flex-center gap-4px">
            {editButton(record)}
            {deleteButton(record)}
          </div>
        );
      }
    }
  ];
  return [...cols, ...otherCols];
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Platform.TenantUser.UPDATE)) {
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
  if (!rolePermission.hasOne(AppPermissions.Platform.TenantUser.DELETE)) {
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
      :status="USERSTATUS"
      :user-info="authStore.userInfo"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="员工列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="
            rolePermission.hasOne(AppPermissions.Platform.TenantUser.CREATE) &&
            authStore.userInfo.isTenant &&
            !authStore.userInfo.isTenantUser
          "
          :delete-btn="rolePermission.hasOne(AppPermissions.Platform.TenantUser.DELETE)"
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
        :status="USERSTATUS"
        @submitted="getDataByPage"
      ></OperateDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
:deep(.ant-tag-success) {
  .cny {
    color: #52c41a;
  }
}

:deep(.ant-tag-error) {
  .cny {
    color: #f5222d;
  }
}
</style>
