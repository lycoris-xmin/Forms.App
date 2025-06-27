<script setup lang="tsx">
import { computed, reactive } from 'vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import {
  fetchGetAlipayLovePayEntSettleList,
  fetchSettlementAlipayLovePayEntSettle
} from '@/service/api/modules/alipay-lovepay-record-ent-settle';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth'; // 登录用户信息
import { platform as PLATFORM } from '@/views/shared/enums';
import SearchFilter from './modules/search-filter.vue';

const authStore = useAuthStore();

const rolePermission = useRolePermission();

const { tableWrapperRef, scrollConfig } = useTableScroll();

const summaryModel = reactive({});

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
  apiFn: async (...filter) => {
    const res = await fetchGetAlipayLovePayEntSettleList(...filter);
    Object.assign(summaryModel, res.data.data.summary);
    return res;
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
      key: 'alipayName',
      dataIndex: 'alipayName',
      title: '亲情卡',
      align: 'center',
      customRender: ({ record }) => {
        if (record.alipayAccount) {
          return (
            <span>
              {record.alipayAccount}({record.alipayName})
            </span>
          );
        }

        return <span>{record.alipayName}</span>;
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
      key: 'planTaskId',
      dataIndex: 'planTaskId',
      title: '任务编号',
      align: 'center'
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
      align: 'center',
      customRender: ({ record }) => {
        return <span style={{ color: record.amount > 0 ? '#52c41a' : '#ff4d4f' }}>{record.amount}</span>;
      }
    },
    {
      key: 'priceDiff',
      dataIndex: 'priceDiff',
      title: '差价',
      align: 'center',
      customRender: ({ record }) => {
        return <span style={{ color: record.priceDiff > 0 ? '#52c41a' : '#ff4d4f' }}>{record.priceDiff}</span>;
      }
    }
  ];
  // 管理员只能查看商户
  if (authStore.userInfo.isTenant) {
    cols.push(
      {
        key: 'usedUserName',
        dataIndex: 'usedUserName',
        title: '使用人',
        align: 'center'
      },
      {
        key: 'shopName',
        dataIndex: 'shopName',
        title: '店铺',
        align: 'center'
      }
    );
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
          return <ATag color="success">已结算</ATag>;
        }

        return <ATag color="error">未结算</ATag>;
      }
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '支付时间',
      align: 'center',
      width: 200
    },
    {
      key: 'operate',
      title: '操作',
      align: 'left',
      width: 80,
      customRender: ({ record }) => {
        if (
          !rolePermission.hasOne(AppPermissions.Report.AlipayLovePayEntSettle) ||
          authStore.userInfo.isTenant ||
          record.settlemented
        ) {
          return '';
        }

        return (
          <ATooltip placement="top" title="结算">
            <AButton size="small" type="link" onClick={() => settlementHandler(record.id)}>
              <icon-cryptocurrency-cny />
            </AButton>
          </ATooltip>
        );
      }
    }
  ];

  return [...cols, ...otherCols];
}

async function settlementHandler(id: string) {
  const ids = id ? [id] : [...checkedRowKeys.value];
  if (ids && ids.length) {
    const { data: res, error } = await fetchSettlementAlipayLovePayEntSettle(ids);
    if (!error && res && res.code === 0) {
      window.$message?.success('结算成功');
      getData();
    }
  }
}

const totals = computed(() => {
  const model = Object.entries(summaryModel).map(([key, value]) => ({ key, value }));
  const array = [];
  for (const item of columns.value) {
    if (item.key !== 'amount' && item.key !== 'priceDiff') {
      if (item.key === 'operate') {
        array.push({ key: item.key, value: '' });
      } else {
        array.push({ key: item.key, value: '——' });
      }
    } else {
      const find = model.find(x => x.key === item.key);
      array.push({ key: item.key, value: find?.value || 0 });
    }
  }
  return array;
});
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
      title="企业亲情卡结算报表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
        >
          <AButton
            v-if="
              authStore.userInfo.isTenant || rolePermission.hasOne(AppPermissions.Report.AlipayLovePayEntSettle.UPDATE)
            "
            size="small"
            ghost
            type="primary"
            :disabled="checkedRowKeys.length === 0"
          >
            <template #icon>
              <icon-cryptocurrency-cny />
            </template>
            <span class="ml-6px">批量结算</span>
          </AButton>
        </TableHeaderOperation>
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
      >
        <template v-if="data && data.length" #summary>
          <ATableSummary>
            <ATableSummaryRow>
              <ATableSummaryCell :index="0">合计</ATableSummaryCell>

              <ATableSummaryCell v-for="item in totals" :key="item.key">
                <ATypographyText :type="isNaN(item.value) ? '' : item.value > 0 ? 'success' : 'danger'">
                  {{ item.value }}
                </ATypographyText>
              </ATableSummaryCell>
            </ATableSummaryRow>
          </ATableSummary>
        </template>
      </ATable>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
.ant-table-summary {
  .ant-table-cell {
    text-align: center;
  }
}
</style>
