<script setup lang="tsx">
  import { reactive } from 'vue';
  import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
  import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
  import { schedulerStatus as SCHEDULER_STATUS, triggerType as TRIGGER_TYPE } from '@/views/shared/enums';
  import { AppPermissions, useRolePermission } from '@/hooks/custome/useRolePermissions';
  import { getSchedulerListApi } from '@/service/api';
  import type { EnumMap } from '@/views/shared/types';
  import OperateDrawer from './modules/operate-drawer.vue';

  interface Model {
    jobGroup: Array<EnumMap>;
    rowData?: Api.Scheduler.Data;
    recordVisible: boolean;
  }

  const rolePermission = useRolePermission();

  const { tableWrapperRef, scrollConfig } = useTableScroll();

  const { columns, columnChecks, data, getData, getDataByPage, loading, mobilePagination, searchParams, resetSearchParams } = useTable({
    apiFn: getSchedulerListApi,
    apiParams: {
      pageIndex: 1,
      pageSize: 10
    },
    columns: getColumns
  });

  const { drawerVisible, operateType, editingData, handleAdd, handleEdit, checkedRowKeys, rowSelection, onBatchDeleted, onDeleted } = useTableOperate(data, getData);

  const model: Model = reactive({
    jobGroup: [],
    rowData: undefined,
    recordVisible: false
  });

  function getColumns() {
    const cols = [
      {
        key: 'jobName',
        dataIndex: 'jobName',
        title: '任务名称',
        width: 150,
        ellipsis: true
      },
      {
        key: 'jobGroup',
        dataIndex: 'jobGroup',
        title: '任务分组',
        width: 150,
        ellipsis: true
      },
      {
        key: 'triggerType',
        dataIndex: 'triggerType',
        title: '定时类型',
        width: 100,
        align: 'center',
        customRender: ({ record }) => {
          const item = TRIGGER_TYPE.filter(x => x.value === record.triggerType);
          return <ATag color={item[0].color}> {item[0].label} </ATag>;
        }
      },
      {
        key: 'trigger',
        dataIndex: 'trigger',
        title: '定时参数',
        width: 100,
        align: 'center',
        customRender: ({ record }) => {
          if (record.triggerType === 0) {
            return record.cron;
          }
          return `${record.intervalSecond}秒/次`;
        }
      },
      {
        key: 'runTimes',
        dataIndex: 'runTimes',
        title: '循环次数',
        width: 100,
        align: 'center',
        customRender: ({ record }) => {
          if (record.runTimes === 0) {
            return <ATag color="pink">无限循环</ATag>;
          }

          return <ATag color="processing">{record.runTimes}</ATag>;
        }
      },
      {
        key: 'status',
        dataIndex: 'status',
        title: '状态',
        align: 'center',
        width: 100,
        customRender: ({ record }) => {
          const item = SCHEDULER_STATUS.filter(x => x.value === record.status);
          return <ATag color={item[0].color}> {item[0].label} </ATag>;
        }
      },
      {
        key: 'runCount',
        dataIndex: 'runCount',
        title: '执行次数',
        width: 100,
        align: 'center',
        customRender: ({ record }) => {
          if (record.runCount === 0) {
            return <span class="text-danger">0</span>;
          }
          return <span class="text-success">{record.runCount}</span>;
        }
      },
      {
        key: 'previousFireTime',
        dataIndex: 'previousFireTime',
        title: '上一次执行时间',
        width: 160,
        align: 'center',
        customRender: ({ record }) => {
          return record.previousFireTime || '-';
        }
      },
      {
        key: 'nextFireTime',
        dataIndex: 'nextFireTime',
        title: '下一次执行时间',
        width: 160,
        align: 'center',
        customRender: ({ record }) => {
          return record.nextFireTime || '-';
        }
      },
      {
        key: 'beginTime',
        dataIndex: 'beginTime',
        title: '开始时间',
        width: 160,
        align: 'center',
        visible: false
      },
      {
        key: 'endTime',
        dataIndex: 'endTime',
        title: '结束时间',
        width: 160,
        align: 'center',
        visible: false,
        customRender: ({ record }) => {
          return record.endTime || '永久';
        }
      },
      {
        key: 'operate',
        title: '操作',
        align: 'center',
        width: 100,
        customRender: ({ record }) => (
          <div class="flex-center gap-4px">
            {recordButton(record)}
            {editButton(record)}
            {deleteButton(record)}
          </div>
        )
      }
    ];

    return cols;
  }

  function recordButton(record) {
    return (
      <ATooltip placement="top" title="执行记录">
        <AButton size="small" type="link" onClick={() => recordHandler(record)}>
          <EditOutlined />
        </AButton>
      </ATooltip>
    );
  }

  function editButton(record) {
    if (!record.canDelete) {
      return '';
    } else if (!rolePermission.hasOne(AppPermissions.System.User.UPDATE)) {
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
    if (!record.canDelete) {
      return '';
    } else if (!rolePermission.hasOne(AppPermissions.System.User.DELETE)) {
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

  function recordHandler(record) {
    model.rowData = record;
    model.recordVisible = true;
  }

  async function deleteHandler(id: string) {
    // const { data: res, error } = await deleteUserApi([id]);

    // if (!error && res && res.code === 0) {
    //   onDeleted();
    // }
    console.log(id);

    onDeleted();
  }

  async function batchDeleteHandler() {
    const ids = checkedRowKeys.value;
    if (ids && ids.length) {
      // const { data: res, error } = await deleteUserApi(ids);
      // if (!error && res && res.code === 0) {
      //   onBatchDeleted();
      // }
      onBatchDeleted();
    }
  }
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter v-model:model="searchParams" :job-group="model.jobGroup" :trigger-type="TRIGGER_TYPE" :status="SCHEDULER_STATUS" @reset="resetSearchParams" @search="() => getDataByPage(1)"></SearchFilter>

    <ACard title="任务列表" :bordered="false" :body-style="{ flex: 1, overflow: 'hidden' }" class="flex-col-stretch sm:flex-1-hidden card-wrapper">
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="rolePermission.hasOne(AppPermissions.System.User.CREATE)"
          :delete-btn="rolePermission.hasOne(AppPermissions.System.User.DELETE)"
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
    </ACard>

    <OperateDrawer v-model:visible="drawerVisible" :jog-group="model.jobGroup" :operate-type="operateType" :row-data="editingData" :trigger-type="TRIGGER_TYPE" @submitted="getDataByPage"></OperateDrawer>
  </div>
</template>

<style scoped></style>
