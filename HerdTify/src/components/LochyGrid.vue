<template>
    <div class="grid-container">
      <v-btn
        v-for="number in 100"
        :key="number"
        class="grid-item"
        :disabled="!items.includes(number)"
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
  
  const emit = defineEmits(['update:selectedLocha']);
  
  const handleClick = (number) => {
    console.log(`Button ${number} clicked`);
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
    border-color: #06838f;
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