/**
 * Namespace Api
 *
 * All backend api type
 */
declare namespace Api {
  namespace Common {
    /** common params of paginating */
    interface PaginatingCommonParams {
      /** current page number */
      current: number;
      /** page size */
      size: number;
      /** total count */
      total: number;
    }

    /** common params of paginating query list data */
    interface PaginatingQueryRecord<T = any> extends PaginatingCommonParams {
      records: T[];
    }

    /** common search params of table */
    type CommonSearchParams = Pick<Common.PaginatingCommonParams, 'current' | 'size'>;

    /**
     * enable status
     *
     * - "1": enabled
     * - "2": disabled
     */
    type EnableStatus = '1' | '2';

    /** common record */
    type CommonRecord<T = any> = {
      /** record id */
      id: number;
      /** record creator */
      createBy: string;
      /** record create time */
      createTime: string;
      /** record updater */
      updateBy: string;
      /** record update time */
      updateTime: string;
      /** record status */
      status: EnableStatus;
    } & T;

    type PageFilter = {
      pageIndex: number;
      pageSize: number;
    };
  }

  /**
   * Namespace Auth
   *
   * Backend api module: "auth"
   */
  namespace Auth {
    interface LoginValidation {
      oathCode: string;
    }

    interface LoginToken {
      token: string;
      refreshToken: string;
    }

    interface Profile {
      id: string;
      email: string;
      nickName: string;
      avatar: string;
      gender: number;
      roleId: string;
      isSuperAdmin: boolean;
      isTenant: boolean;
      isTenantUser: boolean;
      isLogin: boolean;
      isSettlement: boolean;
      tenantType: number;
    }
  }

  /**
   * Namespace Route
   *
   * Backend api module: "route"
   */
  namespace Route {
    type ElegantConstRoute = import('@elegant-router/types').ElegantConstRoute;

    interface MenuRoute extends ElegantConstRoute {
      id: string;
    }

    interface UserRoute {
      routes: MenuRoute[];
      home: import('@elegant-router/types').LastLevelRouteKey;
    }
  }

  namespace Device {
    type SearchFilter = Common.PageFilter & {
      id?: string | null;
      userId?: string | null;
      status?: number | null;
      deviceName?: string | null;
      runCount?: number | null;
      coreRunCount?: number | null;
      type?: number | null;
      operator?: number | null;
    };

    type DeviceData = {
      id: string;
      deviceId: string;
      name: string;
      status: number;
      online: boolean;
      lastOnlineTime: string;
      createTime: string;
      leader: boolean;
      expiredTime?: string | null;
      status: number;
      days: number;
      addressId?: string | null;
      appVersion: string;
    };

    type UpdateDevice = {
      id: string;
      name?: string | null;
      supportedPlatforms?: Array<number> | null;
      type?: boolean | null;
      remark?: string | null;
      canUse?: number | null;
      operator?: number | null;
      days?: number | null;
    };

    type DeviceRecharge = {
      id: string;
      days?: number | null;
    };

    type CommandRecordSearchFilter = Common.PageFilter & {
      deviceId: string;
      command?: number | null;
      beginTime?: string | null;
      endTime?: string | null;
    };

    type DeviceDetail = {
      type: number;
      name: string;
      appVersion: string;
      scriptVersion: string;
      screenHeight: number;
      screenWidth: number;
      status: number;
      lastOnlineTime?: string | null;
      remark: string;
      deviceModel: string;
      deviceBrand: string;
      systemVersion: string;
      createTime: string;
      expiredTime?: string | null;
      ip?: string | null;
      ipAddress?: string | null;
      wifiName?: string | null;
      wifiPassword?: string | null;
      runCount?: number | null;
      coreCount?: number | null;
      runScript?: string | null;
      todayRuNCount: number;
      groupName?: string | null;
      offLineMinutes: string;
      days: number;
    };
  }

  namespace DeviceTaobao {
    type SearchFilter = Common.PageFilter & {
      deviceId?: string | null;
      deviceName?: string | null;
      account?: string | null;
      buyPayType?: number | null;
      init?: number | null;
      userId?: string | null;
    };

    type Data = {
      taoQi?: string | null;
      age?: string | null;
      buyPayType: number;
      payPhone?: string | null;
      payPassword?: string | null;
      init?: number | null;
      lastInitTime?: string | null;
      name?: string | null;
      phone?: string | null;
      province?: string | null;
      city?: string | null;
      address?: string | null;
    };

