using System;

namespace ClinicaSystem.Data
{
    // =======================================================================
    // Excepción "amigable" que envuelve una SqlException original.
    // El mensaje (Message) está pensado para mostrarse directamente en un
    // MessageBox al usuario final, en español y sin detalles técnicos.
    // El detalle técnico original queda disponible en InnerException por si
    // en algún momento se quiere loguearlo (archivo de log, Event Viewer, etc).
    // =======================================================================
    public class DatosException : Exception
    {
        public DatosException(string mensaje, Exception innerException)
            : base(mensaje, innerException)
        {
        }
    }
}
