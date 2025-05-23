<template>

    <v-app-bar title="Menedżer stada">
    </v-app-bar>
        <v-sheet class="mx-auto" width="30%">
        <h1 class="Logowanie">Witaj, gdzie chcesz się udać?</h1>
        <v-form fast-fail @submit.prevent>
        <!-- <v-text-field
            v-model="firstName"
            :rules="firstNameRules"
            label="Login"
            class="textField"
        ></v-text-field>

        <v-text-field
            v-model="lastName"
            :rules="lastNameRules"
            label="Hasło"
            class="textField"
        ></v-text-field> -->

        <!-- <v-btn type="submit" block  style="min-width: 0; transform: translate(200px, -10px); background-color: green;">Zaloguj</v-btn> -->
        <v-btn type="submit" block :to="{ path: '/stado' }" style="min-width: 0; margin: 10px; transform: translate(0px, -10px); background-color: green;">Stado</v-btn>
        <v-btn type="submit" block :to="{ path: '/kartalochy' }" style="min-width: 0; margin: 10px; transform: translate(0px, -10px); background-color: green;">Karta lochy</v-btn>
        <v-btn type="submit" block :to="{ path: '/wydarzenia' }" style="min-width: 0; margin: 10px; transform: translate(0px, -10px); background-color: green;">Wydarzenia</v-btn>
        <!-- <v-btn type="submit" block :to="{ path: '/import' }" style="min-width: 0; margin: 10px; transform: translate(0px, -10px); background-color: green;">Import danych</v-btn> -->
    
        </v-form>
    </v-sheet>
</template>

<script setup>

</script>

<style lang="scss">

.Logowanie {
    padding-top: 5%;
    padding-bottom: 3%;
    text-align: center;
}
.mx-auto {
    margin-top: 5%;
}
.textField {

    margin-left:10%;
    margin-right:10%;
}


</style>
