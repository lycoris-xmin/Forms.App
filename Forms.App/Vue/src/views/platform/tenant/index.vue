<script setup lang="tsx">
import { reactive } from 'vue';
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeleteTenant, fetchGetTenantList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import {
  deviceType as DEVICE_TYPE,
  payType as PAY_TYPE,
  platform as PLATFORM_TYPE,
  tenantType as TENANT_TYPE,
  userStatus as USER_STATUS
} from '@/views/shared/enums';
import SearchFilter from './modules/search-filter.vue';
import OperateDrawer from './modules/operate-drawer.vue';
import RechargeDrawer from './modules/recharge-drawer.vue';

const authStore = useAuthStore();

const { tableWrapperRef, scrollConfig } = useTableScroll();

const rolePermission = useRolePermission();

const model = reactive({
  rechargeVisible: false,
  tenantId: '',
  rowDetail: {},
  expandedRowKeys: []
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
  apiFn: filter => {
    model.expandedRowKeys = [];
    model.rowDetail = [];
    return fetchGetTenantList(filter);
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
    const { data: res, error } = await fetchDeleteTenant(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

function rechargeHandler(id: string) {
  model.tenantId = id;
  model.rechargeVisible = true;
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteTenant([id]);

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
      align: 'center'
    },
    {
      key: 'nickName',
      dataIndex: 'nickName',
      title: '昵称',
      align: 'center'
    },
    {
      key: 'tenantType',
      dataIndex: 'tenantType',
      title: '商户类型',
      align: 'center',
      customRender: ({ record }) => {
        const map = TENANT_TYPE.filter(x => x.value === record.tenantType)[0];
        return <ATag color={map.color}>{map.label}</ATag>;
      }
    },
    {
      key: 'payType',
      dataIndex: 'payType',
      title: '支付方式',
      customRender: ({ record }) => {
        return (
          <div style="display:flex;gap:5px;flex-wrap:wrap;">
            {record.payType.map(x => {
              const map = PAY_TYPE.filter(y => y.value === x)[0];
              return <ATag color={map.color}>{map.label}</ATag>;
            })}
          </div>
        );
      }
    },
    {
      key: 'platformType',
      dataIndex: 'platformType',
      title: '平台权限',
      customRender: ({ record }) => {
        return (
          <div style="display:flex;gap:5px;flex-wrap:wrap;">
            {record.platformType.map(x => {
              const map = PLATFORM_TYPE.filter(y => y.value === x)[0];
              return <ATag color={map.color}>{map.label}联盟</ATag>;
            })}
          </div>
        );
      }
    },
    {
      key: 'deviceType',
      dataIndex: 'deviceType',
      title: '设备类型',
      customRender: ({ record }) => {
        return (
          <div style="display:flex;gap:5px;flex-wrap:wrap;">
            {record.deviceType.map(x => {
              const map = DEVICE_TYPE.filter(y => y.value === x)[0];
              return <ATag color={map.color}>{map.label}</ATag>;
            })}
          </div>
        );
      }
    }
  ];

  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'deviceCount',
      dataIndex: 'deviceCount',
      title: '设备数',
      align: 'center',
      width: 150,
      customRender: ({ record }) => {
        return <ATag color={record.deviceCount > 0 ? 'success' : 'error'}>{record.deviceCount}</ATag>;
      }
    });

    cols.push({
      key: 'unUseDays',
      dataIndex: 'unUseDays',
      title: '剩余使用数',
      align: 'center',
      width: 150,
      customRender: ({ record }) => {
        return <ATag color={record.unUseDays > 0 ? 'success' : 'error'}>{record.unUseDays} 天</ATag>;
      }
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
        const map = USER_STATUS.filter(x => x.value === record.status)[0];
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
        if (!record.permission) {
          return rechargeButton(record);
        }
        return (
          <div class="flex-center gap-4px">
            {editButton(record)}
            {rechargeButton(record)}
            {deleteButton(record)}
          </div>
        );
      }
    }
  ];

  return [...cols, ...otherCols];
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Platform.Tenant.UPDATE)) {
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

function rechargeButton(record) {
  if (authStore.userInfo.isTenant) {
    return '';
  } else if (!rolePermission.hasOne(AppPermissions.Platform.Tenant.UPDATE)) {
    return '';
  }

  return (
    <ATooltip placement="top" title="使用">
      <AButton size="small" type="link" onClick={() => rechargeHandler(record.id)}>
        <icon-cryptocurrency-cny />
      </AButton>
    </ATooltip>
  );
}

function deleteButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Platform.Tenant.DELETE)) {
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
      :status="USER_STATUS"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="商户列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.Platform.Tenant.CREATE)"
          :delete-btn="rolePermission.hasOne(AppPermissions.Platform.Tenant.DELETE)"
          @add="handleAdd"
          @delete="batchDeleteHandler"
        ></TableHeaderOperation>
      </template>

      <ATable
        ref="tableWrapperRef"
        v-model:expanded-row-keys="model.expandedRowKeys"
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
        :status="USER_STATUS"
        :pay-type="PAY_TYPE"
        :device-type="DEVICE_TYPE"
        :user-info="authStore.userInfo"
        :tenant-type="TENANT_TYPE"
        :platform-type="PLATFORM_TYPE"
        @submitted="getDataByPage"
      ></OperateDrawer>

      <RechargeDrawer
        v-model:visible="model.rechargeVisible"
        :tenant-id="model.tenantId"
        @submitted="getDataByPage"
      ></RechargeDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
.expande-row {
  position: relative;
  padding: 15px;

  .item {
    .title {
      h1 {
        font-weight: 600;
        letter-spacing: 1.5px;
      }
    }

    .item-num {
      flex-shrink: 0;

      > div {
        &:first-child {
          content: '1';
          border-right: 1px solid var(--color-secondary);
          padding: 15px;
        }

        &:last-child {
          padding-left: 15px;
        }
      }

      p {
        text-align: center;

        &:first-child {
          font-weight: 600;
          font-size: 16px;
          line-height: 24px;
          letter-spacing: 1.5px;
        }

        &:last-child {
          font-size: 12px;
          color: var(--color-dark-light);
          letter-spacing: 1.5px;
        }
      }
    }
  }
}
</style>
