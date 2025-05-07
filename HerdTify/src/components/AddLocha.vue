<template>
    <v-dialog
      :model-value="addLochaDialog"
      @update:model-value="updateDialog"
      max-width="400"
      persistent
    >
      <v-card
        prepend-icon="mdi-plus"
        title="Nowa locha"
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
                v-model="newLocha.numerLochy"
                required
            ></v-text-field>
            <v-autocomplete
                :items="['Wolna', 'Pokryta', 'Karmiaca', 'Sprzedana', 'Zgon']"
                label="Status*"
                auto-select-first
                v-model:="newLocha.status"
                required
            >
            </v-autocomplete>
            <v-textarea label="Uwagi" v-model="newLocha.uwagi" style="width: 100%;"></v-textarea>
            
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

let newLocha = ref({
    numerLochy: "",
    status: "",
    wydarzeniaLochyId: [],
    miotyId: []
});

let errorMessage = ref(null);
let success = ref(null);

const props = defineProps({
addLochaDialog: {
  type: Boolean,
  required: true
}
});

const emit = defineEmits(['update:addLochaDialog', 'save-locha']);

const closeDialog = () => {
  emit('update:addLochaDialog', false);
};

const saveDialog = () => {
    apiClient.post('/Locha', newLocha.value)
    .then(() => {
      errorMessage.value = null;
      success = true;
      emit('save-locha', newLocha.value);
      clearNowaLocha();
    })
    .catch((e) => {
      success = false;
      let error = JSON.parse(e.response.request.response)
      console.error(error.e.errors[0].errorMessage);
      errorMessage.value = error.e.errors[0].errorMessage;
    });
 
};

const clearNowaLocha = () => {
    newLocha.value = {
        numerLochy: "",
        status: "",
        wydarzeniaLochyId: [],
        miotyId: []
    };
};

</script>

<style scoped>
.AddButton {
margin-bottom: 30px;
}
</style>