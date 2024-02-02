using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DriverCatalog.View
{
    public class DriverModel : INotifyPropertyChanged
    {
        private int id;
        private string name;
        private bool isDelete;
        public int Id
        {
            get { return id; }
            set
            {
                this.id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                this.name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool IsDelete
        {
            get { return isDelete; }
            set
            {
                this.isDelete = value;
                OnPropertyChanged(nameof(IsDelete));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Логика взаимодействия для DriversPage.xaml
    /// </summary>
    public partial class DriversPage : Page
    {
        private BDManager manager;
        public ObservableCollection<DriverModel> Drivers { get; set; }

        // Фильтрованная коллекция для отображения в DataGrid
        public ObservableCollection<DriverModel> FilteredDrivers { get; set; }
        public DriversPage(BDManager manager)
        {
            InitializeComponent();
            DataContext = this;
            this.manager = manager;
            LoadData();
        }
        public void setFilter()
        {
            if (FilteredDrivers == null)
                FilteredDrivers = new ObservableCollection<DriverModel>();

            FilteredDrivers.Clear();
            foreach (var car in Drivers)
            {
                if (!car.IsDelete)
                    FilteredDrivers.Add(car);
            }
        }

        private void LoadData()
        {
            Drivers = new ObservableCollection<DriverModel>();
            foreach (var car in manager.GetAllDrivers())
                Drivers.Add(new DriverModel() { Id = car.Id, Name = car.Name });
            setFilter();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            DriverModel newCar = new DriverModel();
            Drivers.Add(newCar);
            setFilter();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что что-то выбрано
            if (dataGrid.SelectedItem != null)
            {
                // Получаем выбранный объект
                var selectedDriver = (DriverModel)dataGrid.SelectedItem;
                if (manager.GetAllCrews().Exists(crew => crew.DriverId == selectedDriver.Id))
                {
                    MessageBox.Show("Не могу удалить данную запись, т.к в таблице Crews присутствует этот пилот", "Ошибка!");
                    return;
                }
                selectedDriver.IsDelete = true;
            }
            setFilter();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (check())
                return;
            foreach (var driver in Drivers)
            {
                if (driver.IsDelete)
                {
                    if (manager.GetDriverById(driver.Id) != null)
                        manager.DeleteDriver(driver.Id);
                    continue;
                }
                Driver curDriver = new Driver()
                {
                    Id = driver.Id,
                    Name = driver.Name,
                };

                try
                {
                    manager.UpdateDriver(curDriver);
                }
                catch (Exception ex)
                {
                    manager.AddDriver(curDriver);
                }
            }
            LoadData();
        }

        private bool check()
        {
            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                // Получаем строку (DataGridRow) по индексу
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);

                // Проверяем, что строка не null
                if (row != null)
                {
                    // Убираем фон
                    row.Background = Brushes.White;
                }
            }
            bool err = false;
            List<object> ints = new List<object>();
            foreach (var item in dataGrid.Items)
            {
                // Получаем тип элемента в коллекции данных
                var itemType = item.GetType();

                // Предположим, что у вашего объекта данных есть свойство Id
                var idProperty = itemType.GetProperty("Id");

                if (idProperty != null)
                {
                    // Получаем значение свойства Id для текущего элемента
                    var itemId = idProperty.GetValue(item);

                    // Сравниваем с нужным вам Id
                    if (itemId != null && ints.Contains(itemId))
                    {
                        // Найден элемент с нужным Id, выполняем действия
                        // Например, изменяем цвет фона
                        var dataGridRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(item);
                        if (dataGridRow != null)
                        {
                            dataGridRow.Background = Brushes.Red;
                        }
                        err = true;
                    }
                    else
                    {
                        ints.Add(itemId);
                    }
                }
            }

            if (err)
                MessageBox.Show("Красный цветом выделены строки у которых повторяется id", "Ошибка!");
            else
                MessageBox.Show("Успешно обновил данные", "Супер!");

            return err;
        }
    }
}
