<script setup lang="ts">
import { reactive, watch } from 'vue';
import type { TreeProps } from 'ant-design-vue';
import { fetchGetRolePermission, fetchSetRolePermission } from '@/service/api';

defineOptions({
  name: 'RolePermissionDrawer'
});

interface Props {
  id: string;
}

interface Model {
  id: string;
  treeData: TreeProps['treeData'];
  checkedKeys: Array<string>;
  expandedKeys: Array<string>;
}

type LoadingMap = {
  submit: boolean;
};

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const model = reactive<Model>({
  id: '',
  treeData: [],
  checkedKeys: [],
  expandedKeys: []
});

const props = defineProps<Props>();

watch(visible, async () => {
  if (visible.value) {
    await initModelHandler();
  }
});

async function initModelHandler() {
  if (model.id === props.id) {
    return;
  }

  model.id = props.id;
  const { data: res, error } = await fetchGetRolePermission(props.id);

  if (!error && res && res.code === 0) {
    model.expandedKeys = res.data.list.map(x => {
      return x.key;
    });
    model.checkedKeys = res.data.list
      .flatMap(item => item.children)
      .filter(child => child.selectable)
      .map(child => child.key);
    model.treeData = res.data.list.map(x => {
      return {
        key: x.key,
        title: x.title,
        selectable: x.selectable,
        children: x.children.map(y => {
          return {
            key: y.key,
            title: y.title,
            selectable: y.selectable,
            isLeaf: true,
            class: 'isLeaf'
          };
        })
      };
    });
  } else {
    model.expandedKeys = [];
    model.checkedKeys = [];
    model.treeData = [];
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  loadingMap.submit = true;
  try {
    //
    const { data: res, error } = await fetchSetRolePermission(
      props.id,
      model.checkedKeys.filter(x => x.includes('.'))
    );
    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
    }
  } finally {
    loadingMap.submit = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="权限设置" :width="550">
    <div v-if="model.treeData?.length" class="body">
      <ATree
        v-model:checked-keys="model.checkedKeys"
        v-model:expanded-keys="model.expandedKeys"
        checkable
        :tree-data="model.treeData"
        :default-expand-all="true"
      ></ATree>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="loadingMap.submit" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style lang="scss">
.body {
  .ant-tree-list-holder-inner {
    display: block !important;
  }

  .ant-tree-treenode {
    &.isLeaf {
      display: inline-flex !important;
      padding-bottom: 8px;
    }
  }
}
</style>
