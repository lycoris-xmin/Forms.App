<script setup lang="tsx">
import { computed, onMounted, reactive } from 'vue';
import { CloudUploadOutlined, DeleteOutlined, FileTextOutlined, ToolFilled } from '@ant-design/icons-vue';
import { useRoute } from 'vue-router';
import type { TableProps } from 'ant-design-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import {
  fetchDeleteDevice,
  fetchGetDeviceAreaName,
  fetchGetDeviceGroup,
  fetchGetDeviceList,
  fetchGetDeviceSection,
  fetchGetTenantDeviceTypeEnum,
  fetchResetStatus
} from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import {
  commandType as COMMAND_TYPE,
  deviceStatus as DEVICE_STATUS,
  deviceType as DEVICE_TYPE,
  platform as PLATFORM,
  platformAccountInit as PLATFORM_ACCOUNT_INIT
} from '@/views/shared/enums';
import { useRouterPush } from '@/hooks/common/router';
import { usePageParams } from '@/store/modules/page-params';
import SearchFilter from './modules/search-filter.vue';
import BindingDrawer from './modules/binding-drawer.vue';
import SettingDrawer from './modules/setting-drawer.vue';
import RechargeDrawer from './modules/recharge-drawer.vue';
import BatchRechangeDrawer from './modules/batch-rechange-drawer.vue';
import BindingModal from './modules/binding-modal.vue';
import RecordDrawer from './modules/record-drawer.vue';
import CoreLogDrawer from './modules/core-log-drawer.vue';

const route = useRoute();
const authStore = useAuthStore();
const rolePermission = useRolePermission();
const { routerPushByKey } = useRouterPush();
const pageParams = usePageParams();

const model = reactive({
  rowId: '',
  bindingDrawerVisible: false,
  bindingAreaName: '默认分组',
  bindingGroupName: '默认分组',
  bindingSectionName: '默认分组',
  bindingModalVisible: false,
  rechargeVisible: false,
  batchRechargeDrawer: false,
  resetLoading: false,
  deviceType: [10],
  recordDrawerVisible: false,
  rowDetail: {},
  expandedRowKeys: [],
  areaNameEnum: [],
  groupNameEnum: [],
  sectionNameEnum: [],
  coreLogVisible: false
});

const { tableWrapperRef, scrollConfig } = useTableScroll();

