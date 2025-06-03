<template>
  <v-data-table
    :headers="headers"
    :items="items"
    item-value="id"
  >
    <template v-for="header in headers" #[`item.${header.value}`]="{ item }" :key="header.value">
      <div>
        <div v-for="(line, i) in item[header.value]" :key="i">
          {{ line }}
        </div>
      </div>
    </template>
  </v-data-table>
</template>
<script setup>
const headers = [
  { text: 'Kontakt', value: 'contact' },
  { text: 'Adres', value: 'address' }
]

const items = [
  {
    contact: ['Jan Kowalski', 'jan@example.com'],
    address: ['ul. Długa 10', '00-001 Warszawa']
  },
  {
    contact: ['Anna Nowak', 'anna@example.com'],
    address: ['ul. Krótka 5', '30-300 Kraków']
  }
]

</script>
