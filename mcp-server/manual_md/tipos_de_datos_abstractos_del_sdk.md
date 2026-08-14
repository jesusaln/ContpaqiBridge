# Tipos de datos abstractos del SDK

## Definición de las estructuras de datos

Documentos – RegDocumento – tDocumento

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| aFolio | Doble | NA | Folio del documento. |
| aNumMoneda | Entero | NA | Moneda del documento. 1 = Pesos MN, 2 = Moneda extranjera. |
| aTipoCambio | Doble | NA | Tipo de cambio del documento. |
| aImporte | Doble | NA | Importe del documento. Sólo se usa en documentos de cargo/abono. |
| aDescuentoDoc1 | Doble | NA | No tiene uso, valor por omisión = 0 (cero). |
| aDescuentoDoc2 | Doble | NA | No tiene uso, valor por omisión = 0 (cero). |
| aSistemaOrigen | Entero | NA | Valor mayor a 5 que indica una aplicación diferente a los PAQ's. |
| aCodConcepto | Cadena | kLongCodigo + 1 | Código del concepto del documento. |
| aSerie | Cadena | kLongSerie + 1 | Serie del documento. |
| aFecha | Cadena | kLongFecha + 1 | Fecha del documento. Formato mm/dd/aaaa Las “/” diagonales son parte del formato. |
| aCodigoCteProv | Cadena | kLongCodigo + 1 | Código del Cliente/Proveedor. |
| aCodigoAgente | Cadena | kLongCodigo + 1 | Código del Agente. |
| aReferencia | Cadena | kLongReferencia + 1 | Referencia del Documento. |
| aAfecta | Entero | NA | No tiene uso, valor por omisión = 0 (cero). |
| aGasto1 | Double | NA | Valor por omisión = 0 (cero). |
| aGasto2 | Double | NA | Valor por omisión = 0 (cero). |
| aGasto3 | Double | NA | Valor por omisión = 0 (cero). |

Llave del Documento – RegLlaveDoc – tLlaveDoc*

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| aConsepto | Cadena | kLongCodigo + 1 | Código del concepto del documento. |
| aSerie | Cadena | kLongSerie + 1 | Serie del documento. |
| aFolio | Doble | NA | Folio del documento. |

Movimientos – RegMovimiento – tMovimiento

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| aConsecutivo | Entero | NA | Consecutivo del movimiento. |
| aUnidades | Doble | NA | Unidades del movimiento. |
| aPrecio | Doble | NA | Precio del movimiento (para doctos. de venta ). |
| aCosto | Doble | NA | Costo del movimiento (para doctos. de compra). |
| aCodProdSer | Cadena | kLongCodigo + 1 | Códogo del producto o servicio. |
| aCodAlmacen | Cadena | kLongCodigo + 1 | Código del Almacén. |
| aReferencia | Cadena | kLongReferencia + 1 | Referencia del movimiento. |
| aCodClasificacion | Cadena | kLongCodigo + 1 | Código de la clasificacuión |

Movimientos – RegMovimiento – tMovimientoDesc

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| aConsecutivo | Entero | NA | Consecutivo del movimiento. |
| aUnidades | Doble | NA | Unidades del movimiento. |
| aPrecio | Doble | NA | Precio del movimiento (para doctos. de venta ). |
| aCosto | Doble | NA | Costo del movimiento (para doctos. de compra). |
| aPorcDescto1 | Doble | NA | Porcentaje del Descuento 1 |
| aImporteDescto1 | Doble | NA | Importe del Descuento 1 |
| aPorcDescto2 | Doble | NA | Porcentaje del Descuento 2 |
| aImporteDescto2 | Doble | NA | Importe del Descuento 2 |
| aPorcDescto3 | Doble | NA | Porcentaje del Descuento 3 |
| aImporteDescto3 | Doble | NA | Importe del Descuento 3 |
| aPorcDescto4 | Doble | NA | Porcentaje del Descuento 4 |
| aImporteDescto4 | Doble | NA | Importe del Descuento 4 |
| aPorcDescto5 | Doble | NA | Porcentaje del Descuento 5 |
| aImporteDescto5 | Doble | NA | Importe del Descuento 5 |
| aCodProdSer | Cadena | kLongCodigo + 1 | Códogo del producto o servicio. |
| aCodAlmacen | Cadena | kLongCodigo + 1 | Código del Almacén. |
| aReferencia | Cadena | kLongReferencia + 1 | Referencia del movimiento. |
| aCodClasificacion | Cadena | kLongCodigo + 1 | Código de la clasificacuión |

