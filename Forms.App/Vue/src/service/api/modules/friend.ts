import { flatRequest } from '../../request';

export function fetchGetFriendList(input: Api.Friend.SearchFilter) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };

  if (input.id) {
    data.id = input.id;
  }

  return flatRequest({
    url: '/api/friend/list',
    params: { ...data }
  });
}
export function fetchGetFriendShopList(id: string, planformType: number) {
  const data = { id, planformType };

  data.id = id;
  data.planformType = planformType;
  return flatRequest({
    url: '/api/friend/frendshoplist',
    params: { ...data }
  });
}

export function fetchGetCheckFriend(friendAccount: string) {
  return flatRequest({
    url: `/api/friend/${friendAccount}`
  });
}

export function fetchUpdateFriend(input: Api.Friend.UpdateFriend) {
  const { id } = input;
  const data = {
    id
  };

  if (input.friendAccount) {
    data.friendAccount = input.friendAccount;
  }
  if (input.friendNickName) {
    data.friendNickName = input.friendNickName;
  }
  return flatRequest({
    url: '/api/friend',
    method: 'put',
    data
  });
}

export function fetchDeleteFriend(ids: Array) {
  return flatRequest({
    url: '/api/friend/delete',
    method: 'delete',
    params: { ids }
  });
}

export function fetchGetRequestList(input: Api.Friend.RequestSearchFilter) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };
  if (input.Account) {
    data.Account = input.Account;
  }
  if (input.NickName) {
    data.NickName = input.NickName;
  }

  data.status = input.status;

  return flatRequest({
    url: '/api/friend/Requestlist',
    params: { ...data }
  });
}

export function fetchSendFriend(input: Api.Friend.createRequest) {
  const { friendId, friendAccount, reasons } = input;
  const json: Api.Friend.createRequest = {
    friendId,
    friendAccount,
    reasons
  };

  json.friendId = input.friendId;
  json.friendAccount = input.friendAccount;
  json.reasons = input.reasons;

  return flatRequest({
    url: '/api/friend/sendrequest',
    method: 'post',
    data: json
  });
}

export function fetchCreateFriend(input: Api.Friend.CreateFriend) {
  const { id, consent, refuse } = input;
  const json: Api.Friend.CreateFriend = {
    id,
    consent,
    refuse
  };
  if (input.id) {
    json.id = input.id;
  }

  json.consent = input.consent;
  json.refuse = input.refuse;

  return flatRequest({
    url: '/api/friend/request',
    method: 'post',
    data: json
  });
}

export function fetchRefuseFriend(id: string) {
  // const { id } = input;
  // const json: Api.Friend.refuseRequest = {
  //   id
  // };
  // json.id =id;

  // const formData = objectToFormData(json);

  return flatRequest({
    url: '/api/friend/refuse',
    method: 'put',
    params: {
      id
    }
  });
}

export function fetchCreateAuditShop(input: Api.Friend.createAudit) {
  const { auditId, friendId, shopName, planformType } = input;
  const json: Api.Friend.createAudit = {
    auditId,
    friendId,
    shopName,
    planformType
  };

  json.auditId = input.auditId;
  json.friendId = input.friendId;
  json.shopName = input.shopName;
  json.planformType = input.planformType;

  return flatRequest({
    url: '/api/friend/auditcreate',
    method: 'post',
    data: json
  });
}

export function fetchGetAuditShopList(input: Api.Friend.AuditSearchFilter) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };

  if (input.shopName) {
    data.shopName = input.shopName;
  }
  if (input.id) {
    data.id = input.id;
  }

  return flatRequest({
    url: '/api/friend/auditlist',
    params: { ...data }
  });
}

export function fetchRevokeAudit(id: string) {
  return flatRequest({
    url: '/api/friend/revoke',
    method: 'put',
    params: {
      id
    }
  });
}

export function fetchDeleteAudit(ids: Array) {
  return flatRequest({
    url: '/api/friend/deleteaudit',
    method: 'delete',
    params: { ids }
  });
}
