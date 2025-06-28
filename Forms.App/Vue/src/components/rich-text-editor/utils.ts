// 自定义校验视频
export function customCheckVideoFn(src: string): boolean | string | undefined {
  // 返回值有三种选择：
  // 1. 返回 true ，说明检查通过，编辑器将正常插入视频
  // 2. 返回一个字符串，说明检查未通过，编辑器会阻止插入。会 alert 出错误信息（即返回的字符串）
  // 3. 返回 undefined（即没有任何返回），说明检查未通过，编辑器会阻止插入。但不会提示任何信息

  if (!src) {
    return void 0;
  }

  if (src.indexOf('https') !== 0) {
    return '视频地址必须以 https 开头';
  }

  return true;
}

// 自定义转换视频
export function customParseVideoSrc(src: string): string {
  // TS 语法
  // function customParseVideoSrc(src) {               // JS 语法
  if (src.includes('.bilibili.com')) {
    // 转换 bilibili url 为 iframe （仅作为示例，不保证代码正确和完整）
    const arr = location.pathname.split('/');
    const vid = arr[arr.length - 1];
    return `<iframe src="//player.bilibili.com/player.html?bvid=${vid}" scrolling="no" border="0" frameborder="no" framespacing="0" allowfullscreen="true"> </iframe>`;
  }
  return src;
}
