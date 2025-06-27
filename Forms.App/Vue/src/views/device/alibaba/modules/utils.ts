import { debounce } from 'lodash-es';
import ChinaArea from '@/data/china.area.json';

interface AddressInfo {
  province?: string;
  city?: string;
  address?: string;
  name?: string;
  phone?: string;
  areaValues?: string;
}

const MUNICIPALITIES = ['北京', '上海', '天津', '重庆'];
const AUTONOMOUS_REGIONS = ['内蒙古自治区', '广西壮族自治区', '宁夏回族自治区', '西藏自治区', '新疆维吾尔自治区'];
const SPECIAL_REGIONS = ['香港特别行政区', '澳门特别行政区', '台湾省'];

const debounceHandleAddressTextChange = debounce((text, callback) => {
  if (!text) {
    callback(void 0);
  }

  try {
    const result = parseAddressInfo(text);
    if (!result) {
      window.$message?.error('地址解析失败，请确认格式');
      callback(void 0);
    }

    const areaValues = findAreaValues(result.province, result.city);
    if (areaValues.length === 2) {
      result.areaValues = areaValues;
      window.$message?.success('解析成功');
    } else {
      window.$message?.error('无法找到对应的省市选项');
    }

    callback(result);
  } catch {
    window.$message?.error('解析地址失败，请检查格式是否正确');
    callback(void 0);
  }
}, 500);

const ADDRESS_PARSERS = [
  {
    type: 'municipality',
    condition: area => MUNICIPALITIES.some(m => area.startsWith(m)),
    parser: (area, detail) => {
      const muni = MUNICIPALITIES.find(m => area.startsWith(m));
      return muni ? parseDirectMunicipality(area, detail, muni) : null;
    }
  },
  {
    type: 'autonomous',
    condition: area => AUTONOMOUS_REGIONS.some(r => area.startsWith(r) || area.startsWith(r.replace('自治区', ''))),
    parser: (area, detail) => {
      const region = AUTONOMOUS_REGIONS.find(r => area.startsWith(r) || area.startsWith(r.replace('自治区', '')));
      return region ? parseAutonomousRegion(area, detail, region) : null;
    }
  },
  {
    type: 'special',
    condition: area =>
      SPECIAL_REGIONS.some(r => area.startsWith(r) || area.startsWith(r.replace(/(特别行政区|省)/, ''))),
    parser: (area, detail) => {
      const region = SPECIAL_REGIONS.find(r => area.startsWith(r) || area.startsWith(r.replace(/(特别行政区|省)/, '')));
      return region ? parseSpecialRegion(area, detail, region) : null;
    }
  },
  {
    type: 'normal',
    condition: () => true,
    parser: (area, detail) => parseNormalProvince(area, detail)
  }
];

export const Map = ChinaArea;

export function getAddressValidateStatus(text) {
  if (!text) {
    return '';
  }

  const lines = text.split(/[\n\r]+/).map(line => line.trim());
  const hasAllFields = [
    /^姓名[:：]?\s*.+/,
    /^手机号码[:：]?\s*1[3-9]\d{9}/,
    /^所在地区[:：]?\s*[\u4E00-\u9FA5]+/,
    /^详细地址[:：]?\s*.+/
  ].every(regex => lines.some(line => regex.test(line)));

  return hasAllFields ? '' : 'error';
}

export function getAddressValidateHelp(text) {
  if (!text) return '';

  const lines = text.split(/[\n\r]+/).map(line => line.trim());
  const fields = [
    { name: '姓名', regex: /^姓名[:：]?\s*.+/ },
    { name: '手机号码', regex: /^手机号码[:：]?\s*1[3-9]\d{9}/ },
    { name: '所在地区', regex: /^所在地区[:：]?\s*[\u4E00-\u9FA5]+/ },
    { name: '详细地址', regex: /^详细地址[:：]?\s*.+/ }
  ];

  const missingFields = fields.filter(field => !lines.some(line => field.regex.test(line)));

  if (missingFields.length > 0) {
    return `地址格式不正确

当前缺少或格式不正确：${missingFields.map(f => f.name).join('、')}`;
  }

  return '';
}

export function addressTextChangeHandler(text): AddressInfo {
  return new Promise(resolve => {
    debounceHandleAddressTextChange(text, (result: AddressInfo) => {
      resolve(result);
    });
  });
}

export function findAreaValues(province: string, city: string) {
  const provinceCode = Object.entries(ChinaArea['86']).find(([_, name]) => name === province)?.[0];
  if (!provinceCode) {
    return [];
  }

  const cityCode = Object.entries(ChinaArea[provinceCode]).find(([_, name]) => name === city)?.[0];
  if (!cityCode) {
    return [];
  }

  return [provinceCode, cityCode];
}

