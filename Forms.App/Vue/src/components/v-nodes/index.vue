<script setup>
import { defineProps } from 'vue';

const props = defineProps({
  vnodes: {
    type: Object,
    required: true
  }
});
</script>

<template>
  <component :is="props.vnodes" />
</template>
