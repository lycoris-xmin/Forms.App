<script setup lang="ts">
import { debounce } from 'lodash-es';
import { onMounted, reactive } from 'vue';
import { fetchGetTenantFriendEnum } from '@/service/api';

defineOptions({
  name: 'PlanTaskFriendsSelect'
});

const friend = defineModel<string>('value', {
  default: ''
});

const enums = reactive({
  lastSearchFilter: '',
  friendsOptions: []
});

const searchHandler = debounce(async val => {
  enums.lastSearchFilter = val;

  const { data: res, error } = await fetchGetTenantFriendEnum(val);
  if (!error && res && res.code === 0) {
    if (res.data && res.data.list && res.data.list.length) {
      enums.friendsOptions = res.data.list.map(x => {
        return {
          value: x.value,
          label: x.text
        };
      });

      if (enums.friendsOptions.length === 1) {
        friend.value = enums.friendsOptions[0].value;
      }
    }
  }
});

onMounted(() => {
  searchHandler();
});
</script>

<template>
  <AFormItem name="friend">
    <template #label>
      <p>
        指定好友
        <small class="info">* 如未找到好友，请核好友列表是否已添加且已有审核过的店铺</small>
      </p>
    </template>
    <div class="v-center flex gap-8px">
      <ASelect
        v-model:value="friend"
        placeholder="- 请选择 -"
        show-search
        :default-active-first-option="false"
        :show-arrow="false"
        :filter-option="false"
        @search="searchHandler"
      >
        <ASelectOption v-for="item in enums.friendsOptions" :key="item.value" :value="item.value">
          {{ item.label }}
        </ASelectOption>

        <template #notFoundContent>
          <p>没有可分配的好友</p>
        </template>
      </ASelect>
      <AButton @click="() => searchHandler(enums.lastSearchFilter)">刷新</AButton>
    </div>
  </AFormItem>
</template>

<style scoped>
.info {
  color: var(--color-danger);
}
</style>
