## Manejo de errores

fError ()

| Disponibilidad | CONTPAQi Factura Electrónica® 14.1.0 CONTPAQi Comercial Premium® 12.1.0 |  |  |  |
|---|---|---|---|---|
| Sintaxis | fError(aNumError, aMensaje, aLen ) |  |  |  |
| Parámetros | Nombre | Tipo | Uso | Descripción |
| aNumError | Entero | Por valor | Número del error. |  |
| aMensaje | Cadena | Por referencia | Descripción del error. |  |
| aLen | Entero | Por valor | Longitud del mensaje de error. |  |

**Retorna**

**aMensaje**: Al finalizar la función este parámetro contiene el mensaje de error correspondiente al número de error especificado en aNumError.

**Descripción**

Esta función recupera el mensaje de error del SDK.

**Ejemplo**

El siguiente código asigna a la variable lError el resultado de la función fInicializaSDK(), en caso de que suceda algún error (valor distinto de 0), la función fError se ejecuta obteniendo el mensaje correspondiente al número de error enviado, mostrando una longitud de mensaje de 350 caracteres.

lError = fInicializaSDK()

If lError <> 0 Then

fError lError, lMensaje, 350

End

End If