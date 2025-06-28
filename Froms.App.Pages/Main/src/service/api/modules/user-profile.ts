import { flatRequest } from '../../request';

export function saveUserProfileApi(input: Api.UserProfile.SaveProfile) {
  const { nickName, email, wechat } = input;

  const data = { nickName, email };

  if (wechat) {
    data.wechat = wechat;
  }

  return flatRequest<App.Service.Response>({
    url: '/api/userprofile',
    method: 'put',
    data
  });
}

export function changePasswordApi(input: Api.UserProfile.ChangePassword) {
  const { oldPassword, password } = input;

  return flatRequest<App.Service.Response>({
    url: '/api/userprofile/changepassword',
    method: 'put',
    data: {
      oldPassword,
      password
    }
  });
}
