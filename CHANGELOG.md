# 📝 Registro de cambios – ClinicaSystem

Registro breve de los 5 avances y ajustes principales realizados durante el desarrollo del proyecto.

| # | Cambio / Avance | Tipo | Descripción | Referencia |
|---|---|---|---|---|
| 1 | Protección de contraseñas con PBKDF2 | Seguridad | Se reemplazó el hash SHA-256 por PBKDF2 con salt. Los usuarios antiguos se migran de forma transparente al iniciar sesión, sin perder acceso. | `UsuarioRepository.cs` |
| 2 | Bloqueo por intentos fallidos de login | Seguridad | Tras 5 intentos fallidos la cuenta se bloquea 15 minutos, reduciendo el riesgo de ataques de fuerza bruta. | `FrmLogin.cs`, `UsuarioRepository.cs` |
| 3 | Acceso a datos centralizado con `DbHelper` | Arquitectura / Refactor | Se creó `DbHelper.cs` (consultas parametrizadas genéricas) y `DatosException.cs`, que traduce errores de SQL Server (llave foránea, duplicados, timeout, conexión) a mensajes claros en español. Se aplicó primero a `UsuarioRepository`. | `Data/DbHelper.cs`, `Data/DatosException.cs` |
| 4 | Refactor de repositorios al patrón `DbHelper` | Refactor / Calidad | `PacienteRepository`, `MedicoRepository` y `CitaRepository` migrados a `DbHelper` sin cambiar su API pública. En citas se probaron conflictos de horario, edición, cancelación (libera el horario) y cambio de estado. | commits `3a4a1c2`, `405f277` |
| 5 | Control de acceso por roles y mejora visual del menú | Funcionalidad / Interfaz | El menú principal oculta módulos según el rol (Administrador, Recepcionista, Médico) y se unificó el color de los botones. | commit `73f040f`, `FrmMenuPrincipal.cs` |

## Pendiente para próximas versiones

- Migrar `HistorialMedicoRepository` a `DbHelper`.
- Paginación, dashboard y reportes.
- Validación de correo y teléfono.
