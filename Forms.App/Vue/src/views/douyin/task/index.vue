<script setup lang="tsx">
import { onMounted, reactive } from 'vue';
import {
  ArrowDownOutlined,
  BranchesOutlined,
  CloseOutlined,
  DeleteOutlined,
  EyeOutlined,
  FileSyncOutlined,
  RedoOutlined
} from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { useAuthStore } from '@/store/modules/auth';
import {
  apiUrl,
  fetchCheckExportPlanTaskComplete,
  fetchDeleteDouyinPlanTask,
  fetchDouyinShopName,
  fetchExportDouyinPlanTask,
  fetchGetDouyinPlanTaskList,
  fetchStopPlanTask
} from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import {
  planTaskMode as PLANTASK_MODE,
  planTaskStatus as PLANTASK_STATUS,
  planTaskStep as PLANTASK_STEP,
  platform as PLATFORM
} from '@/views/shared/enums';
import { copyToClipboard } from '@/utils/helper';
import { imgFallback } from '@/data/empty.json';
import CompletionDrawer from './modules/completion-drawer.vue';
import SearchFilter from './modules/search-filter.vue';
import AgainDrawer from './modules/again-drawer.vue';
import CreateDrawer from './modules/create-drawer.vue';
import DetailModal from './modules/modules/detail-modal.vue';

const authStore = useAuthStore();

const rolePermission = useRolePermission();

const { tableWrapperRef, scrollConfig } = useTableScroll();

type Switch = {
  id: string;
  loading: boolean;
};

type Model = {
  switch: Switch;
  createVisible: boolean;
  againLoading: boolean;
  detailVisible: boolean;
  confirmProductVisible: boolean;
  commentVisible: boolean;
  againVisible: boolean;
  completionVisible: boolean;
  editRow: object;
  exportLoading: boolean;
  shopNameEnum: Array<any>;
};