const {
  columns,
  columnChecks,
  data,
  getData,
  getDataByPage,
  loading,
  mobilePagination,
  searchParams,
  resetSearchParams,
  reloadColumns,
  sortChangeHandler
} = useTable({
  apiFn: (filter: Api.Device.SearchFilter) => {
    model.expandedRowKeys = [];
    model.rowDetail = {};
    return fetchGetDeviceList(filter);
  },
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { drawerVisible, editingData, handleEdit, checkedRowKeys, rowSelection, onBatchDeleted, onDeleted } =
  useTableOperate(data, getData);

const deviceTypeEnum = computed(() => {
  return authStore.userInfo.isTenant ? DEVICE_TYPE.filter(x => model.deviceType.includes(x.value)) : DEVICE_TYPE;
});

onMounted(async () => {
  if (route.params && route.params.status === 1) {
    searchParams.status = 1;
  }

  if (authStore.userInfo.isTenant) {
    const { data: res, error } = await fetchGetTenantDeviceTypeEnum();
    if (!error && res && res.code === 0) {
      model.deviceType = res.data.list;
      reloadColumns();
    }
  }

  {
    const { data: res, error } = await fetchGetDeviceAreaName();
    if (!error && res && res.code === 0) {
      model.areaNameEnum = res.data.list.map(x => {
        return {
          label: x,
          value: x
        };
      });
    }
  }
  {
    const { data: res, error } = await fetchGetDeviceGroup();
    if (!error && res && res.code === 0) {
      model.groupNameEnum = res.data.list.map(x => {
        return {
          label: x,
          value: x
        };
      });
    }
  }
  {
    const { data: res, error } = await fetchGetDeviceSection();
    if (!error && res && res.code === 0) {
      model.sectionNameEnum = res.data.list.map(x => {
        return {
          label: x,
          value: x
        };
      });
    }
  }
});

function bindingHandler() {
  if (model.deviceType.includes(0)) {
    model.bindingModalVisible = true;
    return;
  }

  model.bindingAreaName = '默认分组';
  model.bindingGroupName = '默认分组';
  model.bindingSectionName = '默认分组';
  model.bindingDrawerVisible = true;
}

function bindingQRcodeHandler({
  areaName,
  groupName,
  sectionName
}: {
  areaName: string;
  groupName: string;
  sectionName: string;
}) {
  model.bindingAreaName = areaName;
  model.bindingGroupName = groupName;
  model.bindingSectionName = sectionName;
  model.bindingDrawerVisible = true;
}

async function batchDeleteHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    const { data: res, error } = await fetchDeleteDevice(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteDevice([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

function getColumns() {
  const cols = [
    {
      key: 'id',
      dataIndex: 'id',
      title: '系统编号',
      width: 170,
      customRender: ({ record }) => {
        return (
          <span class="page-link color" onClick={() => routerPushByKey('device_detail', { params: { id: record.id } })}>
            {record.id}
          </span>
        );
      }
    },
    {
      key: 'name',
      dataIndex: 'name',
      title: '设备名称',
      align: 'center',
      width: 180,
      showSorterTooltip: true,
      sorter: true
    }
  ];

  if (model.deviceType.includes(0)) {
    cols.push({
      key: 'areaName',
      dataIndex: 'areaName',
      title: '设备区域',
      align: 'center',
      width: 150,
      ellipsis: true,
      visible: authStore.userInfo.isTenant
    });

    cols.push({
      key: 'groupName',
      dataIndex: 'groupName',
      title: '设备分组',
      align: 'center',
      width: 150,
      ellipsis: true,
      visible: authStore.userInfo.isTenant
    });

    cols.push({
      key: 'sectionName',
      dataIndex: 'sectionName',
      title: '设备小组',
      align: 'center',
      width: 150,
      ellipsis: true,
      visible: authStore.userInfo.isTenant
    });
  }

  cols.push({
    key: 'appVersion',
    dataIndex: 'appVersion',
    title: 'App版本',
    align: 'center',
    width: model.deviceType.includes(0) ? 80 : 150,
    visible: !authStore.userInfo.isTenant,
    showSorterTooltip: true,
    sorter: true,
    customRender: ({ record }) => {
      return <ATag color="default">{record.appVersion}</ATag>;
    }
  });

  cols.push({
    key: 'scriptVersion',
    dataIndex: 'scriptVersion',
    title: '软件版本',
    align: 'center',
    width: model.deviceType.includes(0) ? 80 : 150,
    visible: !authStore.userInfo.isTenant,
    showSorterTooltip: true,
    sorter: true,
    customRender: ({ record }) => {
      return <ATag color="default">{record.scriptVersion}</ATag>;
    }
  });

  if (model.deviceType.length > 1 || !authStore.userInfo.isTenant) {
    cols.push({
      key: 'type',
      dataIndex: 'type',
      title: '设备类型',
      align: 'center',
      width: 100,
      showSorterTooltip: true,
      sorter: true,
      visible: !authStore.userInfo.isTenant,
      customRender: ({ record }) => {
        const item = DEVICE_TYPE.filter(x => x.value === record.type)[0];
        return <ATag color={item.color}>{item.label}</ATag>;
      }
    });
  }

  cols.push({
    key: 'status',
    dataIndex: 'status',
    title: '状态',
    align: 'center',
    width: 120,
    showSorterTooltip: true,
    sorter: true,
    customRender: ({ record }) => {
      if (record.status === 2 || record.status === 10) {
        const command = COMMAND_TYPE.filter(x => x.value === record.command);
        if (command && command.length) {
          return <ATag color={command[0].color}>在线({command[0].label})</ATag>;
        }
      }

      const item = DEVICE_STATUS.filter(x => x.value === record.status)[0];

      if (item.value === 100) {
        return (
          <ATooltip placement="top" title={<span>{record.remark || 'App异常，请重启App'}</span>}>
            <ATag color={item.color}>
              <span>{item.label}</span>
            </ATag>
          </ATooltip>
        );
      } else if (item.value === -1) {
        return (
          <ATooltip placement="top" title={<span>设备脚本运行异常，建议重启App</span>}>
            <ATag color={item.color}>{item.label}</ATag>
          </ATooltip>
        );
      } else if (item.value === 1001) {
        return (
          <ATooltip placement="top" title={<span>若已确定全部设备已注册完成，请点击重置状态</span>}>
            <ATag color={item.color} onClick={() => resetStatusHandler(record.id)}>
              {item.label}
            </ATag>
          </ATooltip>
        );
      }

      return <ATag color={item.color}>{item.label}</ATag>;
    }
  });

  cols.push({
    key: 'accountInitList',
    dataIndex: 'accountInitList',
    title: '账号状态',
    width: 180,
    ellipsis: true,
    customRender: ({ record }) => {
      return <div>{record.accountInitList.map(item => renderAccountInit(record.id, item))}</div>;
    }
  });

  cols.push({
    key: 'ip',
    dataIndex: 'ip',
    title: '设备IP',
    align: 'center',
    width: 170,
    ellipsis: true,
    customRender: ({ record }) => {
      return `${record.ip}(${record.ipAddress})`;
    }
  });

  cols.push({
    key: 'days',
    dataIndex: 'days',
    title: '倒计时',
    align: 'center',
    width: 120,
    customRender: ({ record }) => {
      const val = Number.parseFloat(record.days);
      if (val <= 7) {
        return (
          <ATag bordered={false} color="error">
            {val} 天
          </ATag>
        );
      } else if (val <= 30) {
        return (
          <ATag bordered={false} color="warning">
            {val} 天
          </ATag>
        );
      }
      return (
        <ATag bordered={false} color="success">
          {val} 天
        </ATag>
      );
    }
  });

  cols.push({
    key: 'createUser',
    dataIndex: 'createUser',
    title: authStore.userInfo.isTenant ? '添加人' : '所属商户',
    align: 'center',
    width: 130,
    ellipsis: true
  });

  cols.push({
    key: 'createTime',
    dataIndex: 'createTime',
    title: '绑定时间',
    align: 'center',
    width: 200,
    visible: false
  });

  cols.push({
    key: 'operate',
    title: '操作',
    align: 'center',
    width: authStore.userInfo.isTenantUser ? 100 : 130,
    fixed: 'right',
    customRender: ({ record }) => (
      <div class="flex-center gap-4px">
        {recordButton(record)}
        {editButton(record)}
        {rechargeButton(record)}
        {deleteButton(record)}
        {coreLogButton(record)}
      </div>
    )
  });

  return cols;
}

function renderAccountInit(id, item) {
  const platform = PLATFORM.filter(x => x.value === item.platform)[0];
  const init = PLATFORM_ACCOUNT_INIT.filter(x => x.value === item.init)[0];

  const platformLabel = platform ? platform.label : '未知平台';
  const initLabel = init ? init.label : '未知状态';

  const handlePlatformClick = () => {
    const routes = {
      0: 'device_taobao',
      10: 'device_jingdong',
      20: 'device_pdd',
      30: 'device_douyin',
      40: 'device_alibaba',
      50: 'device_rednote'
    };
    const routeName = routes[item.platform] || 'device_taobao';

    pageParams.setParams(routeName, { deviceId: id });

    routerPushByKey(routeName);
  };

  return (
    <p class="link-p">
      <span class={`page-link color ${init ? init.color : 'default'}`} key={item} onClick={handlePlatformClick}>
        {platformLabel}
        {initLabel}
      </span>
    </p>
  );
}

function recordHandler(id) {
  model.rowId = id;
  model.recordDrawerVisible = true;
}

function rechargeHandler(id) {
  model.rowId = id;
  model.rechargeVisible = true;
}

function batchReChangeHandler() {
  model.batchRechargeDrawer = true;
}

async function editHandler(record) {
  handleEdit(record.id);
}

function recordButton(record) {
  return (
    <ATooltip placement="top" title="执行记录">
      <AButton size="small" type="link" onClick={() => recordHandler(record.id)}>
        <FileTextOutlined />
      </AButton>
    </ATooltip>
  );
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Device.List.UPDATE) || !authStore.userInfo.isTenant) {
    return '';
  }

  return (
    <ATooltip placement="top" title="设置">
      <AButton size="small" type="link" onClick={() => editHandler(record)}>
        <ToolFilled />
      </AButton>
    </ATooltip>
  );
}

function rechargeButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Device.List.UPDATE)) {
    return '';
  } else if (authStore.userInfo.isTenantUser) {
    return '';
  }

  return (
    <ATooltip placement="top" title="使用码">
      <AButton size="small" type="link" onClick={() => rechargeHandler(record.id)}>
        <icon-cryptocurrency-cny />
      </AButton>
    </ATooltip>
  );
}

function deleteButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Device.List.DELETE) || !authStore.userInfo.isTenant) {
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

function coreLogButton(record) {
  if (authStore.userInfo.isTenant) {
    return '';
  }

  return (
    <ATooltip placement="top" title="核心日志">
      <AButton size="small" type="link" onClick={() => coreLogHandler(record)}>
        <CloudUploadOutlined />
      </AButton>
    </ATooltip>
  );
}

function batchResetHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    window.$modal?.confirm({
      title: '确定要重置设备状态吗？',
      cancelText: '取消',
      okText: '确定',
      content: '如果设备正在运行中，重置状态会导致任务执行异常，确认要重置吗？',
      async onOk() {
        model.resetLoading = true;
        try {
          const { data: res, error } = await fetchResetStatus(ids);
          if (!error && res && res.code === 0) {
            window.$message?.success('重置成功');
            getData();
          }
        } finally {
          model.resetLoading = false;
        }
      }
    });
  }
}

async function resetStatusHandler(id) {
  const { data: res, error } = await fetchResetStatus([id]);
  if (!error && res && res.code === 0) {
    window.$message?.success('重置成功');
    getData();
  }
}

function coreLogHandler(record) {
  model.rowId = record.id;
  model.coreLogVisible = true;
}

function tableChangeHandler(_1: any, _2: any, sorter: any): TableProps['onChange'] {
  sortChangeHandler(sorter);
}
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :user-info="authStore.userInfo"
      :devie-type="deviceTypeEnum"
      :group-name="model.groupNameEnum"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="设备列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="device-card flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :delete-btn="rolePermission.hasOne(AppPermissions.Device.List.DELETE)"
          @delete="batchDeleteHandler"
        >
          <template #prefix>
            <AButton
              v-if="rolePermission.hasOne(AppPermissions.Device.List.CREATE) && authStore.userInfo.isTenant"
              size="small"
              ghost
              type="primary"
              @click="bindingHandler"
            >
              <template #icon>
                <icon-ic-round-plus class="align-sub text-icon" />
              </template>
              <span class="ml-6px">绑定设备</span>
            </AButton>
            <AButton
              v-if="rolePermission.hasOne(AppPermissions.Device.List.UPDATE)"
              size="small"
              ghost
              type="primary"
              :disabled="checkedRowKeys.length === 0"
              @click="batchReChangeHandler"
            >
              <template #icon>
                <icon-cryptocurrency-cny />
              </template>
              <span class="ml-6px">批量使用</span>
            </AButton>
            <AButton
              v-if="rolePermission.hasOne(AppPermissions.Device.List.UPDATE)"
              size="small"
              ghost
              type="primary"
              :disabled="checkedRowKeys.length === 0"
              :loading="model.resetLoading"
              @click="batchResetHandler"
            >
              <template #icon>
                <icon-streamline:interface-setting-wrench-crescent-tool-construction-tools-wrench-setting-edit-adjust />
              </template>
              <span class="ml-6px">重置状态</span>
            </AButton>
          </template>
        </TableHeaderOperation>
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
        @change="tableChangeHandler"
      ></ATable>

      <BindingModal
        v-model:visible="model.bindingModalVisible"
        :device-id="model.rowId"
        @submitted="bindingQRcodeHandler"
      ></BindingModal>

      <BindingDrawer
        v-model:visible="model.bindingDrawerVisible"
        :device-id="model.rowId"
        :area-name="model.bindingAreaName"
        :group-name="model.bindingGroupName"
        :section-name="model.bindingSectionName"
        @refresh="() => getDataByPage()"
      ></BindingDrawer>

      <SettingDrawer
        v-model:visible="drawerVisible"
        :row-data="editingData"
        :devie-type="deviceTypeEnum"
        :area-name="model.areaNameEnum"
        :group-name="model.groupNameEnum"
        :section-name="model.sectionNameEnum"
        @submitted="() => getDataByPage()"
      ></SettingDrawer>

      <RechargeDrawer
        :id="model.rowId"
        v-model:visible="model.rechargeVisible"
        :user-info="authStore.userInfo"
        @submitted="() => getDataByPage()"
      ></RechargeDrawer>

      <BatchRechangeDrawer
        v-model:visible="model.batchRechargeDrawer"
        :ids="checkedRowKeys"
        :user-info="authStore.userInfo"
        @submitted="() => getDataByPage()"
      ></BatchRechangeDrawer>

      <CoreLogDrawer v-model:visible="model.coreLogVisible" :device-id="model.rowId"></CoreLogDrawer>

      <RecordDrawer :id="model.rowId" v-model:visible="model.recordDrawerVisible"></RecordDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
.device-card {
  :deep(.link-p) {
    overflow: hidden;
    text-overflow: ellipsis;
    word-break: break-all;
    white-space: nowrap;
    margin-bottom: 6px;
  }
}
</style>
