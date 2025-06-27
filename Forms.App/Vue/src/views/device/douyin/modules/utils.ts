import { debounce } from 'lodash-es';
import ChinaArea from '@/data/china.area.json';

export interface AddressInfo {
  name: string;
  phone: string;
  province: string;
  city: string;
  address: string;
}

export interface AreaInfo {
  province: string;
  city: string;
}

export interface Model {
  name: string;
  phone: string;
  province: string;
  city: string;
  address: string;
  tbAddressText: string;
  [key: string]: any;
}

// 常量定义
export const SPECIAL_REGIONS = {
  香港特别行政区: '香港特别行政区',
  澳门特别行政区: '澳门特别行政区',
  香港: '香港特别行政区',
  澳门: '澳门特别行政区',
  台湾省: '台湾省',
  台湾: '台湾省'
} as const;

export const DIRECT_CITIES = {
  北京市: '北京市',
  上海市: '上海市',
  天津市: '天津市',
  重庆市: '重庆市',
  北京: '北京市',
  上海: '上海市',
  天津: '天津市',
  重庆: '重庆市'
} as const;

export const AUTONOMOUS_REGIONS = {
  内蒙古自治区: '内蒙古自治区',
  广西壮族自治区: '广西壮族自治区',
  宁夏回族自治区: '宁夏回族自治区',
  西藏自治区: '西藏自治区',
  新疆维吾尔自治区: '新疆维吾尔自治区',
  内蒙古: '内蒙古自治区',
  广西: '广西壮族自治区',
  宁夏: '宁夏回族自治区',
  西藏: '西藏自治区',
  新疆: '新疆维吾尔自治区'
} as const;

// 创建地址解析处理函数
export function createAddressHandler(model: Model, selectedArea: Ref<string[]>) {
  return debounce((text: string) => {
    if (!text) return;

    try {
      const result = parseAddress(text);
      if (!result) {
        window.$message?.error('地址解析失败，请确认格式');
        return;
      }

      model.name = result.name;
      model.phone = result.phone;
      model.province = result.province;
      model.city = result.city;
      model.address = result.address;

      const areaValues = findAreaValues(result.province, result.city);
      if (areaValues.length === 2) {
        selectedArea.value = areaValues;
        window.$message?.success('解析成功');
      } else {
        window.$message?.error('无法找到对应的省市选项');
      }
    } catch {
      window.$message?.error('解析地址失败，请检查格式是否正确');
    }
  }, 500);
}

// 获取地区列表
export function getAreaList() {
  return Object.entries(ChinaArea['86']).map(([code, name]) => {
    const cities = ChinaArea[code];
    const children = cities
      ? Object.entries(cities).map(([cityCode, cityName]) => ({
          value: cityCode,
          label: cityName,
          isLeaf: true
        }))
      : [];

    return {
      value: code,
      label: name,
      children
    };
  });
}

// 处理地区选择变化
export function handleAreaChange(value: string[], model: Model, areaList: any[]) {
  if (value.length === 2) {
    const province = areaList.find(p => p.value === value[0]);
    const city = province?.children?.find(c => c.value === value[1]);
    if (province && city) {
      model.province = province.label;
      model.city = city.label;
    }
  } else {
    model.province = '';
    model.city = '';
  }
}

export function getDefaultCity(region: string): string {
  const defaultCities = {
    香港特别行政区: '香港',
    澳门特别行政区: '澳门',
    台湾省: '台北市'
  };
  return defaultCities[region] || '';
}

export function parseDirectCity(area: string): AreaInfo | null {
  for (const [key, value] of Object.entries(DIRECT_CITIES)) {
    if (area.includes(key)) {
      const cleanArea = area.replace(new RegExp(`${key}${key}`), key);
      const districtMatch = cleanArea.match(new RegExp(`${key}(.+?[区县])`));
      if (districtMatch) {
        return {
          province: value,
          city: districtMatch[1].trim()
        };
      }
    }
  }
  return null;
}

export function parseAutonomousRegion(area: string): AreaInfo | null {
  for (const [key, value] of Object.entries(AUTONOMOUS_REGIONS)) {
    if (area.startsWith(key)) {
      const remaining = area.substring(key.length).trim();
      const match = remaining.match(/^(.+?[市州盟])/);
      if (match) {
        return {
          province: value,
          city: match[1].trim()
        };
      }
    }
  }
  return null;
}

export function parseSpecialRegion(area: string): AreaInfo | null {
  for (const [key, value] of Object.entries(SPECIAL_REGIONS)) {
    if (area.startsWith(key)) {
      const remaining = area.substring(key.length).trim();
      const districtMatch = remaining.match(/^(.+?区)?(.*)$/);
      if (districtMatch) {
        const cityName = districtMatch[1]?.trim() || getDefaultCity(value);
        return {
          province: value,
          city: cityName
        };
      }
    }
  }
  return null;
}