    type Update = {
      id: string;
      taoQi: number;
      age: string;
      buyPayType: number;
      payPhone: string;
      payPassword: string;
      name?: string;
      phone?: string;
      province?: string;
      city?: string;
      address?: string;
    };
  }

  namespace DeviceDouyin {
    type SearchFilter = Common.PageFilter & {
      deviceId?: string | null;
      deviceName?: string | null;
      account?: string | null;
      buyPayType?: number | null;
      init?: number | null;
      userId?: string | null;
      allowPair?: boolean | null;
    };

    type Data = {
      buyPayType: number;
      payPhone?: string | null;
      payPassword?: string | null;
      init?: number | null;
      lastInitTime?: string | null;
      name?: string | null;
      phone?: string | null;
      province?: string | null;
      city?: string | null;
      address?: string | null;
      allowPair: boolean;
    };

    type Update = {
      id: string;
      buyPayType: number;
      allowPair: boolean;
      payPhone?: string;
      payPassword?: string;
      name?: string;
      phone?: string;
      province?: string;
      city?: string;
      address?: string;
    };
  }

  namespace DeviceAli {
    type SearchFilter = Common.PageFilter & {
      deviceId?: string | null;
      deviceName?: string | null;
      account?: string | null;
      buyPayType?: number | null;
      init?: number | null;
      userId?: string | null;
    };

    type Data = {
      currentMonth?: number | null;
      lastMonth?: number | null;
      buyPayType: number;
      payPhone?: string | null;
      payPassword?: string | null;
      init?: number | null;
      lastInitTime?: string | null;
    };

    type Update = {
      id: string;
      currentMonth: number;
      lastMonth: number;
      payPhone: string;
      payPassword: string;
      addressId: string;
      name?: string;
      phone?: string;
      province?: string;
      city?: string;
      address?: string;
    };
  }

  namespace UserProfile {
    type SaveProfile = {
      nickName: string;
      email: string;
      Wechat: string;
    };

    type ChangePassword = {
      oldPassword: string;
      password: string;
    };

    type Settlement = {
      alipayAccount?: string | null;
      alipayQRCode?: object | null;
      wechatPayQRCode?: object | null;
    };
  }

  namespace Product {
    type SearchFilter = Common.PageFilter & {
      shopName?: string | null;
      itemId?: string;
      title?: string | null;
      userId?: string | null;
    };

    type SkuMap = {
      combination: Array<string>;
      price: string;
    };

    type ApiSkuMap = {
      sku: Array<string>;
      price: string;
    };

    type Data = {
      id: string;
      type: number;
      shopName: string;
      itemId: string;
      title: string;
      image: string;
      url: string;
      skuMap: SkuMap;
      userId?: string | null;
      userName?: string | null;
      createTime: string;
    };

    type Create = {
      type: number;
      shopId: string;
      itemId: string;
      title: string;
      image: string;
      url: string;
      skuMap: SkuMap;
    };

    type Update = {
      id: string;
      type: number;
      shopId: string;
      itemId: string;
      title: string;
      image: string;
      url: string;
      skuMap: SkuMap;
    };
  }

  namespace User {
    type UserSearchFilter = Common.PageFilter & {
      email?: string | null;
      roldId?: number | null;
      status?: number | null;
    };

    type UserData = {
      id: string;
      email: string;
      nickName: string;
      status: number;
      roleId: number;
      roleName?: string | null;
      createTime: string;
    };

    type UserCreate = {
      email: string;
      password: string;
      nickName: string;
      roleId: number;
    };

    type UserUpdate = {
      id: string;
      email?: string | null;
      pasword?: string | null;
      nickName?: string | null;
      roleId?: number | null;
      status?: number | null;
    };
  }

  namespace Tenant {
    type ListSearchFilter = Common.PageFilter & {
      email?: string | null;
      status?: number | null;
      tenantId?: string | null;
    };

    type TenantData = {
      id: string;
      email?: string | null;
      nickName: string;
      tenantType: number;
      status: number;
      deviceCount: number;
      permission: boolean;
      createTime: string;
    };

    type TenantCreate = {
      email: string;
      password: string;
      nickName: string;
      days?: number | null;
      payType: Array<number>;
      platformType: Array<number>;
    };

