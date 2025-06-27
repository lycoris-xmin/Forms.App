<script setup lang="ts">
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';

defineOptions({
  name: 'SystemRoleListSearch'
});

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const emit = defineEmits<Emits>();

async function reset() {
  emit('reset');
}

async function search() {
  emit('search');
}
</script>

<template>
  <TableSearchFilter>
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
  </TableSearchFilter>
</template>

<style lang="scss" scoped></style>