Movimientos con Serie/Capas – SeriesCapas – tSeriesCapas

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| aUnidades | Doble | NA | Unidades del movimiento. |
| aTipoCambio | Doble | NA | Tipo de cambio del movimiento. |
| aSeries | Cadena | kLongCodigo + 1 | Series del movimiento. |
| aPedimento | Cadena | kLongDescripcion + 1 | Pedimento del movimiento. |
| aAgencia | Cadena | kLongDescripcion + 1 | Agencia aduanal del movimiento. |
| aFechaPedimento | Cadena | kLongFecha + 1 | Fecha de pedimento del movimiento. |
| aNumeroLote | Cadena | kLongDescripcion + 1 | Número de lote del movimiento. |
| aFechaFabricacion | Cadena | kLongFecha + 1 | Fecha de fabricación del movimiento. |
| aFechaCaducidad | Cadena | kLongFecha + 1 | Fecha de Caducidad del movimiento. |

Movimientos con Caracteristicas – Caracteristicas – tCaracteristicas

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| aUnidades | Doble | NA | Unidades del movimiento. |
| aValorCaracteristica1 | Cadena | kLongDescripcion + 1 | Valor de la xaracteristica 1 del movimiento. |
| aValorCaracteristica2 | Cadena | kLongDescripcion + 1 | Valor de la xaracteristica 2 del movimiento. |
| aValorCaracteristica3 | Cadena | kLongDescripcion + 1 | Valor de la xaracteristica 3 del movimiento. |

Movimientos con datos adicionales – RegTipoProducto – tTipoProducto

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| aSeriesCapas | aSeriesCapas | NA | Tipo de dato abstracto: tSeriesCapas. |
| aCaracteristicas | aCaracteristicas | NA | Tipo de dato abstracto: Caracteristicas. |

Llave de aperturas – RegLlaveAper - tLlaveAper

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| aCodCaja | Cadena | kLongCodigo + 1 | Código de la caja. |
| aFechaApe | Cadena | kLongFecha + 1 | Fecha de apertura. |

