<script setup lang="tsx">
import { reactive } from 'vue';
import { FileTextOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchDeviceCommandUploadLog, fetchGetDeviceCommandDetail, fetchGetDeviceCommandList } from '@/service/api';
import LoadingWrap from '@/components/loading/index.vue';
import { commandStatus as COMMAND_STATUS, commandType as COMMAND_TYPE } from '@/views/shared/enums';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { copyToClipboard, downloadFile } from '@/utils/helper';
import SearchFilter from './modules/search-filter.vue';
import CreateDrawer from './modules/create-drawer.vue';
import DetailDrawer from './modules/device-scriptlog.vue';

type Model = {
  rowDetail: Record<string, any>;
  expandedRowKeys: Array<any>;
  detailVisible: boolean;
  id: string;
};

const rolePermission = useRolePermission();

const model = reactive<Model>({
  rowDetail: {},
  expandedRowKeys: [],
  detailVisible: false,
  id: ''
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
  apiFn: filter => {
    model.rowDetail = {};
    model.expandedRowKeys = [];
    return fetchGetDeviceCommandList(filter);
  },
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { drawerVisible, handleAdd } = useTableOperate(data, getData);

function getColumns() {
  const cols = [
    {
      key: 'id',
      dataIndex: 'id',
      title: '指令编号',
      align: 'center',
      width: 180
    },
    {
      key: 'deviceId',
      dataIndex: 'deviceId',
      title: '执行设备',
      align: 'center',
      width: 280,
      ellipsis: true,
      customRender: ({ record }) => {
        if (!record.deviceId || record.deviceId === '0') {
          return <ATag color="error">设备已删除</ATag>;
        }
        return (
          <p>
            {record.deviceName} ({record.deviceId})
          </p>
        );
      }
    },
    {
      key: 'taskId',
      dataIndex: 'taskId',
      title: '所属任务',
      align: 'center',
      width: 220,
      customRender: ({ record }) => {
        if (record.taskId && record.taskId !== '0') {
          return <p>{record.taskId}</p>;
        }
        return <ATag color="default">-</ATag>;
      }
    },
    {
      key: 'command',
      dataIndex: 'command',
      title: '执行指令',
      align: 'center',
      width: 150,
      customRender: ({ record }) => {
        const commandType = COMMAND_TYPE.find(x => x.value === record.command);
        return commandType ? <ATag color={commandType.color}>{commandType.label}</ATag> : '-';
      }
    },
    {
      key: 'status',
      dataIndex: 'status',
      title: '状态',
      align: 'center',
      width: 100,
      customRender: ({ record }) => {
        const commandStatus = COMMAND_STATUS.filter(x => x.value === record.status)[0];

        // 如果状态为失败且有异常备注，添加tooltip
        if (record.status === 31) {
          // 31 对应 CommandStatusEnum.FAILED
          return (
            <ATooltip placement="top" title={<span>{record.errorRemark || '未知异常'}</span>}>
              <ATag color={commandStatus.color}>{commandStatus.label}</ATag>
            </ATooltip>
          );
        }

        // 正常情况直接显示状态标签
        return <ATag color={commandStatus.color}>{commandStatus.label}</ATag>;
      }
    },
    {
      key: 'ipAddress',
      dataIndex: 'ipAddress',
      title: '设备IP',
      align: 'center',
      ellipsis: true,
      width: 180,
      customRender: ({ record }) => {
        if (record.ipAddress && record.ip) {
          return `${record.ip}(${record.ipAddress})`;
        } else if (record.ip) {
          return `${record.ip}(未知)`;
        }
        return '未知';
      }
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '添加时间',
      align: 'center',
      width: 200
    },
    {
      key: 'assginTime',
      dataIndex: 'assginTime',
      title: '执行时间',
      align: 'center',
      width: 200
    },
    {
      key: 'completeTime',
      dataIndex: 'completeTime',
      title: '完成时间',
      align: 'center',
      width: 200
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center',
      fixed: 'right',
      width: 120,
      customRender: ({ record }) => (
        <div class="flex-center gap-4px">
          <DetailButton record={record} />
        </div>
      )
    }
  ];

  return cols;
}

function DetailButton({ record }) {
  return (
    <ATooltip placement="top" title="详情">
      <AButton type="link" size="small" onClick={() => showDetailDrawer(record.id)}>
        <FileTextOutlined />
      </AButton>
    </ATooltip>
  );
}

function showDetailDrawer(id: string) {
  model.id = id;
  model.detailVisible = true;
}

async function expandHandler(expanded, record) {
  if (!expanded) {
    return;
  }

  if (model.rowDetail[record.id] && Object.keys(model.rowDetail[record.id]).length) {
    return;
  }

  model.rowDetail[record.id] = {};

  model.rowDetail[record.id].loading = true;

  try {
    const { data: res, error } = await fetchGetDeviceCommandDetail(record.id);
    if (!error && res && res.code === 0 && res.data) {
      Object.assign(model.rowDetail[record.id], res.data);
    }
  } finally {
    model.rowDetail[record.id].loading = false;
  }
}

async function uploadLogFile(record) {
  window.$modal?.confirm({
    title: '上传提示',
    cancelText: '取消',
    okText: '确定',
    content: '日志上传指令需要等待设备空闲，请稍后再来查看',
    async onOk() {
      record.loading = true;
      try {
        const { data: res, error } = await fetchDeviceCommandUploadLog(record.id);
        if (!error && res && res.code === 0) {
          window.$message?.success('指令下发成功，请耐心等待！');
          getData();
        }
      } finally {
        record.loading = false;
      }
    }
  });
}
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :command="COMMAND_TYPE"
      :status="COMMAND_STATUS"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    ></SearchFilter>

    <ACard
      title="指令列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.Maintain.Commnad.CREATE)"
          :delete-btn="false"
          @add="handleAdd"
        ></TableHeaderOperation>
      </template>

      <ATable
        ref="tableWrapperRef"
        v-model:expanded-row-keys="model.expandedRowKeys"
        :columns="columns"
        :data-source="data"
        size="small"
        :scroll="scrollConfig"
        :loading="loading"
        row-key="id"
        :pagination="mobilePagination"
        class="h-full"
        @expand="expandHandler"
      >
        <template #expandedRowRender="{ record }">
          <div class="expande-row">
            <ADescriptions bordered :column="3">
              <ADescriptionsItem label="执行内容" :span="3">
                <div class="content">
                  <div>
                    <p>{{ model.rowDetail[record.id]?.data || '' }}</p>
                  </div>
                  <div>
                    <AButton
                      v-if="model.rowDetail[record.id]?.data"
                      size="small"
                      type="primary"
                      ghost
                      @click="copyToClipboard(model.rowDetail[record.id]?.data || '-')"
                    >
                      复制
                    </AButton>
                  </div>
                </div>
              </ADescriptionsItem>
              <ADescriptionsItem label="执行回执" :span="3">
                <div class="content">
                  <div>
                    <p>{{ model.rowDetail[record.id]?.result || '-' }}</p>
                  </div>
                  <div>
                    <AButton
                      v-if="model.rowDetail[record.id]?.result"
                      size="small"
                      type="primary"
                      ghost
                      @click="copyToClipboard(model.rowDetail[record.id]?.result || '')"
                    >
                      复制
                    </AButton>
                  </div>
                </div>
              </ADescriptionsItem>
              <ADescriptionsItem label="日志文件" :span="3">
                <div class="v-center flex gap-10px">
                  <AButton
                    v-if="model.rowDetail[record.id]?.logFile"
                    size="small"
                    type="primary"
                    ghost
                    @click="downloadFile(model.rowDetail[record.id]?.logFile)"
                  >
                    下载日志
                  </AButton>

                  <AButton v-else size="small" danger ghost :loading="record.loading" @click="uploadLogFile(record)">
                    设备端上传日志
                  </AButton>

                  <AButton
                    v-if="model.rowDetail[record.id]?.screenshot"
                    size="small"
                    type="primary"
                    ghost
                    @click="downloadFile(model.rowDetail[record.id]?.screenshot)"
                  >
                    下载截屏
                  </AButton>
                </div>
              </ADescriptionsItem>
            </ADescriptions>
            <LoadingWrap
              v-if="model.rowDetail[record.id]?.loading || false"
              :loading="model.rowDetail[record.id]?.loading || false"
              bg-color="#fff"
            ></LoadingWrap>
          </div>
        </template>
      </ATable>

      <CreateDrawer v-model:visible="drawerVisible" @submitted="() => getDataByPage()"></CreateDrawer>
      <DetailDrawer
        v-model:visible="model.detailVisible"
        :command-id="model.id"
        @close="() => (model.detailVisible = false)"
      ></DetailDrawer>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
.expande-row {
  position: relative;

  :deep(.ant-descriptions-item-label),
  :deep(.ant-descriptions-item-content) {
    padding: 8px 15px;
  }

  :deep(.ant-descriptions-item-label) {
    width: 150px !important;
  }

  .content {
    display: grid;
    grid-template-columns: minmax(100px, 1fr) 100px;
    gap: 10px;

    > div {
      display: flex;
      align-items: center;
    }
  }
}
</style>
