using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol.NVVM.View
{
    public partial class VistaLogin : UserControl
    {
        private readonly UsuarioRepository usuarioRepository;

        public VistaLogin()
        {
            InitializeComponent();
            usuarioRepository = new UsuarioRepository();
        }

        private void BotonIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = NombreTextBox.Text.Trim();

            // Depuración: Verificar el valor del campo de texto
    

            // Verificar que el nombre de usuario no está vacío
            if (string.IsNullOrEmpty(nombreUsuario))
            {
                MessageBox.Show("El nombre de usuario no puede estar vacío.");
                return;
            }

            var usuarios = usuarioRepository.ReadUsuarios();
           
            var usuario = usuarios.FirstOrDefault(u => u.Nombre.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase));

            if (usuario != null)
            {
               
                ((MainWindow)Application.Current.MainWindow).HandleLogin(usuario);
            }
            else
            {
                MessageBox.Show("Nombre de usuario incorrecto");
            }
        }

        private void BotonRegistrar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = NombreTextBox.Text.Trim();
            

            // Depuración: Verificar el valor del campo de texto
            MessageBox.Show($"Valor ingresado para registro: '{nombre}'");

            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("El nombre de usuario no puede estar vacío.");
                return;
            }

            var usuario = new Usuario
            {
                Nombre = nombre,
                Email = "", // Email vacío ya que no se utiliza
              
            };

            usuarioRepository.CreateUsuario(usuario);
            MessageBox.Show("Usuario registrado exitosamente");

            // Depuración: Leer los usuarios nuevamente para verificar el registro
            var usuarios = usuarioRepository.ReadUsuarios();
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
