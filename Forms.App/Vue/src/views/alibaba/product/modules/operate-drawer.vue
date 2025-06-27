<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue';
import {
  DeleteOutlined,
  EditOutlined,
  PlusOutlined,
  PushpinOutlined,
  RetweetOutlined,
  SkypeOutlined
} from '@ant-design/icons-vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { CreateALIProduct, UpdateALIProduct, apiUrl } from '@/service/api';

defineOptions({
  name: 'ALIProductOperateDrawer'
});

interface Props {
  /** the type of operation */
  operateType: AntDesign.TableOperateType;
  /** the edit row data */
  rowData?: Api.Product.ALIData | null;
  productType: Array;
  shopNameEnum: Array;
}

interface Emits {
  (e: 'refreshShopNameEnum'): void;
  (e: 'submitted'): void;
}

type LoadingMap = {
  submit: boolean;
};

type Model = Pick<Api.Product.ALIData, 'id' | 'itemId' | 'url' | 'title' | 'shopId' | 'image' | 'skuMap' | 'type'> & {
  tab: string;
  fileList: Array;
  imagePriview: string;
};

type RuleKey = Extract<keyof Model, 'itemId' | 'url' | 'shopId' | 'title' | 'image', 'type'>;

const { formRef, validate, resetFields } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const props = defineProps<Props>();

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增商品',
    edit: '编辑商品'
  };
  return titles[props.operateType];
});

const model = ref(createDefaultModel());

const rules: Record<RuleKey, App.Global.FormRule> = {
  itemId: defaultRequiredRule,
  shopId: defaultRequiredRule,
  title: defaultRequiredRule
};

watch(visible, () => {
  if (visible.value) {
    emit('refreshShopNameEnum');
    initModelHandler();
    resetFields();
  }
});

