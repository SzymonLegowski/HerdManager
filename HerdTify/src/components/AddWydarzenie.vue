<template>
      <v-dialog
        :model-value="addWydarzenieDialog"
        @update:model-value="updateDialog"
        max-width="600"
        persistent
      >
        <LochyGrid v-if="showLochyGrid" :items="numeryLoch" @update:selectedLocha="updateSelectedLocha" class="lochyGridDialog"/>
        <v-card
          prepend-icon="mdi-plus"
          title="Nowe Wydarzenie"
          :class="{ 'shift-left': showLochyGrid || showMiotyGrid }"
        >
          <v-card-text>
            <v-row dense>
              <v-col
                cols="12"
                md="4"
                sm="6"
              >
              <v-autocomplete
                  :items="['Krycie', 'Szczepienie']"
                  label="Typ"
                  auto-select-first
                ></v-autocomplete>
              </v-col>
  
              <v-col
                cols="12"
                md="4"
                sm="6"
              >
                <v-text-field
                  hint="rrrr-mm-dd"
                  label="Data Wydarzenia*"
                  required
                ></v-text-field>
              </v-col>
  
              <v-col
                cols="12"
                md="4"
                sm="6"
              >
                <v-text-field
                  hint="rrrr-mm-dd"
                  label="Data Wykonania"
                  required
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="4"
                sm="6">
                <v-btn class="AddButton" variant="outlined" text="Dodaj lochy" @click="selectLochy">
                </v-btn>
                <v-btn class="AddButton" variant="outlined" text="Dodaj mioty" @click="selectMioty">
                </v-btn>
              </v-col>
              
              <v-col>
                <h5 @click="wybraneLochyEmpty" style="margin-top: 5%;">Wybrane lochy:{{ wybraneLochy }}</h5>
                <h5 style="margin-top: 10%;">Wybrane mioty:{{ wybraneLochy }}</h5>
              </v-col>
              
            
            </v-row>
  
            <small class="text-caption text-medium-emphasis">*wymagane</small>
            <v-spacer></v-spacer>
            <small class="text-caption text-medium-emphasis">wydarzenie musi zawierać chociaż 1 miot/lochę</small>
          </v-card-text>
  
          <v-divider></v-divider>
  
          <v-card-actions>
            <v-spacer></v-spacer>
  
            <v-btn
              text="Zamknij"
              variant="plain"
              @click="closeDialog()"
            ></v-btn>
  
            <v-btn
              color="primary"
              text="Zapisz"
              variant="tonal"
              @click="saveDialog()"
            ></v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
  </template>

<script setup>
import { onMounted, ref, watch } from 'vue';
import apiClient from "@/plugins/axios";
import LochyGrid from './LochyGrid.vue';

const props = defineProps({
  addWydarzenieDialog: {
    type: Boolean,
    required: true
  }
});

const numeryLoch = ref([]);
const showLochyGrid = ref(false); 
const showMiotyGrid = ref(false);
let wybraneLochy = ref([]);

const emit = defineEmits(['update:addWydarzenieDialog']);

const closeDialog = () => {
  wybraneLochy = [];
  emit('update:addWydarzenieDialog', false);
};

const saveDialog = () => {
  wybraneLochy = [];
  emit('update:addWydarzenieDialog', false);
};

const selectLochy = () => {
  showLochyGrid.value = !showLochyGrid.value;
};

const selectMioty = () => {
  showMiotyGrid.value = !showMiotyGrid.value;
};

const wybraneLochyEmpty = () => {
  wybraneLochy.value = [];
};

onMounted(async () => {
  try {
    const lochy = await apiClient.get(`/Locha/status/0`);
    numeryLoch.value = lochy.data.map(lochy => lochy.numerLochy);
    console.log("numeryLoch", numeryLoch); //Debugowanie
  } catch (e) {
    console.error(e);
  }
});

const updateSelectedLocha = (number) => {
  if(!wybraneLochy.value.includes(number)){
    wybraneLochy.value.push(number);
  }
  console.log("wybrane lochy:", wybraneLochy); //Debugowanie  
};

</script>

<style scoped>
.AddButton {
  margin: 10px;
}
.shift-left {
  transform: translateX(-220px);
  transition: transform 0.3s ease-in-out;
}

.lochyGridDialog {
  position: absolute;
  left: 70%;
}
h5 {
  cursor: pointer; /* Wskazuje, że element jest klikalny */
}
</style>