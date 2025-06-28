<script setup lang="ts">
  import { withDefaults } from 'vue';
  import { $t } from '@/locales';

  defineOptions({
    name: 'TableHeaderOperation'
  });

  interface Props {
    disabledDelete?: boolean;
    loading?: boolean;
    addBtn?: boolean;
    deleteBtn?: boolean;
    refreshBtn?: boolean;
  }

  const props = withDefaults(defineProps<Props>(), {
    disabledDelete: false,
    loading: false,
    addBtn: false,
    deleteBtn: false,
    refreshBtn: false
  });

  interface Emits {
    (e: 'add'): void;
    (e: 'delete'): void;
    (e: 'refresh'): void;
  }

  const emit = defineEmits<Emits>();

  const columns = defineModel<AntDesign.TableColumnCheck[]>('columns', {
    default: () => []
  });

  function add() {
    emit('add');
  }

  function batchDelete() {
    emit('delete');
  }

  function refresh() {
    emit('refresh');
  }
</script>

<template>
  <div class="flex flex-wrap justify-end gap-x-12px gap-y-8px lt-sm:(w-200px py-12px)">
    <slot name="prefix"></slot>

    <AButton v-if="props.addBtn" size="small" ghost type="primary" @click="add">
      <template #icon>
        <icon-ic-round-plus class="align-sub text-icon" />
      </template>
      <span class="ml-8px">{{ $t('common.add') }}</span>
    </AButton>

    <slot name="default"></slot>

    <APopconfirm v-if="props.deleteBtn" :title="$t('common.confirmDelete')" :disabled="props.disabledDelete" @confirm="batchDelete">
      <AButton size="small" danger :disabled="props.disabledDelete">
        <template #icon>
          <icon-ic-round-delete class="align-sub text-icon" />
        </template>

        <span class="ml-8px">{{ $t('common.batchDelete') }}</span>
      </AButton>
    </APopconfirm>

    <AButton v-if="props.refreshBtn" size="small" @click="refresh">
      <template #icon>
        <icon-mdi-refresh class="align-sub text-icon" :class="{ 'animate-spin': props.loading }" />
      </template>
      <span class="ml-8px">{{ $t('common.refresh') }}</span>
    </AButton>

    <slot name="suffix"></slot>

    <TableColumnSetting v-model:columns="columns" />
  </div>
</template>

<style scoped></style>