function createDefaultModel(): Model {
  return {
    id: '',
    type: 0,
    itemId: '',
    url: '',
    shopId: '',
    title: '',
    image: '',
    skuMap: [],
    tab: 'base',
    fileList: [],
    imagePriview: ''
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (props.operateType === 'edit' && props.rowData) {
    Object.assign(model.value, props.rowData);

    model.value.image = '';
    if (props.rowData.image) {
      model.value.imagePriview = window.$isDebugger ? `${apiUrl}${props.rowData.image}` : props.rowData.image;
    }

    model.value.skuMap.forEach(x => (x.value = x.sku.join(',')));
  }
}

function closeDrawer() {
  visible.value = false;
}

function urlInputBlurHandler() {
  // 检查输入的 URL 是否为空
  if (!model.value.url) {
    model.value.itemId = '';
    model.value.shopId = '';
    return;
  }

  try {
    // 解码 URL，避免特殊字符导致解析错误
    const decodedUrl = decodeURIComponent(model.value.url);
    const url = new URL(decodedUrl);

    // 拆分路径部分
    const pathParts = url.pathname.split('/').filter(Boolean); // 过滤掉空字符串

    // 查找 'offer' 的索引
    const offerIndex = pathParts.findIndex(part => part.includes('html') || part.includes('htm'));

    const part = pathParts[offerIndex].split('.');
    if (part && part.length) {
      model.value.itemId = part[0];
    } else {
      model.value.itemId = '';
    }
  } catch {
    model.value.itemId = '';
  }
}

function addSkuMapCardHandler() {
  model.value.skuMap.push({
    sku: [''],
    price: '',
    edit: true,
    add: true
  });
}

function addSkuHandler(index: number) {
  if (model.value.skuMap[index].sku.filter(x => !x).length) {
    window.$message?.warning('存在未填写的SKU，请先填写');
    return;
  }

  model.value.skuMap[index].sku.push('');
}

function deleteSkuHandler(item, index) {
  item.sku.splice(index, 1);
}

function cancelSkuMapHandler(index: number) {
  window.$modal?.confirm({
    title: '提示',
    cancelText: '取消',
    okText: '确定',
    content: '确定要取消编辑吗？',
    onOk() {
      if (model.value.skuMap[index].add) {
        model.value.skuMap.splice(index, 1);
      } else {
        model.value.skuMap[index].edit = false;
      }
    }
  });
}

function addSkuMapHandler(index) {
  //
  if (model.value.skuMap[index].sku.filter(Boolean).length === 0) {
    window.$message?.warning('请填写正确的SKU信息');
    return;
  }

  if (!model.value.skuMap[index].price) {
    window.$message?.warning('请填写正确的价格信息');
    return;
  }

  model.value.skuMap[index].price = Number.parseFloat(model.value.skuMap[index].price);
  if (Number.isNaN(model.value.skuMap[index].price) || model.value.skuMap[index].price <= 0) {
    window.$message?.warning('请填写正确的价格信息');
    return;
  }

  model.value.skuMap[index].value = model.value.skuMap[index].sku.join(',');
  model.value.skuMap[index].edit = false;
  delete model.value.skuMap[index].add;
}

function deleteSkuMapHandler(index) {
  window.$modal?.confirm({
    title: '警告',
    cancelText: '取消',
    okText: '确定',
    content: '确定要删除吗？',
    onOk() {
      model.value.skuMap.splice(index, 1);
    }
  });
}

function imageChangeHandler() {
  if (model.value.fileList && model.value.fileList.length) {
    model.value.image = model.value.fileList[0].originFileObj;
    model.value.imagePriview = URL.createObjectURL(model.value.image);
    model.value.fileList = [];
  }
}

function shopNameFilterOption(input: string, option: any) {
  return props.shopNameEnum.filter(x => x.text.includes(input)).filter(x => x.value === option.value).length > 0;
}

function reloadShoNameEnumHandler() {
  emit('refreshShopNameEnum', true);
}

async function submitHandler() {
  await validate();

  if (props.operateType !== 'edit') {
    if (model.value.skuMap.length === 0) {
      window.$modal?.confirm({
        title: '提示',
        cancelText: '取消',
        okText: '确定',
        content: '您还未添加商品SKU信息，确定该商品没有SKU直接提交吗？',
        async onOk() {
          await createOrUpdate();
        },
        onCancel() {
          if (model.value.tab !== 'sku') {
            model.value.tab = 'sku';
          }
        }
      });
      return;
    }
  }

  await createOrUpdate();
}

async function createOrUpdate() {
  loadingMap.submit = true;

  try {
    const json = { ...model.value };

    json.skuMap = model.value.skuMap.map(x => {
      return {
        sku: x.value.split(',').filter(Boolean),
        price: Number.parseFloat(x.price)
      };
    });

    if (json.skuMap.filter(x => Number.isNaN(x.price) || x.price <= 0).length) {
      window.$message?.warning('SKU价格必须大为0');
      if (model.value.tab !== 'sku') {
        model.value.tab = 'sku';
      }
      return;
    }

    const skuJson = json.skuMap.map(x => JSON.stringify(x.sku));
    if ([...new Set(skuJson)].length !== skuJson.length) {
      window.$message?.warning('存在重复的SKU组合，请核对');
      if (model.value.tab !== 'sku') {
        model.value.tab = 'sku';
      }
      return;
    }

    const { data: res, error } =
      props.operateType === 'edit' ? await UpdateALIProduct(json) : await CreateALIProduct(json);

    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="650">
    <ATabs v-model:active-key="model.tab">
      <ATabPane key="base">
        <template #tab>
          <span>
            <PushpinOutlined />
            基础信息
          </span>
        </template>
        <AForm ref="formRef" layout="vertical" :model="model" :rules="rules">
          <AFormItem v-if="false" label="商品类型" name="type">
            <ASelect v-model:value="model.type">
              <ASelectOption
                v-for="item in props.productType"
                :key="item.value"
                :value="item.value"
                :disabled="item.value === 10"
              >
                <div class="space-between v-center flex">
                  {{ item.label }}
                  <span v-if="item.value === 10">功能开发中</span>
                </div>
              </ASelectOption>
            </ASelect>
          </AFormItem>
          <AFormItem name="image">
            <div class="upload">
              <AUpload
                v-model:file-list="model.fileList"
                accept="image/jpeg,image/png,image/webp"
                name="image"
                list-type="picture-card"
                class="uploader"
                :show-upload-list="false"
                :before-upload="() => false"
                @change="imageChangeHandler"
              >
                <img v-if="model.imagePriview" :src="model.imagePriview" alt="商品主图" />
                <div v-else>
                  <PlusOutlined />
                  <div class="ant-upload-text">上传</div>
                </div>
              </AUpload>

              <p>商品主图</p>
            </div>
          </AFormItem>
          <AFormItem label="商品链接" name="url">
            <AInput v-model:value="model.url" placeholder="请输入商品链接" @blur="urlInputBlurHandler"></AInput>
          </AFormItem>
          <!--
 <AFormItem label="商品编号" name="itemId">
            <AInput v-if="!isEdit" v-model:value="model.itemId" placeholder="根据商品链接自动识别" readonly />
            <ATooltip v-else placement="top" title="涉及到任务关联性，禁止修改商品链接，以免与任务核对不上商品">
              <AInput v-model:value="model.itemId" placeholder="根据商品链接自动识别" disabled />
            </ATooltip>
          </AFormItem> 
-->
          <AFormItem label="商品标题" name="title">
            <AInput v-model:value="model.title" placeholder="请输入商品标题" />
          </AFormItem>
          <AFormItem label="所属店铺" name="shopId">
            <div class="v-center center flex gap-8px">
              <ASelect v-model:value="model.shopId" show-search :filter-option="shopNameFilterOption">
                <ASelectOption v-for="item in props.shopNameEnum" :key="item.value" :value="item.value">
                  {{ item.text }}
                </ASelectOption>
              </ASelect>
              <AButton class="v-center flex" @click="reloadShoNameEnumHandler">
                <RetweetOutlined />
              </AButton>
            </div>
          </AFormItem>
        </AForm>
      </ATabPane>
      <ATabPane key="sku">
        <template #tab>
          <span>
            <SkypeOutlined />
            SKU组合
          </span>
        </template>
        <div class="sku-panel">
          <div class="v-center space-between top text-right">
            <AButton size="small" type="primary" ghost @click="addSkuMapCardHandler">添加SKU组合</AButton>
          </div>
          <AList size="small" :data-source="model.skuMap">
            <template #renderItem="{ item, index }">
              <AListItem>
                <ACard v-if="!item.edit" class="sku-view" :title="`SKU组合 ${index + 1}`">
                  <div>
                    <div class="wrap flex gap-6px">
                      <ATag v-for="sku in item.sku" :key="sku" class="center v-center flex" color="processing">
                        {{ sku }}
                      </ATag>
                    </div>
                    <div class="v-center center flex">
                      <span class="cny">{{ item.price }}</span>
                    </div>
                    <div class="v-center center action flex">
                      <AButton class="v-center flex" size="small" type="link" @click="() => (item.edit = true)">
                        <EditOutlined />
                      </AButton>
                      <AButton
                        class="v-center flex"
                        danger
                        size="small"
                        type="link"
                        @click="deleteSkuMapHandler(index)"
                      >
                        <DeleteOutlined />
                      </AButton>
                    </div>
                  </div>
                </ACard>

                <ACard v-else class="edit pt-6px">
                  <template #title>
                    <div class="v-center space-between flex">
                      <p>SKU编辑</p>
                      <AButton size="small" type="primary" ghost @click="addSkuHandler(index)">添加SKU</AButton>
                    </div>
                  </template>
                  <div class="sku-item">
                    <AFormItem v-for="(sku, skuIndex) in item.sku" :key="skuIndex">
                      <AInputGroup compact class="flex gap-0px">
                        <AInput v-model:value="item.sku[skuIndex]" placeholder="请输入SKU"></AInput>
                        <AButton danger @click="deleteSkuHandler(item, skuIndex)">删除</AButton>
                      </AInputGroup>
                    </AFormItem>
                    <AFormItem>
                      <AInputNumber
                        v-model:value="item.price"
                        prefix="￥"
                        :min="0"
                        string-mode
                        :controls="false"
                        :precision="2"
                      ></AInputNumber>
                    </AFormItem>
                  </div>
                  <div class="v-center end flex gap-8px">
                    <AButton size="small" @click="cancelSkuMapHandler(index)">取消</AButton>
                    <AButton size="small" type="primary" ghost @click="addSkuMapHandler(index)">确定</AButton>
                  </div>
                </ACard>
              </AListItem>
            </template>
          </AList>
        </div>
      </ATabPane>
    </ATabs>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="loadingMap.submit" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.ant-tabs-tab-btn {
  cursor: pointer;
}

.textarea {
  max-height: 75vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding-right: 5px;
}

.cny {
  padding-right: 10px;
  color: #ff5000;
  font-weight: 600;

  &::before {
    content: '￥';
    padding-right: 2px;
  }
}

.sku-panel {
  max-height: 70vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 15px 5px;

  li {
    padding: 10px 0;

    .sku-view {
      width: 100%;

      :deep(.ant-card-body) {
        > div {
          display: grid;
          grid-template-columns: 1fr 100px 60px;
          width: 100%;
        }
      }
    }
  }
}

.upload {
  :deep(.ant-upload-wrapper) {
    display: flex;
    justify-content: center;
  }

  > p {
    text-align: center;
  }
}

.uploader {
  overflow: hidden;
}

.edit {
  width: 100%;

  :deep(.ant-input-number-affix-wrapper) {
    width: 100%;
  }
}
</style>
