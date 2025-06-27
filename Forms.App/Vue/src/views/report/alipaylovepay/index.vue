<script setup lang="tsx">
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchGetAlipayLovePayRecordList } from '@/service/api';
import { useAuthStore } from '@/store/modules/auth';
import { platform as PLATFORM } from '@/views/shared/enums';
import SearchFilter from './modules/search-filter.vue';

const authStore = useAuthStore();

const { tableWrapperRef, scrollConfig } = useTableScroll();

const {
  columns,
  columnChecks,
  searchParams,
  resetSearchParams,
  data,
  getData,
  getDataByPage,
  loading,
  mobilePagination
} = useTable({
  apiFn: fetchGetAlipayLovePayRecordList,
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
      key: 'alipayName',
      dataIndex: 'alipayName',
      title: '亲情卡',
      align: 'center',
      customRender: ({ record }) => {
        if (record.alipayName) {
          if (record.alipayAccount) {
            return (
              <span>
                {record.alipayAccount}({record.alipayName})
              </span>
            );
          }

          return <span>{record.alipayName}</span>;
        }
        return <span>{record.unknownLovePayName}("未记录")</span>;
      }
    },
    {
      key: 'platform',
      dataIndex: 'platform',
      title: '支付平台',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        let item = PLATFORM.filter(x => x.value === record.platform);
        item = item.length ? item[0] : { label: '未知', value: record.platform, color: '' };
        return <ATag color={item.color}>{item.label}</ATag>;
      }
    },
    {
      key: 'orderId',
      dataIndex: 'orderId',
      title: '支付订单号',
      align: 'center'
    },
    {
      key: 'amount',
      dataIndex: 'amount',
      title: '支付金额',
      align: 'center'
    }
  ];

  if (!authStore.userInfo.isTenantUser) {
    if (authStore.userInfo.isTenant) {
      cols.push(
        {
          key: 'userName',
          dataIndex: 'userName',
          title: '持有者',
          align: 'center'
        },
        {
          key: 'usedUserName',
          dataIndex: 'usedUserName',
          title: '使用人',
          align: 'center'
        }
      );
    }
  }

  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'tenantName',
      dataIndex: 'tenantName',
      title: '所属商户',
      align: 'center'
    });
  }

  const otherCols = [
    {
      key: 'settlemented',
      dataIndex: 'settlemented',
      title: '结算状态',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        if (record.settlemented) {
          return <ATag type="success">已结算</ATag>;
        }

        return <ATag type="error">未结算</ATag>;
      }
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '支付时间',
      align: 'center',
      width: 200
    }
  ];

  return [...cols, ...otherCols];
}
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :user-info="authStore.userInfo"
      :platform="PLATFORM"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="亲情卡支付记录"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
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
    </ACard>
  </div>
</template>

<style lang="scss" scoped></style>
