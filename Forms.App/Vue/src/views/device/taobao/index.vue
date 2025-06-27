<script setup lang="tsx">
import { ToolFilled, UnlockOutlined } from '@ant-design/icons-vue';
import { Modal } from 'ant-design-vue';
import { computed, onActivated, onMounted, reactive, ref } from 'vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeviceTaobaoInit, fetchGetDeviceTaobaoList, fetchGetTenantPayTypeEnum } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import {
  deviceTaobaoPayType as DEVICE_TAOBAO_PAY_TYPE,
  deviceTaobaoStatus as DEVICE_TAOBAO_STATUS,
  gender as GENDER,
  platformAccountInit as PLATFORM_ACCOUNT_INIT
} from '@/views/shared/enums';
import { useAuthStore } from '@/store/modules/auth';
import { usePageParams } from '@/store/modules/page-params';
import SearchFilter from './modules/search-filter.vue';
import EditDrawer from './modules/edit-drawer.vue';
import AllowPairDrawer from './modules/allow-pair-drawer.vue';

const rolePermission = useRolePermission();
const authStore = useAuthStore();
const { tableWrapperRef, scrollConfig } = useTableScroll();
const pageParams = usePageParams();

const payTypeEnum = ref<Array<number>>([0]);

const payType = computed(() => {
  if (authStore.userInfo.isTenant) {
    return DEVICE_TAOBAO_PAY_TYPE.filter(x => payTypeEnum.value.includes(x.value));
  }

  return DEVICE_TAOBAO_PAY_TYPE;
});

const model = reactive({
  allowPairVisible: false
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
  apiFn: fetchGetDeviceTaobaoList,
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { drawerVisible, editingData, handleEdit, checkedRowKeys, rowSelection } = useTableOperate(data, getData);

onMounted(() => {
  const params = pageParams.getParams();
  if (params && params.deviceId) {
    searchParams.deviceId = params.deviceId;
    setTimeout(() => getDataByPage(1), 0);
  }
});

onActivated(() => {
  const params = pageParams.getParams();
  if (params && params.deviceId) {
    searchParams.deviceId = params.deviceId;
    getDataByPage(1);
  }
});

function getColumns() {
  const cols = [
    {
      key: 'accountInfo',
      dataIndex: 'accountInfo',
      title: '账号信息',
      align: 'center',
      width: 300,
      customRender: ({ record }) => {
        return (
          <div style="font-size:14px">
            <p class="info mb-6px">
              <span>TB号：</span>
              <span title={record.account || '-'}>{record.account || '-'}</span>
            </p>
            <p>
              <p class="info mb-6px">
                <span>性别：</span>
                <span title={record.gender || '-'}>{getGenderLabel(record.gender) || '-'}</span>
              </p>
            </p>
            <p class="info mb-6px">
              <span>淘龄：</span>
              <span title={record.age || '-'}>{record.age || '-'}</span>
            </p>
            <p class="info mb-6px">
              <span>淘气值：</span>
              <span title={record.taoQi || '-'}>{record.taoQi || '-'}</span>
            </p>
          </div>
        );
      }
    },
    {
      key: 'buyPayType',
      dataIndex: 'buyPayType',
      title: '支付方式',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        const map = DEVICE_TAOBAO_PAY_TYPE.filter(x => x.value === record.buyPayType)[0];
        return <ATag color={map.color}>{map.label}</ATag>;
      }
    },
    {
      key: 'init',
      dataIndex: 'init',
      title: '账号初始化',
      align: 'center',
      width: 120,
      customRender: ({ record }) => {
        const map = PLATFORM_ACCOUNT_INIT.filter(x => x.value === record.init)[0];
        if (record.init === 3) {
          return (
            <ATooltip placement="top" title={<span>{record.remark}</span>}>
              <ATag color={map.color}>{map.label}</ATag>
            </ATooltip>
          );
        }
        return <ATag color={map.color}>{map.label}</ATag>;
      }
    },
    {
      key: 'status',
      dataIndex: 'status',
      title: '账号状态',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        const map = DEVICE_TAOBAO_STATUS.filter(x => x.value === record.status)[0];
        return <ATag color={map.color}>{map.label}</ATag>;
      }
    },
    {
      key: 'allowPair',
      dataIndex: 'allowPair',
      title: '任务分配',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        return <ATag color={record.allowPair ? 'green' : 'red'}>{record.allowPair ? '允许' : '停止'}</ATag>;
      }
    },
    {
      key: 'dayCount',
      dataIndex: 'dayCount',
      title: '当日执行任务数',
      align: 'center',
      width: 150,
      customRender: ({ record }) => {
        return <span>{record.dayCount || 0}</span>;
      }
    },
    {
      key: 'monthCount',
      dataIndex: 'monthCount',
      title: '当月执行任务数',
      align: 'center',
      width: 150,
      customRender: ({ record }) => {
        return <span>{record.monthCount || 0}</span>;
      }
    },
    {
      key: 'nextPairTime',
      dataIndex: 'nextPairTime',
      title: '下次执行日期',
      align: 'center',
      width: 150,
      customRender: ({ record }) => {
        return <span>{record.nextPairTime || '-'}</span>;
      }
    },
    {
      key: 'lastInitTime',
      dataIndex: 'lastInitTime',
      title: '最后同步时间',
      align: 'center',
      width: 200,
      customRender: ({ record }) => {
        return record.lastInitTime || '-';
      }
    },
    {
      key: 'device',
      dataIndex: 'device',
      title: '所属设备',
      align: 'center',
      width: 200,
      customRender: ({ record }) => {
        return `${record.deviceName} (${record.deviceId})`;
      }
    }
  ];

  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'tenantName',
      dataIndex: 'tenantName',
      title: '所属商户',
      align: 'center',
      width: 100
    });
  }

  cols.push({
    key: 'operate',
    title: '操作',
    align: 'center',
    fixed: 'right',
    width: 80,
    customRender: ({ record }) => <div class="flex-center gap-4px">{editButton(record)}</div>
  });

  return cols;
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Device.Taobao.UPDATE)) {
    return '';
  }

  return (
    <ATooltip placement="top" title="编辑">
      <AButton size="small" type="link" onClick={() => handleEdit(record.id)}>
        <ToolFilled />
      </AButton>
    </ATooltip>
  );
}

