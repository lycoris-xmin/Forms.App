<script setup lang="tsx">
import { nextTick, reactive } from 'vue';
import { EditOutlined } from '@ant-design/icons-vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import { fetchGetConfigurationList, fetchUpdateConfiguration } from '@/service/api';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';

const rolePermission = useRolePermission();

const { formRef, resetFields } = useAntdForm();

const { defaultRequiredRule } = useFormRules();

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
  apiFn: (...filter) => {
    return fetchGetConfigurationList(...filter);
  },
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: () => [
    {
      key: 'name',
      dataIndex: 'name',
      title: '配置名称',
      width: 350,
      ellipsis: true
    },
    {
      key: 'value',
      dataIndex: 'value',
      title: '配置值'
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center',
      width: 80,
      customRender: ({ record }) => {
        return <div class="flex-center gap-4px">{editButton(record)}</div>;
      }
    }
  ]
});

const { drawerVisible, handleEdit, closeDrawer } = useTableOperate(data, getData);

const model = reactive({
  submitLoading: false,
  id: '',
  name: '',
  value: ''
});

const rules = {
  value: defaultRequiredRule
};

async function reset() {
  await resetFields();
  resetSearchParams();
}

async function search() {
  getData();
}

function editButton(record) {
  if (rolePermission.hasOne(AppPermissions.System.Setting.UPDATE)) {
    return (
      <ATooltip placement="top" title="编辑">
        <AButton type="link" size="small" onClick={() => editHandler(record)}>
          <EditOutlined />
        </AButton>
      </ATooltip>
    );
  }

  return '';
}

function editHandler(record) {
  handleEdit(record.id);
  nextTick(() => {
    model.id = record.id;
    model.name = record.name;
    model.value = record.value;
  });
}

async function submitHandler() {
  model.submitLoading = true;
  try {
    const { data: res, error } = await fetchUpdateConfiguration({ ...model });
    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      getDataByPage();
    }
  } finally {
    model.submitLoading = false;
  }
}
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <TableSearchFilter>
      <AForm
        ref="formRef"
        name="filer"
        :model="searchParams"
        :label-col="{
          span: 5,
          md: 7
        }"
      >
        <ARow :gutter="[16, 16]" wrap>
          <div class="flex-1">
            <AFormItem class="m-0">
              <div class="w-full flex-y-center justify-end gap-12px">
                <AButton @click="reset">
                  <template #icon>
                    <icon-ic-round-refresh class="align-sub text-icon" />
                  </template>
                  <span class="ml-8px">重置</span>
                </AButton>
                <AButton type="primary" ghost @click="search">
                  <template #icon>
                    <icon-ic-round-search class="align-sub text-icon" />
                  </template>
                  <span class="ml-8px">搜索</span>
                </AButton>
              </div>
            </AFormItem>
          </div>
        </ARow>
      </AForm>
    </TableSearchFilter>

    <ACard
      title="配置列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation v-model:columns="columnChecks"></TableHeaderOperation>
      </template>

      <ATable
        ref="tableWrapperRef"
        :columns="columns"
        :data-source="data"
        size="small"
        :scroll="scrollConfig"
        :loading="loading"
        row-key="id"
        :pagination="mobilePagination"
        class="h-full"
      />
    </ACard>

    <ADrawer v-model:open="drawerVisible" :title="`编辑${model.name}配置`" :width="650">
      <div class="body">
        <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
          <AFormItem label="配置值" name="value">
            <ATextarea v-model:value="model.value" :auto-size="{ minRows: 8 }" />
          </AFormItem>
        </AForm>
      </div>
      <template #footer>
        <ASpace :size="16">
          <AButton @click="drawerVisible = false">取消</AButton>
          <AButton type="primary" :loading="model.submitLoading" @click="submitHandler">保存</AButton>
        </ASpace>
      </template>
    </ADrawer>
  </div>
</template>

<style lang="scss" scoped></style>
