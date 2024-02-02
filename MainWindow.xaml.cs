using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DriverCatalog.View;

namespace DriverCatalog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BDManager manager;
        public MainWindow()
        {
            InitializeComponent();
            MyDbContext myDbContext = new MyDbContext();
            manager = new BDManager(myDbContext);
            ContentFrame.Content = new CarsPage(manager);
        }
        #region controls
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizeGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(10 + (150 * index), 0, 0, 0);

            switch (index)
            {
                case 0:
                    ContentFrame.Content = new CarsPage(manager);
                    break;
                case 1:
                    ContentFrame.Content = new DriversPage(manager);
                    break;
                case 2:
                    ContentFrame.Content = new CrewsPage(manager);
                    break;
            }
        }
    }
}
