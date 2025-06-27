<script setup lang="ts">
defineOptions({
  name: 'PlanTaskActions'
});

const model = defineModel<Api.PlanTask.Actions>('actions', {
  default: {
    shopAround: {
      min: 3,
      max: 5
    },
    addCart: false,
    favorite: false,
    lookComment: true,
    lookPlantingGrass: true,
    lookAskEveryone: true,
    useFirstBrowse: true,
    useFindBrowse: true,
    useWaitForBuy: true
  }
});
</script>

<template>
  <AFormItem>
    <template #label>
      <p>
        货比三家
        <small class="text-danger ml-1">* 都为0不进行货比</small>
      </p>
    </template>
    <div class="v-center flex gap-8px">
      <AInputNumber v-model:value="model.shopAround.min" :min="0" :controls="false" :precision="0">
        <template #addonBefore>
          <span>最少</span>
        </template>
        <template #addonAfter>
          <span>次</span>
        </template>
      </AInputNumber>
      <AInputNumber v-model:value="model.shopAround.max" :min="0" :controls="false" :precision="0">
        <template #addonBefore>
          <span>最多</span>
        </template>
        <template #addonAfter>
          <span>次</span>
        </template>
      </AInputNumber>
    </div>
  </AFormItem>

  <AFormItem label="商品互动">
    <ul class="actions">
      <li>
        <ACheckbox v-model:checked="model.addCart">商品加购</ACheckbox>
      </li>
      <li>
        <ACheckbox v-model:checked="model.favorite">商品收藏</ACheckbox>
      </li>
      <li>
        <ACheckbox v-model:checked="model.useFirstBrowse">随机搜索浏览多次</ACheckbox>
      </li>
      <li>
        <ACheckbox v-model:checked="model.lookComment">
          随机看商品评价
          <span class="text-danger">（3-10次动态）</span>
        </ACheckbox>
      </li>
      <li>
        <ACheckbox v-model:checked="model.lookPlantingGrass">
          随机看种草信息
          <span class="text-danger">（3-10次动态）</span>
        </ACheckbox>
      </li>
      <li>
        <ACheckbox v-model:checked="model.useFindBrowse">找到指定商品后不购买，再次随机浏览后下单</ACheckbox>
      </li>
    </ul>
  </AFormItem>
</template>

<style scoped lang="scss">
.actions {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 8px;

  li {
    list-style: none;
    margin-bottom: 14px;

    &:last-child {
      margin-bottom: 0;
    }
  }
}

.ant-input-number-group-wrapper {
  width: 100% !important;
}
</style>
