import { ref } from 'vue';
import china from '@/data/china.area.json';

interface ChinaCityArea {
  value: string;
  title: string;
}

interface ChinaArea {
  value: string;
  title: string;
  children: Array<ChinaCityArea>;
}

export default function useChinAreaTree() {
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
