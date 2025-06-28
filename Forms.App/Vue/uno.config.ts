import { defineConfig } from '@unocss/vite';
import transformerDirectives from '@unocss/transformer-directives';
import transformerVariantGroup from '@unocss/transformer-variant-group';
import presetUno from '@unocss/preset-uno';
import type { Theme } from '@unocss/preset-uno';
import { presetSoybeanAdmin } from '@sa/uno-preset';
import { themeVars } from './src/theme/vars';

export default defineConfig<Theme>({
  content: {
    pipeline: {
      exclude: ['node_modules', 'dist']
    }
  },
  theme: {
    ...themeVars,
    fontSize: {
      'icon-xs': '0.875rem',
      'icon-small': '1rem',
      icon: '1.125rem',
      'icon-large': '1.5rem',
      'icon-xl': '2rem'
    }
  },
  shortcuts: {
    'card-wrapper': 'rd-8px shadow-sm'
  },
  transformers: [transformerDirectives(), transformerVariantGroup()],
  presets: [presetUno({ dark: 'class' }), presetSoybeanAdmin()],
  rule: [
    [/^p-l-(\d+)$/, ([_, value]: RegExpMatchArray) => ({ 'padding-left': `${value}px` })],
    [/^p-r-(\d+)$/, ([_, value]: RegExpMatchArray) => ({ 'padding-right': `${value}px` })],
    [/^p-(\d+)$/, ([_, value]: RegExpMatchArray) => ({ padding: `${value}px` })],
    [/^m-(\d+)$/, ([_, value]: RegExpMatchArray) => ({ margin: `${value}px` })],
    [/^m-b-(\d+)$/, ([_, value]: RegExpMatchArray) => ({ 'margin-bottom': `${value}px` })],
    [/^m-t-(\d+)$/, ([_, value]: RegExpMatchArray) => ({ 'margin-top': `${value}px` })],
    [/^gap-(\d+)$/, ([_, value]: RegExpMatchArray) => ({ gap: `${value}px` })]
  ]
});
