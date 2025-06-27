import { defineStore } from 'pinia';
import { ref } from 'vue';
import { SetupStoreId } from '@/enum';
import { fetchGetSettlement } from '@/service/api';

export const useSettlementStore = defineStore(SetupStoreId.Settlement, () => {
  let init = false;
  const alipayAccount = ref('');
  const alipayQRCode = ref('');
  const wechatPayQRCode = ref('');

  async function initSettlement(force = false) {
    if (init && !force) {
      return;
    }

    const { data: res, error } = await fetchGetSettlement();
    if (!error && res && res.code === 0) {
      setSettlement(res.data);
      init = true;
    }
  }

  function setSettlement(input: Api.UserProfile.Settlement) {
    alipayAccount.value = input.alipayAccount || '';
    alipayQRCode.value = input.alipayQRCode || '';
    wechatPayQRCode.value = input.wechatPayQRCode || '';
    init = false;
  }

  return {
    alipayAccount,
    alipayQRCode,
    wechatPayQRCode,
    initSettlement,
    setSettlement
  };
});
