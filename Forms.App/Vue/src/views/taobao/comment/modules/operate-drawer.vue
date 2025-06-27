<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { UploadOutlined } from '@ant-design/icons-vue';
import { fetchCreatePlanTaskComment, fetchUpdatePlanTaskComment } from '@/service/api';

type Model = {
  id: string;
  taskId: string;
  productTitle: string;
  text: string;
  images: string[];
  imageFiles: any[];
  videos: string[];
  videoFiles: any[];
};

// 定义props
type Props = {
  operateType?: 'add' | 'edit';
  rowData?: Model;
};

type Emits = {
  (e: 'submitted'): void;
};

const visible = defineModel<boolean>('visible', { default: false });
const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  rowData: null
});
const emit = defineEmits<Emits>();

// 使用计算属性正确判断编辑模式
const isEdit = computed(() => props.operateType === 'edit');

// 动态计算标题
const title = computed(() => (isEdit.value ? '编辑评价' : '新增评价'));

const model = ref<Model>({
  id: '',
  taskId: '',
  productTitle: '',
  text: '',
  images: [],
  imageFiles: [],
  videos: [],
  videoFiles: []
});

// 监听props变化，更新表单数据
watch(
  [() => props.operateType, () => props.rowData, () => visible.value],
  ([newOperateType, newRowData, newVisible]) => {
    if (newVisible) {
      if (newOperateType === 'edit' && newRowData) {
        // 编辑模式：使用传入的rowData初始化表单
        model.value = {
          ...newRowData,
          imageFiles:
            newRowData.images?.map((x, i) => ({
              uid: -i,
              name: `${i}.png`,
              status: 'done',
              url: window.$isDebugger ? `${apiUrl}${x}` : x,
              thumbUrl: window.$isDebugger ? `${apiUrl}${x}` : x
            })) || [],
          videoFiles:
            newRowData.videos?.map((x, i) => ({
              uid: -i,
              name: `${i}.mp4`,
              status: 'done',
              url: window.$isDebugger ? `${apiUrl}${x}` : x,
              thumbUrl: window.$isDebugger ? `${apiUrl}${x}` : x
            })) || []
        };
      } else {
        // 新增模式：重置表单
        model.value = {
          id: '',
          taskId: '',
          productTitle: '',
          text: '',
          images: [],
          imageFiles: [],
          videos: [],
          videoFiles: []
        };
      }
    }
  },
  { immediate: true }
);

const beforeUploadImage = (file: any) => {
  const isImage = file.type.startsWith('image/');
  if (!isImage) {
    window.$message.error('只能上传图片文件');
    return false;
  }
  return true;
};

const beforeUploadVideo = (file: any) => {
  const isVideo = file.type.startsWith('video/');
  if (!isVideo) {
    window.$message.error('只能上传视频文件');
    return false;
  }
  return true;
};

const handleSubmit = async () => {
  try {
    const { data: res, error } = isEdit.value
      ? await fetchUpdatePlanTaskComment(model.value.id, {
          taskId: model.value.taskId,
          productTitle: model.value.productTitle,
          text: model.value.text,
          images: model.value.images,
          videos: model.value.videos
        })
      : await fetchCreatePlanTaskComment({
          taskId: model.value.taskId,
          productTitle: model.value.productTitle,
          text: model.value.text,
          images: model.value.images,
          videos: model.value.videos
        });
    if (!error && res?.code === 0) {
      window.$message.success(isEdit.value ? '编辑评价成功' : '新增评价成功');
      emit('submitted');
      visible.value = false;
    } else {
      window.$message.error(res?.message || '操作失败，请检查输入');
    }
  } catch {
    window.$message.error('操作失败，请联系管理员');
  }
};
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="800" :destroy-on-close="true">
    <AForm :model="model" @finish="handleSubmit">
      <AFormItem label="任务ID" :required="true">
        <AInput v-model:value="model.taskId" />
      </AFormItem>
      <AFormItem label="商品标题" :required="true">
        <AInput v-model:value="model.productTitle" />
      </AFormItem>
      <AFormItem label="评价内容">
        <ATextarea v-model:value="model.text" />
      </AFormItem>
      <AFormItem label="图片上传">
        <AUpload v-model:file-list="model.imageFiles" :before-upload="beforeUploadImage" multiple>
          <AButton>
            <UploadOutlined />
            上传图片
          </AButton>
        </AUpload>
      </AFormItem>
      <AFormItem label="视频上传">
        <AUpload v-model:file-list="model.videoFiles" :before-upload="beforeUploadVideo" multiple>
          <AButton>
            <UploadOutlined />
            上传视频
          </AButton>
        </AUpload>
      </AFormItem>
      <AFormItem>
        <AButton type="primary" html-type="submit">
          {{ isEdit ? '保存修改' : '立即创建' }}
        </AButton>
        <AButton type="default" @click="visible = false">取消</AButton>
      </AFormItem>
    </AForm>
  </ADrawer>
</template>

<style scoped></style>
