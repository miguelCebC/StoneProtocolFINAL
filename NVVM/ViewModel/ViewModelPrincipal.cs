using System.Windows;
using System.Windows.Input;
using StoneProtocol.Nucleo;
using StoneProtocol.NVVM.View;

namespace StoneProtocol.NVVM.ViewModel
{
    public class ViewModelPrincipal : ObjetoObservable
    {
        public ViewModel1 VM1 { get; set; }

        public string text { get; set; }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand ShowHomeViewCommand { get; private set; }
        public ICommand ShowProductsViewCommand { get; private set; }
        public ICommand ShowStoreViewCommand { get; private set; }
        public ICommand ShowVistaPrincipalCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand PedidosCommand { get; private set; }


        public ViewModelPrincipal()
        {
            VM1 = new ViewModel1();
            CurrentView = new VistaLogin(); 
            text = "Hola";

            ShowHomeViewCommand = new RelayCommand(_ => ShowHomeView());
            ShowProductsViewCommand = new RelayCommand(_ => ShowProductsView());
            ShowStoreViewCommand = new RelayCommand(_ => ShowStoreView());
            ShowVistaPrincipalCommand = new RelayCommand(_ => ShowVistaPrincipal());
            CloseCommand = new RelayCommand(_ => CloseApplication());
            PedidosCommand = new RelayCommand(_ => ShowPedidosView());
        }

        public void ShowHomeView()
        {
            CurrentView = new VistaPrincipal();
        }

        public void ShowPedidosView()
        {
            CurrentView = new VistaFactura();
        }

        public void ShowProductsView()
        {
            CurrentView = new VistaProducto();
        }

        public void ShowStoreView()
        {
            CurrentView = new VistaLogin();
        }

        public void ShowVistaPrincipal()
        {
            CurrentView = new VistaPrincipal();
        }

        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
