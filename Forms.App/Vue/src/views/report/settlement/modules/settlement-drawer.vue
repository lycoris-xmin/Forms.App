<script setup lang="ts">
import { reactive, watch } from 'vue';
import { Modal } from 'ant-design-vue';
import { fetchAuditPlanTaskSettlement, fetchGetPlanTaskSettlementDetail } from '@/service/api';
import LoadingWrap from '@/components/loading/index.vue';

defineOptions({
  name: 'SettlementAuditDrawer'
});

interface Props {
  id: string;
  isAdmin: boolean;
}

interface TaskMap {
  title: string;
  count: number;
  price: string;
}

interface Model {
  id: string;
  task: TaskMap;
  pairTask: striTaskMapng;
  alipayAccount: string;
  alipayQRCode: string;
  wechatPayQRCode: string;
  diffPayPrice: string;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  submit: boolean;
  loading: boolean;
};

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false,
  loading: true
});

const model = reactive<Model>({
  id: '',
  task: {},
  pairTask: {},
  alipayAccount: '',
  alipayQRCode: '',
  wechatPayQRCode: '',
  diffPayPrice: ''
});

const emit = defineEmits<Emits>();

const props = defineProps<Props>();

watch(visible, async () => {
  if (visible.value && props.id !== model.id) {
    await initModelHandler();
    model.id = props.id;
  }
});

async function initModelHandler() {
  //
  loadingMap.loading = true;
  try {
    const { data: res, error } = await fetchGetPlanTaskSettlementDetail(props.id);
    if (!error && res && res.code === 0) {
      Object.assign(model, res.data);
    }
  } finally {
    loadingMap.loading = false;
  }
}

function closeDrawer() {
  visible.value = false;
}

function submitHandler() {
  Modal.confirm({
    title: props.isAdmin ? '警告' : '提示',
    content: props.isAdmin ? '确定要替该商家进行结算处理？' : '确定已经结算了？',
    okText: '确定',
    cancelText: '取消',
    onOk: async () => {
      model.id = '';
      loadingMap.submit = true;
      try {
        //
        const { data: res, error } = await fetchAuditPlanTaskSettlement(props.id);
        if (!error && res && res.code === 0) {
          window.$message?.success('保存成功');
          closeDrawer();
          emit('submitted');
        }
      } finally {
        loadingMap.submit = false;
      }
    }
  });
}
</script>

<template>
  <ADrawer v-model:open="visible" title="差价结算" :width="550">
    <div class="body">
      <ACard title="任务概况">
        <ADescriptions bordered class="expanded-row" :column="1">
          <ADescriptionsItem :span="1">
            <template #label>
              <p class="label">我方任务</p>
            </template>
            <div class="plan-grid">
              <p>商品标题：</p>
              <p>{{ model.pairTask.title }}</p>
              <p>购买件数：</p>
              <p>{{ model.pairTask.count }}</p>
              <p>支付金额：</p>
              <p class="cny">{{ model.pairTask.price }}</p>
            </div>
          </ADescriptionsItem>

          <ADescriptionsItem :span="1">
            <template #label>
              <p class="label">配对任务</p>
            </template>
            <div class="plan-grid">
              <p>商品标题：</p>
              <p>{{ model.task.title }}</p>
              <p>购买件数：</p>
              <p>{{ model.task.count }}</p>
              <p>支付金额：</p>
              <p class="cny">{{ model.task.price }}</p>
            </div>
          </ADescriptionsItem>
        </ADescriptions>
      </ACard>
      <ACard title="支付信息">
        <div class="pay-grid">
          <div class="v-center center flex">
            <img v-if="model.alipayQRCode" :src="model.alipayQRCode" />
            <div v-else class="v-center center flex">
              <p>对方暂未提供收款码</p>
            </div>
          </div>
          <div class="v-center center flex">
            <img v-if="model.wechatPayQRCode" :src="model.wechatPayQRCode" />
            <div v-else class="v-center center flex">
              <p>对方暂未提供收款码</p>
            </div>
          </div>
        </div>
        <div class="p-10px">
          <p class="price">支付宝：{{ model.alipayAccount }}</p>
          <p class="price">
            待补差价：
            <span class="cny">{{ model.diffPayPrice }}</span>
          </p>
        </div>
      </ACard>

      <LoadingWrap :loading="loadingMap.loading" bg-color="#fff"></LoadingWrap>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="loadingMap.submit" @click="submitHandler">已结算</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style lang="scss">
.body {
  position: relative;
  overflow: hidden;

  .ant-card {
    &:first-child {
      margin-bottom: 24px;
    }
  }

  .pay-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 10px;

    .flex {
      > img,
      > div {
        height: 220px;
        width: 220px;
      }

      img {
        object-fit: fill;
      }
    }
  }

  .expanded-row {
    .label {
      width: 65px !important;
      text-align: center;
    }

    .plan-grid {
      display: grid;
      grid-template-columns: 70px 210px;
      gap: 5px;
    }
  }

  .price {
    font-size: 16px;
    span {
      font-weight: 600;
    }
  }
}
</style>
