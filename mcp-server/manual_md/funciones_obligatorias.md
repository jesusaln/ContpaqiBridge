# Funciones obligatorias

****

Son las funciones que forzosamente deben incluirse en cualquier aplicación que use el SDK.

El método, a grandes ragos, se compone de:

- Inicializar el SDK al inicio de cada proceso: fInicializaSDK. Esta función se llama una sola vez al iniciar un proceso o acción completa.

Ejemplo: El alta de un documento y todos sus movimientos. Se inicia el SDK, se hace el llamado a todas las funciones requeridas y luego se termina el SDK.

- Funciones para abrir y cerrar empresa:

Se usan para indicar las bases de datos de la empresa a la cual afectará la aplicación que hace uso del SDK. (fAbreEmpresa / fCierraEmpresa)

Sólo se puede trabajar en una empresa a la vez (a menos que se corran la misma aplicación dos veces).

- Incluir la función fError del SDK para recuperar la descripción de los posibles errores. La mayoría de las funciones regresan un código de error, donde 0 indica que no se presentaron errores y un número diferente de 0 cuando ocurrió algún error.

Se utiliza la función **fError** para recuperar la descripción de dicho error.

- Usar siempre la función fTerminaSDK para liberar todos los recursos solicitados por el SDK, al final de cada proceso completo. Ésta función se llama una sola vez al finalizar un proceso o acción completa.

Estructura general de una aplicación desarrollada con el SDK.

Establecer el directorio del MGW_SDK

Inicializar SDK

Abrir Empresa

Tu función o proceso completo

Cerrar Empresa

Terminar SDK