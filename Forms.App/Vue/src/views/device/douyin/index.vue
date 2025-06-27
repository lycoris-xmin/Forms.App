<script setup lang="tsx">
import { ToolFilled, UnlockOutlined } from '@ant-design/icons-vue';
import { Modal } from 'ant-design-vue';
import { computed, onActivated, onMounted, reactive, ref } from 'vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { useAuthStore } from '@/store/modules/auth';
import { usePageParams } from '@/store/modules/page-params';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import {
  deviceDouyinPayType as DEVICE_DY_PAY_TYPE,
  deviceDouyinStatus as DEVICE_DY_STATUS,
  platformAccountInit as PLATFORM_ACCOUNT_INIT
} from '@/views/shared/enums';
import { fetchDeviceDouyinInit, fetchGetDeviceDouyinList } from '@/service/api/modules/device-douyin';

import { fetchGetTenantPayTypeEnum } from '@/service/api/modules/enums';
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
    return DEVICE_DY_PAY_TYPE.filter(x => payTypeEnum.value.includes(x.value));
  }
  return DEVICE_DY_PAY_TYPE;
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
  apiFn: fetchGetDeviceDouyinList,
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { drawerVisible, editingData, handleEdit, checkedRowKeys, rowSelection } = useTableOperate(data, getData);

const model = reactive({
  allowPairVisible: false
});

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
      key: 'account',
      dataIndex: 'account',
      title: '账号',
      width: 200,
      customRender: ({ record }) => {
        if (record.init !== 2) {
          return <ATag>未初始化</ATag>;
        }

        return record.account;
      }
    },
    {
      key: 'buyPayType',
      dataIndex: 'buyPayType',
      title: '支付方式',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        const map = DEVICE_DY_PAY_TYPE.filter(x => x.value === record.buyPayType)[0];
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
        const map = DEVICE_DY_STATUS.filter(x => x.value === record.status)[0];
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
  if (!rolePermission.hasOne(AppPermissions.Device.DY.UPDATE)) {
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
    title: '提示',
    content: '初始化设备前，请确认主控设备已返回App主界面，初始化过程中，请不要添加商品及任务！',
    okText: '确认',
    cancelText: '取消',
    onOk: async () => {
      try {
        const ids = checkedRowKeys.value;
        if (ids && ids.length) {
          const { data: res, error } = await fetchDeviceDouyinInit(ids);
          if (!error && res && res.code === 0) {
            window.$message?.success('指令已下发');
            getDataByPage();
          }
        }
      } catch {}
    }
  });
}

function allowPairHandler() {
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
      title="抖音设备"
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
            v-if="rolePermission.hasOne(AppPermissions.Device.DY.UPDATE) && authStore.userInfo.isTenant"
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
            v-if="rolePermission.hasOne(AppPermissions.Device.DY.UPDATE) && authStore.userInfo.isTenant"
            :disabled="checkedRowKeys.length === 0"
            size="small"
            ghost
            type="primary"
            class="v-center flex"
            @click="allowPairHandler()"
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

      <EditDrawer
        v-model:visible="drawerVisible"
        :auth-store="authStore"
        :row-data="editingData"
        :pay-type="payType"
        @submitted="getDataByPage"
      ></EditDrawer>

      <AllowPairDrawer v-model:visible="model.allowPairVisible" :ids="checkedRowKeys" @submitted="getDataByPage" />
    </ACard>
  </div>
</template>

<style lang="scss" scoped></style>
