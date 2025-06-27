<script setup lang="ts">
defineOptions({
  name: 'RedNoteList'
});
</script>

<template>
  <div class="size-full min-h-520px flex-col-center gap-24px overflow-hidden">
    <div class="flex text-400px text-primary">
      <SvgIcon local-icon="expectation" />
    </div>
    <slot>
      <h3 class="text-28px text-primary font-500">内测邀请中，敬请期待</h3>
      <p class="info">找客服提建议去，进入内测。</p>
    </slot>
  </div>
</template>

<style scoped>
.info {
  font-size: 32px;
  font-weight: 500;
  color: #646cff;
  letter-spacing: 2.5px;
}
</style>
