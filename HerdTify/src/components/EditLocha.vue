<template>
    <v-dialog
      :model-value="editLochaDialog"
      @update:model-value="editLochaDialog"
      max-width="400"
      persistent
    >
      <v-card
        prepend-icon="mdi-pencil"
        title="Edytuj lochę">
        <v-card-text>
          <v-alert
            v-if="errorMessage"
            type="error"
            variant="tonal"
            v-model="errorAlert"
            style="margin-bottom: 10px;">{{ errorMessage }}</v-alert>
          <v-alert
            v-if="success"
            type="success"
            variant="tonal"
            text="Zapisano pomyślnie!"
            style="margin-bottom: 10px;"/>
          <v-text-field
            label="Numer lochy"
            v-model="editedLocha.numerLochy"
            variant="outlined"
            :rules="[required, numberRules]"/>
          <v-autocomplete
            :items="['Luzna', 'Pokryta', 'Prosna', 'Karmiaca', 'Sprzedana', 'Zgon']"
            label="Status"
            auto-select-first
            v-model:="editedLocha.status"
            variant="outlined"
            :rules="[required]"
            style="margin-top: 10px; margin-bottom: 10px;"/>
          <v-textarea 
            label="Uwagi" 
            v-model="editedLocha.uwagi" 
            variant="outlined"
            style="width: 100%;"/>
          </v-card-text>
          <v-card-actions>
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
const props = defineProps({
editLochaDialog: {
  type: Boolean,
  required: false
},
locha: {
  type: Object,
  required: false
}
});

let errorMessage = ref(null);
let success = ref(null);
let editedLocha = ref(null);

const emit = defineEmits(['update:editLochaDialog', 'update-locha']);

const closeDialog = () => {
  success.value = null;
  errorMessage.value = null;
  emit('update:editLochaDialog', false);
};

watch(() => props.locha,(newLocha) => {
    editedLocha.value = { ...newLocha };
  },
  { deep: true, immediate: true }
);

const saveDialog = () => {
    editedLocha.value.wydarzeniaLochyId = null;
    editedLocha.value.miotyId = null;
    apiClient.put('/Locha/' + editedLocha.value.id, editedLocha.value)
    .then(() => {
      errorMessage.value = null;
      success.value = true;
      emit('update-locha', editedLocha.value);
    })
    .catch((e) => {
      success.value = false;
      console.error(e);
      let error = JSON.parse(e.response.request.response)
      console.error(error.e.errors[0].errorMessage);
      errorMessage.value = error.e.errors[0].errorMessage;
    });
 
};

const required = (v) => { return !!v || 'Pole jest wymagane' };
const numberRules = (v) => { return !isNaN(v) && Number(v) < 101 || 'Numer nie może przekraczać 100' };

</script>

<style scoped>
.AddButton {
margin-bottom: 30px;
}
</style>