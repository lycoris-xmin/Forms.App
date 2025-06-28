<script setup lang="tsx">
  import { nextTick, reactive } from 'vue';
  import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
  import { getLoggerReqeustListApi, getLoggerRequestDetailApi } from '@/service/api';
  import { loggerHttpMethod } from '@/views/shared/enums';
  import LoadingWrap from '@/components/loading/index.vue';
  import { useClipboard } from '@/hooks/custome/useClipboard';
  import SearchFilter from './modules/search-filter.vue';

  const model = reactive({
    expandedRowKeys: [],
    row: {}
  });

  const { copy } = useClipboard();

  const { tableWrapperRef, scrollConfig } = useTableScroll();

  const { columns, columnChecks, data, getData, getDataByPage, loading, mobilePagination, searchParams, resetSearchParams } = useTable({
    apiFn: (...args) => {
      model.expandedRowKeys = [];
      model.row = {};
      return getLoggerReqeustListApi(...args);
    },
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
        key: 'httpMethod',
        dataIndex: 'httpMethod',
        title: '请求方式',
        align: 'center',
        width: 100,
        customRender: ({ record }) => {
          const item = loggerHttpMethod.filter(x => x.value === record.httpMethod);
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
        key: 'traceId',
        dataIndex: 'traceId',
        title: '请求标识',
        width: 270
      },
      {
        key: 'url',
        dataIndex: 'url',
        title: '请求地址',
        minWidth: 160,
        ellipsis: true,
        customRender: ({ record }) => {
          return (
            <span class="request-url" onClick={() => copy(record.url)}>
              {record.url}
            </span>
          );
        }
      },
      {
        key: 'httpStatusCode',
        dataIndex: 'httpStatusCode',
        title: '状态码',
        width: 100,
        align: 'center',
        customRender: ({ record }) => {
          if (record.httpStatusCode < 300) {
            return (
              <ATag color="success" ghost>
                {record.httpStatusCode}
              </ATag>
            );
          } else if (record.httpStatusCode < 500) {
            return (
              <ATag color="warning" ghost>
                {record.httpStatusCode}
              </ATag>
            );
          }
          return (
            <ATag color="error" ghost>
              {record.httpStatusCode}
            </ATag>
          );
        }
      },
      {
        key: 'requestTime',
        dataIndex: 'requestTime',
        title: '请求时间',
        width: 200,
        align: 'center'
      },
      {
        key: 'responseTime',
        dataIndex: 'responseTime',
        title: '响应时间',
        width: 200,
        align: 'center'
      },
      {
        key: 'milliseconds',
        dataIndex: 'milliseconds',
        title: '耗时',
        width: 100,
        align: 'right',
        customRender: ({ record }) => {
          if (record.milliseconds <= 1500) {
            return <span class="text-success">{record.milliseconds}ms</span>;
          } else if (record.milliseconds <= 3000) {
            return <span class="text-info">{record.milliseconds}ms</span>;
          } else if (record.milliseconds <= 10000) {
            return <span class="text-warn">{record.milliseconds}ms</span>;
          } else if (record.milliseconds <= 15000) {
            return <span class="text-purple">{record.milliseconds}ms</span>;
          }
          return <span class="text-danger">{record.milliseconds}ms</span>;
        }
      },
      {
        key: 'requestIP',
        dataIndex: 'requestIP',
        title: '客户端IP',
        width: 150,
        align: 'center'
      },
      {
        key: 'requestIPAddress',
        dataIndex: 'requestIPAddress',
        title: '归属地',
        width: 160
      }
    ];

    return cols;
  }

  async function expandHandler(expanded, record) {
    if (!expanded) {
      return;
    }

    if (model.row[record.id] && Object.keys(model.row[record.id]).length) {
      return;
    }

    model.row[record.id] = {};

    model.row[record.id].loading = true;
    try {
      //
      const { data: res, error } = await getLoggerRequestDetailApi(record.id);
      if (!error && res && res.code === 0) {
        model.row[record.id] = res.data;
      }
    } finally {
      nextTick(() => {
        model.row[record.id].loading = false;
      });
    }
  }

  function jsonFormat(str) {
    try {
      return JSON.stringify(JSON.parse(str));
    } catch {
      return str;
    }
  }
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter v-model:model="searchParams" :http-method="loggerHttpMethod" @reset="resetSearchParams" @search="getDataByPage"></SearchFilter>

    <ACard title="日志列表" :bordered="false" :body-style="{ flex: 1, overflow: 'hidden' }" class="flex-col-stretch sm:flex-1-hidden card-wrapper">
      <template #extra>
        <TableHeaderOperation v-model:columns="columnChecks" :disabled-delete="checkedRowKeys.length === 0"></TableHeaderOperation>
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
        @expand="expandHandler"
      >
        <template #expandedRowRender="{ record }">
          <div class="expand-body">
            <ADescriptions bordered :column="1">
              <ADescriptionsItem v-if="model.row[record.id]?.request" label="请求内容" :span="1">
                <div class="v-center flex">
                  <div class="content">
                    <p>{{ jsonFormat(model.row[record.id].request) }}</p>
                  </div>
                  <div class="btn center flex">
                    <AButton size="small" type="primary" ghost @click="copy(model.row[record.id]?.request || '')">复制</AButton>
                  </div>
                </div>
              </ADescriptionsItem>
              <ADescriptionsItem v-if="model.row[record.id]?.response && record.httpStatusCode < 500" label="响应内容" :span="1">
                <div class="v-center flex">
                  <div class="content">
                    <p v-if="model.row[record.id].response !== model.row[record.id]?.exception">
                      {{ jsonFormat(model.row[record.id].response) }}
                    </p>
                  </div>
                  <div class="btn center flex">
                    <AButton v-if="model.row[record.id]?.response !== model.row[record.id]?.exception" size="small" type="primary" ghost @click="copy(model.row[record.id]?.response || '')">复制</AButton>
                  </div>
                </div>
              </ADescriptionsItem>
              <ADescriptionsItem v-if="model.row[record.id]?.exception" label="异常信息" :span="1">
                <div class="v-center flex">
                  <div class="content">
                    <p>
                      {{ model.row[record.id]?.exception || '' }}
                    </p>
                  </div>
                  <div class="btn center flex">
                    <AButton size="small" type="primary" ghost @click="copy(model.row[record.id]?.exception || '')">复制</AButton>
                  </div>
                </div>
              </ADescriptionsItem>
              <ADescriptionsItem v-if="model.row[record.id]?.stackTrace" label="异常堆栈" :span="1">
                <div class="v-center flex">
                  <div class="content">
                    <p v-if="!model.row[record.id].stackTrace.includes('at ')">
                      {{ model.row[record.id].stackTrace || '' }}
                    </p>
                    <ul v-for="item in model.row[record.id].stackTrace.trim().split('at ').filter(Boolean)" :key="item">
                      <li class="stack-trace">at {{ item }}</li>
                    </ul>
                  </div>
                  <div class="btn center flex">
                    <AButton size="small" type="primary" ghost @click="copy(model.row[record.id]?.stackTrace || '')">复制</AButton>
                  </div>
                </div>
              </ADescriptionsItem>
              <ADescriptionsItem v-if="!model.row[record.id]?.request && !model.row[record.id]?.response && !model.row[record.id]?.exception && !model.row[record.id]?.stackTrace">
                <p class="text-center">无其他详细信息</p>
              </ADescriptionsItem>
            </ADescriptions>
            <LoadingWrap v-if="model.row[record.id]?.loading || false" :loading="model.row[record.id]?.loading || false"></LoadingWrap>
          </div>
        </template>
      </ATable>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
  :deep(.request-url) {
    cursor: pointer;
    transition: 0.3s ease-in-out;

    &:hover {
      color: var(--color-purple);
    }
  }

  .expand-body {
    position: relative;

    :deep(.ant-descriptions-item-label) {
      width: 130px;
    }

    .content {
      flex: 1;
    }

    .btn {
      flex-shrink: 0;
      width: 100px;
    }

    .stack-trace {
      margin-bottom: 10px;

      &:last-child {
        margin-bottom: 0;
      }
    }
  }
</style>