Productos – RegProducto – tProducto

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| cCodigoProducto | Cadena | kLongCodigo + 1 | Código del producto. |
| cNombreProducto | Cadena | kLongNombre + 1 | Nombre del producto. |
| cDescripcionProducto | Cadena | kLongNombreProducto + 1 | Descripción del producto. |
| cTipoProducto | Entero | NA | 1- Producto, 2 - Paquete, 3 - Servicio |
| cFechaAltaProducto | Cadena | kLongFecha + 1 | Fecha de alta del producto. |
| cFechaBaja | Cadena | kLongFecha + 1 | Fecha de baja del producto. |
| cStatusProducto | Entero | NA | 0 - Baja Lógica, 1 – Alta |
| cControlExistencia | Entero | NA | Control de exixtencia. |
| cMetodoCosteo | Entero | NA | 1 - Costo Promedio Base a Entradas, 2 - Costo Promedio Base a Entradas Almacen 3 - Último costo, 4 - UEPS, 5 - PEPS, 6 - Costo específico, 7 - Costo Estandar. |
| cCodigoUnidadBase | Cadena | kLongCodigo + 1 | Código de la unidad base. |
| cCodigoUnidadNoConvertible | Cadena | kLongCodigo + 1 | Código de la unidad no convertible. |
| cPrecio1 | Doble | NA | Lista de precios 1. |
| cPrecio2 | Doble | NA | Lista de precios 2. |
| cPrecio3 | Doble | NA | Lista de precios 3. |
| cPrecio4 | Doble | NA | Lista de precios 4. |
| cPrecio5 | Doble | NA | Lista de precios 5. |
| cPrecio6 | Doble | NA | Lista de precios 6. |
| cPrecio7 | Doble | NA | Lista de precios 7. |
| cPrecio8 | Doble | NA | Lista de precios 8. |
| cPrecio9 | Doble | NA | Lista de precios 9. |
| cPrecio10 | Doble | NA | Lista de precios 10. |
| cImpuesto1 | Doble | NA | Impuesto 1. |
| cImpuesto2 | Doble | NA | Impuesto 2. |
| cImpuesto3 | Doble | NA | Impuesto 3. |
| cRetencion1 | Doble | NA | Retención 1. |
| cRetencion2 | Doble | NA | Retención 2. |
| cNombreCaracteristica1 | Cadena | kLongAbreviatura + 1 | Nombre de la caracteristica 1. |
| cNombreCaracteristica2 | Cadena | kLongAbreviatura + 1 | Nombre de la caracteristica 2. |
| cNombreCaracteristica3 | Cadena | kLongAbreviatura + 1 | Nombre de la caracteristica 3. |
| cCodigoValorClasificacion1 | Cadena | kLongCodValorClasif + 1 | Código del valor de la clasificación 1. |
| cCodigoValorClasificacion2 | Cadena | kLongCodValorClasif + 1 | Código del valor de la clasificación 2. |
| cCodigoValorClasificacion3 | Cadena | kLongCodValorClasif + 1 | Código del valor de la clasificación 3. |
| cCodigoValorClasificacion4 | Cadena | kLongCodValorClasif + 1 | Código del valor de la clasificación 4. |
| cCodigoValorClasificacion5 | Cadena | kLongCodValorClasif + 1 | Código del valor de la clasificación 5. |
| cCodigoValorClasificacion6 | Cadena | kLongCodValorClasif + 1 | Código del valor de la clasificación 6. |
| cTextoExtra1 | Cadena | kLongTextoExtra + 1 | Texto extra 1. |
| cTextoExtra2 | Cadena | kLongTextoExtra + 1 | Texto extra 2. |
| cTextoExtra3 | Cadena | kLongTextoExtra + 1 | Texto extra 3. |
| cFechaExtra | Cadena | kLongFecha + 1 | Fecha extra |
| cImporteExtra1 | Doble | NA | Importe Extra 1. |
| cImporteExtra2 | Doble | NA | Importe Extra 2. |
| cImporteExtra3 | Doble | NA | Importe Extra 3. |
| cImporteExtra4 | Doble | NA | Importe Extra 4. |

