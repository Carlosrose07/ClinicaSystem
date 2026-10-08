# 🏥 ClinicaSystem

Sistema de gestión de clínica desarrollado en **C# Windows Forms** con **SQL Server**, organizado en una arquitectura por capas.

---

## 📌 Descripción general

| Campo | Detalle |
|---|---|
| **Nombre del sistema** | ClinicaSystem |
| **Tipo** | Aplicación de escritorio (Windows Forms) |
| **Autor** | Carlos Daniel Flores |
| **Asignatura** | Ingeniería de Software I – UAPA |
| **Repositorio** | https://github.com/Carlosrose07/ClinicaSystem |

## 🎯 Objetivo

Digitalizar y centralizar la gestión administrativa y clínica básica de un consultorio: pacientes, médicos, citas, historiales médicos y usuarios, con acceso controlado según el rol de cada persona.

## ❗ Problema que resuelve

En muchas clínicas pequeñas el registro de pacientes y la agenda de citas se llevan en papel o en hojas de cálculo sueltas. Esto provoca:

- Citas duplicadas o solapadas para un mismo médico.
- Información del paciente dispersa y difícil de consultar.
- Falta de control sobre quién puede ver o modificar datos sensibles.

ClinicaSystem centraliza esa información en una base de datos, valida los conflictos de horario y restringe el acceso por rol.

---

## ✨ Funcionalidades principales

- Inicio de sesión con contraseñas protegidas mediante **PBKDF2 + salt** (con migración transparente desde hashes SHA-256 antiguos).
- Bloqueo temporal de la cuenta tras **5 intentos fallidos** (15 minutos).
- CRUD completo de **pacientes, médicos, citas, historial médico y usuarios**.
- **Control de acceso por roles** (el menú oculta los módulos que no corresponden).
- **Validación de conflictos de horario** al agendar o editar citas.
- Búsqueda en tiempo real.
- Manejo de integridad referencial con mensajes de error claros en español.
- Instalador distribuible creado con Inno Setup.

---

## 🏗️ Arquitectura seleccionada

**Arquitectura en capas**, elegida porque coincide con la estructura real del código (WinForms + Repositories + SQL Server) y separa responsabilidades sin agregar complejidad innecesaria para un sistema de escritorio.

```
┌──────────────────────────────────────┐
│ Presentación        (Forms/)         │
├──────────────────────────────────────┤
│ Lógica de negocio   (reglas, roles,  │
│                      validaciones)   │
├──────────────────────────────────────┤
│ Acceso a datos      (Data/)          │
│  Repositories + DbHelper             │
├──────────────────────────────────────┤
│ Base de datos       (SQL Server)     │
└──────────────────────────────────────┘
   Autenticación: servicio transversal
   (usado por Presentación y Lógica de negocio)
```

Principios aplicados: separación de responsabilidades, Repository Pattern, consultas parametrizadas y manejo centralizado de excepciones.

---

## 🧩 Organización en Frontend y Backend

Como ClinicaSystem es una aplicación de escritorio, la separación es **lógica** (dentro de una misma solución), no una separación web con servidores distintos.

### Frontend (capa de Presentación)

**Ubicación:** `Forms/`

**Responsabilidades:**

- Mostrar las pantallas (login, menú, pacientes, médicos, citas, historial, usuarios).
- Capturar los datos que escribe el usuario y validar el formato básico (campos vacíos, tipos de dato).
- Mostrar u ocultar opciones del menú según el rol.
- Presentar al usuario los resultados y los mensajes de error.

El frontend **no** contiene consultas SQL ni accede directamente a la base de datos.

### Backend (lógica de negocio + acceso a datos)

**Ubicación:** `Models/` y `Data/`

**Responsabilidades:**

- **Models:** definir las entidades del sistema (`Paciente`, `Medico`, `Usuario`, `Rol`, `Cita`, `HistorialMedico`).
- **Repositories:** ejecutar las operaciones CRUD y las reglas de datos (por ejemplo, detectar conflictos de horario en las citas).
- **DbHelper:** punto único de acceso a SQL Server mediante consultas parametrizadas.
- **DatosException:** traducir los errores de SQL Server (llave foránea, duplicados, timeout, conexión) a mensajes comprensibles en español.
- **Autenticación:** verificar credenciales con PBKDF2, controlar el bloqueo por intentos y entregar el rol del usuario.

### Comunicación entre ambos

