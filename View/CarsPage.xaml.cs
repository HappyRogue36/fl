using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
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

namespace DriverCatalog
{
    public class CarModel : INotifyPropertyChanged
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
    /// Логика взаимодействия для CarsPage.xaml
    /// </summary>
    public partial class CarsPage : Page
    {
        private BDManager manager;
        public ObservableCollection<CarModel> Cars { get; set; }

        // Фильтрованная коллекция для отображения в DataGrid
        public ObservableCollection<CarModel> FilteredCars { get; set; }

        public CarsPage(BDManager manager)
        {
            InitializeComponent();
            DataContext = this;
            this.manager = manager;
            LoadData();
        }

        public void setFilter()
        {
            if (FilteredCars == null) 
                FilteredCars = new ObservableCollection<CarModel>();

            FilteredCars.Clear();
            foreach (var car in Cars)
            {
                if (!car.IsDelete)
                    FilteredCars.Add(car);
            }
        }

        private void LoadData()
        {
            Cars = new ObservableCollection<CarModel>();
            foreach (var car in manager.GetAllCars())
                Cars.Add(new CarModel() {Id = car.Id, Name = car.Name });
            setFilter();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CarModel newCar = new CarModel();
            Cars.Add(newCar);
            setFilter();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            
            // Проверяем, что что-то выбрано
            if (dataGrid.SelectedItem != null)
            {
                // Получаем выбранный объект
                var selectedCar = (CarModel)dataGrid.SelectedItem;
                if (manager.GetAllCrews().Exists(crew => crew.CarId == selectedCar.Id))
                {
                    MessageBox.Show("Не могу удалить данную запись, т.к в таблице Crews присутствует эта машина", "Ошибка!");
                    return;
                }
                selectedCar.IsDelete = true;
            }
            setFilter();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (check())
                return;
            foreach (var car in Cars)
            {
                if (car.IsDelete)
                {
                    if (manager.GetCarById(car.Id) != null)
                        manager.DeleteCar(car.Id);
                    continue;
                }
                Car curCar = new Car() 
                {
                    Id = car.Id,
                    Name = car.Name,
                };

                try
                {
                    manager.UpdateCar(curCar);
                }
                catch (Exception ex) 
                {
                    manager.AddCar(curCar);
                }
            }
            LoadData();
        }

        private bool check()
        {
            bool err = false;
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
