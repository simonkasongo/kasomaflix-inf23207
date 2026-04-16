using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace KasomaFlix.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Attendre que la fenetre soit chargee avant de naviguer (sinon le Frame peut rester vide ou planter en silence selon la config)
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            MainFrame.Navigate(new Views.AccueilCatalogue());
        }
    }
}