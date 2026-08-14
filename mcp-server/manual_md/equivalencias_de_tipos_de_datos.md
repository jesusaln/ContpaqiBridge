# Equivalencias de tipos de datos

| Contenido y tamaño | Visual Basic | C++ | C# | Jscript | Visual FoxPro |
|---|---|---|---|---|---|
| Datos desconocidos | no disponible | VARIANT | Derive los tipos y después vincule al nodo Derived Types. | Object | Variant |
| Decimal | Decimal (estructura de .NET Framework) | DECIMAL | decimal | decimal | no disponible |
| Fecha | Date (estructura de .NET Framework) | DATE | DateTime | DateTime ObjetoDate | Date DateTime |
| Carácter SBCS (1 byte) | no disponible | signed char int8 | no disponible | sbyte | Character |
| Carácter Unicode (2 bytes) | Char (estructura de .NET Framework) | wchar_t | char | char | no disponible |
| Secuencia de caracteres Unicode | String (clase de .NET Framework) | wchar_t* | string | String | VarChar |
| Booleano (depende de la plataforma) | Boolean (estructura de .NET Framework) | VARIANT_BOOL | bool | boolean | Logical |
| 1 byte | SByte (Tipo de datos, Visual Basic)(estructura de .NET Framework) | signed char | sbyte | no disponible | no disponible |
| 2 bytes | Short (estructura de .NET Framework) | signed short int int16 | short | short | no disponible |
|  |  |  |  |  |  |
| 4 bytes | Integer (estructura de .NET Framework) | long, (long int, signed long int) | int | int | Integer |
| 8 bytes | Long (estructura de .NET Framework) | int64 | long | long | Float |
| 1 byte sin signo | Byte (estructura de .NET Framework) | BYTE bool | byte | byte | Integer |
| 2 bytes sin signo | UShort (Tipo de datos, Visual Basic)(estructura de .NET Framework) | unsigned short | ushort | no disponible | no disponible |
| 4 bytes sin signo | UInteger (Tipo de datos) (estructura de .NET Framework) | unsigned int yunsigned long | uint | no disponible | no disponible |
| 8 bytes sin signo | ULong (Tipo de datos, Visual Basic)(estructura de .NET Framework) | unsigned _int64 | ulong | no disponible | no disponible |
| Punto flotante de 4 bytes | Single (estructura de .NET Framework) | float | float | float | Float |
| Punto flotante de 8 bytes | Double (estructura de .NET Framework) | double | double | Double | Double |
| Secuencia de caracteres modificable (Buffer) | StringBuilder (clase de .NET Framework) | wchar_t* o char* (parámetro de salida/puntero) | StringBuilder | no disponible | no disponible |