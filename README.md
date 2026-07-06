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

- # 📚 Buenas prácticas implementadas

- Arquitectura en tres capas.
- Repository Pattern.
- Programación Orientada a Objetos.
- Consultas parametrizadas.
- Manejo de excepciones.
- Separación de responsabilidades.
- Hash SHA-256 para contraseñas.
- Validaciones en la interfaz.
- Control de acceso basado en roles.

- # ⚙️ Instalación y ejecución

## Requisitos

- Visual Studio 2019 o superior
- .NET Framework 4.7.2 o superior
- SQL Server Express
- SQL Server Management Studio (opcional)

## Pasos

### 1. Clonar el repositorio

```bash
git clone https://github.com/Carlosrose07/ClinicaSystem.git
### 2. Abrir la solución
Abrir el archivo **ClinicaSystem.sln** desde Visual Studio.
### 3. Restaurar la base de datos
Ejecutar el script SQL incluido en el proyecto utilizando SQL Server Management Studio.
### 4. Configurar la cadena de conexión

Modificar la cadena de conexión ubicada en:
Data/ConexionDB.cs
con el nombre de su instancia de SQL Server.
### 5. Ejecutar el proyecto
Presionar **F5** o seleccionar **Start** desde Visual Studio.

💻 Tecnologías utilizadas

| Tecnología | Descripción |
| C# | Lenguaje de programación principal |
| .NET Framework | Plataforma de desarrollo |
| Windows Forms | Interfaz gráfica |
| SQL Server Express | Base de datos |
| ADO.NET | Acceso a datos |
| SHA-256 | Cifrado de contraseñas |
| Visual Studio | Entorno de desarrollo |
| Git | Control de versiones |
| GitHub | Repositorio del proyecto |

📂 Estructura del proyecto

```
ClinicaSystem
│
├── Models
│   ├── Paciente.cs
│   ├── Medico.cs
│   ├── Usuario.cs
│   ├── Rol.cs
│   ├── Cita.cs
│   └── HistorialMedico.cs
│
├── Data
│   ├── ConexionDB.cs
│   ├── PacienteRepository.cs
│   ├── MedicoRepository.cs
│   ├── UsuarioRepository.cs
│   ├── CitaRepository.cs
│   ├── HistorialMedicoRepository.cs
│   └── RolRepository.cs
│
├── Forms
│   ├── FrmLogin.cs
│   ├── FrmMenuPrincipal.cs
│   ├── FrmPacientes.cs
│   ├── FrmMedicos.cs
│   ├── FrmCitas.cs
│   ├── FrmHistorialMedico.cs
│   └── FrmUsuarios.cs
│
└── Program.cs

## 👥 Roles

- Administrador
- Recepcionista
- Médico



## 🔐 Usuario inicial

Usuario: admin

Contraseña: admin123

##imagenes
![admin](prueba%20admin.PNG)
![citas](prueba%20citas.PNG)
![prueba session](prueba%20inicio%20de%20session.PNG)
![prueba pacientes](prueba%20pacientes.PNG)






## 🚀 Mejoras futuras

- Exportación a PDF y Excel
- Agenda tipo calendario
- Auditoría
- 2FA
- Migración a .NET 8

## 👨‍💻 Autor

Carlos Daniel Flores

Proyecto desarrollado para Programación III - UAPA.
