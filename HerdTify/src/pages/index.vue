<template>
  <div class="page-container">

    <header class="app-bar">
      <h2 class="app-bar-title">Menedżer Stada - Strona Główna</h2>
    </header>


    <main class="main-content">
      <div class="welcome-section">
        <h1 class="main-title">Witaj w Menedżerze Stada!</h1>
        <p class="subtitle">
          Wybierz moduł, którym chcesz zarządzać:
        </p>
      </div>

    
      <div class="card-grid">
    
        <div class="card" @click="navigateTo('/kartalochy')" title="Przejdź do zarządzania kartami loch">
          <div class="card-image-container">
            <img :src="lochaImageUrl" alt="Obraz lochy" class="card-image"/>
            <div class="card-title-overlay">
              <h3 class="card-title">Karta Lochy</h3>
            </div>
          </div>
          <div class="card-content dark-bg">
            <p>
              Zarządzaj szczegółowymi kartami loch, przeglądaj historię miotów i dane produkcyjne.
            </p>
          </div>
          <div class="card-actions dark-bg">
            <span class="card-button">
              Przejdź <span class="arrow">→</span>
            </span>
          </div>
        </div>

        <div class="card" @click="navigateTo('/wydarzenia')" title="Przejdź do rejestru wydarzeń">
          <div class="card-image-container">
            <img :src="wydarzeniaImageUrl" alt="Obraz przedstawiający kalendarz lub wydarzenia" class="card-image"/>
            <div class="card-title-overlay">
              <h3 class="card-title">Wydarzenia</h3>
            </div>
          </div>
          <div class="card-content dark-bg">
            <p>
              Rejestruj i przeglądaj kluczowe wydarzenia w stadzie, takie jak szczepienia, leczenie czy zmiany grup.
            </p>
          </div>
          <div class="card-actions dark-bg">
            <span class="card-button">
              Przejdź <span class="arrow">→</span>
            </span>
          </div>
        </div>

      
        <div class="card" @click="navigateTo('/pokrycia')" title="Przejdź do zarządzania pokryciami">
          <div class="card-image-container">
            <img :src="pokryciaImageUrl" alt="Obraz knura lub procesu krycia" class="card-image"/>
            <div class="card-title-overlay">
              <h3 class="card-title">Pokrycia</h3>
            </div>
          </div>
          <div class="card-content dark-bg">
            <p>
              Planuj, rejestruj i monitoruj pokrycia loch, zarządzaj danymi knurów i terminami porodów.
            </p>
          </div>
          <div class="card-actions dark-bg">
            <span class="card-button">
              Przejdź <span class="arrow">→</span>
            </span>
          </div>
        </div>
        
      </div>
    </main>
  </div>
</template>

<script>
import lochaImage from '@/assets/locha.jpg';
import pokryciaImage from '@/assets/pokrycia.jpg';
import wydarzeniaImage from '@/assets/wydarzenia.jpg'; 

export default {
  name: 'IndexPageNative',
  data: () => ({
    lochaImageUrl: lochaImage,
    pokryciaImageUrl: pokryciaImage,
    wydarzeniaImageUrl: wydarzeniaImage
  }),
  methods: {
    navigateTo(path) {
     
      if (this.$router) {
        this.$router.push({ path: path });
      } else {
        console.warn('Vue Router is not available.');
       
      }
    }
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,100..900;1,100..900&display=swap');

.page-container {
  font-family: "Roboto", sans-serif; 
  background-color: #212121; 
  color: #f0f0f0; 
  min-height: 100vh;
}


.app-bar {
  background-color: #333333;
  color: white;
  padding: 12px 24px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.3);
}

.app-bar-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 500;
}


.main-content {
  padding: 24px;
  margin-top: 5rem; 
}

.welcome-section {
  text-align: center;
  margin-bottom: 48px; 
}

.main-title {
  font-size: 2.25rem; 
  font-weight: 400;
  margin-bottom: 16px;
  color: #ffffff; 
}

.subtitle {
  font-size: 1.25rem;
  color: #bdbdbd; 
  margin-bottom: 32px;
}


.card-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 24px; 
  justify-content: center;
}


.card {
  border-radius: 6px; 
  box-shadow: 0 3px 6px rgba(0,0,0,0.16), 0 3px 6px rgba(0,0,0,0.23); 
  overflow: hidden;
  display: flex;
  flex-direction: column;
  transition: transform 0.25s ease-in-out, box-shadow 0.25s ease-in-out;
  width: 100%;
  background-color: #333333; 
  cursor: pointer; 
}

.card:hover {
  transform: translateY(-6px); 
  box-shadow: 0 10px 20px rgba(0,0,0,0.2), 0 6px 6px rgba(0,0,0,0.23);
}


.card-image-container {
  position: relative;
  height: 160px;
  background-color: #555; 
}

.card-image {
  display: block;
  width: 100%;
  height: 100%;
  object-fit: cover; 
}

.card-title-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 14px 18px; 
  background: linear-gradient(to top, rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0)); 
}

.card-title {
  color: white;
  margin: 0;
  font-size: 1.3rem;
  font-weight: 500;
  text-shadow: 1px 1px 3px rgba(0,0,0,0.8); 
}


.card-content {
  padding: 18px;
  flex-grow: 1; 
  line-height: 1.6; 
  font-size: 0.98rem;
}

.card-content.dark-bg {
  background-color: #333333; 
  color: #e0e0e0;
}

.card-actions {
  background-color: #333333; 
  padding: 14px 18px; 
  text-align: right;
  border-top: none;
}


.card-button {
  display: inline-block;
 
  color: white !important; 
  padding: 9px 18px; 

  text-decoration: none;
  font-size: 0.9rem;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.06em; 
  border: none;

  transition: background-color 0.2s ease, box-shadow 0.2s ease;
  /* box-shadow: 0 2px 3px rgba(0,0,0,0.15), 0 1px 3px rgba(0,0,0,0.1); */
}

/* .card:hover .card-button { 
  background-color: #1565C0; 
  box-shadow: 0 3px 5px rgba(0,0,0,0.2), 0 2px 4px rgba(0,0,0,0.15);
} */

.card-button .arrow {
    margin-left: 8px;
    display: inline-block;
    font-weight: bold; 
}

/* --- media dla responsywność --- */
@media (min-width: 600px) {
  .card {
    width: calc(50% - 12px); 
  }
}

@media (min-width: 960px) {
  .card {
    width: calc(33.333% - 16px); 
  }
}


.card:focus-visible {
  outline: 2px solid #1976D2;
  outline-offset: 2px;
  box-shadow: 0 10px 20px rgba(0,0,0,0.2), 0 6px 6px rgba(0,0,0,0.23); 
}
</style>