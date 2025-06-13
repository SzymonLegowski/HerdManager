<template class="hideScroll">
<NavigationDrawer/>
<v-app-bar title="Kalendarz" class="appBar"/>
  <div class="month-container-outer"> 
    <div class="change-month-button" @click="nextMonth"> &and; </div>
    <div class="month-container-inner"> {{ monthName }} </div> 
    <div class="change-month-button" @click="previousMonth"> &or; </div>
  </div>
  <div class="calendar">
    <div class="day-name" v-for="(name, index) in weekdays" :key="index">
      {{ name }}
    </div>
    <div v-for="n in firstWeekday" :key="'empty-' + n" class="day empty"></div>
    <div v-for="day in daysInMonth" :key="day.date" class="day">
        <div class="day-header">
            {{ day.date }}
        </div>
        <ul >
            <li id="krycia-list"
              v-for="(event, index) in Wydarzenia?.filter(e => e.dataWydarzenia.getDate() === day.date)"
              :key="index"
            >{{ event.typWydarzenia }} : {{ event.numeryLoch.join(", ") }}</li>
        <li id="przewProszenie-list"
          v-for="(data, index) in przewidywaneProszenia?.filter(e => 
            e.data.toDateString() === new Date(year, month, day.date).toDateString())"
          :key="index"
          >
          Przew. Proszenie: {{ data.nrLochy.join(", ") }}
        </li>
        <li id="proszenia-list"
          v-for="(data, index) in proszenia?.filter(e => 
            e.data.toDateString() === new Date(year, month, day.date).toDateString())"
          :key="index"
          >
          Proszenie: {{ data.nrLochy.join(", ") }}
        </li>
        <li id="odsadzenia-list"
          v-for="(data, index) in odsadzenia?.filter(e => 
            e.data.toDateString() === new Date(year, month, day.date).toDateString())"
          :key="index"
          >
          Odsadzanie: {{ data.nrLochy.join(", ") }}
        </li>
      </ul>
    </div>
  </div>
</template>
<script setup>
import { ref } from 'vue'
import NavigationDrawer from '@/components/NavigationDrawer.vue'
import apiClient from '@/plugins/axios';
import "@/styles/appBar.scss"
const weekdays = ['Poniedziałek', 'Wtorek', 'Środa', 'Czwartek', 'Piątek', 'Sobota', 'Niedziela']
const today = ref(new Date(2025, 8, 1));
const daysInMonth = ref([]);
const firstWeekday = ref(0);
const monthName = ref('');
const Wydarzenia = ref();
const Mioty = ref();
const przewidywaneProszenia = ref([]);
const proszenia = ref([]);
const odsadzenia = ref([]);
let year = today.value.getFullYear();
let month = today.value.getMonth();

init();

function getDaysInMonth(year, month) {
  const totalDays = new Date(year, month + 1, 0).getDate()
  const days = []
  for (let i = 1; i <= totalDays; i++) {
    days.push({ date: i })
  }
  return days
}

function previousMonth() {
    today.value.setMonth(today.value.getMonth() + 1);
    init();
}

function nextMonth() {
    today.value.setMonth(today.value.getMonth() - 1);
    init();
}

