<script setup lang="ts">
  import { defineProps, ref } from 'vue';
  import { useThemeStore } from '@/store/modules/theme';

  defineOptions({
    name: 'TextSortSlider'
  });

  interface Word {
    text: string;
    x: number;
    y: number;
    clickable: boolean;
    color: string;
    rotation: string;
  }

  type Props = {
    words: Word[];
    image: string;
  };

  const themeStore = useThemeStore();
  const props = defineProps<Props>();
  const clickedIndices = ref<number[]>([]); // 存储已点击的文字索引
  const clickMarks = ref<{ x: number; y: number; index: number }[]>([]); // 存储点击的坐标和顺序

  // 初始化每个单词的颜色和旋转角度
  props.words.forEach(word => {
    word.color = randomColor();
    word.rotation = randomRotation();
  });

  // 允许的点击误差范围
  const tolerance = 20; // 允许的误差范围（单位：像素）

  function handleClick(event: MouseEvent) {
    const rect = event.currentTarget.getBoundingClientRect(); // 获取图片的位置信息
    const clickX = event.clientX - rect.left; // 点击的相对位置
    const clickY = event.clientY - rect.top;

    // 在点击位置显示标志
    clickMarks.value.push({
      x: clickX,
      y: clickY,
      index: clickMarks.value.length + 1 // 顺序从 1 开始
    });

    // 遍历所有单词，判断点击位置是否接近单词区域
    props.words.forEach((word, index) => {
      const wordCenterX = rect.width * (word.x / 100);
      const wordCenterY = rect.height * (word.y / 100);

      // 计算点击位置与文字中心点的差值
      const diffX = Math.abs(clickX - wordCenterX);
      const diffY = Math.abs(clickY - wordCenterY);

      // 如果点击位置在误差范围内，并且该单词是可点击的
      if (diffX <= tolerance && diffY <= tolerance && word.clickable) {
        if (!clickedIndices.value.includes(index)) {
          clickedIndices.value.push(index); // 添加已点击的单词索引
        }
      }
    });
  }

  function resetCaptcha() {
    clickedIndices.value = [];
    clickMarks.value = []; // 清空点击标记
  }

  function confirmSelection() {
    const clickableWords = props.words.filter(word => word.clickable);
    const clickedWords = clickedIndices.value.map(index => props.words[index]);

    // 如果点击的单词数量与需要点击的单词数量不匹配，则验证失败
    if (clickableWords.length !== clickedWords.length) {
      return false;
    }

    // 检查点击的单词是否与顺序一致
    return clickedWords.every((word, i) => word.clickable && clickableWords[i]?.text === word.text);
  }

  function wordStyle(word: Word) {
    const margin = 10;
    return {
      position: 'absolute',
      top: `calc(${word.y}% - ${margin}px)`,
      left: `calc(${word.x}% - ${margin}px)`,
      color: word.color,
      fontWeight: 'bold',
      cursor: 'pointer',
      whiteSpace: 'nowrap',
      transform: `rotate(${word.rotation})`
    };
  }

  // 随机生成颜色
  function randomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i += 1) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

  // 随机生成旋转角度，范围在 10 到 360 度之间
  function randomRotation() {
    return `${Math.floor(Math.random() * 351) + 10}deg`; // 旋转角度在 10 到 360 之间
  }

  defineExpose({
    confirmSelection,
    resetCaptcha
  });
</script>

<template>
  <div class="captcha-container" :class="{ dark: themeStore.darkMode }">
    <div class="captcha-image" @click="handleClick">
      <img :src="props.image" alt="验证码图片" />
      <div v-for="(word, index) in props.words" :key="index" class="captcha-word" :style="wordStyle(word)">
        {{ word.text }}
      </div>
      <!-- 显示点击位置的标记 -->
      <div v-for="(mark, index) in clickMarks" :key="index" class="click-mark" :style="{ top: `${mark.y - 12}px`, left: `${mark.x - 12}px` }">
        <span class="click-order">{{ mark.index }}</span>
      </div>
    </div>
    <p>
      请依次点击 【{{
        props.words
          .filter(word => word.clickable)
          .map(word => word.text)
          .join('、')
      }}】
    </p>
  </div>
</template>

<style scoped lang="scss">
  .captcha-container {
    width: 300px;
    background: #fff;
    color: #333;
    padding: 10px;
    border-radius: 8px;
    text-align: center;
    border: 1px solid #e4e4e7;

    &.dark {
      background: #333;
      color: #fff;
    }
  }

  .captcha-image {
    position: relative;
    margin: 10px auto;
    overflow: hidden;
    border-radius: 5px;
  }

  .captcha-word {
    font-size: 20px;
    user-select: none;
    position: relative;
  }

  .click-order {
    position: absolute;
    inset: 0;
    background: #006be6;
    color: #fff;
    border-radius: 50%;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 12px;
  }

  .click-mark {
    position: absolute;
    border-radius: 50%;
    width: 20px;
    height: 20px;
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 10; /* 确保标志不被覆盖 */
    border: 2px solid #fff;
    overflow: hidden;
  }
</style>
