import type dayjs from 'dayjs';

export type AntdDesignTimePicker = {
  beginTime?: string;
  endTime?: string;
};

export type AntdDesignTimePickerProps = {
  placeholder?: Array<string>;
  format?: string;
  defaultValue?: Array<dayjs.Dayjs>;
};