    type TenantUpdate = {
      id: string;
      email?: string | null;
      pasword?: string | null;
      nickName?: string | null;
      tenantType?: number | null;
      status?: number | null;
      payType?: Array<number>;
      platformType: Array<number>;
    };

    type TenantRecharge = {
      id: string;
      days?: number | null;
      months?: number | null;
      quarters?: number | null;
      years?: number | null;
    };

    type TenantSubscription = {
      days: number;
      totalDays: number;
      months: number;
      totalMonths: number;
      quarters: number;
      totalQuarters: number;
      years: number;
      totalYears: number;
    };
  }

  namespace TenantUser {
    type ListSearchFilter = Common.PageFilter & {
      email?: string | null;
      status?: number | null;
    };

    type TenantUserData = {
      id: string;
      email?: string | null;
      nickName: string;
      status: number;
      tenantName?: string | null;
      usedAmount?: string | null;
      createTime: string;
    };

    type TenantUserCreate = {
      email: string;
      password: string;
      nickName: string;
      isTenantAdmin: boolean;
    };

    type TenantUserUpdate = {
      id: string;
      email?: string | null;
      pasword?: string | null;
      nickName?: string | null;
      status?: number | null;
      isTenantAdmin?: boolean;
    };
  }

  namespace Enums {
    type Map<T = unknown> = {
      text: string;
      value: T;
    };

    type MapData<T = unknown, TData = unknown> = Map<T> & {
      data: TData;
    };

    type DeviceAddress = {
      id: string;
      deviceId?: string | null;
      province: string;
      city: string;
      address: string;
      name: string;
      phone: string;
    };
  }

  namespace Role {
    type RoleData = {
      id: string;
      roleName: string;
      count: string;
    };

    type UpdateRole = {
      id: string;
      roleName: string;
    };

    type RolePermissionChild = {
      key: string;
      title: string;
      selectable: boolean;
    };

    type RolePermission = {
      key: string;
      title: string;
      selectable: boolean;
      child: Array<RolePermissionChild>;
    };
  }

  namespace PlanTask {
    type ListSearchFilter = Common.PageFilter & {
      platform?: number | string | null;
      userId?: string | null;
      deviceId?: string | null;
      title?: string | null;
      status?: number[];
      mode?: number | string | null;
      shopId?: string | null;
      shopName?: string | null;
      success?: boolean | null;
      orderId?: string | null;
      beginTime?: string | null;
      endTime?: string | null;
    };

    type PlanTaskData = {
      id: string;
      platform: number;
      pairId: string;
      title: string;
      count: string;
      price: string;
      totalPrice: string;
      addCart: boolean;
      favorite: boolean;
      status: number;
      beginTime: string;
      endTime: string;
      creatorId?: string | null;
      creator?: string | null;
    };

    type SkuMap = {
      sku: Array<string>;
      price: string;
      count: number;
    };

    type Actions = {
      shopAround: ShopAround;
      addCart: boolean;
      favorite: boolean;
      lookComment: boolean;
      lookPlantingGrass: boolean;
      lookAskEveryone: boolean;
      useFirstBrowse: boolean;
      useFindBrowse: boolean;
      useWaitForBuy: boolean;
    };

    type ShopAround = {
      min: number;
      max: number;
    };

    type OperatePlanTask = {
      id: '';
      platform: number;
      shopName: string;
      title: string;
      keyword: string;
      skuList: Array<SkuMap>;
      count: number;
      scope: number;
      pairScope: number;
      actions: Actions;
      excludeArea: Array<string>;
      beginTime: string;
      endTime: string;
      deviceId?: string | null;
    };

    type ProductSearchFilter = Common.PageFilter & {
      platform: number;
      shopId?: string;
      itemId?: string;
      title?: string;
    };

    type ProductData = {
      id: string;
      shopName: string;
      itemId: string;
      title: string;
      url: string;
      skuMap: SkuMap;
    };

    type ProductSku = {
      skuList: Array<string>;
      price: string;
      count: number;
    };

    type CreatePlanTask = {
      mode: number;
      multiplePlatform: Array<int> | null;
      shopId: string;
      shopName: string;
      friendId?: string | null;
      deviceId?: string | null;
      products: Array<any>;
      pairScope: number;
      maxPairProductCoun?: number | null;
      actions?: object;
      excludeArea?: Array<string>;
      beginTime: string;
      endTime: string;
      chatMessages?: string[];
    };

