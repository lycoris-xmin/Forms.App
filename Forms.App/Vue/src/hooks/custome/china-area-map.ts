import { ref } from 'vue';
import china from '@/data/china.area.json';

interface AreaMap {
  value: string;
  title: string;
}

interface ChinaArea {
  value: string;
  title: string;
  children: Array<AreaMap>;
}

interface SelecOption {
  value: string;
  label: string;
  key: number;
}

export function useChinaSelect() {
  const province = ref<Array<SelecOption>>([]);

  function init() {
    for (const key in china['86']) {
      if (Object.hasOwn(china['86'], key)) {
        province.value.push({
          key,
          label: china['86'][key],
          value: china['86'][key]
        });
      }
    }
  }

  function useCity(value: string) {
    const opts = [];
    if (value) {
      const option = province.value.filter(x => x.value === value);

      for (const key in china[option[0].key]) {
        if (Object.hasOwn(china[option[0].key], key)) {
          opts.push({
            label: china[option[0].key][key],
            value: china[option[0].key][key]
          });
        }
      }
    }

    return opts;
  }

  init();

  return {
    provinceOptions: province,
    useCity
  };
}

export function useChinAreaTree() {
  const chinaArea = ref<ChinaArea>([]);

  function init() {
    const chinaMap = china['86'];
    if (!chinaMap) return; // 早期返回，避免嵌套

    for (const key in chinaMap) {
      if (Object.hasOwn(chinaMap, key)) {
        const map = {
          value: chinaMap[key],
          title: chinaMap[key],
          children: []
        };

        addSubAreas(map, key);
        chinaArea.value.push(map);
      }
    }
  }

  // 提取函数，减少主函数的嵌套
  function addSubAreas(map, key) {
    const subArea = china[key]; // 获取子区域（如果有）
    if (!subArea) return;

    for (const area in subArea) {
      if (Object.hasOwn(subArea, area)) {
        map.children.push({
          title: subArea[area],
          value: `${map.value} ${subArea[area]}`
        });
      }
    }
  }

  init();

  return {
    json: china,
    chinaArea
  };
}