export function parseAddress(text: string): AddressInfo | null {
  if (!text) return null;

  const normalizedInput = text.replace(/：/g, ':').trim();
  const fields = normalizedInput.split(/,\s*/);

  const nameMatch = fields.find(f => f.startsWith('收货人:'))?.match(/收货人:\s*(.+)/);
  const phoneMatch = fields.find(f => f.startsWith('手机号码:'))?.match(/手机号码:\s*(\d{11})/);
  const areaMatch = fields.find(f => f.startsWith('所在地区:'))?.match(/所在地区:\s*(.+)/);
  const addressMatch = fields.find(f => f.startsWith('详细地址:'))?.match(/详细地址:\s*(.+)/);

  if (!nameMatch || !phoneMatch || !areaMatch || !addressMatch) {
    return null;
  }

  const name = nameMatch[1].trim();
  const phone = phoneMatch[1].trim();
  const area = areaMatch[1].trim();
  const address = addressMatch[1].trim();

  if (!/^1[3-9]\d{9}$/.test(phone)) {
    return null;
  }

  if (!name) {
    return null;
  }

  const areaInfo = parseAreaInfo(area);
  if (!areaInfo) {
    return null;
  }

  return {
    name,
    phone,
    ...areaInfo,
    address
  };
}

function parseAreaInfo(area: string): AreaInfo | null {
  const directCityResult = parseDirectCity(area);
  if (directCityResult) {
    return directCityResult;
  }

  const autonomousRegionResult = parseAutonomousRegion(area);
  if (autonomousRegionResult) {
    return autonomousRegionResult;
  }

  const specialRegionResult = parseSpecialRegion(area);
  if (specialRegionResult) {
    return specialRegionResult;
  }

  // 处理普通省份
  const match = area.match(/^(.+?省)(.+?市)(.+)?$/);
  if (match) {
    return {
      province: match[1].trim(),
      city: match[2].trim()
    };
  }

  // 尝试匹配没有"省"字的省份
  const provinceMatch = area.match(/^(.+?)(.+?市)(.+)?$/);
  if (provinceMatch) {
    const possibleProvince = provinceMatch[1].trim();
    const foundProvince = Object.entries(ChinaArea['86']).find(
      ([_, name]) =>
        name.startsWith(possibleProvince) || name.replace(/(省|自治区|特别行政区)/, '').startsWith(possibleProvince)
    );
    if (foundProvince) {
      return {
        province: foundProvince[1],
        city: provinceMatch[2].trim()
      };
    }
  }

  return null;
}

export function findAreaValues(province: string, city: string): string[] {
  const result: string[] = [];

  // 处理特殊行政区
  const SPECIAL_REGIONS_MAP = {
    香港特别行政区: '香港',
    澳门特别行政区: '澳门',
    台湾省: '台北'
  };

  // 处理自治区
  const AUTONOMOUS_REGIONS_MAP = {
    内蒙古自治区: '内蒙古',
    广西壮族自治区: '广西',
    宁夏回族自治区: '宁夏',
    西藏自治区: '西藏',
    新疆维吾尔自治区: '新疆'
  };

  // 处理直辖市
  const DIRECT_CITIES_MAP = {
    北京市: '北京',
    上海市: '上海',
    天津市: '天津',
    重庆市: '重庆'
  };

  // 标准化省份名称
  let normalizedProvince = province;

  // 处理特殊行政区
  if (province in SPECIAL_REGIONS_MAP) {
    normalizedProvince = SPECIAL_REGIONS_MAP[province];
  }

  // 处理自治区
  if (province in AUTONOMOUS_REGIONS_MAP) {
    normalizedProvince = AUTONOMOUS_REGIONS_MAP[province];
  }

  // 处理直辖市
  if (province in DIRECT_CITIES_MAP) {
    normalizedProvince = DIRECT_CITIES_MAP[province];
  }

  // 移除省份中的"省"字
  normalizedProvince = normalizedProvince.replace(/省$/, '');

  // 查找省份代码
  const provinceEntry = Object.entries(ChinaArea['86']).find(
    ([_, name]) => name === normalizedProvince || name.startsWith(normalizedProvince)
  );

  if (provinceEntry) {
    const [provinceCode] = provinceEntry;
    result.push(provinceCode);

    // 获取该省份的城市列表
    const cities = ChinaArea[provinceCode];
    if (cities) {
      // 标准化城市名称
      const normalizedCity = city.replace(/市$/, '');

      // 查找城市代码
      const cityEntry = Object.entries(cities).find(
        ([_, name]) => name === normalizedCity || name.startsWith(normalizedCity)
      );

      if (cityEntry) {
        result.push(cityEntry[0]);
      }
    }
  }

  return result;
}

export function getAddressValidateStatus(text: string): string {
  if (!text) return '';

  const fields = text.split(/,\s*/);
  const hasAllFields = [/^收货人:\s*.+/, /^手机号码:\s*\d{11}/, /^所在地区:\s*.+/, /^详细地址:\s*.+/].every(regex =>
    fields.some(field => regex.test(field))
  );

  return hasAllFields ? '' : 'error';
}

export function getAddressValidateHelp(text: string): string {
  if (!text) return '';

  const fields = text.split(/,\s*/);
  const requiredFields = [
    { name: '收货人', regex: /^收货人:\s*.+/ },
    { name: '手机号码', regex: /^手机号码:\s*\d{11}/ },
    { name: '所在地区', regex: /^所在地区:\s*.+/ },
    { name: '详细地址', regex: /^详细地址:\s*.+/ }
  ];

  const missingFields = requiredFields.filter(field => !fields.some(f => field.regex.test(f)));

  if (missingFields.length > 0) {
    return `地址格式不正确\n\n当前缺少或格式不正确：${missingFields.map(f => f.name).join('、')}`;
  }

  return '';
}