1. El usuario realiza una acción en un formulario (por ejemplo, agendar una cita).
2. El formulario valida los datos básicos y llama a un método del repositorio correspondiente, pasando un objeto del modelo (`Cita`).
3. El repositorio aplica las reglas, arma la consulta parametrizada y la envía a SQL Server a través de `DbHelper`.
4. SQL Server responde; si hay un error, `DatosException` lo convierte en un mensaje claro.
5. El repositorio devuelve el resultado (objeto, lista o confirmación) al formulario, que lo muestra al usuario.

La comunicación se hace mediante **llamadas directas a métodos** dentro de la misma aplicación; no hay API REST ni servidor intermedio.

---

## 💻 Herramientas y tecnologías utilizadas

| Tecnología / Herramienta | Uso |
|---|---|
| C# | Lenguaje principal |
| .NET Framework | Plataforma de desarrollo |
| Windows Forms | Interfaz gráfica |
| SQL Server Express | Base de datos (`ClinicaDB`) |
| ADO.NET | Acceso a datos |
| PBKDF2 | Protección de contraseñas |
| Visual Studio | Entorno de desarrollo |
| Git / GitHub | Control de versiones y repositorio |
| GitHub Projects | Tablero Kanban de gestión del avance |
| GitHub Issues | Seguimiento de tareas, riesgos y mejoras |
| Visual Paradigm Online | Modelado UML y diagrama de arquitectura |
| Inno Setup | Instalador distribuible |

---

## 📊 Estado actual del proyecto

**Versión actual:** v1.0 (versión inicial controlada)

| Área | Estado |
|---|---|
| CRUD de pacientes, médicos, citas, historial y usuarios | ✅ Completado |
| Autenticación con PBKDF2 y bloqueo por intentos | ✅ Completado |
| Control de acceso por roles | ✅ Completado |
| Validación de conflictos de horario | ✅ Completado |
| Centralización del acceso a datos con `DbHelper` | 🔄 En progreso (pendiente `HistorialMedicoRepository`) |
| Documentación técnica (requisitos, UML, arquitectura, gestión) | ✅ Completado |
| Paginación, dashboard y reportes | ⏳ Pendiente |

---

## 📚 Buenas prácticas implementadas

- Arquitectura por capas.
- Repository Pattern.
- Programación Orientada a Objetos.
- Consultas parametrizadas.
- Manejo centralizado de excepciones.
- Separación de responsabilidades.
- PBKDF2 con salt para contraseñas.
- Validaciones en la interfaz.
- Control de acceso basado en roles.

---

## 📂 Estructura del proyecto

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
│   ├── DbHelper.cs
│   ├── DatosException.cs
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
├── docs
│   └── (requisitos, UML, arquitectura, gestión del avance, informe)
│
└── Program.cs
```

---

## ⚙️ Instalación y ejecución

### Requisitos

- Visual Studio 2019 o superior
- .NET Framework 4.7.2 o superior
- SQL Server Express
- SQL Server Management Studio (opcional)

### Pasos

**1. Clonar el repositorio**

```bash
git clone https://github.com/Carlosrose07/ClinicaSystem.git
```

**2. Abrir la solución**

Abrir el archivo `ClinicaSystem.sln` desde Visual Studio.

**3. Restaurar la base de datos**

Ejecutar el script SQL incluido en el proyecto usando SQL Server Management Studio.

**4. Configurar la cadena de conexión**

Modificar la cadena de conexión ubicada en `Data/ConexionDB.cs` con el nombre de su instancia de SQL Server.

**5. Ejecutar el proyecto**

Presionar **F5** o seleccionar **Start** en Visual Studio.

---

## 👥 Roles del sistema

| Rol | Acceso |
|---|---|
| Administrador | Todos los módulos |
| Recepcionista | Todos excepto Historial médico y Usuarios |
| Médico | Todos excepto Médicos y Usuarios |

## 🔐 Usuario inicial (solo para pruebas)

- **Usuario:** `admin`
- **Contraseña:** `admin123`

> ⚠️ Estas credenciales son únicamente para el entorno de pruebas académico. En un uso real deben cambiarse en el primer inicio de sesión.

---

## 🖼️ Capturas del sistema

![Panel de administrador](prueba%20admin.PNG)
![Gestión de citas](prueba%20citas.PNG)
![Inicio de sesión](prueba%20inicio%20de%20session.PNG)
![Gestión de pacientes](prueba%20pacientes.PNG)

---

## 🚀 Mejoras futuras

- Exportación a PDF y Excel
- Agenda tipo calendario
- Auditoría de acciones
- Autenticación en dos pasos (2FA)
- Migración a .NET 8
- Paginación, dashboard y reportes
