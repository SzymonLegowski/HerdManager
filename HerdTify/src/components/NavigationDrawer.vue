<template>
  <div class="navigation">
    <div class="drawer">
      <v-list-item class="navItem" title="Menedżer stada"/>
      <v-divider style="color: #bbdd00; background-color: #bbdd00; opacity: 1; min-height: 2px;"/>
      <v-list-item class="navItem" :to="{ path: '/kartalochy' }" link title="Karta lochy"   />
      <v-list-item class="navItem" :to="{ path: '/wydarzenia' }" link title="Wydarzenia"    />
      <v-list-item class="navItem" :to="{ path: '/stado'      }" link title="Stado"         />
      <v-list-item class="navItem" :to="{ path: '/import'     }" link title="Importuj dane" />
    </div>
  </div>
</template>
<style>
.navigation
{
  position: absolute;
  z-index: 1000;
  top: 0;
  left: 0;
  height: 100%;
  width: 0px;
  padding-right: 10px;
  opacity: 0;
  overflow: hidden;
  transition: width 0.4s ease-in-out, opacity 0.4s;
}
.navigation:hover
{
  width: 200px;
  opacity: 1;
}
.drawer
{
  position:inherit;
  top:0;
  left:0;
  height:100%;
  background-color: #111111;
  width: inherit;
}
.navItem
{
  max-width: 0px ;
  min-width: 0px ;
}

</style>