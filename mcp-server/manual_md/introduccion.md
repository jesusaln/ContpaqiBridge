# Introducción

## ¿Qué es un SDK?

****

Software Development Kit (SDK) o kit de desarrollo de software. Es generalmente un conjunto de herramientas de desarrollo que le permite a un programador crear aplicaciones para un sistema bastante concreto, por ejemplo ciertos paquetes de software, frameworks, plataformas de hardware, ordenadores, videoconsolas, sistemas operativos, etcétera.

En el caso de **CONTPAQi Factura Electrónica®**, el SDK es un conjunto de archivos que contienen funciones publicadas, las cuales pueden ser usadas por desarrolladores externos para manipular (consultar o modificar) información de la base de datos de estos sistemas.

## ¿Cómo funciona?

****

Las funciones disponibles en el SDK se comunican con **CONTPAQi Factura Electrónica®** a través de métodos de clases, estas a su vez, hacen llamados a las clases “base” de **CONTPAQi Factura Electrónica®**, es decir, a las clases usadas dentro de dichos sistemas.

El SDK controla la concurrencia en un ambiente multiusuario, es decir las funciones dan el soporte para los bloqueos y protegen los accesos. (Permite operar como si se tratara de una estación de **CONTPAQi Factura Electrónica®**). Protege las bases de datos, sus relaciones y sigue las reglas de negocio de dichos sistemas.

****