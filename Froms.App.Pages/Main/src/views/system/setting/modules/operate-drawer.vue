<script setup lang="ts">
  import { ref, watch, withDefaults } from 'vue';
  import VueJsonPretty from 'vue-json-pretty';
  import { useThemeStore } from '@/store/modules/theme';
  import 'vue-json-pretty/lib/styles.css';
  import '@/styles/css/vue-json-pretty.css';
  import { updateConfigurationListApi } from '@/service/api';

  defineOptions({
    name: 'SystemSettingOperateDrawer'
  });

  interface Props {
    rowData?: Api.Configuration.Data | null;
  }

  interface Model {
    id: string;
    value: string;
    valueType: number;
    loading: boolean;
  }

  interface Emits {
    (e: 'submitted'): void;
  }

  const visible = defineModel<boolean>('visible', {
    default: false
  });

  const props = withDefaults(defineProps<Props>(), {
    rowData: null
  });

  const themeStore = useThemeStore();

  const emit = defineEmits<Emits>();

  const model: Model = ref(createDefaultModel());

  watch(visible, () => {
    if (visible.value) {
      initModelHandler();
    }
  });

  function createDefaultModel(): Model {
    return {
      id: '',
      value: '{}',
      valueType: 0,
      loading: false
    };
  }

  function initModelHandler() {
    model.value = createDefaultModel();

    if (props.rowData) {
      Object.keys(model.value).forEach(key => {
        if (props.rowData[key]) {
          if (key === 'value') {
            if (props.rowData?.valueType === 0) {
              model.value[key] = props.rowData[key] || '';
            } else {
              try {
                model.value[key] = JSON.parse(props.rowData[key]) || {};
              } catch {
                model.value[key] = {};
              }
            }
          } else {
            model.value[key] = props.rowData[key];
          }
        }
      });
    }
  }

  function closeDrawer() {
    visible.value = false;
  }

  async function submitHandler() {
    model.value.loading = true;
    try {
      //
      const data = {
        id: model.value.id,
        value: JSON.stringify(model.value.value)
      };

      const { data: res, error } = await updateConfigurationListApi(data);
      if (!error && res && res.code === 0) {
        window.$message?.success('保存成功');
        closeDrawer();
        emit('submitted');
      }
    } finally {
      model.value.loading = false;
    }
  }
</script>

<template>
  <ADrawer v-model:open="visible" title="配置编辑" :width="props.rowData?.valueType === 0 ? '600px' : '40vw'">
    <div class="body">
      <VueJsonPretty
        v-if="model.valueType === 1"
        v-model:data="model.value"
        class="json-editor"
        :class="{ 'dark-mode': themeStore.darkMode }"
        :show-line-number="true"
        :editable="true"
        editable-trigger="dblclick"
        :theme="themeStore.darkMode ? 'dark' : 'light'"
        :show-icon="true"
      />

      <ATextarea v-else v-model:value="model.value" :rows="6" />
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="model.loading" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
  .body {
    min-height: 70vh;
  }
</style>
