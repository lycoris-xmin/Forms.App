import Decimal from 'decimal.js';

export interface DecimalCalculatorOptions {
  precision?: number; // 全局精度设置，默认18位
}

export default class DecimalCalculator {
  private result: Decimal;

  constructor(initialValue: string | number, options: DecimalCalculatorOptions = {}) {
    const precision = options.precision ?? 18;
    Decimal.set({ precision }); // 设置全局精度
    this.result = new Decimal(initialValue);
  }

  // 加法
  add(value: string | number) {
    this.result = this.result.plus(value);
    return this;
  }

  // 减法
  subtract(value: string | number) {
    this.result = this.result.minus(value);
    return this;
  }

  // 乘法
  multiply(value: string | number) {
    this.result = this.result.times(value);
    return this;
  }

  // 除法
  divide(value: string | number) {
    if (value === 0 || value === '0') {
      throw new Error('除数不能为零');
    }
    this.result = this.result.dividedBy(value);
    return this;
  }

  // 向上取整
  ceil() {
    this.result = this.result.ceil();
    return this;
  }

  // 向下取整
  floor() {
    this.result = this.result.floor();
    return this;
  }

  // 转为整数（截断）
  toInteger() {
    this.result = this.result.trunc();
    return this;
  }

  // 获取结果（number 类型）
  getResult(): number {
    return this.result.toNumber();
  }

  // 获取结果（string 类型，保留原始精度）
  toString(): string {
    return this.result.toFixed();
  }
}