    type UpdatePlanTask = {
      id: string;
      keyword: string;
      skuList?: Array<string> | null;
      count?: number | null;
      scope?: number | null;
      actions: Actions;
      excludeArea?: Array<string> | null;
      beginTime?: string | null;
      endTime?: string | null;
    };

    type PlanTaskDetail = {
      owner?: PlanTaskDetailOwner | null;
      pair?: PlanTaskDetailPair | null;
    };

    type PlanTaskDetailOwner = {
      platform: number;
      deviceId?: number;
      deviceName?: string;

      taobaoAccount?: object;
      aliAccount?: object;

      products: Array<object>;
      productCount: number;
      maxPairProductCount: number;
      totalPrice: number;

      actions: object;
      success?: boolean;
      status: number;

      pairScope?: number;
      excludeArea?: string[];

      orderId?: string;
      payPrice?: number;
      payType?: number;

      completeTime?: string;
      remark?: string;

      diffPrice?: number;
      note?: string;
    };

    type PlanTaskDetailPair = {
      platform: number;

      deviceId?: number;

      deviceName?: string;

      taobaoAccount?: object;

      aliAccount?: object;

      products: Array<object>;

      productCount: number;

      totalPrice: number;

      success?: boolean;

      status: number;

      orderId?: string;

      payPrice?: number;

      payType?: number;

      completeTime?: string;

      remark?: string;
    };

    type PlanTaskContinue = {
      id: string;
      continue: boolean;
      cancel: boolean;
      skuList?: Array<string>;
      count?: number;
    };

    type AgainPlanTask = {
      oldTaskId: string;
      mode: number;
      multiplePlatform: Array<int> | null;
      shopId: string;
      shopName: string;
      friendId?: string | null;
      deviceId?: string | null;
      products: Array<any>;
      pairScope: number;
      maxPairProductCoun?: number | null;
      actions?: object;
      excludeArea?: Array<string>;
      beginTime: string;
      endTime: string;
      chatMessages?: string[];
    };

    type PlanTaskCompletion = {
      id: string;
      success: boolean;
      orderId?: string | null;
      payPrice?: number | string;
      province?: string | null;
      city?: string | null;
      address?: string | null;
      name?: string | null;
      phone?: string | null;
      note?: string | null;
      lovePayId?: string | null;
      lovePayName?: string | null;
    };

    type PlanTaskUploadChunkRequest = {
      name: string;
      chunk: string;
      chunkIndex: number;
      chunkFolder?: string | null;
      totalSize: number;
      chunkSize: number;
      totalChunks: number;
    };
    type PlanTaskMergeChunkRequest = {
      name: string;
      chunkFolder: string;
      totalChunks: number;
      fileExtension: string;
    };
  }

  namespace AutoJsScript {
    type ListSearchFilter = Common.PageFilter & {
      name?: string | null;
      version?: string | null;
    };

    type AutoJsScriptData = {
      id: string;
      name: string;
      version: string;
      remark?: string | null;
      filePath: string;
    };

    type CreateAutoJsScript = {
      name: string;
      version: string;
      remark?: string | null;
      filePath: string;
      size: string;
    };

    type UploadChunkRequest = {
      name: string;
      version: string;
      chunk: string;
      chunkIndex: number;
      chunkFolder?: string | null;
      totalSize: number;
      chunkSize: number;
      totalChunks: number;
    };

    type MergeChunkRequest = {
      name: string;
      version: string;
      chunkFolder: string;
      totalChunks: number;
    };

    type MergeChunkResponse = {
      filePath: string;
      size: string;
    };
  }

  namespace Dashboard {
    type HeaderData = {
      unSettlement: number;
      settlemented: number;
      settlementFailed: number;
    };

    type CardData = {
      totalDevice: number;
      onlineDevice: number;
      product: number;
      planTask: number;
    };

    type LineChart = {
      xAxis: Array<string>;
      total: Array<number>;
      success: Array<number>;
      failed: Array<number>;
    };

    type PieChart = {
      settlemented: number;
      settlementFailed: number;
      pairSettlementFailed: number;
      unSettlement: number;
      pairUnSettlement: number;
    };
  }

