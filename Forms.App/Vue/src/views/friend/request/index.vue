<script setup lang="tsx">
import { reactive, ref } from 'vue';
import { Icon } from '@iconify/vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchCreateFriend, fetchGetRequestList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import { friendRequestStatus as REQUEST_STATUS } from '@/views/shared/enums';
import SearchFilter from './modules/search-filter.vue';

const authStore = useAuthStore();
const rolePermission = useRolePermission();

const modelupdate = reactive({
  id: '',
  consent: 0,
  refuse: ''
});

const model = reactive({
  rowId: '',
  resetLoading: false,
  rowDetail: {},
  expandedRowKeys: [],
  settingDrawerVisible: false,
  auditDrawerVisible: false
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
  apiFn: fetchGetRequestList,
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { checkedRowKeys, rowSelection } = useTableOperate(data, getData);

const refuseReason = ref('');
const refuseModalVisible = ref(false);

const showRefuseModal = (consent: number, id: string) => {
  modelupdate.consent = consent;
  modelupdate.id = id;
  refuseReason.value = '';
  refuseModalVisible.value = true;
};

const handleConfirmRefuse = async () => {
  if (!refuseReason.value) {
    window.$message?.warning('请填写拒绝原因');
    return;
  }
  modelupdate.refuse = refuseReason.value;

  const { data: res, error } = await fetchCreateFriend(modelupdate);
  if (!error && res) {
    if (res.code === 0) {
      window.$message?.success('拒绝成功');
      refuseModalVisible.value = false;
      getDataByPage(1);
    } else {
      window.$message?.error(res?.msg || '拒绝失败');
    }
  }
};

const submittedHandler = async (consent: number, id: string) => {
  modelupdate.consent = consent;
  modelupdate.id = id;

  const { data: res, error } = await fetchCreateFriend(modelupdate);
  if (!error && res) {
    if (res.code === 0) {
      if (consent === 1) {
        window.$message?.success('添加成功');
      } else {
        window.$message?.success('拒绝成功');
      }
    }
  }
  getDataByPage(1);
};

function getColumns() {
  const cols = [
    { key: 'email', dataIndex: 'email', title: '好友账号', width: 100 },
    { key: 'nickName', dataIndex: 'nickName', title: '对方昵称', width: 60 },
    { key: 'reasons', dataIndex: 'reasons', title: '申请备注', width: 200 },
    {
      key: 'status',
      dataIndex: 'status',
      title: '申请状态',
      align: 'center',
      width: 50,
      customRender: ({ record }) => {
        const item = REQUEST_STATUS.find(x => x.value === record.status);
        if (item?.value === 10) {
          return (
            <ATooltip placement="top" title={<span>{record.refuseReason || ''}</span>}>
              <ATag color={item?.color}>{item?.label}</ATag>
            </ATooltip>
          );
        }
        return <ATag color={item?.color}>{item?.label}</ATag>;
      }
    },
    { key: 'createTime', dataIndex: 'createTime', title: '申请时间', width: 100 },
    {
      key: 'operate',
      title: '操作',
      align: 'center',
      width: 100,
      fixed: 'right',
      customRender: ({ record }) => {
        if (record.status === 10 || record.status === 1) return '';
        else if (!record.havState) return '';

        return (
          <div class="flex-center gap-4px">
            {consentButton(record)}
            {refuseButton(record)}
          </div>
        );
      }
    }
  ];
  return cols;
}

function consentButton(record: any) {
  if (!rolePermission.hasOne(AppPermissions.Friend.Request.CREATE) || !authStore.userInfo.isTenant) {
    return '';
  }

  return (
    <APopconfirm
      placement="left"
      title="确定添加对方为好友吗?"
      ok-text="确定"
      cancel-text="取消"
      onConfirm={() => submittedHandler(1, record.id)}
    >
      <ATooltip placement="top" title="通过">
        <AButton size="small" type="link">
          <Icon icon="covid:social-distancing-correct-5" />
        </AButton>
      </ATooltip>
    </APopconfirm>
  );
}

function refuseButton(record: any) {
  if (!rolePermission.hasOne(AppPermissions.Friend.Request.UPDATE) || !authStore.userInfo.isTenant) {
    return '';
  }

  return (
    <ATooltip placement="top" title="拒绝">
      <AButton size="small" type="link" onClick={() => showRefuseModal(0, record.id)}>
        <Icon icon="ic:baseline-disabled-by-default" />
      </AButton>
    </ATooltip>
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
    />

    <ACard
      title="申请列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="device-card flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
        />
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
      />
    </ACard>

    <AModal
      v-model:visible="refuseModalVisible"
      title="请输入拒绝原因"
      ok-text="确认"
      cancel-text="取消"
      @ok="handleConfirmRefuse"
    >
      <AInput v-model:value="refuseReason" placeholder="请输入拒绝原因" :rows="4" />
    </AModal>
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