Cliente/Proveedor-RegCteProv-TcteProv

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| cCodigoCliente | Cadena | kLongCodigo + 1 | Código del Cliente / Proveedor. |
| cRazonSocial | Cadena | kLongNombre + 1 | Razón social. |
| cFechaAlta | Cadena | kLongFecha + 1 | Fecha de alta. |
| cRFC | Cadena | kLongRFC + 1 | RFC. |
| cCURP | Cadena | kLongCURP + 1 | CURP. |
| cDenComercial | Cadena | kLongDenComercial + 1 | Denominación comercial. |
| cRepLegal | Cadena | kLongRepLegal + 1 | Representante legal. |
| cNombreMoneda | Cadena | kLongNombre + 1 | Nombre de la moneda. |
| cListaPreciosCliente | Entero | NA | Lista de precios. |
| cDescuentoMovto | Doble | NA | Descuento. |
| cBanVentaCredito | Entero | NA | Bandera de venta a crédito. 0 – No se permite, 1 – Se permite. |
| cCodigoValorClasificacionCliente1 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 1. |
| cCodigoValorClasificacionCliente2 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 2. |
| cCodigoValorClasificacionCliente3 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 3. |
| cCodigoValorClasificacionCliente4 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 4. |
| cCodigoValorClasificacionCliente5 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 5. |
| cCodigoValorClasificacionCliente6 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 6. |
| cTipoCliente | Entero | NA | 1 – Cliente, 2 – Cliente/Proveedor, 3 – Proveedor. |
| cEstatus | Entero | NA | Estado: 0 – Inactivo, 1 – Activo. |
| cFechaBaja | Cadena | kLongFecha + 1 | Fecha de baja. |
| cFechaUltimaRevision | Cadena | kLongFecha + 1 | Fecha de última revisión. |
| cLimiteCreditoCliente | Doble | NA | Limite de crédito. |
| cDiasCreditoCliente | Entero | NA | Días de crédito del cliente. |
| cBanExcederCredito | Entero | NA | Bandera de exceder crédito. 0 – No se permite, 1 – Se permite. |
| cDescuentoProntoPago | Doble | NA | Descuento por pronto pago. |
| cDiasProntoPago | Entero | NA | Días para pronto pago. |
| cInteresMoratorio | Doble | NA | Interes moratorio. |
| cDiaPago | Entero | NA | Día de pago. |
| cDiasRevision | Entero | NA | Días de revisión. |
| cMensajeria | Cadena | kLongDesCorta + 1 | Mensajeria. |
| cCuentaMensajeria | Cadena | kLongDescripcion + 1 | Cuenta de mensajeria. |
| cDiasEmbarqueCliente | Entero | NA | Dias de embarque del cliente. |
| cCodigoAlmacen | Cadena | kLongCodigo + 1 | Código del almacén. |
| cCodigoAgenteVenta | Cadena | kLongCodigo + 1 | Código del agente de venta. |
| cCodigoAgenteCobro | Cadena | kLongCodigo + 1 | Código del agente de cobro. |
| cRestriccionAgente | Entero | NA | Restricción de agente. |
| cImpuesto1 | Doble | NA | Impuesto 1. |
| cImpuesto2 | Doble | NA | Impuesto 2. |
| cImpuesto3 | Doble | NA | Impuesto 3. |
| cRetencionCliente1 | Doble | NA | Retención al cliente 1. |
| cRetencionCliente2 | Doble | NA | Retención al cliente 2. |
| cCodigoValorClasificacionProveedor1 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 1. |
| cCodigoValorClasificacionProveedor2 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 2. |
| cCodigoValorClasificacionProveedor3 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 3. |
| cCodigoValorClasificacionProveedor4 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 4. |
| cCodigoValorClasificacionProveedor5 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 5. |
| cCodigoValorClasificacionProveedor6 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 6. |
| cLimiteCreditoProveedor | Doble | NA | Limite de credito del proveedor. |
| cDiasCreditoProveedor | Entero | NA | Días de credito del proveedor. |
| cTiempoEntrega | Entero | NA | Tiempo de entrega. |
| cDiasEmbarqueProveedor | Entero | NA | Días de embarque. |
| cImpuestoProveedor1 | Doble | NA | Impuesto proveedor 1. |
| cImpuestoProveedor2 | Doble | NA | Impuesto proveedor 2. |
| cImpuestoProveedor3 | Doble | NA | Impuesto proveedor 3. |
| cRetencionProveedor1 | Doble | NA | Retención proveedor 1. |
| cRetencionProveedor2 | Doble | NA | Retención proveedor 2. |
| cBanInteresMoratorio | Entero | NA | Bandera de cálculo de interes moratorio. 0 – No se calculan, 1 – Si se calculan. |
| cTextoExtra1 | Cadena | kLongTextoExtra + 1 | Texto extra 1. |
| cTextoExtra2 | Cadena | kLongTextoExtra + 1 | Texto extra 2. |
| cTextoExtra3 | Cadena | kLongTextoExtra + 1 | Texto extra 3. |
| cFechaExtra | Cadena | kLongFecha + 1 | Fecha extra. |
| cImporteExtra1 | Doble | NA | Importe extra 1. |
| cImporteExtra2 | Doble | NA | Importe extra 2. |
| cImporteExtra3 | Doble | NA | Importe extra 3. |
| cImporteExtra4 | Doble | NA | Importe extra 4. |
| cCodigoValorClasificacionProveedor1 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 1. |
| cCodigoValorClasificacionProveedor2 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 2. |
| cCodigoValorClasificacionProveedor3 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 3. |
| cCodigoValorClasificacionProveedor4 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 4. |
| cCodigoValorClasificacionProveedor5 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 5. |
| cCodigoValorClasificacionProveedor6 | Cadena | kLongCodValorClasif + 1 | Código del valor de clasificación 6. |
| cLimiteCreditoProveedor | Doble | NA | Limite de credito del proveedor. |
| cDiasCreditoProveedor | Entero | NA | Días de credito del proveedor. |
| cTiempoEntrega | Entero | NA | Tiempo de entrega. |
| cDiasEmbarqueProveedor | Entero | NA | Días de embarque. |
| cImpuestoProveedor1 | Doble | NA | Impuesto proveedor 1. |
| cImpuestoProveedor2 | Doble | NA | Impuesto proveedor 2. |
| cImpuestoProveedor3 | Doble | NA | Impuesto proveedor 3. |
| cRetencionProveedor1 | Doble | NA | Retención proveedor 1. |
| cRetencionProveedor2 | Doble | NA | Retención proveedor 2. |
| cBanInteresMoratorio | Entero | NA | Bandera de cálculo de interes moratorio. 0 – No se calculan, 1 – Si se calculan. |
| cTextoExtra1 | Cadena | kLongTextoExtra + 1 | Texto extra 1. |
| cTextoExtra2 | Cadena | kLongTextoExtra + 1 | Texto extra 2. |
| cTextoExtra3 | Cadena | kLongTextoExtra + 1 | Texto extra 3. |
| cFechaExtra | Cadena | kLongFecha + 1 | Fecha extra. |
| cImporteExtra1 | Doble | NA | Importe extra 1. |
| cImporteExtra2 | Doble | NA | Importe extra 2. |
| cImporteExtra3 | Doble | NA | Importe extra 3. |
| cImporteExtra4 | Doble | NA | Importe extra 4. |

