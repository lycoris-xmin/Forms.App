<script lang="ts" setup>
import type { VueElement } from 'vue';
import { h, reactive } from 'vue';
import { AndroidOutlined } from '@ant-design/icons-vue';
import type { ItemType } from 'ant-design-vue';
import docJson from '@/data/doc.json';
import DocLayout from './modules/doc-layout.vue';
import type { DocView } from './shard';

interface GetItemOptions {
  icon?: any;
  children?: ItemType[];
  type?: 'group';
}

interface Model {
  rootSubmenuKeys: Array<string>;
  openKeys: Array<string>;
  selectedKeys: Array<string>;
  doc: Record<string, DocView>;
}

const model = reactive<Model>({
  rootSubmenuKeys: ['app-setting'],
  openKeys: ['app-setting'],
  selectedKeys: ['taobao-setting'],
  doc: {}
});

const items: ItemType[] = reactive([
  getItem('App设置', 'app-setting', {
    icon: () => h(AndroidOutlined),
    children: [getItem('淘宝App设置', 'taobao-setting')]
  }),
  getItem('平台操作', 'platform-actions', {
    icon: () => h(AndroidOutlined),
    children: [
      getItem('设备绑定', 'platform-device'),
      getItem('店铺', 'platform-shop'),
      getItem('商品', 'platform-product')
    ]
  })
]);

const onOpenChange = (openKeys: string[]) => {
  const latestOpenKey = openKeys.find(key => !model.openKeys.includes(key));
  if (!model.rootSubmenuKeys.includes(latestOpenKey)) {
    model.openKeys = openKeys;
  } else {
    model.openKeys = latestOpenKey ? [latestOpenKey] : [];
  }
};

function getItem(label: VueElement | string, key: string, options?: GetItemOptions): ItemType {
  return {
    key,
    icon: options?.icon,
    children: options?.children,
    label,
    type: options?.type
  } as ItemType;
}
</script>

<template>
  <div class="page min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <div class="body">
      <AMenu
        v-model:selected-keys="model.selectedKeys"
        class="menu"
        mode="inline"
        :open-keys="model.openKeys"
        :items="items"
        @open-change="onOpenChange"
      ></AMenu>
      <div class="content">
        <ACard>
          <div class="view">
            <Transition name="view-transition" mode="out-in">
              <DocLayout
                v-if="model.selectedKeys.includes('taobao-setting')"
                :title="docJson['taobao-setting'].title"
                :content="docJson['taobao-setting'].content"
              ></DocLayout>
              <DocLayout
                v-else-if="model.selectedKeys.includes('phone-mi-setting')"
                :title="docJson['phone-mi-setting'].title"
                :content="docJson['phone-mi-setting'].content"
              ></DocLayout>
            </Transition>
          </div>
        </ACard>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.body {
  height: 100%;
  width: 100%;
  display: flex;
  gap: 14px;
}

.menu {
  width: 256px;
  flex-shrink: 0;
  border-radius: 8px;
}

.content {
  flex: 1;
  overflow-y: auto;

  .view {
    min-height: calc(100vh - 230px);
    overflow-x: hidden;
  }
}

.view-transition-enter-active {
  animation: slide-in-right 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94) both;

  @keyframes slide-in-right {
    0% {
      transform: translateX(1000px);
      opacity: 0;
    }
    100% {
      transform: translateX(0);
      opacity: 1;
    }
  }
}

.view-transition-leave-active {
  animation: slide-out-left 0.4s cubic-bezier(0.55, 0.085, 0.68, 0.53) both;

  @keyframes slide-out-left {
    0% {
      transform: translateX(0);
      opacity: 1;
    }
    100% {
      transform: translateX(-1000px);
      opacity: 0;
    }
  }
}
</style>
