# Trabajando con documentos

****

Cuando se trabaje con documentos siempre se deben afectar.

Al crear documentos, la existencia y los costos se afectan, sin embargo los acumulados del sistema no, por lo que es necesario afectarlos después de crear documentos con sus movimientos correspondientes.

En el SDK existen dos tipos de afectación, una para los documentos de cargo y abono y otra para los demás tipos de documento.

Estructura general de una aplicación que da de alta documentos y sus movimientos con el SDK.

Establecer el directorio del MGW_SDK

Inicializar SDK

Abrir Empresa

Alta de documento

Alta de movimientos

Afectar documento

Cerrar Empresa

Terminar SDK

Estructura general de una aplicación que da de alta documentos de Cargo y Abono con el SDK.

Establecer el directorio del MGW_SDK

Inicializar SDK

Abrir Empresa

Alta de documento Cargo/Abono

Afectar documento

Cerrar Empresa

Terminar SDK

|  | Nota: Las funciones de afectación de documentos son: fAfectaDocto_Param () y fAfectaDocto (), bajo y alto nivel respectivamente. |
|---|---|

Estructura general de un documento que maneja series y/o pedimentos

Establecer el directorio del MGW_SDK

Inicializar SDK

Abrir Empresa

Alta de documento

Alta de movimientos

Alta del movimiento con series o pedimentos

Calcula los movimentos con series o pedimentos

Afectar documento

Cerrar Empresa

Terminar SDK