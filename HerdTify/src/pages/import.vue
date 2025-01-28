<template>
    <v-navigation-drawer :width="200">
    <v-list-item title="Menedżer stada"></v-list-item>
    <v-divider></v-divider>
      
      <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy"></v-list-item>
      <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia"></v-list-item>
      <v-list-item :to="{ path: '/stado' }" link title="Stado"></v-list-item>
      <v-list-item :to="{ path: '/import' }" link title="Importuj dane"></v-list-item>
    
  </v-navigation-drawer>

  <v-app-bar title="Importuj dane">
</v-app-bar>
  <v-container>
    <v-row justify="center">
      <v-col cols="12" md="8" lg="6">
        <v-card>
          <v-card-title>
            Wybierz plik do przesłania
          </v-card-title>
          <v-card-text>
            <v-file-input
              label="Wybierz plik"
              outlined
              dense
              v-model="selectedFileLochy"
              accept=".csv"
            />
          </v-card-text>
          <v-card-actions>
            <v-btn :disabled="!selectedFileLochy" color="primary" @click="ImportLochy">
              Importuj lochy
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
  <v-container>
    <v-row justify="center">
      <v-col cols="12" md="8" lg="6">
        <v-card>
          <v-card-title>
            Wybierz plik do przesłania
          </v-card-title>
          <v-card-text>
            <v-file-input
              label="Wybierz plik"
              outlined
              dense
              v-model="selectedFileWydarzenia"
              accept=".csv"
            />
          </v-card-text>
          <v-card-actions>
            <v-btn :disabled="!selectedFileWydarzenia" color="primary" @click="ImportWydarzenia">
              Importuj Wydarzenia
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container> 
  <v-container>
    <v-row justify="center">
      <v-col cols="12" md="8" lg="6">
        <v-card>
          <v-card-title>
            Wybierz plik do przesłania
          </v-card-title>
          <v-card-text>
            <v-file-input
              label="Wybierz plik"
              outlined
              dense
              v-model="selectedFileMioty"
              accept=".csv"
            />
          </v-card-text>
          <v-card-actions>
            <v-btn :disabled="!selectedFileMioty" color="primary" @click="ImportMioty">
              Importuj mioty
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>  
</template>

<script setup>
import axios from 'axios';

const selectedFileLochy = ref(null);
const selectedFileMioty = ref(null);
const selectedFileWydarzenia = ref(null);

const ImportLochy = async () => {
  if (!selectedFileLochy.value) return;

  const formData = new FormData();
  formData.append("file", selectedFileLochy.value);

  try {
    const response = await axios.post("http://localhost:5039/api/Locha/import", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    console.log("Sukces:", response.data);
    alert("Plik został przesłany pomyślnie!"); // Możesz użyć innej metody powiadomienia
  } catch (error) {
    console.error("Błąd przesyłania pliku:", error);
    alert("Nie udało się przesłać pliku."); // Możesz użyć innej metody powiadomienia
  }
};

const ImportMioty = async () => {
  if (!selectedFileLochy.value) return;

const formData = new FormData();
formData.append("file", selectedFileMioty.value);

try {
  const response = await axios.post("http://localhost:5039/api/Miot/import", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
  console.log("Sukces:", response.data);
  alert("Plik został przesłany pomyślnie!"); // Możesz użyć innej metody powiadomienia
} catch (error) {
  console.error("Błąd przesyłania pliku:", error);
  alert("Nie udało się przesłać pliku."); // Możesz użyć innej metody powiadomienia
}
};

const ImportWydarzenia = async () => {
  if (!selectedFileWydarzenia.value) return;

const formData = new FormData();
formData.append("file", selectedFileWydarzenia.value);

try {
  const response = await axios.post("http://localhost:5039/api/Wydarzenie/import", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
  console.log("Sukces:", response.data);
  alert("Plik został przesłany pomyślnie!"); // Możesz użyć innej metody powiadomienia
} catch (error) {
  console.error("Błąd przesyłania pliku:", error);
  alert("Nie udało się przesłać pliku."); // Możesz użyć innej metody powiadomienia
}
};

</script>