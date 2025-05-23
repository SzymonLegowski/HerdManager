<template>
    <v-dialog
      :model-value="addMiotDialog"
      @update:model-value="addMiotDialog"
      max-width="400"
      persistent>
      <v-card
        prepend-icon="mdi-plus"
        title="Nowy miot">
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
            <v-row>
                <v-col>
                    <v-text-field
                        label="Urodzone żywe"
                        variant="outlined"
                        v-model="newMiot.urodzoneZywe"/>
                    <v-text-field
                        label="Urodzone martwe"
                        variant="outlined"
                        v-model="newMiot.urodzoneMartwe"/>
                    <v-text-field
                        label="Odsadzone"
                        variant="outlined"
                        v-model="newMiot.odsadzone"/>
                </v-col>
                <v-col>
                    <v-text-field
                        label="Przygniecone"
                        variant="outlined"
                        v-model="newMiot.przygniecone"/>
                    <v-text-field
                        label="Data proszenia"
                        variant="outlined"
                        v-model="newMiot.dataProszenia"
                        hint="rrrr-mm-dd"/>
                    <v-text-field
                        label="Data odsadzenia"
                        variant="outlined"
                        v-model="newMiot.dataOdsadzenia"
                        hint="rrrr-mm-dd"/>
                </v-col> 
            </v-row>
            <v-text-field label="Ocena" variant="outlined" v-model="newMiot.ocena"/>
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
addMiotDialog: {
  type: Boolean,
  required: true
},
idLochy: {
    type: Number,
    required: false
},
krycieId: {
    type: Number,
    required: false
}
});
let {idLochy, krycieId} = toRefs(props);
let newMiot = ref({
    urodzoneZywe: 0,
    urodzoneMartwe: 0,
    przygniecone: 0,
    odsadzone: 0,
    ocena: 0,
    dataProszenia: null,
    dataOdsadzenia: null,
    lochaId: 0,
    wydarzeniaMiotuId: []
});
let message = ref(null);
let alert = ref(false);
let success = ref(false);
const emit = defineEmits(['add:addMiotDialog', 'save-miot']);
const closeDialog = () => {
    success.value=false;
    alert.value=false;
    emit('add:addMiotDialog', false);
};

const saveDialog = () => {
    newMiot.value.lochaId = idLochy.value;
    newMiot.value.wydarzeniaMiotuId = [krycieId.value];
        console.info(newMiot.value);
    apiClient.post('/miot', newMiot.value).then((response) => {
        alert.value = false;
        success.value = true;
        emit('save-miot', response.data);
    }).catch((e) => {
        success.value = false;
        let error = JSON.parse(e.response.request.response)
        console.log(error);
        // console.log(error.e.errors[0].errorMessage);
        // message.value = error.e.errors[0].errorMessage;
        alert.value = true;
    });
};

const clearNewMiot = () => {
    newMiot.value = {
        urodzoneZywe: "",
        urodzoneMartwe: "",
        przygniecone: "",
        odsadzone: "",
        ocena: "",
        dataProszenia: "",
        dataOdsadzenia: "",
        ocena: "",
        lochaId: "",
        wydarzeniaMiotuId: "",
    };
};

</script>

<style scoped>
.AddButton {
margin-bottom: 30px;
}
</style>