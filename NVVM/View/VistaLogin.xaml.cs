using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol.NVVM.View
{
    public partial class VistaLogin : UserControl
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly FacturaRepository _facturaRepository;

        public VistaLogin()
        {
            InitializeComponent();
            _usuarioRepository = new UsuarioRepository();
            _facturaRepository = new FacturaRepository();
        }

        private void BotonIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = NombreTextBox.Text.Trim();
            string contrasena = ContrasenaBox.Password;

            // Verificar que el nombre de usuario y la contraseña no estén vacíos
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
            {
                MessageBox.Show("El nombre de usuario y la contraseña no pueden estar vacíos.");
                return;
            }

            var usuarios = _usuarioRepository.ReadUsuarios();
            var usuario = usuarios.FirstOrDefault(u => u.Nombre.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase) && u.Contrasena.Equals(contrasena));

            if (usuario != null)
            {
                AppState.UserId = usuario.Id; // Guardar el ID del usuario en el estado de la aplicación

                // Obtener la primera factura no confirmada del usuario
                var factura = _facturaRepository.GetFirstUnconfirmedFacturaByUserId(usuario.Id);
                if (factura != null)
                {
                    AppState.FacturaId = factura.Id; // Guardar el ID de la factura en el estado de la aplicación
                }

                ((MainWindow)Application.Current.MainWindow).HandleLogin(usuario);
            }
            else
            {
                MessageBox.Show("Nombre de usuario o contraseña incorrectos.");
            }
        }

        private void BotonRegistrar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = NombreTextBox.Text.Trim();
            string contrasena = ContrasenaBox.Password;

            // Verificar el valor del campo de texto
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contrasena))
            {
                MessageBox.Show("El nombre de usuario y la contraseña no pueden estar vacíos.");
                return;
            }

            var usuario = new Usuario
            {
                Nombre = nombre,
                Email = "", // Email vacío ya que no se utiliza
                Contrasena = contrasena
            };

            _usuarioRepository.CreateUsuario(usuario);
            MessageBox.Show("Usuario registrado exitosamente");

            // Leer los usuarios nuevamente para verificar el registro
            var usuarios = _usuarioRepository.ReadUsuarios();
            string debugMessage = "Usuarios leídos de la base de datos después del registro:\n";
            foreach (var user in usuarios)
            {
                debugMessage += $"ID: {user.Id}, Nombre: {user.Nombre}, Email: {user.Email}, Admin: {user.Admin}\n";
            }
            MessageBox.Show(debugMessage);
        }

        private void ContrasenaBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.Tag = passwordBox.Password;
            }
        }
    }
}
