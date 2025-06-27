import { defineStore } from 'pinia';
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { SetupStoreId } from '@/enum';

export const usePageParams = defineStore(SetupStoreId.PageParams, () => {
  const data = ref<Record<string, any>>({});

  const setParams = (name, _params) => {
    data.value[name] = _params;
  };

  function getParams() {
    const route = useRoute();

    const value = data.value[route.name];

    if (value === void 0) {
      return void 0;
    }

    Reflect.deleteProperty(data.value, route.name);

    return value;
  }

  return { setParams, getParams };
});
