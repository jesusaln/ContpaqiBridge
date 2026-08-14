# Recomendaciones y consideraciones importantes

****

## Tips y conceptos básicos

****

- Siempre ten en cuenta que las funciones del SDK están en C++, el objetivo al declarar las funciones en tu lenguaje es pasar los tipos de datos que C++ pueda recibir. Busca el tipo de datos en tu lenguaje que coincida mejor con el tipo de C++.
- En C++ todas las cadenas son de tipo Char*, por lo que si en tu lenguaje de programación utilizas el tipo String estos siempre se deberán pasar Por Valor.
- Antes de hacer accesos mediante el SDK, asegurarse que CONTPAQi Comercial Premium® o CONTPAQi Factura Electrónica® funciona correctamente y que la información que está generando es correcta.
- Estar familiarizado con la estructura de la base de datos de CONTPAQi Comercial Premium® o CONTPAQi Factura Electrónica®.
- Tener claro y bien conceptualizado el fin y el alcance de la aplicación a desarrollar.
- Ir por “partes”, es decir: Primero crear la conexión a la base de datos, inicializar el SDK y generar un documento desde la aplicación; posteriormente verificar que funciona correctamente (que se crea sin problemas el documento en CONTPAQi Comercial Premium® o CONTPAQi Factura Electrónica®).
- Modularizar el código (Si el entorno de programación lo permite). Esto es crear diversos módulos para separar funcionalidad global y local. Ejemplo: Usar un módulo en el cual se realice la declaración de constantes, variables globales, estructuras de datos y enlace a las funciones del archivo MGW_SDK.DLL; y usar otro modulo para las funciones creadas por el desarrollador y que modificaran la información que se recibe y envía de la base de datos de CONTPAQi Comercial Premium® o CONTPAQi Factura Electrónica®. Esto facilitará la portabilidad y la reutilización de código, así como el mantenimiento y actualización de la funcionalidad.
- Revisar que los documentos y sus movimientos se graban/actualizan de manera correcta en CONTPAQi Comercial Premium® o CONTPAQi Factura Electrónica®.
- Validar desde la aplicación que se desarrolla que los datos que se envían sea consistente y que tenga el formato correcto.
- Probar continuamente la aplicación con todas las posibles combinaciones que permita.