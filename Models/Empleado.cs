using SQLite;

namespace RegistroEmpleadosExamen1.Models
{
    public class Empleado
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Nombres { get; set; } = string.Empty;

        [NotNull]
        public string Apellidos { get; set; } = string.Empty;

        [NotNull]
        public string Puesto { get; set; } = string.Empty;

        public decimal Salario { get; set; }

        [NotNull]
        public string Correo { get; set; } = string.Empty;

        public DateTime FechaIngreso { get; set; }
    }
}