Valor de cClasificación - RegValorClasificacion - TValorClasificacion

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| cClasificacionDe | Entero | NA | Clasificación. |
| cNumClasificacion | Entero | NA | Número de la clasificación. |
| cCodigoValorClasificacion | Cadena | kLongCodValorClasif + 1 | Código del valor de la clasificación. |
| cValorClasificacion | Cadena | kLongDescripcion + 1 | Valor de la clasificación. |

Unidad - RegUnidad - TUnidad

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| cNombreUnidad | Cadena | kLongNombre + 1 | Nombre de la unidad. |
| cAbreviatura | Cadena | kLongAbreviatura + 1 | Abreviatura. |
| cDespliegue | Cadena | kLongAbreviatura + 1 | Valor de despliegue. |

Direcciones – RegDireccion– tDireccion

| Campo | Tipo | Longitud | Descripción |
|---|---|---|---|
| cCodCteProv | Cadena | kLongCodigo + 1 | Código cliente / proveedor. |
| cTipoCatalogo | Entero | NA | Tipo de catálogo. |
| cTipoDireccion | Entero | NA | Tipo de dirección. |
| cNombreCalle | Cadena | kLongDescripcion + 1 | Calle. |
| cNumeroExterior | Cadena | kLongNumeroExtInt + 1 | Número exterior. |
| cNumeroInterior | Cadena | kLongNumeroExtInt + 1 | Número interior. |
| cColonia | Cadena | kLongDescripcion + 1 | Colonia. |
| cCodigoPostal | Cadena | kLongCodigoPostal + 1 | Código postal. |
| cTelefono1 | Cadena | kLongTelefono + 1 | Telefono 1. |
| cTelefono2 | Cadena | kLongTelefono + 1 | Telefono 2. |
| cTelefono3 | Cadena | kLongTelefono + 1 | Telefono 3. |
| cTelefono4 | Cadena | kLongTelefono + 1 | Telefono 4. |
| cEmail | Cadena | kLongEmailWeb + 1 | Correo electrónico. |
| cDireccionWeb | Cadena | kLongEmailWeb + 1 | Página web. |
| cCiudad | Cadena | kLongDescripcion + 1 | Ciudad, |
| cEstado | Cadena | kLongDescripcion + 1 | Estado. |
| cPais | Cadena | kLongDescripcion + 1 | País. |
| cTextoExtra | Cadena | kLongDescripcion + 1 | Texto extra. |