<template>
    <v-dialog
      :model-value="editMiotDialog"
      @update:model-value="editMiotDialog"
      max-width="400"
      persistent>
      <v-card
        prepend-icon="mdi-pencil"
        title="Edytuj miot">
        <v-card-text>
            <v-row dense>
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
            </v-row>
            <v-row>
                <v-col>
                    <v-text-field
                        label="Urodzone żywe"
                        v-model="editedMiot.urodzoneZywe"
                        variant="outlined"/>
                    <v-text-field
                        label="Przygniecone"
                        v-model="editedMiot.przygniecone"
                        variant="outlined"/>
                    <v-text-field
                        label="Data proszenia"
                        v-model="editedMiot.dataProszenia"
                        hint="rrrr-mm-dd"
                        variant="outlined"/>
                </v-col>
                <v-col>
                    <v-text-field
                        label="Urodzone martwe"
                        v-model="editedMiot.urodzoneMartwe"
                        variant="outlined"/>
                    <v-text-field
                        label="Odsadzone"
                        v-model="editedMiot.odsadzone"
                        variant="outlined"/>
                    <v-text-field
                        label="Data odsadzenia"
                        v-model="editedMiot.dataOdsadzenia"
                        hint="rrrr-mm-dd"
                        variant="outlined"/>
                </v-col>
            </v-row>
            <v-text-field
                label="Ocena"
                v-model="editedMiot.ocena"
                variant="outlined"/>
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

let success = ref(""); 
let errorMessage = ref("");
const editedMiot = ref(null);
const editedMiotSent = ref({
    id: "",
    urodzoneZywe: "",
    urodzoneMartwe: "",
    przygniecone: "",
    odsadzone: "",
    ocena: "",
    dataProszenia: "",
    dataOdsadzenia: "",
    lochaId: "",
    wydarzeniaMiotuId: ""
});

const props = defineProps({
editMiotDialog: {
  type: Boolean,
  required: true
},
miot: {
    type: Object,
    required: false
}
});

const emit = defineEmits(['update:editMiotDialog', 'save-miot']);

const closeDialog = () => {
    success.value = "";
    errorMessage.value = "";
    emit('update:editMiotDialog', false);
};

const saveDialog = () => {
    editedToSent();
    apiClient.put(`/Miot/${editedMiotSent.value.id}`, editedMiotSent.value)
    .then((response) => {
        success.value = response.data;
        errorMessage.value = "";
        emit('save-miot', editedMiot.value);
    })
    .catch((e) => {
      console.error("Błąd podczas pobierania danych:", e);
      success.value = "";
      errorMessage.value = "Wystąpił błąd podczas zapisywania zmian w miocie";
    });
};

watch(() => props.miot,(selectedMiot) => {
    console.log("Przed kopią:", selectedMiot);  // 🔍 Debugowanie
    editedMiot.value = JSON.parse(JSON.stringify(selectedMiot));
    console.log("Po kopii:", editedMiot.value); // 🔍 Sprawdzenie poprawności
},
{ deep: true, immediate: true }
);

const editedToSent = () => {
    editedMiotSent.value.id = editedMiot.value.id;
    editedMiotSent.value.urodzoneZywe = editedMiot.value.urodzoneZywe;
    editedMiotSent.value.urodzoneMartwe = editedMiot.value.urodzoneMartwe;
    editedMiotSent.value.przygniecone = editedMiot.value.przygniecone;
    editedMiotSent.value.odsadzone = editedMiot.value.odsadzone;
    editedMiotSent.value.ocena = editedMiot.value.ocena;
    editedMiotSent.value.dataProszenia = editedMiot.value.dataProszenia;
    editedMiotSent.value.dataOdsadzenia = editedMiot.value.dataOdsadzenia;
    editedMiotSent.value.dataPrzewidywanegoProszenia = editedMiot.value.dataPrzewidywanegoProszenia;
    editedMiotSent.value.lochaId = editedMiot.value.lochaId;
    editedMiotSent.value.wydarzeniaMiotuId = [];
};

</script>

<style scoped>
.AddButton {
margin-bottom: 30px;
}
</style>