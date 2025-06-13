<template>
<div id="menu"><div>&#9776;</div></div>
  <div class="navigation">
    <div class="drawer">
      <v-list-item class="navItem" title="Menedżer stada"/>
      <v-divider style="color: #bbdd00; background-color: #bbdd00; opacity: 1; min-height: 2px;"/>
      <v-list-item class="navItem" :to="{ path: '/kartalochy' }" link title="Karta lochy"   />
      <v-list-item class="navItem" :to="{ path: '/kalendarz'  }" link title="Kalendarz"   />
      <v-list-item class="navItem" :to="{ path: '/wydarzenia' }" link title="Wydarzenia"    />
      <v-list-item class="navItem" :to="{ path: '/stado'      }" link title="Stado"         />
      <v-list-item class="navItem" :to="{ path: '/import'     }" link title="Importuj dane" />
    </div>
  </div>
</template>
<style>
#menu{
  padding: 7.8px 3px 16.8px 4px;
  position: absolute;
  font-size: 30px;
  top: 0px;
  z-index: 1000;
  left: 0px;
  display: flex;
  justify-content: center;
  align-items: center;
}
.navigation
{
  position: absolute;
  z-index: 1000;
  top: 0;
  left: 0;
  height: 10%;
  opacity: 1;
  width: 230px;
  transform: translate(-200px);
  transition: all 0.4s ease-in-out;
}
.navigation:hover
{
  transform:  translate(0);
  opacity: 1;
}
#menu:hover{
  .navigation{
    transform: translate(0);
  }
}

.drawer
{
  position:inherit;
  top:0;
  left:0;
  height:1000%;
  background-color: #111111;
  width: 200px;
}
.navItem
{
  max-width: 0px ;
  min-width: 0px ;
}

</style>