function setDeviceInit() {
  Modal.confirm({
    title: '警告',
    content: '初始化设备前，请确认主控设备已返回App主界面，初始化过程中，请不要添加商品及任务！',
    okText: '确认',
    cancelText: '取消',
    onOk: async () => {
      try {
        const ids = checkedRowKeys.value;
        if (ids && ids.length) {
          const { data: res, error } = await fetchDeviceTaobaoInit(ids);
          if (!error && res && res.code === 0) {
            window.$message?.success('指令已下发');
            getDataByPage();
          }
        }
      } catch {}
    }
  });
}

function allorPairHandler() {
  model.allowPairVisible = true;
}

onMounted(async () => {
  if (authStore.userInfo.isTenant) {
    await getTenantPayType();
  }
});

onActivated(async () => {
  if (authStore.userInfo.isTenant) {
    await getTenantPayType();
  }
});

async function getTenantPayType() {
  try {
    const { data: res, error } = await fetchGetTenantPayTypeEnum();
    if (!error && res && res.code === 0) {
      payTypeEnum.value = res.data.list;
    }
  } catch {}
}

function getGenderLabel(gender) {
  const map = GENDER.filter(x => x.value === gender)[0];
  return <ATag color={map.color}>{map.label}</ATag>;
}
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :user-info="authStore.userInfo"
      :init="PLATFORM_ACCOUNT_INIT"
      :buy-pay-type="payType"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="淘宝设备"
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
        >
          <AButton
            v-if="rolePermission.hasOne(AppPermissions.Device.Taobao.UPDATE) && authStore.userInfo.isTenant"
            size="small"
            ghost
            type="primary"
            :disabled="checkedRowKeys.length === 0"
            @click="setDeviceInit"
          >
            <template #icon>
              <icon-dashicons:smartphone />
            </template>
            <span class="ml-6px">初始化</span>
          </AButton>

          <AButton
            v-if="rolePermission.hasOne(AppPermissions.Device.Taobao.UPDATE) && authStore.userInfo.isTenant"
            :disabled="checkedRowKeys.length === 0"
            size="small"
            ghost
            type="primary"
            class="v-center flex"
            @click="allorPairHandler()"
          >
            <template #icon><UnlockOutlined /></template>
            <span class="ml-6px">任务分配</span>
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
      />
      <AllowPairDrawer v-model:visible="model.allowPairVisible" :ids="checkedRowKeys" @submitted="getDataByPage" />
      <EditDrawer
        v-model:visible="drawerVisible"
        :auth-store="authStore"
        :row-data="editingData"
        :pay-type="payType"
        @submitted="getDataByPage"
      ></EditDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
.account-info {
  > div {
    margin-bottom: 2px;
    &:last-child {
      margin-bottom: 0;
    }
  }
}

:deep(.info) {
  display: grid;
  grid-template-columns: 70px 1fr;
  gap: 12px;

  > span {
    &:first-child {
      text-align: right;
    }

    &:last-child {
      text-align: left;
      width: 150px;
    }
  }
}
</style>
