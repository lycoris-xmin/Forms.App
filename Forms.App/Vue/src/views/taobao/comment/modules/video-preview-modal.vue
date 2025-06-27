<script setup lang="ts">
type Props = {
  video?: string;
};

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = withDefaults(defineProps<Props>(), {
  video: ''
});

const handleCancel = () => {
  visible.value = false;
};
</script>

<template>
  <AModal :open="visible" title="视频查看" :width="600" :footer="null" @cancel="handleCancel">
    <div class="v-center center flex">
      <video class="preview" :src="props.video" width="300" controls loop muted />
    </div>
  </AModal>
</template>

<style scoped>
video {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>
