<template>

<div class="ramka">
  <table id="Pdf-Table" class="PdfTable">
    <thead class="PdfTableHead">
      <tr>
        <th :colspan="colspanValue + 8">NR LOCHY</th>
        <th > {{ miotyStore.nrLochy }}  </th>
      </tr>
      <tr>
        <th :colspan="colspanValue + 1">Sektor krycia</th>
        <th colspan="7">Porodówka</th>
        <th class="poleOceny, PdfTableHeader" rowspan="4">Ocena prosiąt urodzonych(1-5)</th>
      </tr>
      <tr>
        <th rowspan="3">Nr miotu</th>
        <th :colspan="colspanValue">Data Pokrycia</th>
        <th colspan="3">Data</th>
        <th colspan="4">Liczba Prosiąt</th>
      </tr>
      <tr v-for="(row, rowIndex) in tableData" :key="rowIndex">
        <th v-for="(cell, cellIndex) in row" :key="cellIndex" rowspan="2">{{ cell }} </th>
        <th rowspan="2">Przew. oproszenia</th>
        <th rowspan="2">Oproszenia</th>
        <th rowspan="2">Odsadzenia</th>
        <th colspan="2">Urodzonych</th>
        <th rowspan="2">Przygniecionych</th>
        <th rowspan="2">Odsadzonych</th>
      </tr>
      <tr>
        <th >Żywe</th>
        <th >Martwe</th>
      </tr>
    </thead>
    <tbody class="PdfTableBody">
      <tr v-for="(row, rowIndex) in rowData" :key="rowIndex">
        <td class="PdfTableData" v-for="(cell, cellIndex) in row" :key="cellIndex">{{ cell }}</td>
      </tr>
    </tbody>
  </table>

  <div class="addColumnButtonBackground">
    <button class="addColumnButton" @click="addColumn">Dodaj kolumnę</button>
  </div>

  <div class="addColumnButtonBackground">
    <button class="addColumnButton" @click="addRow">Dodaj wiersz</button>
  </div>

</div>
</template>
<script setup>
  import { useMiotyStore } from '@/stores/miotyStore';
  import { ref } from 'vue';

  let colspanValue = ref(1);
  let tableData = ref([['1 (rasa kn.)']]);
  let rowData = ref([['test','test 1','test','test','test','test','test','test','test','test']]);
  let row = rowData;
  const miotyStore = useMiotyStore();
  const mioty = miotyStore.mioty;
  const nrLochy = miotyStore.nrLochy;
  
  

  const addColumn = () => {
    colspanValue.value += 1;
    tableData.value.forEach(row => {
      row.push(colspanValue.value + ' (rasa kn.)')
    })
    row.value[0].splice(colspanValue.value, 0, 'test ' + colspanValue.value);
  }
  
  const addRow = () => {
    console.log(mioty);
    console.log(nrLochy);
    //row.value.push(rowData.value[0]);
  }

</script>
<style>
  .addColumnButtonBackground {
    background-color: aquamarine;
    width: 110px;
    margin: 20px;
  }

  .addColumnButton:hover {
    border: 2px solid #3498db;
    background-color: green;
  }

  .addColumnButton {
    width: 100%;
    color: black;
    border: 2px solid transparent; 
  }

  .PdfTable {
    width: 100%;
    border-collapse: collapse;
    font-family: Arial, sans-serif;
    background-color: white;
    overflow: hidden;
  }

  .poleOceny {
    width: 105px;
  }

  #Pdf-Table th{
    padding: 0.25rem;
    text-align: center;
    border: 2px solid #000000;
    white-space: normal;
    word-wrap: break-word;
    word-break: break-word;
    background-color: #4a90e2;
    color: white;
  }

  .PdfTableData {
    border: 1.5px solid #000000;
    text-align: center;
    color: black;
  }

  .PdfTableBody tr:nth-child(even) {
    background-color: #f1f1f1;
  }



</style>