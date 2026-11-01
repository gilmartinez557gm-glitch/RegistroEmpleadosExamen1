
using RegistroEmpleadosExamen1.Models;
using RegistroEmpleadosExamen1.Services;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RegistroEmpleadosExamen1  ;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _database;

    private Empleado? empleadoSeleccionado;

    public MainPage()
    {
        InitializeComponent();

        _database = new DatabaseService();

        InicializarAplicacionAsync();
    }

    private async void InicializarAplicacionAsync()
    {
        try
        {
            await _database.InicializarAsync();
            await CargarEmpleadosAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                "No se pudo iniciar la base de datos: " + ex.Message,
                "Aceptar");
        }
    }

    private async Task CargarEmpleadosAsync()
    {
        var empleados = await _database.ObtenerEmpleadosAsync();

        lvEmpleados.ItemsSource = empleados;

        lblTotal.Text = $"Total de empleados: {empleados.Count}";
    }

    private bool ValidarFormulario()
    {
        if (string.IsNullOrWhiteSpace(txtNombres.Text))
        {
            MostrarError("Ingrese los nombres del empleado.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtApellidos.Text))
        {
            MostrarError("Ingrese los apellidos del empleado.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtPuesto.Text))
        {
            MostrarError("Ingrese el puesto del empleado.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtSalario.Text))
        {
            MostrarError("Ingrese el salario.");
            return false;
        }

        string salarioTexto = txtSalario.Text.Trim();

        bool salarioValido =
            decimal.TryParse(
                salarioTexto,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal salario)
            ||
            decimal.TryParse(
                salarioTexto,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out salario);

        if (!salarioValido || salario <= 0)
        {
            MostrarError("El salario debe ser un número mayor que cero.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtCorreo.Text))
        {
            MostrarError("Ingrese el correo electrónico.");
            return false;
        }

        string patronCorreo =
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if (!Regex.IsMatch(txtCorreo.Text.Trim(), patronCorreo))
        {
            MostrarError("Ingrese un correo electrónico válido.");
            return false;
        }

        return true;
    }

    private async void MostrarError(string mensaje)
    {
        await DisplayAlert("Validación", mensaje, "Aceptar");
    }

    private Empleado CrearEmpleado()
    {
        string salarioTexto = txtSalario.Text!.Trim();

        if (!decimal.TryParse(
            salarioTexto,
            NumberStyles.Number,
            CultureInfo.CurrentCulture,
            out decimal salario))
        {
            decimal.TryParse(
                salarioTexto,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out salario);
        }

        return new Empleado
        {
            Nombres = txtNombres.Text!.Trim(),
            Apellidos = txtApellidos.Text!.Trim(),
            Puesto = txtPuesto.Text!.Trim(),
            Salario = salario,
            Correo = txtCorreo.Text!.Trim(),
            FechaIngreso = dpFechaIngreso.Date ?? DateTime.Today
            
        };
    }

    private async void Guardar_Clicked(
        object sender, EventArgs e)
    {
        if (!ValidarFormulario())
            return;

        try
        {
            var empleado = CrearEmpleado();

            await _database.AgregarEmpleadoAsync(empleado);

            await DisplayAlert(
                "Éxito",
                "Empleado registrado correctamente.",
                "Aceptar");

            LimpiarFormulario();
            await CargarEmpleadosAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Aceptar");
        }
    }

    private async void Actualizar_Clicked(
        object sender, EventArgs e)
    {
        if (empleadoSeleccionado == null)
        {
            await DisplayAlert(
                "Atención",
                "Seleccione un empleado de la lista.",
                "Aceptar");

            return;
        }

        if (!ValidarFormulario())
            return;

        try
        {
            var empleado = CrearEmpleado();

            empleado.Id = empleadoSeleccionado.Id;

            await _database.ActualizarEmpleadoAsync(empleado);

            await DisplayAlert(
                "Éxito",
                "Empleado actualizado correctamente.",
                "Aceptar");

            LimpiarFormulario();
            await CargarEmpleadosAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Aceptar");
        }
    }

    private async void Eliminar_Clicked(
        object sender, EventArgs e)
    {
        if (empleadoSeleccionado == null)
        {
            await DisplayAlert(
                "Atención",
                "Seleccione un empleado para eliminar.",
                "Aceptar");

            return;
        }

        bool confirmar = await DisplayAlert(
            "Confirmar eliminación",
            $"¿Desea eliminar a {empleadoSeleccionado.Nombres} {empleadoSeleccionado.Apellidos}?",
            "Sí",
            "No");

        if (!confirmar)
            return;

        try
        {
            await _database.EliminarEmpleadoAsync(
                empleadoSeleccionado);

            await DisplayAlert(
                "Éxito",
                "Empleado eliminado correctamente.",
                "Aceptar");

            LimpiarFormulario();
            await CargarEmpleadosAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Aceptar");
        }
    }

    private void lvEmpleados_ItemSelected(
        object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is not Empleado empleado)
            return;

        empleadoSeleccionado = empleado;

        txtNombres.Text = empleado.Nombres;
        txtApellidos.Text = empleado.Apellidos;
        txtPuesto.Text = empleado.Puesto;
        txtSalario.Text = empleado.Salario.ToString(
            CultureInfo.CurrentCulture);
        txtCorreo.Text = empleado.Correo;
        dpFechaIngreso.Date = empleado.FechaIngreso;

        lblId.Text = $"Editando empleado ID: {empleado.Id}";

        lvEmpleados.SelectedItem = null;
    }

    private void LimpiarFormulario()
    {
        txtNombres.Text = string.Empty;
        txtApellidos.Text = string.Empty;
        txtPuesto.Text = string.Empty;
        txtSalario.Text = string.Empty;
        txtCorreo.Text = string.Empty;

        dpFechaIngreso.Date = DateTime.Today;

        empleadoSeleccionado = null;

        lblId.Text = "Nuevo empleado";
    }

    private void Limpiar_Clicked(
        object sender, EventArgs e)
    {
        LimpiarFormulario();
    }
}

