import { ref } from 'vue';
import china from '@/data/china.area.json';

export default function useChinaProvinceCity() {
  const province = ref<string[]>([]); // 省份列表
  const city = ref<ChinaCityArea[]>([]); // 根据省份自动更新的城市列表

  // 初始化省份列表
  function initProvince() {
    const chinaMap = china['86']; // 获取国家级别数据
    if (!chinaMap) return; // 早期返回，避免嵌套

    province.value = Object.keys(chinaMap); // 直接获取省份的 key 值
  }

  // 根据传入的省份更新城市列表
  function useCityByProvince(provinceCode: string) {
    const subArea = china[provinceCode];
    if (subArea) {
      city.value = Object.entries(subArea).map(([key, value]) => ({
        title: value,
        value: key // 城市的 code
      }));
    } else {
      city.value = []; // 如果没有匹配的省份数据，则清空城市列表
    }
  }

  // 初始化省份列表
  initProvince();

  return {
    province,
    city,
    useCityByProvince
  };
}
