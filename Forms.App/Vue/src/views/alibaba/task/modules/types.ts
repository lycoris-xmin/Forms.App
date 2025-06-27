export interface ProductData {
  id: string;
  shopName: string;
  url?: string;
  image?: string;
  title: string;
  keyword?: string;
  skuMap: Array<ProductSkuMap>;
  selectedSkuMap: string;
  skuList?: Array<string>;
  count: number;
  price: number;
  comment: Array<ProductComment>;
}

export interface ProductSkuMap {
  sku: Array<string>;
  price: number;
}

export interface ProductComment {
  hasComment: boolean;
  mode?: number | null;
  text?: string | null;
  images?: Array<File> | null;
  videos?: Array<File> | null;
}

export interface TaskActionOptions {
  min: number;
  max: number;
}

export interface TaskActions {
  shopAround: TaskActionOptions;
  addCart: boolean;
  favorite: boolean;
  lookComment: boolean;
  lookPlantingGrass: boolean;
  lookAskEveryone: boolean;
  useFirstBrowse: boolean;
  useFindBrowse: boolean;
  useWaitForBuy: boolean;
}

export interface TaskData {
  mode: number;
  friend: any; // 如果你知道具体类型可以替换 `any`
  deviceId: string | null;
  shopId: string;
  products: any[]; // 如果你有产品结构可以细化
  price: number;
  count: number;
  pairScope: number;
  maxPairProductCount: number;
  calculatePairScope: number;
  pairMultiple: number;
  actions: TaskActions;
  excludeArea: any[]; // 如果有具体结构可以细化
  beginTime: string;
  endTime: string;
}