function parseDirectMunicipality(cleanArea, detail, muni) {
  const province = `${muni}市`;
  let rest = cleanArea;
  rest = rest.replace(new RegExp(`^${muni}${muni}市`), muni);
  rest = rest.replace(new RegExp(`^${muni}市${muni}市`), `${muni}市`);
  rest = rest.replace(new RegExp(`^${muni}${muni}`), muni);
  rest = rest.replace(new RegExp(`^${muni}市`), muni);
  rest = rest.replace(new RegExp(`^${muni}`), muni);

  const districtMatch = rest.match(new RegExp(`${muni}(.+?区|.+?县)(.+)?$`));
  if (districtMatch) {
    const city = districtMatch[1];
    const street = districtMatch[2] || '';
    const address = street + (detail ? ` ${detail}` : '');
    return { province, city, address };
  }

  const district = rest.replace(new RegExp(`^${muni}`), '') + (detail ? ` ${detail}` : '');
  return { province, city: district, address: '' };
}

function parseAddressInfo(input): AddressInfo {
  const normalizedInput = input.replace(/：/g, ':').replace(/\s+/g, ' ').trim();

  const requiredFields = ['姓名:', '手机号码:', '所在地区:', '详细地址:'];
  const hasAllFields = requiredFields.every(field => normalizedInput.includes(field));

  if (!hasAllFields) {
    return null;
  }

  const nameMatch = normalizedInput.match(/姓名:\s*(.+?)\s*(?=手机号码:|$)/);
  const phoneMatch = normalizedInput.match(/手机号码:\s*(1[3-9]\d{9})\s*(?=所在地区:|$)/);
  const areaMatch = normalizedInput.match(/所在地区:\s*([^\n]+?)(?=详细地址:|$)/);
  const detailMatch = normalizedInput.match(/详细地址:\s*([^\n]+?)(?=$)/);

  if (!nameMatch || !phoneMatch || !areaMatch || !detailMatch) {
    return null;
  }

  const name = nameMatch[1].trim();
  const phone = phoneMatch[1].trim();
  const area = areaMatch[1].trim();
  const detail = detailMatch[1].trim();

  if (!name || !phone || !area || !detail) {
    return null;
  }

  const cleanArea = area.replace(/\s+/g, '');

  const result = parseRegionAddress(cleanArea, detail);

  if (!result || !result.province || !result.city) {
    return null;
  }

  return {
    ...result,
    phone,
    name
  };
}

function parseAutonomousRegion(cleanArea, detail, region) {
  const province = region;
  const rest = cleanArea.slice(cleanArea.startsWith(region) ? region.length : region.replace('自治区', '').length);

  const cityMatch = rest.match(/^[^市]+市/);
  const city = cityMatch ? cityMatch[0] : '';
  const district = cityMatch
    ? rest.slice(city.length) + (detail ? ` ${detail}` : '')
    : rest + (detail ? ` ${detail}` : '');

  return { province, city, address: district };
}

function parseSpecialRegion(cleanArea, detail, region) {
  const province = region;
  const rest = cleanArea.slice(
    cleanArea.startsWith(region) ? region.length : region.replace(/(特别行政区|省)/, '').length
  );
  return {
    province,
    city: '',
    address: rest + (detail ? ` ${detail}` : '')
  };
}

function parseNormalProvince(cleanArea, detail) {
  const match = cleanArea.match(/^(.*?省)(.*?市)(.*)$/);
  if (match) {
    return {
      province: match[1],
      city: match[2],
      address: match[3] + (detail ? ` ${detail}` : '')
    };
  }

  const altMatch = cleanArea.match(/^(.*?)(市)(.*)$/);
  if (altMatch) {
    const possibleProvince = altMatch[1];
    const foundProvince = Object.entries(ChinaArea['86']).find(
      ([_, name]) =>
        name.startsWith(possibleProvince) || name.replace(/(省|自治区|特别行政区)/, '').startsWith(possibleProvince)
    );
    if (foundProvince) {
      return {
        province: foundProvince[1],
        city: altMatch[1] + altMatch[2],
        address: altMatch[3] + (detail ? ` ${detail}` : '')
      };
    }
  }
  return null;
}

function parseRegionAddress(cleanArea, detail): AddressInfo {
  for (const { condition, parser } of ADDRESS_PARSERS) {
    if (condition(cleanArea)) {
      const result = parser(cleanArea, detail);
      if (result) {
        return result;
      }
    }
  }
  return null;
}
