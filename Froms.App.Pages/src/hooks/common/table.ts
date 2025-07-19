import { computed, effectScope, onScopeDispose, reactive, ref, shallowRef, watch } from 'vue';
import { useElementSize } from '@vueuse/core';
import { useBoolean, useHookTable } from '@sa/hooks';
import { jsonClone } from '@sa/utils';
import { useAppStore } from '@/store/modules/app';
import { $t } from '@/locales';

export function useTable(config) {
  const scope = effectScope();
  const appStore = useAppStore();

  const { apiFn, apiParams, immediate } = config;

  const {
    loading,
    empty,
    data,
    columns,
    columnChecks,
    reloadColumns,
    getData,
    searchParams,
    updateSearchParams,
    resetSearchParams
  } = useHookTable({
    apiFn,
    apiParams,
    columns: config.columns,
    transformer: ({ data: res, error }) => {
      if (error) {
        return {
          pageNum: 1,
          pageSize: 10,
          total: 0,
          data: []
        };
      }

      const { list = [], pageIndex = 1, pageSize = 10, count = 0 } = res.data || {};

      const recordsWithIndex = list.map((item, index) => {
        return {
          ...item,
          index: (pageIndex - 1) * pageSize + index + 1
        };
      });

      return {
        pageIndex,
        pageSize,
        total: count,
        data: recordsWithIndex
      };
    },
    getColumnChecks: cols => {
      const checks = [];

      cols.forEach(column => {
        if (column.key) {
          checks.push({
            key: column.key,
            title: column.title,
            checked: typeof column.visible === 'boolean' ? column.visible : true
          });
        }
      });

      return checks;
    },
    getColumns: (cols, checks) => {
      const columnMap = new Map();

      cols.forEach(column => {
        if (column.key) {
          columnMap.set(column.key, column);
        }
      });

      return checks.filter(item => item.checked).map(check => columnMap.get(check.key));
    },
    onFetched: async transformed => {
      const { pageIndex, pageSize, total } = transformed;

      updatePagination({
        pageIndex,
        pageSize,
        total
      });
    },
    immediate
  });

  const pagination = reactive({
    pageIndex: 1,
    pageSize: 10,
    showSizeChanger: true,
    pageSizeOptions: ['10', '20', '50', '100', '500'],
    total: 0,
    onChange: async (pageIndex, size) => {
      pagination.pageIndex = pageIndex;

      updateSearchParams({
        pageIndex,
        pageSize: size
      });

      getData();
    },
    showTotal: total => `总共 ${total} 条`
  });

  const mobilePagination = computed(() => {
    return {
      ...pagination,
      simple: appStore.isMobile
    };
  });

  function updatePagination(update) {
    Object.assign(pagination, update);
  }

  async function getDataByPage(pageNum = 1) {
    updatePagination({ pageIndex: pageNum });

    updateSearchParams({
      pageIndex: pageNum,
      pageSize: pagination.pageSize
    });

    await getData();
  }

  scope.run(() => {
    watch(
      () => appStore.locale,
      () => {
        reloadColumns();
      }
    );
  });

  onScopeDispose(() => {
    scope.stop();
  });

  return {
    loading,
    empty,
    data,
    columns,
    columnChecks,
    reloadColumns,
    pagination,
    mobilePagination,
    updatePagination,
    getData,
    getDataByPage,
    searchParams,
    updateSearchParams,
    resetSearchParams
  };
}

export function useTableOperate(data, getData) {
  const { bool: drawerVisible, setTrue: openDrawer, setFalse: closeDrawer } = useBoolean();

  const operateType = ref('add');

  function handleAdd() {
    operateType.value = 'add';
    openDrawer();
  }

  const editingData = ref(null);

  function handleEdit(id) {
    operateType.value = 'edit';
    const findItem = data.value.find(item => item.id === id) || null;
    editingData.value = jsonClone(findItem);

    openDrawer();
  }

  const checkedRowKeys = ref([]);
  const checkedRows = ref([]);

  function onSelectChange(keys, rows) {
    checkedRowKeys.value = keys;
    checkedRows.value = rows;
  }

  const rowSelection = computed(() => {
    return {
      columnWidth: 48,
      type: 'checkbox',
      selectedRowKeys: checkedRowKeys.value,
      selectedRows: checkedRows.value,
      onChange: onSelectChange
    };
  });

  async function onBatchDeleted() {
    window.$message?.success($t('common.deleteSuccess'));

    checkedRowKeys.value = [];

    await getData();
  }

  async function onDeleted() {
    window.$message?.success($t('common.deleteSuccess'));

    await getData();
  }

  return {
    drawerVisible,
    openDrawer,
    closeDrawer,
    operateType,
    handleAdd,
    editingData,
    handleEdit,
    checkedRowKeys,
    onSelectChange,
    rowSelection,
    onBatchDeleted,
    onDeleted
  };
}

export function useTableScroll(scrollX = 702) {
  const tableWrapperRef = shallowRef(null);
  const { height: wrapperElHeight } = useElementSize(tableWrapperRef);

  const scrollConfig = computed(() => {
    return {
      y: wrapperElHeight.value - 72,
      x: scrollX
    };
  });

  return {
    tableWrapperRef,
    scrollConfig
  };
}
