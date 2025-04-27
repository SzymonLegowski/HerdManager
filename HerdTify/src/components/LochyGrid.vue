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
    case 'Wolna': return 'bg-wolna';
    case 'Pokryta': return 'bg-pokryta';
    default: return 'bg-default'; // Jeśli nie ma statusu, tło domyślne
  }
};
  
  const emit = defineEmits(['update:selectedLocha']);
  
  const handleClick = (number) => {
    
    if(props.items.map(locha => locha.numerLochy).includes(number))
      {console.log(`Button ${number} clicked`);}
    emit('update:selectedLocha', number);
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

  .bg-karmiaca {
    background: #ce1212;
  }
  .bg-wolna {
    background: #159719;
  }
  .bg-pokryta {
    background: #ab6a08;
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