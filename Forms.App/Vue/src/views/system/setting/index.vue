<script setup lang="tsx">
  import { EditOutlined } from '@ant-design/icons-vue';
  import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
  import { AppPermissions, useRolePermission } from '@/hooks/custome/useRolePermissions';
  import { getConfigurationListApi } from '@/service/api';
  import SearchFilter from './modules/search-filter.vue';
  import OperateDrawer from './modules/operate-drawer.vue';

  const rolePermission = useRolePermission();

  const { tableWrapperRef, scrollConfig } = useTableScroll();

  const { columns, data, getData, loading, mobilePagination, getDataByPage, searchParams, resetSearchParams } = useTable({
    apiFn: getConfigurationListApi,
    apiParams: {
      pageIndex: 1,
      pageSize: 10
    },
    columns: getColumns
  });

  const { drawerVisible, editingData, handleEdit } = useTableOperate(data, getData);

  function getColumns() {
    const cols = [
      {
        key: 'id',
        dataIndex: 'id',
        title: '配置标识',
        width: 150,
        ellipsis: true
      },
      {
        key: 'configName',
        dataIndex: 'configName',
        title: '配置名称',
        width: 150,
        ellipsis: true
      },
      {
        key: 'value',
        dataIndex: 'value',
        title: '配置值',
        width: 500,
        ellipsis: true
      },
      {
        key: 'operate',
        title: '操作',
        align: 'center',
        width: 80,
        customRender: ({ record }) => editButton(record)
      }
    ];

    return cols;
  }

  function editButton(record) {
    if (!rolePermission.hasOne(AppPermissions.System.Setting.UPDATE)) {
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
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter v-model:model="searchParams" @reset="resetSearchParams" @search="getDataByPage"></SearchFilter>

    <ACard title="配置列表" :bordered="false" :body-style="{ flex: 1, overflow: 'hidden' }" class="flex-col-stretch sm:flex-1-hidden card-wrapper">
      <ATable ref="tableWrapperRef" :columns="columns" :data-source="data" size="small" :scroll="scrollConfig" :loading="loading" row-key="id" :pagination="mobilePagination" class="h-full" />
    </ACard>

    <OperateDrawer v-model:visible="drawerVisible" :row-data="editingData" @submitted="() => getDataByPage()"></OperateDrawer>
  </div>
</template>

<style lang="scss" scoped></style>
