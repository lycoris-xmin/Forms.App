<script setup lang="tsx">
import { watch } from 'vue';
import { ArrowDownOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { apiUrl, fetchTaobaoPlanTaskAmountList } from '@/service/api';

// interface Props {
//   userInfo: object;
// }

// const props = withDefaults(defineProps<Props>(), {});//预留处理商户
const visible = defineModel<boolean>('visible', {
  default: false
});

const { tableWrapperRef, scrollConfig } = useTableScroll();

const { columns, data, getData, getDataByPage, loading, searchParams, resetSearchParams, mobilePagination } = useTable({
  apiFn: getAccountList,
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { rowSelection } = useTableOperate(data, getData);

function getAccountList(searchFilter: object) {
  if (!visible.value) {
    return Promise.resolve({
      code: 0,
      msg: '',
      data: {
        count: 0,
        list: [],
        pageIndex: 1,
        pageSize: 10
      }
    });
  }
  return fetchTaobaoPlanTaskAmountList(searchFilter);
}

function getColumns() {
  const cols = [
    {
      key: 'importFileName',
      dataIndex: 'importFileName',
      title: '导入文件',
      align: 'center'
    },
    {
      key: 'exportFileName',
      dataIndex: 'exportFileName',
      title: '统计报表',
      align: 'center',
      ellipsis: true
    },
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '导入时间',
      align: 'center',
      width: 200
    }
  ];

  cols.push({
    key: 'operate',
    title: '操作',
    align: 'center',
    width: 140,
    fixed: 'right',
    customRender: ({ record }) => (
      <div class="flex-center gap-4px">
        <ATooltip placement="top" title="下载报表">
          <AButton type="link" size="small" onClick={() => downloadExcel(record.exportFilePath, record.exportFileName)}>
            <ArrowDownOutlined />
          </AButton>
        </ATooltip>
      </div>
    )
  });

  return cols;
}
watch(
  () => visible.value,
  val => {
    if (val) {
      getData();
    }
  }
);

async function downloadExcel(exportFilePath, exportFileName) {
  try {
    const url = `${apiUrl}/api/plantask/excel/account?filePath=${exportFilePath}`;

    const response = await fetch(url, {
      mode: 'cors'
    });

    if (!response.ok) {
      window.$message?.error('下载异常');
      return;
    }

    const blob = await response.blob();
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = exportFileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(a.href);
  } catch (error) {
    window.$message?.error(`下载失败: ${error}`);
  }
}

function closeDrawer() {
  visible.value = false;
}
</script>

<template>
  <ADrawer v-model:open="visible" title="金额统计报表" :width="1000">
    <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
      <AForm
        :model="searchParams"
        :label-col="{
          span: 5,
          md: 7
        }"
      >
        <ARow class="row" :gutter="[16, 16]" wrap>
          <div class="flex-1">
            <!--
 <AFormItem class="m-0">
              <div class="w-full flex-y-center justify-end gap-12px">
                <AButton @click="resetSearchParams()">
                  <template #icon>
                    <icon-ic-round-refresh class="align-sub text-icon" />
                  </template>
                  <span class="ml-8px">重置</span>
                </AButton>
                <AButton type="primary" ghost @click="getDataByPage(1)">
                  <template #icon>
                    <icon-ic-round-search class="align-sub text-icon" />
                  </template>
                  <span class="ml-8px">搜索</span>
                </AButton>
              </div>
            </AFormItem>
-->
          </div>
        </ARow>
      </AForm>

      <ATable
        ref="tableWrapperRef"
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
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="() => closeDrawer">关闭</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.flex-1 {
  padding-right: 10px;
}
</style>
