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
            <v-text-field
                label="Numer lochy*"
                v-model="newLocha.numerLochy"
                required
            ></v-text-field>
            <v-autocomplete
                :items="['Aktywna', 'Sprzedana', 'Zgon']"
                label="Status*"
                auto-select-first
                v-model:="newLocha.status"
                required
            >
            </v-autocomplete>
            <v-text-field
                hint="id wydarzenia"
                label="Wydarzenia lochy"
                v-model="newLocha.WydarzeniaLochyId"
                required
            ></v-text-field>
            
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


let newLocha = ref({
    numerLochy: "",
    status: "",
    wydarzeniaLochyId: [],
    miotyId: []
});

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
    // apiClient.post('/Locha', newLocha.value)
    // .then(() => {
    //   closeDialog();
    emit('save-locha', newLocha.value);
    clearNowaLocha();
    // })
    // .catch((e) => {
    //   console.error(e);
    // });
 
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