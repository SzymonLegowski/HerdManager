<template>
    <div class="grid-container" >
      <v-btn
        v-for="number in 100"
        :key="number"
        class="grid-item" 
        :class="getColorClass(number)"
        @click="handleClick(number)"
        outlined
      >
        {{ number }}
      </v-btn>
    </div>
  </template>
  
  <script setup>
import apiClient from '@/plugins/axios';

  
  const props = defineProps({
    items: {
      type: Array,
      required: true
    }
  });

  const lochyMap = computed(() => {
  const map = new Map();
  props.items.forEach(locha => {
    map.set(locha.numerLochy, locha.statusLochy);
  });
  return map;
});

  const getColorClass = (number) => {
  const status = lochyMap.value.get(number);
  switch (status) {
    case 'Karmiaca': return 'bg-karmiaca';
    case 'Luzna': return 'bg-luzna';
    case 'Pokryta': return 'bg-pokryta';
    case 'Prosna': return 'bg-prosna';
    default: return 'bg-default'; // Jeśli nie ma statusu, tło domyślne
  }
};
  
  const emit = defineEmits(['update:selectedLocha','quickAdd:newLochaId']);
  
  const handleClick = async (number) => {
    
    if(props.items.map(locha => locha.numerLochy).includes(number))
      {
        emit('update:selectedLocha', number);
      } 
    else
      {
        let newLocha = {numerLochy: number,
                        status: "Luzna",
                        wydarzeniaLochyId: [],
                        miotyId: []
        };
        let response = await apiClient.post('/Locha', newLocha);
        console.log(response.data);
        emit('quickAdd:newLochaId', response.data);
      }
  };
  </script>
  
  <style scoped>
  .grid-container {
    display: grid;
    grid-template-columns: repeat(10, 1fr);
    gap: 5px;
    position: absolute;
    background-color: #101010;
    transform: translate(100px, 5px);
    padding: 10px;
    border-radius: 10px;
    border-style: solid;
    border-color: #740fb7;
  }

  .bg-prosna {
    background: #ccaa00;
  }
  .bg-karmiaca {
    background: #ce1212;
  }
  .bg-luzna {
    background: #159719;
  }
  .bg-pokryta {
    background: #0f8ce5;
  }
  .bg-default {
    background: #353535;
  }

  .grid-item {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 40px;
    min-width: 0;
    min-height: 40px;
    }
  </style>