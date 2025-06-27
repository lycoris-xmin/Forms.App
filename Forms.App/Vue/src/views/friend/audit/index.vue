<script setup lang="tsx">
import { onMounted, reactive } from 'vue';
import { DeleteOutlined } from '@ant-design/icons-vue';
import { useRoute } from 'vue-router';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeleteAudit, fetchGetAuditShopList } from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { useAuthStore } from '@/store/modules/auth';
import { Platform as PLATRORM_TYPE } from '@/views/shared/enums';
import SearchFilter from './modules/search-filter.vue';
const route = useRoute();
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
  data,
  getData,
  getDataByPage,
  loading,
  mobilePagination,
  searchParams,
  resetSearchParams,
  reloadColumns
} = useTable({
  apiFn: fetchGetAuditShopList,
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { rowSelection, onDeleted } = useTableOperate(data, getData);

onMounted(async () => {
  if (route.params && route.params.status === 1) {
    searchParams.status = 1;
  }
  reloadColumns();
});

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteAudit([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

function getColumns() {
  const cols = [
    {
      key: 'platform',
      dataIndex: 'platform',
      title: '平台',
      align: 'center',
      width: 120,
      customRender: ({ record }) => {
        const item = PLATRORM_TYPE.find(x => x.value === record.platform);
        return <ATag color={item?.color}>{item?.label}</ATag>;
      }
    },
    {
      key: '店铺名称',
      dataIndex: 'shopName',
      title: '店铺名称',
      width: 180
    },
    {
      key: '好友昵称',
      dataIndex: 'nickName',
      title: '好友昵称',
      width: 170
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '审核时间',
      align: 'center',
      width: 170
    }
  ];

  cols.push({
    key: 'operate',
    title: '操作',
    align: 'center',
    width: 100,
    fixed: 'right',
    customRender: ({ record }) => <div class="flex-center gap-4px">{deleteButton(record)}</div>
  });

  return cols;
}

function deleteButton(record) {
  if (!rolePermission.hasOne(AppPermissions.Friend.Audit.DELETE) || !authStore.userInfo.isTenant) {
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

.flex-center {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
