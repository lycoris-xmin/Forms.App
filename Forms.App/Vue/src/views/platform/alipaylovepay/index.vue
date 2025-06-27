<script setup lang="tsx">
import { onMounted, reactive } from 'vue';
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeleteAlipayLovePay, fetchGetAlipayLovePayList, fetchGetTaobaoDictionaryShopName } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import { alipayLovePay as ALIPAY_LOVE_PAY } from '@/views/shared/enums';
import OperateDrawer from './modules/operate-drawer.vue';
import SearchFilter from './modules/search-filter.vue';

const authStore = useAuthStore();

const rolePermission = useRolePermission();

const model = reactive({
  shopNameEnum: [] // 1:店铺名称 2:店铺id
});

const { tableWrapperRef, scrollConfig } = useTableScroll();

const {
  columns,
  columnChecks,
  searchParams,
  resetSearchParams,
  data,
  getData,
  loading,
  mobilePagination,
  getDataByPage,
  reloadColumns
} = useTable({
  apiFn: fetchGetAlipayLovePayList,
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
// const alipayLovePay = () => {
//   return ALIPAY_LOVE_PAY;
// };
async function batchDeleteHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    const { data: res, error } = await fetchDeleteAlipayLovePay(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteAlipayLovePay([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

function getColumns() {
  const cols = [
    {
      key: 'alipayAccount',
      dataIndex: 'alipayAccount',
      title: '支付宝账号',
      align: 'center'
    },
    {
      key: 'alipayName',
      dataIndex: 'alipayName',
      title: '亲情卡备注',
      align: 'center'
    }
  ];
  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'tentName',
      dataIndex: 'tentName',
      title: '所属商户',
      align: 'center'
    });
  } else if (authStore.userInfo.isTenant && !authStore.userInfo.isTenantUser) {
    cols.push({
      key: 'alipayLovePayType',
      dataIndex: 'alipayLovePayType',
      title: '亲情卡类型',
      align: 'center',
      customRender: ({ record }) => {
        const alipayLovePayType = ALIPAY_LOVE_PAY.filter(x => x.value === record.alipayLovePayType);

        return <ATag color={alipayLovePayType[0].color}>{alipayLovePayType[0].label}</ATag>;
      }
    });
    cols.push({
      key: 'createUserAndShop',
      dataIndex: 'createUserAndShop',
      title: '绑定员工/绑定店铺',
      align: 'center'
    });
  }

  const otherCols = [
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
      align: 'left',
      width: 120,
      customRender: ({ record }) => (
        <div class="v-center flex gap-8px">
          {editButton(record)}
          {deleteButton(record)}
        </div>
      )
    }
  ];

  return [...cols, ...otherCols];
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Platform.AlipayLovePay.DELETE)) {
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
  if (!rolePermission.hasOne(AppPermissions.Platform.AlipayLovePay.DELETE)) {
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
onMounted(async () => {
  if (
    authStore.userInfo.isTenant &&
    !authStore.userInfo.isTenantUser &&
    rolePermission.hasOne(AppPermissions.Platform.AlipayLovePay.CREATE)
  ) {
    const { data: res, error } = await fetchGetTaobaoDictionaryShopName();
    if (!error && res && res.code === 0) {
      model.shopNameEnum = res.data.list;
      reloadColumns();
    }
  }
});
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :user-info="authStore.userInfo"
      :alipay-love-pay="ALIPAY_LOVE_PAY"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="亲情卡列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.Platform.AlipayLovePay.CREATE)"
          :delete-btn="rolePermission.hasOne(AppPermissions.Platform.AlipayLovePay.DELETE)"
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
        :shop-name-enum="model.shopNameEnum"
        :row-data="editingData"
        :user-info="authStore.userInfo"
        :alipay-love-pay="ALIPAY_LOVE_PAY"
        @submitted="getDataByPage"
      ></OperateDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped></style>
