<script setup lang="ts">
import { Modal } from 'ant-design-vue';
import { reactive, ref, watch } from 'vue';
import { number } from 'echarts';
import { fetchCreateAuditShop, fetchGetFriendShopList, fetchRevokeAudit } from '@/service/api';
import LoadingWrap from '@/components/loading/index.vue';
import { platform as PLATFROM_TYPE } from '@/views/shared/enums';

defineOptions({
  name: 'FriendSettingDrawer'
});

type Model = Api.Friend.UpdateFriend & {
  id: string;
  friendNickName?: string | null;
  status: number;
  loading: boolean;
  submitLoading: boolean;
  planformType: number;
};

type Props = {
  rowData: Api.Friend.FriendData;
};

type Emits = {
  (e: 'submitted'): void;
};

const emit = defineEmits<Emits>();
const visible = defineModel<boolean>('visible', { default: false });
const props = defineProps<Props>();

const model = reactive<Model>({
  id: '',
  friendNickName: '',
  status: 0,
  loading: true,
  submitLoading: false,
  planformType: 0
});

const shopList = ref<Api.Friend.ShopData[]>([]);

// 弹窗打开时初始化数据
watch(visible, async () => {
  if (visible.value) {
    await initModelHandler();
    model.planformType = 0;
  }
});

async function initModelHandler() {
  try {
    Object.keys(model).forEach(key => {
      if (Object.keys(props.rowData).includes(key)) {
        model[key] = props.rowData[key];
      }
    });
    await loadFriendShops(model.planformType);
  } finally {
    model.loading = false;
  }
}

// 加载好友店铺列表（传入平台类型）
async function loadFriendShops(planformType = model.planformType) {
  model.loading = true;
  try {
    const { id } = model;
    const { data: res, error } = await fetchGetFriendShopList(id, planformType);
    if (!error && res?.data) {
      shopList.value = JSON.parse(JSON.stringify(res.data.list));
    }
  } finally {
    model.loading = false;
  }
}

// 平台类型变动时自动刷新列表
watch(
  () => model.planformType,
  val => {
    loadFriendShops(val);
  }
);

// 审核 / 撤销操作
const handleAudit = (shop: any, planformType: number) => {
  if (shop.auditId === '0') {
    Modal.confirm({
      title: '确认审核',
      content: `确认通过该店铺「${shop.shopName}」吗？`,
      okText: '确认',
      cancelText: '取消',
      onOk: async () => {
        const { id } = model;
        const json: Api.Friend.createAudit = {
          auditId: shop.auditId,
          friendId: id,
          shopName: shop.shopName,
          planformType
        };
        const { data: res, error } = await fetchCreateAuditShop(json);
        if (!error && res?.code === 0) {
          window.$message?.success('申请成功');
          loadFriendShops(planformType);
          emit('submitted');
        }
      }
    });
  } else {
    Modal.confirm({
      title: '确认撤销',
      content: `确认撤销该店铺「${shop.shopName}」吗？`,
      okText: '确认',
      cancelText: '取消',
      onOk: async () => {
        const { data: res, error } = await fetchRevokeAudit(shop.auditId);
        if (!error && res?.code === 0) {
          window.$message?.success('撤销成功');
          loadFriendShops(planformType);
          emit('submitted');
        }
      }
    });
  }
};
</script>

<template>
  <ADrawer v-model:open="visible" title="店铺审核" :width="600">
    <AForm :model="model">
      <AFormItem label="平台" name="planformType" class="m-0">
        <ASelect
          v-model:value="model.planformType"
          class="planform-type"
          placeholder="- 全部 -"
          :options="PLATFROM_TYPE"
        />
      </AFormItem>
    </AForm>

    <div class="body" :class="{ loading: model.loading }">
      <div v-if="shopList.length > 0" class="shop-list">
        <h3>好友店铺</h3>
        <div v-for="shop in shopList" :key="shop.id" class="shop-item">
          <span class="shop-name">{{ shop.shopName }}</span>
          <span class="shop-status center flex">
            <ATag :color="shop.auditId > 0 ? 'green' : 'red'">
              {{ shop.auditId > 0 ? '已审核' : '未审核' }}
            </ATag>
          </span>
          <div class="shop-actions center flex">
            <AButton size="small" type="link" @click="handleAudit(shop, model.planformType)">
              {{ shop.auditId > 0 ? '撤销' : '审核' }}
            </AButton>
          </div>
        </div>
      </div>
      <div v-else>
        <AEmpty description="该账户没有店铺需要审核" />
      </div>
      <LoadingWrap :loading="model.loading" />
    </div>
  </ADrawer>
</template>

<style lang="scss" scoped>
.form-label {
  gap: 15px;
}

.input-number {
  width: 100%;
}
.planform-type {
  width: 100%;
}

.body {
  position: relative;
  &.loading {
    max-height: 70vh;
    overflow: hidden;
  }
}

.shop-list {
  margin-top: 20px;
  padding: 10px 0;

  h3 {
    margin-bottom: 10px;
    font-size: 16px;
    font-weight: bold;
  }

  .shop-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 6px 0;
    border-bottom: 1px dashed #e0e0e0;

    .shop-name {
      flex: 1;
      font-weight: 500;
      color: #333;
    }

    .shop-status {
      width: 100px;
      flex-shrink: 0;
    }

    .shop-actions {
      width: 80px;
      flex-shrink: 0;
    }
  }
}
</style>