const model = reactive<Model>({
  switch: {
    id: '',
    loading: false
  },
  createVisible: false,
  againLoading: false,
  detailVisible: false,
  commentVisible: false,
  againVisible: false,
  completionVisible: false,
  editRow: {},
  exportLoading: false,
  shopNameEnum: []
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
  apiFn: (...args) => {
    return fetchGetDouyinPlanTaskList(...args);
  },
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { onBatchDeleted, onDeleted, checkedRowKeys, rowSelection } = useTableOperate(data, getData);

onMounted(async () => {
  searchParams.platform = 30;
  await getShopNameEnum();
});

async function getShopNameEnum() {
  const { data: res, error } = await fetchDouyinShopName('');
  if (!error && res && res.code === 0) {
    model.shopNameEnum = res.data.list;
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteDouyinPlanTask([id]);

  if (!error && res && res.code === 0) {
    onDeleted();
  }
}

async function batchDeleteHandler() {
  const ids = checkedRowKeys.value;
  if (ids && ids.length) {
    const { data: res, error } = await fetchDeleteDouyinPlanTask(ids);
    if (!error && res && res.code === 0) {
      onBatchDeleted();
    }
  }
}

function againHandler(record) {
  model.editRow = record;
  model.againVisible = true;
}

function completionHandler(record) {
  model.editRow = record;
  model.completionVisible = true;
}

function completionButton(record) {
  if (!authStore.userInfo.isTenant) {
    return '';
  }

  if (![30, 31, 40, 41].includes(record.status)) {
    return '';
  }

  return (
    <ATooltip placement="top" title="任务信息">
      <AButton type="link" size="small" onClick={() => completionHandler(record)}>
        <BranchesOutlined />
      </AButton>
    </ATooltip>
  );
}

function getColumns() {
  const cols = [
    {
      key: 'title',
      dataIndex: 'title',
      title: '任务商品',
      width: 500,
      ellipsis: true,
      customRender: ({ record }) => {
        let mode = PLANTASK_MODE.filter(x => x.value === record.mode);
        mode = mode.length ? mode[0] : { label: '未知', value: record.mode, color: '' };

        let platform = PLATFORM.filter(x => x.value === record.platform);
        platform = platform.length ? platform[0] : { label: '未知', value: record.platform, color: '' };
        function renderDevice() {
          if (record.mode === 0) {
            return '';
          }

          if (record.mode !== 20 && (record.status === 0 || record.status === 10)) {
            return '';
          }

          if (record.deviceName && record.deviceId !== '0') {
            return (
              <ATag color="cyan" onClick={() => copyToClipboard(record.deviceId)} style="cursor:pointer;">
                执行设备：{record.deviceName} ({record.deviceId})
              </ATag>
            );
          }

          return <ATag color="error">执行设备：设备已删除</ATag>;
        }

        return (
          <div class="plantask-product">
            <div class="v-center flex gap-10px">
              <ATag color="default" style="cursor:pointer;" onClick={() => copyToClipboard(record.id)}>
                任务编号：{record.id}
              </ATag>
              <ATag color={mode.color}>任务模式：{mode.label}</ATag>
              {renderDevice()}
            </div>
            <div class="flex">
              <div class="plantask-image">
                <AImage width={120} height={120} src={record.image} fallback={imgFallback} />
              </div>
              <div class="plantask-info">
                <div class="plantask-title v-center flex">
                  <ATag color={platform.color}>DY联盟</ATag>
                  <ATooltip placement="top" title={record.title}>
                    <a href={record.productUrl} target="_blank">
                      {record.title}
                    </a>
                  </ATooltip>
                </div>
                <p class="plantask-shop">
                  <span class="label">店铺名称：</span>
                  {record.shopName}
                </p>
                <p class="plantask-sku v-center flex">
                  <span class="label">任务SKU：</span>
                  {renderSku(record.skuList)}
                </p>
                <div class="v-center flex gap-10px">
                  <p class="plantask-price f-12px">
                    <span class="label">商品单价：</span>
                    <span>{renderCNY(record.price)}</span>
                  </p>
                  <p class="f-12px">
                    <span class="label">购买件数：</span>
                    <span>x {record.count}</span>
                  </p>
                  <p class="plantask-price f-12px">
                    <span class="label">商品总价：</span>
                    <span>{renderCNY(record.totalPrice, true)}</span>
                  </p>
                </div>
                <p class="plantask-keyword f-12px">
                  <span class="text-right">宝贝排名：</span>
                  <span title={record.ranking || '-'}>{record.ranking > 0 ? record.ranking : '-'}</span>
                </p>
                <p class="plantask-keyword f-12px" title={`关键词：${record.keyword || ''}`}>
                  <span class="label">关键词：</span>
                  {record.keyword}
                </p>
              </div>
            </div>
          </div>
        );
      }
    },
    {
      key: 'account',
      dataIndex: 'account',
      title: '任务信息',
      width: 300,
      ellipsis: true,
      customRender: ({ record }) => {
        let ip = record.ip;
        if (ip && record.ipAddress) {
          ip = `${ip} (${record.ipAddress})`;
        }

        return (
          <div style="font-size:14px">
            <p class="mb-6px" style="display:grid;grid-template-columns:70px 1fr">
              <span class="text-right">DY号：</span>
              <span title={record.account || '-'}>{record.account || '-'}</span>
            </p>
            <p class="mb-6px" style="display:grid;grid-template-columns:70px 1fr">
              <span class="text-right">订单号：</span>
              <span title={record.orderId || '-'}>{record.orderId || '-'}</span>
            </p>
            <p class="mb-6px" style="display:grid;grid-template-columns:70px 1fr">
              <span class="text-right">下单IP：</span>
              <span title={ip || '-'}>{ip || '-'}</span>
            </p>
            <p class="mb-6px" style="display:grid;grid-template-columns:70px 1fr">
              <span class="text-right">收货地址：</span>
              <span title={record.address || '-'}>{record.address || '-'}</span>
            </p>
          </div>
        );
      }
    },
    {
      key: 'success',
      dataIndex: 'success',
      title: '购买状态',
      width: 100,
      align: 'center',
      customRender: ({ record }) => {
        if (record.status >= 30) {
          if (record.success) {
            return <ATag color="success">购买成功</ATag>;
          }
          return <ATag color="error">购买失败</ATag>;
        }
        return '-';
      }
    },
    {
      key: 'status',
      dataIndex: 'status',
      title: '状态',
      width: 100,
      align: 'center',
      customRender: ({ record }) => {
        if (record.status === 20) {
          const step = PLANTASK_STEP.filter(x => x.value === record.step);
          if (step && step.length) {
            return <ATag color={step[0].color}>{step[0].label}</ATag>;
          }
        }

        let item = PLANTASK_STATUS.filter(x => x.value === record.status);
        item = item.length ? item[0] : { label: '未知', value: record.platform, color: 'error' };

        if (record.status === 31 || record.status === 41) {
          return (
            <ATooltip placement="top" title={<span>{record.remark || '程序执行异常'}</span>}>
              <ATag color={item.color}>{item.label}</ATag>
            </ATooltip>
          );
        }

        return <ATag color={item.color}>{item.label}</ATag>;
      }
    },
    {
      key: 'note',
      dataIndex: 'note',
      title: '任务备注',
      width: 200,
      ellipsis: true
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '添加时间',
      align: 'center',
      width: 200
    }
  ];

  if (!authStore.userInfo.isTenant) {
    cols.push({
      key: 'creator',
      dataIndex: 'creator',
      title: '所属商户',
      align: 'center',
      width: 130
    });
  } else if (authStore.userInfo.isTenant && !authStore.userInfo.isTenantUser) {
    cols.push({
      key: 'creator',
      dataIndex: 'creator',
      title: '创建人',
      align: 'center',
      width: 130
    });
  }

  cols.push({
    key: 'operate',
    title: '操作',
    align: 'center',
    width: 140,
    fixed: 'right',
    customRender: ({ record }) => (
      <div class="flex-center gap-4px">
        {detailButton(record)}
        {againButton(record)}
        {stopButton(record)}
        {deleteButton(record)}
        {completionButton(record)}
      </div>
    )
  });

  return cols;
}

function detailButton(record) {
  if (record.status === 0 || record.status === 5 || record.status > 11) {
    return (
      <ATooltip placement="top" title="任务详情">
        <AButton type="link" size="small" onClick={() => viewDetailHandler(record)}>
          <EyeOutlined />
        </AButton>
      </ATooltip>
    );
  }

  return '';
}

function deleteButton(record) {
  if (rolePermission.hasOne(AppPermissions.DY.Task.DELETE) && authStore.userInfo.isTenant) {
    if ([30, 31, 40, 41, 11, 20].includes(record.status)) {
      return '';
    }
    if (record.status === 0 || record.status === 10 || record.status === 100) {
      return (
        <APopconfirm title="确认删除吗？" onConfirm={() => deleteHandler(record.id)}>
          <ATooltip placement="top" title="删除">
            <AButton danger size="small" type="link">
              <DeleteOutlined />
            </AButton>
          </ATooltip>
        </APopconfirm>
      );
    } else if (record.mode === 10 || record.mode === 20) {
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
  }

  return '';
}

function stopButton(record) {
  if (rolePermission.hasOne(AppPermissions.DY.Task.UPDATE) && authStore.userInfo.isTenant) {
    if ([20, 21].includes(record.status)) {
      return (
        <ATooltip placement="top" title="停止任务">
          <AButton danger size="small" type="link" onClick={() => stopPlanTaskHandler(record.id)}>
            <CloseOutlined />
          </AButton>
        </ATooltip>
      );
    }
  }

  return '';
}

function againButton(record) {
  if (rolePermission.hasOne(AppPermissions.DY.Task.CREATE) && authStore.userInfo.isTenant) {
    return (
      <ATooltip placement="top" title="快速发布">
        <AButton loading={model.againLoading} type="link" size="small" onClick={() => againHandler(record)}>
          <FileSyncOutlined />
        </AButton>
      </ATooltip>
    );
  }

  return '';
}

function renderSku(skuList) {
  if (!skuList || !skuList.length) {
    return <ATag>无SKU</ATag>;
  }

  return skuList.map(item => (
    <ATag key={item} color="processing">
      {item}
    </ATag>
  ));
}

function renderCNY(value, blod = false) {
  return (
    <span class="cny" style={blod ? 'font-weight:600;letter-spacing: 1px;' : ''}>
      {value}
    </span>
  );
}

function createHandler() {
  model.createVisible = true;
}

function viewDetailHandler(record) {
  model.editRow = record;
  model.detailVisible = true;
}

async function exportHandler() {
  if (!searchParams.beginTime || !searchParams.endTime) {
    window.$message?.warning('请选择要下载的订单报表的时间范围');
    return;
  }

  await getDataByPage(1);

  if (data.value.length <= 0) {
    return;
  }

  window.$message?.success('正在生成报表');

  model.exportLoading = true;
  try {
    const { data: res, error } = await fetchExportDouyinPlanTask(searchParams);
    if (!error && res && res.code === 0) {
      checkExportComplete(res.data);
    }
  } catch {
    model.exportLoading = false;
  }
}

function checkExportComplete(excelId) {
  let i = 0;
  const timer = setInterval(async () => {
    if (i > 100) {
      window.$message?.error('报表导出失败，请稍后再试');
      clearInterval(timer);
      model.exportLoading = false;
      return;
    }

    const { data: res, error } = await fetchCheckExportPlanTaskComplete(excelId);
    if (!error && res && res.code === 0 && res.data.complete) {
      model.exportLoading = false;
      clearInterval(timer);
      downloadExcel(excelId);
      return;
    }

    i += 1;
  }, 3000);
}

async function downloadExcel(excelId) {
  try {
    const response = await fetch(`${apiUrl}/api/plantask/excel/${excelId}`, {
      mode: 'cors'
    });
    if (!response.ok) {
      window.$message?.error('下载异常');
    }

    const blob = await response.blob();
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = `订单信息报表_${new Date().getTime()}.xlsx`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(a.href); // 释放 URL
  } catch (error) {
    window.$message?.error(`下载失败:${error}`);
  }
}

function stopPlanTaskHandler(id) {
  window.$modal?.confirm({
    title: '停止提示',
    cancelText: '取消',
    okText: '确定',
    content: '停止任务后无法恢复，确定要停止当前任务吗？',
    async onOk() {
      const { data: res, error } = await fetchStopPlanTask(id);
      if (!error && res && res.code === 0) {
        window.$message.success('停止指令已发送，请耐心等待！');
        getDataByPage();
      }
    }
  });
}
</script>

<template>
  <div class="page min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :user-info="authStore.userInfo"
      :platform="PLATFORM"
      :status="PLANTASK_STATUS"
      :mode="PLANTASK_MODE"
      :shop-name-enum="model.shopNameEnum"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    />

    <ACard
      title="任务列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="plantask flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :add-btn="authStore.userInfo.isTenant && rolePermission.hasOne(AppPermissions.DY.Task.CREATE)"
          :delete-btn="authStore.userInfo.isTenant && rolePermission.hasOne(AppPermissions.DY.Task.DELETE)"
          delete-confirm-text="仅会删除未开始与配对中的任务，确认删除吗？"
          @add="createHandler"
          @delete="batchDeleteHandler"
        >
          <AButton
            size="small"
            ghost
            type="primary"
            class="v-center flex"
            :disabled="!data.length"
            @click="() => getDataByPage()"
          >
            <template #icon>
              <RedoOutlined />
            </template>

            <span class="ml-6px">任务进度刷新</span>
          </AButton>
          <AButton
            size="small"
            ghost
            type="primary"
            class="v-center flex"
            :disabled="!data.length"
            :loading="model.exportLoading"
            @click="exportHandler"
          >
            <template #icon>
              <ArrowDownOutlined />
            </template>

            <span class="ml-6px">下载订单报表</span>
          </AButton>
        </TableHeaderOperation>
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
        :row-selection="rowSelection"
        :pagination="mobilePagination"
        class="h-full"
      ></ATable>
    </ACard>

    <CreateDrawer
      v-model:visible="model.createVisible"
      :user-info="authStore.userInfo"
      :mode="PLANTASK_MODE"
      @submitted="getDataByPage"
    ></CreateDrawer>

    <CompletionDrawer
      v-model:visible="model.completionVisible"
      :row-data="model.editRow"
      @submitted="getDataByPage"
    ></CompletionDrawer>

    <AgainDrawer
      v-model:visible="model.againVisible"
      :row-data="model.editRow"
      :user-info="authStore.userInfo"
      :mode="PLANTASK_MODE"
      @submitted="getDataByPage(1)"
    ></AgainDrawer>

    <CompletionDrawer
      v-model:visible="model.completionVisible"
      :row-data="model.editRow"
      @submitted="getDataByPage"
    ></CompletionDrawer>

    <DetailModal :id="model.editRow.id" v-model:visible="model.detailVisible" @submitted="getDataByPage"></DetailModal>
  </div>
</template>

<style lang="scss" scoped>
.plantask {
  :deep(.plantask-product) {
    width: 100%;
    overflow: hidden;

    > .flex {
      &:first-child {
        padding-bottom: 5px;
      }

      &:last-child {
        gap: 10px;
        overflow: hidden;
      }
    }

    .ant-tag {
      font-size: 12px;
      line-height: 16px;
    }

    .plantask-image {
      flex-shrink: 0;
      width: 120px;
      height: 120px;
      border-radius: 5px;
      overflow: hidden;
    }

    .plantask-info {
      height: 100%;
      padding: 5px 0;
      overflow: hidden;
      text-overflow: ellipsis;
      word-wrap: break-word;
      white-space: nowrap;
    }

    .plantask-title {
      padding-bottom: 5px;

      .ant-tag {
        &.tb {
          background-color: rgb(9, 3, 0);
        }
      }

      a {
        overflow: hidden;
        text-overflow: ellipsis;
        word-break: break-word;
        white-space: nowrap;
        font-weight: 600;
        letter-spacing: 1.5px;
      }
    }

    .plantask-shop {
      font-size: 12px;
      padding-bottom: 5px;
    }

    .plantask-sku {
      font-size: 12px;
      padding-bottom: 5px;

      .ant-tag {
        font-size: 12px;
        line-height: 14px;
        overflow: hidden;
        text-overflow: ellipsis;
        word-break: break-all;
        white-space: nowrap;
      }
    }

    .f-12px {
      font-size: 12px;
    }

    .plantask-shop,
    .plantask-sku,
    .plantask-price,
    .plantask-keyword {
      .label {
        display: inline-block;
        width: 60px;
        text-align: right;
      }
    }
  }

  :deep(.screenshot) {
    padding: 0 10px;
    flex-wrap: nowrap;
    position: relative;
    // -webkit-mask-image: linear-gradient(to right, rgba(0, 0, 0, 1) 80%, rgba(0, 0, 0, 0));
    overflow-x: auto;

    .img-div {
      flex-shrink: 0;
      overflow: hidden;
      border-radius: 5px;
      height: 100px;
      width: 50px;
    }
  }
}
</style>
