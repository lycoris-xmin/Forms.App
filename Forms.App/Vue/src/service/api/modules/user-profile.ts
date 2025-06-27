import { baseResp, flatRequest } from '../../request';

export function fetchSaveUserProfile(input: Api.UserProfile.SaveProfile) {
  const { nickName, email, wechat } = input;

  const data = { nickName, email };

  if (wechat) {
    data.wechat = wechat;
  }

  return flatRequest({
    url: '/api/userprofile',
    method: 'put',
    data
  });
}

export function fetchChangePassword(input: Api.UserProfile.ChangePassword) {
  const { oldPassword, password } = input;

  return flatRequest({
    url: '/api/userprofile/changepassword',
    method: 'put',
    data: {
      oldPassword,
      password
    }
  });
}

export function fetchGetSettlement() {
  return flatRequest({
    url: '/api/userprofile/settlement',
    method: 'get'
  });
}

export function fetchSaveSettlement(input: Api.UserProfile.Settlement) {
  const { alipayAccount, alipayQRCode, wechatPayQRCode } = input;

  if (!alipayAccount && !wechatPayQRCode) {
    return baseResp();
  }

  const formData = new FormData();
  if (alipayAccount) {
    formData.append('alipayAccount', alipayAccount);
  }

  if (alipayQRCode) {
    formData.append('alipayQRCode', alipayQRCode);
  }

  if (wechatPayQRCode) {
    formData.append('wechatPayQRCode', wechatPayQRCode);
  }

  return flatRequest({
    url: '/api/userprofile/settlement',
    method: 'post',
    data: formData,
    headers: {
      'Content-Type': alipayQRCode || wechatPayQRCode ? 'multipart/form-data' : 'application/x-www-form-urlencoded'
    }
  });
}

export function fetchGetProfileSubscription() {
  return flatRequest({
    url: '/api/userprofile/subscription'
  });
}
