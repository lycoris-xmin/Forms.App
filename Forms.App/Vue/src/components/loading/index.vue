<script setup lang="ts">
import { computed, withDefaults } from 'vue';
import { useThemeStore } from '@/store/modules/theme';

type Props = {
  loading?: boolean;
  tip?: string;
};

const themeStore = useThemeStore();

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  tip: ''
});

const bgColorComputed = computed(() => {
  return themeStore.darkMode ? '#1c1c1c' : '#ffffff';
});
</script>

<template>
  <div v-show="props.loading" class="overlay center v-center flex" :style="{ '--bg-color': bgColorComputed }">
    <div class="center flex flex-col">
      <ASpin class="spin mb-3" size="large" :tip="props.tip" />
      <!-- <span class="text-2xl">Loading...</span> -->
    </div>
  </div>
</template>

<style lang="scss" scoped>
.overlay {
  --bg-color: #1c1c1c;
  --bg-opacity: 1;
  position: absolute;
  inset: 0;
  border-radius: inherit;
  background-color: var(--bg-color);
  opacity: var(--bg-opacity);
}
</style>
