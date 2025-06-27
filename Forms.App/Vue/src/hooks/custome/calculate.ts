import Decimal from 'decimal.js';

export interface DecimalCalculatorOptions {
  precision?: number; // 计算精度（默认18位）
}

export default function useDecimalCalculator(options: DecimalCalculatorOptions = {}) {
  const precision = options.precision ?? 18;

  Decimal.set({ precision });

  class DecimalCalculator {
    private result: Decimal;

    constructor(initialValue: string | number) {
      this.result = new Decimal(initialValue);
    }

    add(value: string | number) {
      this.result = this.result.plus(value);
      return this;
    }

    subtract(value: string | number) {
      this.result = this.result.minus(value);
      return this;
    }

    multiply(value: string | number) {
      this.result = this.result.times(value);
      return this;
    }

    divide(value: string | number) {
      if (value === 0) {
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

    // 转为整数（截断小数部分）
    toInteger() {
      this.result = this.result.trunc();
      return this;
    }

    getResult(): number {
      return this.result.toNumber();
    }
  }

  const createCalculator = (initialValue: string | number) => {
    return new DecimalCalculator(initialValue);
  };

  return {
    createCalculator
  };
}
