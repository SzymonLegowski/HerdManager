<template>
<div id="menu"><div style="margin-left: 10px;">&#9776;</div></div>
  <div class="navigation">
    <div class="drawer">
      <v-list-item title="Menedżer stada"/>
      <v-divider style="color: #bbdd00; background-color: #bbdd00; opacity: 1; min-height: 2px;"/>
      <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy"   />
      <v-list-item :to="{ path: '/kalendarz'  }" link title="Kalendarz"   />
      <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia"    />
      <v-list-item :to="{ path: '/stado'      }" link title="Stado"         />
      <v-list-item :to="{ path: '/import'     }" link title="Importuj dane" />
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
  height: 7%;
  width: 250px;
  transform: translate(-200px);
  transition: all 0.4s ease-in-out;
}
.navigation:hover
{
  transform:  translate(0px);
}
#menu:hover{
  .navigation{
    transform: translate(0px);
  }
}
.drawer
{
  position:inherit;
  top:0;
  left:0;
  height:100vh;
  background-color: #111111;
  width: 200px;
}

</style>