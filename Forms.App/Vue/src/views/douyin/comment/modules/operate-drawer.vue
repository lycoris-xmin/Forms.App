<script setup lang="ts">
import { computed, ref, watch, withDefaults } from 'vue';

defineOptions({
  name: 'PlanTaskCommentOperateDrawer'
});

type Model = Api.PlanTaskComment.UpdateComment & {
  imageFiles: Array;
  videoFiles: Array;
  loading: false;
  row: object;
};

type Props = {
  operateType?: string;
  rowData?: object;
};

interface Emits {
  (e: 'submitted'): void;
}

const visible = defineModel<boolean>('visible', {
  default: false
});

const model: Model = ref(createDefaultModel());

const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  rowData: null
});

const isEdit = computed<boolean>(() => {
  return props.operateType === 'edit';
});

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增评价',
    edit: '编辑评价'
  };
  return titles[props.operateType];
});

const emit = defineEmits<Emits>();

watch(visible, async () => {
  if (visible.value) {
    model.value = createDefaultModel();
    if (isEdit.value) {
      Object.assign(model.value.row, props.rowData || {});

      let i = 0;
      model.value.imageFiles = model.value.images?.map(x => {
        return {
          uid: (i -= 1),
          name: `${i}.png`,
          status: 'done',
          url: window.$isDbugger ? `${apiUrl}${x}` : x,
          thumbUrl: window.$isDbugger ? `${apiUrl}${x}` : x
        };
      });

      model.value.videoFiles = model.value.videos?.map(x => {
        return {
          uid: (i -= 1),
          name: `${i}.png`,
          status: 'done',
          url: window.$isDbugger ? `${apiUrl}${x}` : x,
          thumbUrl: window.$isDbugger ? `${apiUrl}${x}` : x
        };
      });
    }
  }
});

function createDefaultModel(): Model {
  return {
    id: '',
    taskId: '',
    mode: 0,
    text: '',
    images: [],
    imageFiles: [],
    videos: [],
    videoFiles: [],
    loading: false,
    row: {}
  };
}
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="650">//</ADrawer>
</template>

<style scoped></style>
