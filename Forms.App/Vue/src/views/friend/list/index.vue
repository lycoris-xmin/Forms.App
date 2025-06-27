<script setup lang="tsx">
import { reactive } from 'vue';
import { DeleteOutlined, ToolFilled } from '@ant-design/icons-vue';
import { Icon } from '@iconify/vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeleteFriend, fetchGetFriendList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import SearchFilter from './modules/search-filter.vue';
import SendFriendModal from './modules/sendfriend-modal.vue';
import SettingDrawer from './modules/setting-drawer.vue';
import AuditShop from './modules/audit-shop.vue';

const authStore = useAuthStore();
const rolePermission = useRolePermission();

const model = reactive({
  rowId: '',
  resetLoading: false,
  rowDetail: {},
  expandedRowKeys: [],
  settingDrawerVisible: false, // 设置抽屉
  auditDrawerVisible: false //  审核抽屉
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
  resetSearchParams
} = useTable({
  apiFn: fetchGetFriendList,
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { editingData, handleEdit, checkedRowKeys, rowSelection, onBatchDeleted, onDeleted } = useTableOperate(
  data,
  getData
);

function bindingHandler() {
  model.bindingModalVisible = true;
}

function sendFriendQRcodeHandler() {
  model.bindingDrawerVisible = true;
}

async function batchDeleteHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    const { data: res, error } = await fetchDeleteFriend(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteFriend([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

function getColumns() {
  const cols = [
    {
      key: 'friendAccount',
      dataIndex: 'friendAccount',
      title: '好友账号',
      width: 170
    }
  ];

  cols.push({
    key: 'friendNickName',
    dataIndex: 'friendNickName',
    title: '好友名称（备注）',
    align: 'center',
    width: 180
  });

  cols.push({
    key: 'auditShopCount',
    dataIndex: 'auditShopCount',
    title: '审核店铺数量',
    width: 180,
    ellipsis: true
  });

  cols.push({
    key: 'status',
    dataIndex: 'status',
    title: '状态',
    align: 'center',
    width: 120,
    customRender: ({ record }) => {
      const isOnline = record.status === true;
      return <ATag color={isOnline ? 'green' : 'red'}>{isOnline ? '在线' : '离线'}</ATag>;
    }
  });

  cols.push({
    key: 'createTime',
    dataIndex: 'createTime',
    title: '添加时间',
    align: 'center',
    width: 200,
    visible: false
  });

  if (authStore.userInfo.isTenant) {
    cols.push({
      key: 'operate',
      title: '操作',
      align: 'center',
      width: 130,
      fixed: 'right',
      customRender: ({ record }) => (
        <div class="v-center center flex gap-4px">
          {editButton(record)}
          {auditButton(record)}
          {deleteButton(record)}
        </div>
      )
    });
  }

  return cols;
}

async function editHandler(record, type = 'setting') {
  handleEdit(record.id); // 设置 editingData

  if (type === 'setting') {
    model.settingDrawerVisible = true;
  } else if (type === 'audit') {
    model.auditDrawerVisible = true;
  }
}

function editButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Friend.List.UPDATE) || !authStore.userInfo.isTenant) {
    return '';
  }

  return (
    <ATooltip placement="top" title="设置">
      <AButton size="small" type="link" onClick={() => editHandler(record, 'setting')} class="v-center center flex">
        <ToolFilled />
      </AButton>
    </ATooltip>
  );
}

// 另一个按钮，用来打开 Audit 抽屉（示例）
function auditButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Friend.Audit.VIEW) || !authStore.userInfo.isTenant) {
    return '';
  }

  return (
    <ATooltip placement="top" title="审核">
      <AButton size="small" type="link" onClick={() => editHandler(record, 'audit')} class="v-center center flex">
        <Icon icon="carbon:report" />
      </AButton>
    </ATooltip>
  );
}

function deleteButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Friend.List.DELETE) || !authStore.userInfo.isTenant) {
    return '';
  }

  return (
    <APopconfirm title="确认删除吗？" onConfirm={() => deleteHandler(record.id)}>
      <ATooltip placement="top" title="删除">
        <AButton danger size="small" type="link" class="v-center center flex">
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
      :user-info="authStore.userInfo"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="好友列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="device-card flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :delete-btn="rolePermission.hasOne(AppPermissions.Friend.List.DELETE)"
          @delete="batchDeleteHandler"
        >
          <template #prefix>
            <AButton
              v-if="rolePermission.hasOne(AppPermissions.Friend.List.CREATE) && authStore.userInfo.isTenant"
              size="small"
              ghost
              type="primary"
              @click="bindingHandler"
            >
              <template #icon>
                <icon-ic-round-plus class="align-sub text-icon" />
              </template>
              <span class="ml-6px">添加好友</span>
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
      ></ATable>

      <SendFriendModal
        v-if="authStore.userInfo.isTenant"
        v-model:visible="model.bindingModalVisible"
        @submitted="sendFriendQRcodeHandler"
      ></SendFriendModal>

      <SettingDrawer
        v-if="authStore.userInfo.isTenant"
        v-model:visible="model.settingDrawerVisible"
        :row-data="editingData"
        @submitted="getDataByPage"
      />

      <AuditShop
        v-if="authStore.userInfo.isTenant"
        v-model:visible="model.auditDrawerVisible"
        :row-data="editingData"
        @submitted="getDataByPage"
      />
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
