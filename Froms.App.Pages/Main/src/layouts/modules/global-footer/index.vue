<script setup lang="ts">
  defineOptions({
    name: 'GlobalFooter'
  });
</script>

<template>
  <DarkModeContainer class="h-full flex-center gap-16px">
    <a rel="noopener noreferrer">Copyright © 2021-{{ new Date().getFullYear() }} 版权所有</a>
    <a rel="noopener noreferrer">MIT 许可 © Soybean</a>
  </DarkModeContainer>
</template>

<style scoped></style>
