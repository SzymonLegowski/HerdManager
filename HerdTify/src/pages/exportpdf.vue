<template>

<div class="ramka">
  <table id="Pdf-Table" class="PdfTable">
    <thead class="PdfTableHead">
      <tr>
        <th :colspan="colspanValue + 8">NR LOCHY</th>
        <th > {{ miotyStore.nrLochy }}  </th>
      </tr>
      <tr>
        <th :colspan="colspanValue + 2">Sektor krycia</th>
        <th colspan="7">Porodówka</th>
      </tr>
      <tr>
        <th rowspan="3">Nr miotu</th>
        <th :colspan="colspanValue">Data Pokrycia</th>
        <th colspan="3">Data</th>
        <th colspan="4">Liczba Prosiąt</th>
        <th rowspan="3">Ocena prosiąt urodzonych(1-5)</th>
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
        <!-- <td class="PdfTableData" v-for="(cell, cellIndex) in row" :key="cellIndex">{{ cell }}</td> -->
        <td class="PdfTableData" v-for="(cell, cellIndex) in row" :key="cellIndex">
          <template v-if="typeof cell === 'string' && cell.includes('\n')">
            <div v-for="line in cell.split('\n')" :key="line">{{ line }}</div>
          </template>
          <template v-else>
            {{ cell }}
          </template>
        </td>
      </tr>
    </tbody>
  </table>
</div>
</template>
<script setup>
  import { useMiotyStore } from '@/stores/miotyStore';
  import { ref } from 'vue';

  let colspanValue = ref(1);
  let tableData = ref([['1 (rasa kn.)']]);
  let maxKryc=0;
  const miotyStore = useMiotyStore();
  const mioty = miotyStore.mioty;
  let rowData = ref([[]]);
  let test = 0;
  
  for(let miot = 0; miot < mioty.length; miot++)
  {
    if(maxKryc < mioty[miot].datyKrycia.length)
      maxKryc = mioty[miot].datyKrycia.length;
  };
  console.log("maxKryc", maxKryc)
  for(let krycie = 1; krycie < maxKryc; krycie++)
  {
    colspanValue.value += 1;
    tableData.value.forEach(row => {row.push(colspanValue.value + ' (rasa kn.)')})
  };
  for(let miot = 0; miot < mioty.length; miot++)
  {
    // let spliceIndex = 1;
    // rowData.value[miot] = ([miot+1, 
    //                     mioty[miot].dataPrzewidywanegoProszenia,
    //                     mioty[miot].dataProszenia,
    //                     mioty[miot].dataOdsadzenia,
    //                     mioty[miot].urodzoneZywe,
    //                     mioty[miot].urodzoneMartwe,
    //                     mioty[miot].przygniecone,
    //                     mioty[miot].odsadzone,
    //                     mioty[miot].ocena,
    //                     ]);
    // for(let dataKrycia = 0; dataKrycia < maxKryc; dataKrycia++)
    // {
    //   test++;
    //   if(mioty[miot].datyKrycia[dataKrycia] !== undefined)
    //   {
    //     rowData.value[miot].splice(spliceIndex, 0, mioty[miot].datyKrycia[dataKrycia]);
    //   }
    //   else
    //   {
    //     rowData.value[miot].splice(spliceIndex,0,'');
    //   }
    //   spliceIndex++;
    //   console.log(rowData.value);
    // };         
    let datyKryciaCells = [];
    for (let dataKrycia = 0; dataKrycia < maxKryc; dataKrycia++) {
      const krycie = mioty[miot].datyKrycia?.[dataKrycia];
      if (krycie) {
        datyKryciaCells.push(`${krycie.data}\n${krycie.uwagi ?? ''}`);
      } else {
        datyKryciaCells.push('');
      }
    }
    rowData.value[miot] = [
      miot + 1,
      ...datyKryciaCells,
      mioty[miot].dataPrzewidywanegoProszenia ?? '',
      mioty[miot].dataProszenia ?? '',
      mioty[miot].dataOdsadzenia ?? '',
      mioty[miot].urodzoneZywe ?? '',
      mioty[miot].urodzoneMartwe ?? '',
      mioty[miot].przygniecone ?? '',
      mioty[miot].odsadzone ?? '',
      mioty[miot].ocena ?? ''
    ];
  };
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