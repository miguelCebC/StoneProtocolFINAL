using System.Windows;
using System.Windows.Controls;
using StoneProtocol.NVVM.Model;

namespace StoneProtocol.NVVM.View
{
    /// <summary>
    /// Lógica de interacción para VistaLogin.xaml
    /// </summary>
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
            string nombreUsuario = NombreTextBox.Text;

            var usuarios = usuarioRepository.ReadUsuarios();
            var usuario = usuarios.FirstOrDefault(u => u.Nombre == nombreUsuario);

            if (usuario != null)
            {
                MessageBox.Show("Inicio de sesión exitoso");
                ((MainWindow)Application.Current.MainWindow).HandleLogin(usuario);
            }
            else
            {
                MessageBox.Show("Nombre de usuario incorrecto");
            }
        }

        private void BotonRegistrar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = NombreTextBox.Text;
            bool esAdmin = EsAdminCheckBox.IsChecked ?? false;

            var usuario = new Usuario
            {
                Nombre = nombre,
                Email = "", // Email vacío ya que no se utiliza
                Admin = esAdmin // Asignar admin si está marcado
            };

            usuarioRepository.CreateUsuario(usuario);
            MessageBox.Show("Usuario registrado exitosamente");
        }
    }
}
