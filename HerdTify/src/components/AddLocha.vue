<template>
    <v-dialog
      :model-value="addLochaDialog"
      @update:model-value="addLochaDialog"
      max-width="400"
      persistent>
      <v-card
        prepend-icon="mdi-plus"
        title="Nowa locha">
        <v-card-text>
            <v-alert
              v-if="alert"
              type="error"
              variant="tonal"
              style="margin-bottom: 10px;">{{ message }}</v-alert>
            <v-alert
              v-if="success"
              type="success"
              variant="tonal"
              text="Zapisano pomyślnie!"
              style="margin-bottom: 10px;"/>
            <v-text-field
              label="Numer lochy"
              v-model="newLocha.numerLochy"
              variant="outlined"
              :rules="[required, numberRules]"/>
            <v-autocomplete
              :items="['Luzna', 'Pokryta', 'Prosna', 'Karmiaca', 'Sprzedana', 'Zgon']"
              label="Status"
              auto-select-first
              v-model:="newLocha.status"
              variant="outlined"
              :rules="[required]"
              style="margin-top: 10px; margin-bottom: 10px;"/>
            <v-textarea 
              label="Uwagi" 
              v-model="newLocha.uwagi" 
              variant="outlined"
              style="width: 100%;"/>
            <v-text-field
              hint="rrrr-mm-dd"
              label="Data Brakowania"
              v-model="newLocha.dataBrakowania"
              variant="outlined"/>
        </v-card-text>
        <v-divider/>
        <v-card-actions>
          <v-spacer/>
          <v-btn
            text="Zamknij"
            variant="plain"
            @click="closeDialog()"/>
          <v-btn
            color="primary"
            text="Zapisz"
            variant="tonal"
            @click="saveDialog()"/>
        </v-card-actions>
      </v-card>
    </v-dialog>
</template>

<script setup>
import apiClient from '@/plugins/axios';

let newLocha = ref({
    numerLochy: "",
    status: "",
    dataBrakowania: null,
    wydarzeniaLochyId: [],
    miotyId: []
});

let message = ref(null);
let alert = ref(false);
let success = ref(false);

const props = defineProps({
addLochaDialog: {
  type: Boolean,
  required: true
}
});

const emit = defineEmits(['update:addLochaDialog', 'save-locha']);

const closeDialog = () => {
  success.value = false;
  emit('update:addLochaDialog', false);
};

const saveDialog = async () => {
await apiClient.post('/Locha', newLocha.value)
    .then((response) => {
      alert.value = false;
      success.value = true;
      console.log(response);
      newLocha.value.id = response.data;
      emit('save-locha', newLocha.value);
      clearNowaLocha();
    })
    .catch((e) => {
      success.value = false;
      console.log(e);
      message.value = "Błąd podczas dodawania lochy";
      alert.value = true;
    });
 
};

function clearNowaLocha () {
    newLocha.value = {
        numerLochy: "",
        status: "",
        dataBrakowania: null,
        wydarzeniaLochyId: [],
        miotyId: []
    };
};

const required = (v) => { return !!v || 'Pole jest wymagane' };
const numberRules = (v) => { return !isNaN(v) && Number(v) < 101 || 'Numer nie może przekraczać 100' };

</script>

<style scoped>
.AddButton {
margin-bottom: 30px;
}
</style>