<script setup lang="tsx">
  import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
  import { getLoggerSystemListApi } from '@/service/api';
  import { loggerLevel } from '@/views/shared/enums';
  import SearchFilter from './modules/search-filter.vue';

  const { tableWrapperRef, scrollConfig } = useTableScroll();

  const { columns, columnChecks, data, getData, getDataByPage, loading, mobilePagination, searchParams, resetSearchParams } = useTable({
    apiFn: getLoggerSystemListApi,
    apiParams: {
      pageIndex: 1,
      pageSize: 20
    },
    columns: getColumns
  });

  const { checkedRowKeys, rowSelection } = useTableOperate(data, getData);

  function getColumns() {
    const cols = [
      {
        key: 'createTime',
        dataIndex: 'createTime',
        title: '日志时间',
        width: 160
      },
      {
        key: 'level',
        dataIndex: 'level',
        title: '日志类型',
        align: 'center',
        width: 100,
        customRender: ({ record }) => {
          const item = loggerLevel.filter(x => x.value === record.level);
          if (item && item.length) {
            return (
              <ATag color={item[0].color} ghost>
                {item[0].label}
              </ATag>
            );
          }

          return (
            <ATag color="purple" ghost>
              未知
            </ATag>
          );
        }
      },
      {
        key: 'className',
        dataIndex: 'className',
        title: '类名',
        width: 200,
        ellipsis: true
      },
      {
        key: 'methodName',
        dataIndex: 'methodName',
        title: '方法名',
        width: 160,
        ellipsis: true
      },
      {
        key: 'message',
        dataIndex: 'message',
        title: '日志信息'
      }
    ];

    return cols;
  }
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter v-model:model="searchParams" :level="loggerLevel" @reset="resetSearchParams" @search="getDataByPage"></SearchFilter>

    <ACard title="日志列表" :bordered="false" :body-style="{ flex: 1, overflow: 'hidden' }" class="flex-col-stretch sm:flex-1-hidden card-wrapper">
      <template #extra>
        <TableHeaderOperation v-model:columns="columnChecks" :disabled-delete="checkedRowKeys.length === 0"></TableHeaderOperation>
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
  </div>
</template>

<style lang="scss" scoped></style>