  namespace PlanTaskSettlement {
    type SearchFilter = Common.PageFilter & {
      status: number;
      userId: string;
      pairUserId: string;
    };

    type PlanTaskSettlementData = {
      id: string;
      taskId: string;
      pairTaskId: string;
      payPrice: string;
      pairPayPrice: string;
      pairDiffPrice: string;
      pairUserId: string;
      status: number;
      create: string;
      settlementTime?: string | null;
    };
  }

  namespace PlanTaskComment {
    type SearchFilter = Common.PageFilter & {
      platform?: number | null;
      taskId?: string | null;
      deviceId?: string | null;
      orderId?: string | null;
      status?: number | null;
      beginTime?: string | null;
      endTime?: string | null;
      tenantId?: string | null;
      userId?: string | null;
    };

    type CommentData = {
      id: string;
      taskId: string;
      productTitle: string;
      productImage: string;
      orderId: string;
      text?: string | null;
      images?: Array<string> | null;
      videos?: Array<string> | null;
      status: number;
      creator?: string | null;
      tenantUser?: string | null;
      completeTime?: string | null;
    };

    type CreateComment = {
      taskId: string;
      mode: number;
      text?: string | null;
      images?: Array<object> | null;
      videos?: Array<object> | null;
    };

    type UpdateComment = {
      id: string;
      taskId: string;
      mode: number;
      text?: string | null;
      images?: object | null;
      videos?: object | null;
    };
  }

  namespace Configuration {
    type EmailConfiguration = {
      emailAddress: string;
      emailUser: string;
      stmpServer: string;
      stmpPort: number;
      emailPassword: string;
      emailSignature: string;
      useSSL: boolean;
    };

    type ConfigurationData = {
      id: string;
      name: string;
      value: string;
    };

    type UpdateConfiguration = {
      id: string;
      value: string;
    };
  }

  namespace DeviceCommand {
    type SearchFilter = Common.PageFilter & {
      id?: string | null;
      deviceId?: string | null;
      taskId?: stirng | null;
      command?: number | null;
      status?: number | null;
      beginTime?: string | null;
      endTime?: string | null;
    };

    type DeviceCommandData = {
      id: string;
      deviceId: string;
      appDeviceId: string;
      deviceName: string;
      taskId: string;
      command: number;
      status: number;
      createTime: string;
      assignTime: string;
      ipAddress: string;
      logFile?: string;
      screenshot?: string;
    };

    type DeviceCommandDetail = {
      data?: string | null;
      result?: string | null;
      completeTime?: string | null;
      assignExpiredTime?: string | null;
    };
  }

  namespace CategoryRepeatPurchase {
    type SearchFilter = Common.PageFilter & {
      name?: string | null;
    };

    type CategoryRepeatPurchaseData = {
      id: string;
      name: string;
      keyword: string;
      limitDays: number;
    };

    type Create = {
      name: string;
      keyword: string;
      limitDays: number;
    };

    type Update = Create & {
      id: string;
    };
  }

  namespace Common {
    type Slider = {
      path: string;
      width: number;
      height: number;
      l: number;
      r: number;
      timespan: string;
    };

    type SliderData = {
      x: number;
      y: number;
      validate: string;
    };
  }

  namespace Logger {
    type SystemSearchFilter = Common.PageFilter & {
      level: number;
      beginTime: string;
      endTime: string;
    };

    type SystemData = {
      id: string;
      level: number;
      className: string;
      methodName: string;
      message: string;
      stackTrace: string;
      createTime: string;
    };

    type RequestSearchFilter = Common.PageFilter & {
      traceId: string;
      url: string;
      httpMethod: string;
      success: number;
      beginTime: string;
      endTime: string;
    };
  }

  namespace AlipayLovePay {
    type ListSearchFilter = Common.PageFilter & {
      alipayName?: string | null;
      alipayAccount?: string | null;
      userId?: string | null;
      shopName?: string | null;
      tenantUser?: string | null;
      alipayLovePayType?: number | null;
    };

    type AlipayLovePayData = {
      id: string;
      alipayName?: string | null;
      alipayAccount?: string | null;
      createUserAndShop?: string | null;
      alipayLovePayType?: number | null;
      createId?: string | null;
    };

    type AlipayLovePayCreate = {
      alipayLovePayType?: number;
      alipayName?: string | null;
      alipayAccount?: string | null;
      tenantUser?: string | null;
      shopName?: string | null;
    };

