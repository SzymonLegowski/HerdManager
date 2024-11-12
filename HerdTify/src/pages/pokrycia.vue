<template>
    <v-navigation-drawer :width="200" >
        <v-list-item title="Menedżer stada" ></v-list-item>
        <v-divider></v-divider>
        
        <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy">
        </v-list-item>
        
        <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia">
        </v-list-item>
        
        <v-list-item :to="{ path: '/pokrycia' }" link title="Pokrycia">
        </v-list-item>

    </v-navigation-drawer>
  <v-app-bar title="Pokrycia">
  </v-app-bar>
</template>