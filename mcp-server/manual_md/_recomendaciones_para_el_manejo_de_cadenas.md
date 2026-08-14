# Recomendaciones para el manejo de cadenas

****

La forma en que cada lenguaje de programación define los tipos de datos cadena es varía entre lenguajes (en cuanto a su tamaño en bytes). Por esta razón los tipos de datos manejados por distintos lenguajes pueden presentar problemas al pasar información al SDK. En C++ Builder y Delphi éste inconveniente no se presenta.

Al usar el SDK en Visual Basic. Para llenar los campos cadena que forman parte de la estructura, es necesario llenar con espacios en blanco las variables tipo cadena hasta alcanzar la longitud requerida por el SDK, por la diferencia que existe con este lenguaje al manejar los tipos de datos.

El error que se produce cuando no se llenan adecuadamente las estructuras es “código no existe”. Para contrarrestar este error se usan dos funciones de manipulación de cadenas.

La función para llenar espacios en Visual Basic es la siguiente:

Para realizar comparaciones dentro de VB es necesario quitar el caracter nulo.