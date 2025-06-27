export interface RowListContent {
  title: string;
  text: Array<string>;
}

export interface RowContent {
  title: string;
  text?: Array<string>;
  list?: Array<RowListContent>;
}

export interface DocView {
  title: string;
  content: Array<RowContent>;
}
