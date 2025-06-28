<script setup lang="ts">
  import { onMounted, reactive } from 'vue';
  import Profile from './modules/profile.vue';
  import Password from './modules/password.vue';

  type MenuAction = {
    name: Array<string>;
    active: string;
  };

  const menu = reactive<MenuAction>({
    name: ['profile', 'pwd', 'settlement', 'subscription'],
    active: 'profile'
  });

  const menuClickHandler = (name: string) => {
    if (menu.active === name) {
      return;
    }

    menu.active = name;
    location.href = `${location.origin}${location.pathname}#${name}`;
  };

  onMounted(() => {
    if (location.hash) {
      const hash = location.hash.replace('#', '');
      if (menu.name.includes(hash)) {
        menuClickHandler(hash);
      }
    }
  });
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <ACard title="个人中心" :bordered="false" :body-style="{ flex: 1, overflow: 'hidden' }" class="flex-col-stretch sm:flex-1-hidden card-wrapper">
      <div class="card-body">
        <div>
          <div class="menu-group">
            <p class="title">基础信息</p>

            <ul>
              <li :class="{ active: menu.active === 'profile' }" @click="menuClickHandler('profile')">个人信息</li>
              <li :class="{ active: menu.active === 'pwd' }" @click="menuClickHandler('pwd')">修改密码</li>
            </ul>
          </div>
        </div>
        <div class="card-container">
          <Transition name="tab-view" mode="out-in">
            <Profile v-if="menu.active === 'profile'"></Profile>
            <Password v-else-if="menu.active === 'pwd'"></Password>
          </Transition>
        </div>
      </div>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
  .card-body {
    display: grid;
    grid-template-columns: 200px 1fr;
    gap: 15px;
    height: 100%;
    overflow: hidden;

    > div {
      &:has(.menu-group) {
        padding: 0 15px;
      }
    }
  }

  .menu-group {
    padding: 15px 0 24px 0;
    border-bottom: 1px solid var(--color-secondary);

    .title {
      margin-bottom: 10px;
      font-size: 16px;
      font-weight: 600;
    }

    li {
      margin-bottom: 10px;
      cursor: pointer;
      transition: 0.3s ease-in-out;

      &:last-child {
        margin-bottom: 0;
      }

      &.active,
      &:hover {
        color: var(--color-purple);
      }

      &.active {
        font-weight: 600;
      }
    }
  }

  .card-container {
    width: 100%;
    padding: 10px;
    overflow: hidden;
  }

  .tab-view-enter-active {
    animation: slide-in-right 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94) both;

    @keyframes slide-in-right {
      0% {
        transform: translateX(1000px);
        opacity: 0;
      }

      100% {
        transform: translateX(0);
        opacity: 1;
      }
    }
  }

  .tab-view-leave-active {
    animation: slide-out-left 0.4s cubic-bezier(0.55, 0.085, 0.68, 0.53) both;

    @keyframes slide-out-left {
      0% {
        transform: translateX(0);
        opacity: 1;
      }

      100% {
        transform: translateX(-1000px);
        opacity: 0;
      }
    }
  }
</style>
