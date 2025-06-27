<script setup lang="ts">
defineOptions({
  name: 'GlobalFooter'
});
</script>

<template>
  <DarkModeContainer class="h-full flex-center">
    <a rel="noopener noreferrer">Copyright © 2021-{{ new Date().getFullYear() }} Soybean 版权所有</a>
  </DarkModeContainer>
</template>

<style scoped></style>
