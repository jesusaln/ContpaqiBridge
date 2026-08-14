## Alta de dirección de cliente mediante SDK

Una empresa dedicada a la distribución de productos necesita mantener actualizada la información de contacto de sus clientes y/o proveedores dentro del sistema **CONTPAQi Comercial Premium®**, especialmente las direcciones fiscales y de envío.

En muchos casos, un cliente puede contar con múltiples direcciones, por lo que resulta fundamental registrarlas correctamente.

Para optimizar este proceso y evitar capturas manuales, la empresa utiliza el SDK de CONTPAQi, ya que mediante este procedimiento, se garantiza que la información del cliente esté completa, estandarizada y disponible para su uso en operaciones comerciales y fiscales.

Para lograrlo, se hace uso de dos funciones principales del SDK:

- fBuscaCteProv() que permite localizar y posicionar un cliente o proveedor previamente registrado.
- fAltaDireccion() que registra una nueva dirección asociada al cliente o proveedor.

Configuración en el sistema de CONTPAQi Comercial Premium®

| Paso | Acción |
|---|---|
|  | Antes de ejecutar el proceso, es necesario que el cliente o proveedor ya se encuentre registrado en el sistema: |
|  | Implementación C# private void Direccion_btn_Click(object sender, EventArgs e) { SDK.tDireccion lDireccion = new SDK.tDireccion(); int nError = 0; nError = SDK.fBuscaCteProv("CL01"); if (nError != 0) { MessageBox.Show("Error Buscar cliente: " + SDK.rError(nError)); } else { int idDireccion = 1; lDireccion.cCiudad = "San Pedro Tlaquepaque"; lDireccion.cPais = "Mexico"; lDireccion.cEstado = "Jalisco"; lDireccion.cCodigoPostal = "45638"; lDireccion.cCodCteProv = "CL01"; lDireccion.cColonia = "Los Puestos"; lDireccion.cNombreCalle = "C. Francisco I. Madero"; lDireccion.cNumeroExterior = "1029"; lDireccion.cTipoDireccion = 0; //FISCAL = 0, ENVIO = 1; lDireccion.cTipoCatalogo = 1; // 1 = Clientes nError = SDK.fAltaDireccion(ref idDireccion, ref lDireccion); if(nError != 0 ) { MessageBox.Show("Error Alta de la direccion: " + SDK.rError(nError)); } else { MessageBox.Show("Se dio de Alta la direccion de manera correcta"); } } |
|  | Resultado Al ejecutar el proceso de manera exitosa, la dirección queda asociada al cliente o proveedor indicado y puede consultarse desde la pestaña Domicilios dentro de su registro en CONTPAQi Comercial Premium®. |