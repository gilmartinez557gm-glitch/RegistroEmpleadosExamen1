
using SQLite;
using RegistroEmpleadosExamen1.Models;

namespace RegistroEmpleadosExamen1.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            string ruta = Path.Combine(
                FileSystem.AppDataDirectory,
                "Empleados.db3"
            );

            _database = new SQLiteAsyncConnection(ruta);
        }

        public async Task InicializarAsync()
        {
            await _database.CreateTableAsync<Empleado>();
        }

        public async Task<int> AgregarEmpleadoAsync(Empleado empleado)
        {
            return await _database.InsertAsync(empleado);
        }

        public async Task<List<Empleado>> ObtenerEmpleadosAsync()
        {
            return await _database.Table<Empleado>()
                .OrderBy(e => e.Nombres)
                .ToListAsync();
        }

        public async Task<int> ActualizarEmpleadoAsync(Empleado empleado)
        {
            return await _database.UpdateAsync(empleado);
        }

        public async Task<int> EliminarEmpleadoAsync(Empleado empleado)
        {
            return await _database.DeleteAsync(empleado);
        }
    }
}

