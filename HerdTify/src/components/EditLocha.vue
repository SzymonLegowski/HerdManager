<template>
    <v-dialog
      :model-value="editLochaDialog"
      @update:model-value="editLochaDialog"
      max-width="400"
      persistent
    >
      <v-card
        prepend-icon="mdi-pencil"
        title="Edytuj lochę"
      >
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
                label="Numer lochy*"
                v-model="editedLocha.numerLochy"
                required
            ></v-text-field>
            <v-autocomplete
                :items="['Wolna', 'Pokryta', 'Karmiaca', 'Sprzedana', 'Zgon']"
                label="Status*"
                auto-select-first
                v-model:="editedLocha.status"
                required
            ></v-autocomplete>

            <v-textarea label="Uwagi" v-model="editedLocha.uwagi" style="width: 100%;"></v-textarea>
            
            <small class="text-caption text-medium-emphasis">*wymagane</small>
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
      success = true;
      emit('update-locha', editedLocha.value);
    })
    .catch((e) => {
      success = false;
      console.error(e);
      let error = JSON.parse(e.response.request.response)
      console.error(error.e.errors[0].errorMessage);
      errorMessage.value = error.e.errors[0].errorMessage;
    });
 
};

</script>

<style scoped>
.AddButton {
margin-bottom: 30px;
}
</style>