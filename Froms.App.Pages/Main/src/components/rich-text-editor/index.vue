<script setup lang="tsx">
  import { computed, nextTick, onBeforeUnmount, ref, render, shallowRef } from 'vue';
  import { Editor, Toolbar } from '@wangeditor/editor-for-vue';
  import '@wangeditor/editor/dist/css/style.css';
  import type { IEditorConfig, IToolbarConfig } from '@wangeditor/editor';
  import { useQiniuUploader } from '@/hooks/custome/useQiniuUploader.ts';
  import type { QiniuUploadFile } from '@/hooks/custome/useQiniuUploader.ts';
  import { useThemeStore } from '@/store/modules/theme';
  import type { InsertFn, Props } from './types.js';
  import { customCheckVideoFn, customParseVideoSrc } from './utils.js';

  const mode = 'simple';

  const themeStore = useThemeStore();
  const toolbarRef = ref<HTMLElement | null>(null);
  const editorRef = ref<HTMLElement | null>(null);
  const editorShallowRef = shallowRef();

  const html = defineModel<string>('html', {
    defalut: ''
  });

  const text = defineModel<string>('text', {
    default: ''
  });

  const loading = defineModel<boolean>('loading', {
    default: true
  });

  const deleted = defineModel<string[]>('deleted', {
    default: []
  });

  const editorHieght = ref<string>('100%');

  const lastHtml = ref<string>('');

  const props = withDefaults(defineProps<Props>(), {
    height: '100%',
    uploadPath: 'default'
  });

  const qiniuUploader = useQiniuUploader({ path: props.uploadPath, maxSize: 3 });

  const editorEl = computed<HTMLElement | null>(() => {
    return editorRef?.value?.$el;
  });

  const loadingBgColor = computed(() => {
    return themeStore.darkMode ? '#1c1c1c' : '#ffffff';
  });

  const toolbarConfig: Partial<IToolbarConfig> = {
    toolbarKeys: [
      'blockquote',
      'header1',
      'header2',
      'header3',
      '|',
      'bold',
      'underline',
      'italic',
      'through',
      'color',
      'bgColor',
      'clearStyle',
      '|',
      'bulletedList',
      'numberedList',
      'todo',
      'justifyLeft',
      'justifyRight',
      'justifyCenter',
      '|',
      'insertLink',
      {
        key: 'group-image',
        title: '图片',
        iconSvg:
          '<svg viewBox="0 0 1024 1024"><path d="M959.877 128l0.123 0.123v767.775l-0.123 0.122H64.102l-0.122-0.122V128.123l0.122-0.123h895.775zM960 64H64C28.795 64 0 92.795 0 128v768c0 35.205 28.795 64 64 64h896c35.205 0 64-28.795 64-64V128c0-35.205-28.795-64-64-64zM832 288.01c0 53.023-42.988 96.01-96.01 96.01s-96.01-42.987-96.01-96.01S682.967 192 735.99 192 832 234.988 832 288.01zM896 832H128V704l224.01-384 256 320h64l224.01-192z"></path></svg>',
        menuKeys: ['insertImage', 'uploadImage']
      },
      {
        key: 'group-video',
        title: '视频',
        iconSvg:
          '<svg viewBox="0 0 1024 1024"><path d="M981.184 160.096C837.568 139.456 678.848 128 512 128S186.432 139.456 42.816 160.096C15.296 267.808 0 386.848 0 512s15.264 244.16 42.816 351.904C186.464 884.544 345.152 896 512 896s325.568-11.456 469.184-32.096C1008.704 756.192 1024 637.152 1024 512s-15.264-244.16-42.816-351.904zM384 704V320l320 192-320 192z"></path></svg>',
        menuKeys: ['insertVideo', 'uploadVideo']
      },
      'insertTable',
      'codeBlock',
      '|',
      'undo',
      'redo',
      '|',
      'fullScreen'
    ]
  };

  const editorConfig: Partial<IEditorConfig> = {
    placeholder: '请输入内容...',
    MENU_CONF: {
      uploadImage: {
        customUpload: async (file: File, insertFn: InsertFn) => {
          loading.value = true;
          try {
            qiniuUploader.pushFile(file);
            const resp: QiniuUploadFile[] = await qiniuUploader.upload();
            const result = resp[0];
            insertFn(result.src, '', '');
          } finally {
            qiniuUploader.files.value = [];
            loading.value = false;
          }
        }
      },
      uploadVideo: {
        // 自定义上传
        async customUpload(file: File, insertFn: InsertFn) {
          loading.value = true;
          try {
            qiniuUploader.pushFile(file);
            const resp: QiniuUploadFile[] = await qiniuUploader.upload();
            const result = resp[0];
            insertFn(result.src, '');
          } finally {
            qiniuUploader.files.value = [];
            loading.value = false;
          }
        }
      },
      insertVideo: {
        checkVideo: customCheckVideoFn, // 也支持 async 函数
        parseVideoSrc: customParseVideoSrc // 也支持 async 函数
      }
    }
  };

  onBeforeUnmount(() => {
    if (editorShallowRef.value !== null) {
      editorShallowRef.value.destroy();
    }
  });

  function editorCreateHandler(editor: IDomEditor) {
    // 记录 editor 实例，重要！
    editorShallowRef.value = editor;

    nextTick(() => {
      editorEl.value.style.position = 'relative';

      editorHieght.value = calcEditorHeight();

      // 创建一个挂载点
      const mountNode = document.createElement('div');
      mountNode.classList.add('editor-loading');

      // 创建 TSX 节点
      const vnode = (
        <div class="center flex flex-col">
          <ASpin class="spin mb-3" size="large" />
        </div>
      );

      // 渲染并挂载
      render(vnode, mountNode);

      editorEl.value.appendChild(mountNode);

      document.querySelector('button[data-menu-key="insertVideo"]').querySelector('span.title').innerHTML = '网络视频';
    });
  }

  function editorChangeHandler(editor: IDomEditor) {
    if (!editor.isEmpty()) {
      text.value = editor.getText();
    } else {
      text.value = '';
      html.value = '';
    }

    nextTick(() => {
      // 获取媒体 URL 集合
      const lastMediaUrls = extractMediaUrls(lastHtml.value);
      const currentMediaUrls = extractMediaUrls(html.value);

      // 差集找出被删除的媒体
      const removed = new Set<string>();
      for (const url of lastMediaUrls) {
        if (!currentMediaUrls.has(url)) {
          removed.add(url);
        }
      }

      // 追加新删除的媒体资源（防止重复）
      const uniqueDeleted = new Set(deleted.value);
      for (const url of removed) {
        uniqueDeleted.add(url);
      }
      deleted.value = Array.from(uniqueDeleted);

      // 修复点：最后再更新 lastHtml.value，确保下次比对正确
      lastHtml.value = html.value;
    });
  }

  function customAlertHandler(s: string, t: string) {
    switch (t) {
      case 'success':
        window.$message?.success(s);
        break;
      case 'warning':
        window.$message?.warning(s);
        break;
      case 'error':
        window.$message?.error(s);
        break;
      default:
        window.$message?.info(s);
        break;
    }
  }

  function calcEditorHeight() {
    if (typeof props.height === 'number') {
      return `${props.height}px`;
    } else if (props.height === '100%') {
      const toolbarHeight = toolbarRef?.value?.$el.getBoundingClientRect().height || 0;
      return `calc(100% - ${toolbarHeight}px)`;
    }

    return props.height;
  }

  function extractMediaUrls(content: string): Set<string> {
    const urls = new Set<string>();

    const imgRegex = /<img[^>]*src=["']([^"']+)["']/g;
    const videoRegex = /<(video|source)[^>]*src=["']([^"']+)["']/g;

    let match;
    while ((match = imgRegex.exec(content)) !== null) {
      urls.add(match[1]);
    }

    while ((match = videoRegex.exec(content)) !== null) {
      urls.add(match[2]);
    }

    return urls;
  }
</script>

<template>
  <div class="editor" :class="{ loading: loading }">
    <Toolbar ref="toolbarRef" class="editor-toolbar" :editor="editorShallowRef" :default-config="toolbarConfig" :mode="mode" />
    <Editor ref="editorRef" v-model="html" class="editor-body" :default-config="editorConfig" :mode="mode" @on-created="editorCreateHandler" @on-change="editorChangeHandler" @custom-alert="customAlertHandler" />
  </div>
</template>

<style scoped lang="scss">
  .editor {
    position: relative;
    height: 100%;
    overflow: hidden;
    border: 1px solid #ccc;
    border-radius: 5px;

    &-toolbar {
      border-bottom: 1px solid #ccc;
    }

    &-body {
      height: v-bind(editorHieght) !important;
      overflow-y: hidden;
    }

    :deep(.w-e-text-container) {
      position: relative;
    }

    &.loading {
      :deep(.editor-loading) {
        display: flex;
      }
    }

    :deep(.editor-loading) {
      position: absolute;
      inset: 0;
      display: none;
      justify-content: center;
      align-items: center;
      background-color: v-bind(loadingBgColor);
      opacity: 0.6;
    }
  }
</style>
