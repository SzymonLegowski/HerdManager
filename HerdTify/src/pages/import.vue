<template>
  <NavigationDrawer/>
  <v-app-bar title="Import danych"/>
  <div class ="importContainer">
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
      text="Przesłano pomyślnie!"
      style="margin-bottom: 10px;"/>
    <h3 style="margin-bottom:20px">Podaj plik w formacie .csv</h3>
    <v-file-input label="Kliknij aby wybrać plik" variant="outlined" v-model="selectedFile"/>
    <v-btn color="primary" @click="ImportData">
      Importuj dane
    </v-btn>
  </div>
</template>

<script setup>
import NavigationDraver from '@/components/NavigationDrawer.vue';
import axios from 'axios';

const selectedFile = ref(null);
let errorMessage = ref(null);
let success = ref(null);

const ImportData = async () => {
  if (!selectedFile.value) 
  {
    errorMessage.value = "Nie podano pliku";
    return;
  }

  const formData = new FormData();
  formData.append("file", selectedFile.value);

  try {
    const response = await axios.post("http://localhost:5039/api/File/upload", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    console.log("Sukces:", response.data);
    success.value = true;
    errorMessage = null;
  } catch (error) {
    console.error("Błąd przesyłania pliku:", error);
    success.value = false;
    errorMessage.value = "Błąd przesyłania pliku";
  }
};

</script>
<style scoped>

.importContainer{
  width: 300px;
  margin: auto;
  margin-top: 20px;
  padding: 20px;
  background-color: #222222;
}

</style>