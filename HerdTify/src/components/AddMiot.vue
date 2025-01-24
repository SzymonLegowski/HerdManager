<template>
    <v-dialog
      :model-value="addMiotDialog"
      @update:model-value="updateDialog"
      max-width="400"
      persistent
    >
      <v-card
        prepend-icon="mdi-plus"
        title="Nowy miot"
      >
        <v-card-text>
            <v-row>
                <v-col>
                    <v-text-field
                        label="Urodzone żywe"
                        v-model="newMiot.urodzoneZywe"
                    ></v-text-field>
                    <v-text-field
                        label="Urodzone martwe"
                        v-model="newMiot.urodzoneMartwe"
                    ></v-text-field>
                    <v-text-field
                        label="Przygniecone"
                        v-model="newMiot.przygniecone"
                    ></v-text-field>
                    <v-text-field
                        label="Odsadzone"
                        v-model="newMiot.odsadzone"
                    ></v-text-field>
                </v-col>
                <v-col>
                    <v-text-field
                        label="Ocena"
                        v-model="newMiot.ocena"
                    ></v-text-field>
                    <v-text-field
                        label="Przygniecone"
                        v-model="newMiot.przygniecone"
                    ></v-text-field>
                    <v-text-field
                        label="Data proszenia"
                        v-model="newMiot.dataProszenia"
                        hint="rrrr-mm-dd"
                    ></v-text-field>
                    <v-text-field
                        label="Data odsadzenia"
                        v-model="newMiot.dataOdsadzenia"
                        hint="rrrr-mm-dd"
                    ></v-text-field>
                </v-col>
            </v-row>
            
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


let newMiot = ref({
    urodzoneZywe: "",
    urodzoneMartwe: "",
    przygniecone: "",
    odsadzone: "",
    ocena: "",
    dataProszenia: "",
    dataOdsadzenia: "",
    ocena: "",
    lochaId: "",
});

const props = defineProps({
addMiotDialog: {
  type: Boolean,
  required: true
},
nrLochy: {
    required: false
},
krycieId: {
    required: false
}
});

const emit = defineEmits(['update:addMiotDialog', 'save-miot']);

const closeDialog = () => {
  emit('update:addMiotDialog', false);
};

const saveDialog = () => {
    apiClient.post('/Miot', newMiot.value)
    emit('save-miot', newMiot.value);
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
    };
};

</script>

<style scoped>
.AddButton {
margin-bottom: 30px;
}
</style>