async function init() {
    przewidywaneProszenia.value = [];
    proszenia.value = [];
    odsadzenia.value = [];
    year = today.value.getFullYear();
    month = today.value.getMonth();
    daysInMonth.value = getDaysInMonth(year, month);
    const firstDay = new Date(year, month, 1);
    firstWeekday.value = firstDay.getDay() - 1;
    if (firstWeekday.value < 0) firstWeekday.value = 6;
    monthName.value = today.value.toLocaleString('pl-PL', { month: 'long' }).toLocaleUpperCase() + " " + year;
    const responseWydarzenia = await apiClient.get(`/Wydarzenie/month/${year}/${month+1}`);
    const responseMioty = await apiClient.get(`/Miot/${year}/${month+1}`);
    Wydarzenia.value = responseWydarzenia.data;
    Wydarzenia.value.forEach(element => {
        element.dataWydarzenia = new Date(element.dataWydarzenia);
        element.numeryLoch = [];
        element.lochyId.forEach(async id => {
            const response = await apiClient.get(`/Locha/${id}`);
            element.numeryLoch.push(response.data.numerLochy);
        })
    });
    Mioty.value = responseMioty.data;
    console.log(Mioty.value[0]);
    daysInMonth.value.forEach( day => {
      let currentDay = new Date( year, month, day.date);
      Mioty.value.forEach(async miot => {
        miot.numerLochy = ((await apiClient.get(`/Locha/${miot.lochaId}`)).data.numerLochy);
        if(miot.dataPrzewidywanegoProszenia !== null)
        {
          miot.dataPrzewidywanegoProszenia = new Date(miot.dataPrzewidywanegoProszenia);
          if(miot.dataPrzewidywanegoProszenia.toDateString() === currentDay.toDateString())
          {
            const existing = przewidywaneProszenia.value.find(entry =>
              new Date(entry.data).toDateString() === currentDay.toDateString()
            );
            if (existing) {
              existing.nrLochy.push(miot.numerLochy);
            } else {
              przewidywaneProszenia.value.push({
                data: miot.dataPrzewidywanegoProszenia,
                nrLochy: [miot.numerLochy]
              });
            }
          }
        }
        if(miot.dataProszenia !== null)
        {
          miot.dataProszenia = new Date(miot.dataProszenia);
          if(miot.dataProszenia.toDateString() === currentDay.toDateString())
          { 
            const existing = proszenia.value.find(entry =>
              new Date(entry.data).toDateString() === currentDay.toDateString()
            );
            if (existing) {
              existing.nrLochy.push(miot.numerLochy);
            } else {
              proszenia.value.push({
                data: miot.dataProszenia,
                nrLochy: [miot.numerLochy]
              });
            }
          }
        }
        if(miot.dataOdsadzenia !== null)
        {
          miot.dataOdsadzenia = new Date(miot.dataOdsadzenia);
          if(miot.dataOdsadzenia.toDateString() === currentDay.toDateString())
          {
            const existing = odsadzenia.value.find(entry =>
              new Date(entry.data).toDateString() === currentDay.toDateString()
            );
            if (existing) {
              existing.nrLochy.push(miot.numerLochy);
            } else {
              odsadzenia.value.push({
                data: miot.dataOdsadzenia,
                nrLochy: [miot.numerLochy]
              });
            }
          }
        }
      });
    });

}
</script>
<style scoped>
.calendar {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 4px;
  margin: 0px auto;
  text-align: center;
  font-family: sans-serif;
}

.change-month-button {
  background-color: #222222;
  font-size: 1cm;
  height: 100%;
  width: 50px;
  user-select: none;
}
.change-month-button:hover {
  cursor: pointer;
  background-color: #333333;
}

.day-header {
  font-weight: bold; 
  display: block; 
  background-color: #333333; 
  width: 100%; 
  text-align: left; 
  padding: 2px; 
  padding-left: 6px;
}

.day-name {
  font-weight: bold;
  background: #222222;
  padding: 8px;
  border: 1px solid #ffcc01;
}

.day {
  background: #222222;
  border: 1px solid #a2e7f0;
  font-weight: bold;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: flex-start;
  height: 120px;
}

.empty {
  background: transparent;
  border: none;
}

#krycia-list
{
  text-align: left; 
  list-style: none; 
  margin:0; 
  display: block; 
  background-color: #22226f; 
  padding: 2px;
}

#przewProszenie-list
{
  text-align: left; 
  list-style: none; 
  margin: 0; 
  display: block; 
  background-color: #d18202ab; 
  padding: 2px;
}

#odsadzenia-list
{
text-align: left; 
  list-style: none; 
  margin: 0; 
  display: block; 
  background-color: #02d128ab; 
  padding: 2px;
}

#proszenia-list
{
text-align: left; 
  list-style: none; 
  margin: 0; 
  display: block; 
  background-color: #d10202ab; 
  padding: 2px;
}

.month-container-outer {
  width: 100%; 
  text-align: center; 
  display: flex; 
  justify-content: center; 
  align-items: center;
}

.month-container-inner {
  width:  400px ; 
  font-weight: bold; 
  font-size:1cm; 
  background-color: #222222; 
  padding: 10px;
  margin: 5px;
}
</style>
