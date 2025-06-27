// 直辖市列表
const DIRECT_MUNICIPALITIES = ['北京', '上海', '天津', '重庆'];

/**
 * 解析地址文本
 *
 * @param {string} addressText 要解析的地址文本
 * @returns {Object} 解析结果对象
 */
export function parseAddress(addressText) {
  const result = {
    province: '',
    city: '',
    district: '',
    remaining: ''
  };

  if (!addressText) return result;

  // 移除所有空格
  const temp = addressText.replace(/\s/g, '');

  // 尝试解析直辖市
  if (tryParseDirectMunicipality(temp, result)) {
    return result;
  }

  // 解析普通省份
  parseNormalProvince(temp, result);

  return result;
}

/**
 * 尝试解析直辖市地址
 *
 * @param {string} addressText 地址文本
 * @param {Object} result 解析结果对象
 * @returns {boolean} 是否解析成功
 */
export function tryParseDirectMunicipality(addressText, result) {
  for (const municipality of DIRECT_MUNICIPALITIES) {
    if (addressText.startsWith(municipality)) {
      result.province = `${municipality}市`;

      // 移除省份部分
      let remaining = addressText.substring(municipality.length);

      // 处理可能的第二个"市"字
      if (remaining.startsWith('市')) {
        remaining = remaining.substring(1);
      }

      // 尝试提取区县（作为city）
      const districtMatch = remaining.match(/^(.+?(区|县|市|旗))(.*)$/);
      if (districtMatch && districtMatch.length >= 4) {
        result.city = districtMatch[1]; // city设为区县
        result.district = districtMatch[3];
      } else {
        result.city = remaining; // 没有区县时，city设为剩余部分
      }

      return true;
    }
  }
  return false;
}

/**
 * 解析普通省份地址
 *
 * @param {string} addressText 地址文本
 * @param {Object} result 解析结果对象
 */
export function parseNormalProvince(addressText, result) {
  // 提取省份（以"省"结尾）
  const provinceMatch = addressText.match(/^(.+?省)(.*)$/);
  if (provinceMatch && provinceMatch.length >= 3) {
    result.province = provinceMatch[1];
    let remaining = provinceMatch[2];

    // 提取城市（以"市"结尾）
    const cityMatch = remaining.match(/^(.+?市)(.*)$/);
    if (cityMatch && cityMatch.length >= 3) {
      result.city = cityMatch[1];
      remaining = cityMatch[2];

      // 提取区县
      const districtMatch = remaining.match(/^(.+?(区|县|市|旗))(.*)$/);
      if (districtMatch && districtMatch.length >= 4) {
        result.district = districtMatch[1];
        result.remaining = districtMatch[3];
      } else {
        result.remaining = remaining;
      }
    } else {
      // 没有明确的城市，将剩余部分作为城市和区县
      result.city = remaining;
    }
  } else {
    // 无法识别省份，将整个字符串作为剩余部分
    result.remaining = addressText;
  }
}
