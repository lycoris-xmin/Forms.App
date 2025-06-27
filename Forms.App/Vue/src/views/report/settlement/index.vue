<script setup lang="tsx">
import { reactive } from 'vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchGetPlanTaskSettlementList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import { settlementStatus as SETTLEMENTSTATUS } from '@/views/shared/enums';
import SearchFilter from './modules/search-filter.vue';
import SettlementDrawer from './modules/settlement-drawer.vue';

const authStore = useAuthStore();
const rolePermission = useRolePermission();

const { tableWrapperRef, scrollConfig } = useTableScroll();

const model = reactive({
  id: '',
  visible: false
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
    return fetchGetPlanTaskSettlementList(filter);
  },
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { checkedRowKeys, rowSelection } = useTableOperate(data, getData);

function getColumns() {
  const cols = [
    {
      key: 'payPrice',
      dataIndex: 'payPrice',
      title: authStore.userInfo.isTenant ? '我方支付金额' : '任务方支付金额',
      customRender: ({ record }) => {
        if (authStore.userInfo.isTenant) {
          const price = record.userId === authStore.userInfo.id ? record.payPrice : record.pairPayPrice;
          if (Number.parseFloat(price) > 0) {
            return <span class="cny">{price}</span>;
          }

          return <ATag color="error">支付失败</ATag>;
        }

        return <span class="cny">{record.payPrice}</span>;
      }
    }
  ];

  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'userName',
      dataIndex: 'userName',
      title: '任务方商户'
    });
  } else if (!authStore.userInfo.isTenantUser) {
    cols.push({
      key: 'userName',
      dataIndex: 'userName',
      title: '员工',
      customRender: ({ record }) => {
        if (record.userId === authStore.userInfo.id) {
          if (record.userName) {
            return record.userName;
          }
        } else if (record.pairUserName) {
          return record.pairUserName;
        }
        return '用户异常';
      }
    });
  }

  cols.push({
    key: 'pairPayPrice',
    dataIndex: 'pairPayPrice',
    title: authStore.userInfo.isTenant ? '对方支付金额' : '配对方支付金额',
    customRender: ({ record }) => {
      if (authStore.userInfo.isTenant) {
        const price = record.userId === authStore.userInfo.id ? record.pairPayPrice : record.payPrice;
        if (Number.parseFloat(price) > 0) {
          return <span class="cny">{price}</span>;
        }
        return <span class="cny">支付失败</span>;
      }
      return <span class="cny">{record.pairPayPrice}</span>;
    }
  });

  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'pairUserName',
      dataIndex: 'pairUserName',
      title: '配对方商户'
    });
  }

  const other = [
    {
      key: 'pairDiffPrice',
      dataIndex: 'pairDiffPrice',
      title: '支付差价',
      align: 'right',
      customRender: ({ record }) => {
        return <span class="cny">{record.pairDiffPrice}</span>;
      }
    },
    {
      key: 'status',
      dataIndex: 'status',
      title: '状态',
      align: 'center',
      width: 150,
      customRender: ({ record }) => {
        const map = SETTLEMENTSTATUS.filter(x => x.value === record.status)[0];
        return <ATag color={map.color}>{map.label}</ATag>;
      }
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '创建时间',
      align: 'center',
      width: 200
    },
    {
      key: 'settlementTime',
      dataIndex: 'settlementTime',
      title: '结算时间',
      align: 'center',
      width: 200
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        if (record.userId === authStore.userInfo.id || !authStore.userInfo.isTenant) {
          if (record.status === 0) {
            return <div class="flex-center gap-4px">{editButton(record)}</div>;
          }
        }
        return <ATag ghost>无可用操作</ATag>;
      }
    }
  ];

  return [...cols, ...other];
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Report.Settlement.UPDATE)) {
    return '';
  }

  return (
    <ATooltip placement="top" title="结算">
      <AButton size="small" type="link" onClick={() => auditHandler(record.id)}>
        <icon-cryptocurrency-cny />
      </AButton>
    </ATooltip>
  );
}

function auditHandler(id) {
  model.visible = true;
  model.id = id;
}
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :status="SETTLEMENTSTATUS"
      :user-info="authStore.userInfo"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="结算列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="false"
          :delete-btn="false"
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

      <SettlementDrawer
        :id="model.id"
        v-model:visible="model.visible"
        :is-admin="!authStore.userInfo.isTenant"
        @submitted="getDataByPage"
      ></SettlementDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
:deep(.cny) {
  &.none {
    color: var(--color-success) !important;
  }
}
</style>
