# 🏥 ClinicaSystem

Sistema de Gestión de Clínica desarrollado en C# Windows Forms utilizando SQL Server y arquitectura de tres capas.

## 📌 Descripción

ClinicaSystem es una aplicación de escritorio diseñada para administrar los procesos de una clínica,
permitiendo gestionar pacientes, médicos, citas, historiales médicos y usuarios mediante una interfaz intuitiva 
y un sistema de autenticación basado en roles.

## ✨ Características

- Inicio de sesión con autenticación segura (SHA-256)
- Gestión de pacientes
- Gestión de médicos
- Gestión de citas
- Historial médico
- Administración de usuarios
- Control de acceso por roles
- Búsqueda en tiempo real
- Validación de conflictos de horario
- Manejo de integridad referencial

## 🛠 Tecnologías

- C#
- Windows Forms
- .NET Framework
- SQL Server Express
- ADO.NET
- Repository Pattern
- Arquitectura de 3 capas

## 📂 Arquitectura

Presentation
↓
Repositories
↓
Models
↓
SQL Server

## 👥 Roles

- Administrador
- Recepcionista
- Médico

## 📦 Instalación

1. Clonar el repositorio.
2. Restaurar la base de datos.
3. Abrir la solución en Visual Studio.
4. Ejecutar la aplicación.

## 🔐 Usuario inicial

Usuario: admin

Contraseña: admin123





## 🚀 Mejoras futuras

- Exportación a PDF y Excel
- Agenda tipo calendario
- Auditoría
- 2FA
- Migración a .NET 8

## 👨‍💻 Autor

Carlos Daniel Flores

Proyecto desarrollado para Programación III - UAPA.