    type AlipayLovePayUpdate = {
      id: string;
      alipayLovePayType: number;
      alipayName?: string | null;
      alipayAccount?: string | null;
      tenantUser?: string;
      shopName: string;
      newShopName: string;
      shopNameEnum: Array<any>;
    };
  }

  namespace AlipayLovePayRecord {
    type ListSearchFilter = Common.PageFilter & {
      platform?: number | null;
      alipayLovePayId?: string | null;
      userId?: string | null;
      settlemented?: number | null;
    };

    type AlipayLovePayRecordData = {
      id: string;
      platform: number;
      alipayAccount: string;
      alipayName: string;
      usedUserId: string | null;
      usedUserName: string | null;
      userId: string | null;
      userName: string | null;
      planTaskId: string;
      orderId: string;
      amount: string;
      createTime: string;
      tenantName?: string | null;
      settlemented: boolean;
    };
  }
  namespace AlipayLovePayEntSettle {
    type ListSearchFilter = Common.PageFilter & {
      platform?: number | null;
      beginTime?: string | null;
      endTime?: string | null;
      alipayLovePayId?: string | null;
      shopName?: string | null;
      settlemented?: number | null;
      userId?: string | null;
    };
    type AlipayLovePayEntSettleData = {
      id: string;
      platform: number;
      alipayAccount: string;
      alipayName: string;
      shopName: string;
      usedUserId: string | null;
      usedUserName: string | null;
      // userId: string | null;
      // userName: string | null;
      planTaskId: string;
      orderId: string;
      amount: string;
      priceDiff: string;
      createTime: string;
      tenantName?: string | null;
      settlemented: boolean;
    };
  }
  namespace AlipayLovePayPerSettle {
    type ListSearchFilter = Common.PageFilter & {
      platform?: number | null;
      beginTime?: string | null;
      endTime?: string | null;
      settlemented?: number | null;
      userId?: string | null;
      usedUserId?: string | null;
    };
    type AlipayLovePayEntSettleData = {
      id: string;
      platform: number;
      alipayAccount: string;
      alipayName: string;
      usedUserId: string | null;
      usedUserName: string | null;
      userId: string | null;
      userName: string | null;
      planTaskId: string;
      orderId: string;
      amount: string;
      priceDiff: string;
      createTime: string;
      tenantName?: string | null;
      settlemented: boolean;
    };
  }
  namespace AlipayLovePayRecordPayPerSettle {}
  namespace Shop {
    type BaseSearchFilter = Common.PageFilter & {
      shopName?: string | null;
      tenantId?: string | null;
    };

    type BaseData = {
      id: stirng;
      shopName: string;
      count: number;
      tenantUser?: string | null;
      createTime: string;
    };

    type Update = {
      id: string;
      shopName: string;
    };
  }

  namespace Friend {
    type SearchFilter = Common.PageFilter & {
      id: string;
    };

    type shopFilter = Common.PageFilter & {
      id: string;
      planformType: number;
    };

    type ShopSearchFilter = Common.PageFilter & {
      id: string;
    };

    type AuditSearchFilter = Common.PageFilter & {
      shopName?: string | null;
      id?: string;
    };

    type FriendData = {
      id: string;
      friendAccount: string;
      friendNickName: string;
      auditShopCount: number;
      states: boolean;
    };

    type createRequest = {
      friendId: string;
      friendAccount: string;
      reasons: string;
    };

    type createAudit = {
      auditId: string | null;
      friendId: string;
      shopName: string;
      planformType: number;
    };

    type refuseRequest = {
      id: string;
    };

    type UpdateFriend = {
      id: string;
      friendAccount: string;
      friendNickName?: string | null;
    };

    type CreateFriend = {
      id?: string | null;
      consent: number;
      refuse: string | null;
    };

    type RequestSearchFilter = Common.PageFilter & {
      Account: string | null;
      NickName?: string | null;
      status?: number;
    };
  }

  namespace NetworkPairing {
    interface LevelLimit {
      level: number;
      dayLimit: number;
      monthLimit: number;
    }

    interface Configuration {
      levelLimits: LevelLimit[];
      selectedLevel: number;
    }

    interface Response<T> {
      data: T;
      success: boolean;
      message?: string;
    }
  }
}
