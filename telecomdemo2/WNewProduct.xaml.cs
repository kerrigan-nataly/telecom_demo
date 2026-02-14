using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using telecomdemo2.Data;
using telecomdemo2.Models;

namespace telecomdemo2
{
    /// <summary>
    /// Логика взаимодействия для WNewProduct.xaml
    /// </summary>
    public partial class WNewProduct : Window
    {
        private readonly AppDbContext _context;
        private Component _newComponent;
        private ComponentStorage _newComponentStorage;
        private bool _isTemperatureModeValid = true;

        public Component NewComponent => _newComponent;
        public ComponentStorage NewComponentStorage => _newComponentStorage;

        public WNewProduct(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadComboBoxes();

            // Подписка на изменения выбранных значений
            cmbStorage.SelectionChanged += OnStorageOrTemperatureChanged;
            cmbTemperatureMode.SelectionChanged += OnStorageOrTemperatureChanged;
        }

        // Добавьте этот метод в LoadComboBoxes
private void LoadComboBoxes()
{
    try
    {
        // Загрузка типов компонентов с отображением префикса
        var componentTypes = _context.ComponentTypes
            .Select(ct => new
            {
                ct.IdComponentType,
                ct.NameComponentType,
                Prefix = GetPrefixFromName(ct.NameComponentType),
                DisplayName = $"{ct.NameComponentType} ({GetPrefixFromName(ct.NameComponentType)})"
            })
            .ToList();
        
        cmbComponentType.ItemsSource = componentTypes;
        cmbComponentType.DisplayMemberPath = "DisplayName";
        cmbComponentType.SelectedValuePath = "IdComponentType";
        
        // Остальная загрузка...
        cmbManufacturer.ItemsSource = _context.Manufacturers.ToList();
        
        // Загрузка температурных режимов
        var temperatureModes = _context.TemperatureModes
            .Select(t => new
            {
                t.IdTemperatureMode,
                t.NameTemperatureMode,
                t.MinTemperature,
                t.MaxTemperature,
                t.MinHumidity,
                t.MaxHumidity,
                DisplayName = $"{t.NameTemperatureMode} (T: {t.MinTemperature}°C - {t.MaxTemperature}°C, Влажность: {t.MinHumidity}% - {t.MaxHumidity}%)"
            })
            .ToList();
        cmbTemperatureMode.ItemsSource = temperatureModes;
        cmbTemperatureMode.DisplayMemberPath = "DisplayName";
        cmbTemperatureMode.SelectedValuePath = "IdTemperatureMode";
        
        // Загрузка складов
        var storages = _context.Storages
            .Select(s => new
            {
                s.IdStorage,
                s.NameStorage,
                s.TemperatureModeId,
                TemperatureModeName = s.TemperatureMode != null ? s.TemperatureMode.NameTemperatureMode : "Без режима",
                DisplayInfo = s.TemperatureMode != null 
                    ? $"{s.NameStorage} - {s.TemperatureMode.NameTemperatureMode}"
                    : $"{s.NameStorage} - Без режима"
            })
            .ToList();
        
        cmbStorage.ItemsSource = storages;
        cmbStorage.DisplayMemberPath = "DisplayInfo";
        cmbStorage.SelectedValuePath = "IdStorage";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
            MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

// Вспомогательный метод для получения префикса из названия
private string GetPrefixFromName(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return "XXX";
        
    string[] words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    
    if (words.Length == 1)
    {
        string word = words[0].ToUpper();
        return word.Length >= 3 ? word.Substring(0, 3) : word.PadRight(3, 'X');
    }
    else
    {
        string prefix = "";
        foreach (var word in words)
        {
            if (!string.IsNullOrEmpty(word))
            {
                prefix += word[0].ToString().ToUpper();
            }
        }
        while (prefix.Length < 3)
        {
            prefix += "X";
        }
        return prefix.Length > 3 ? prefix.Substring(0, 3) : prefix;
    }
}


        private void OnStorageOrTemperatureChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckTemperatureModeCompatibility();
        }

        private void CheckTemperatureModeCompatibility()
        {
            // Скрываем предупреждение по умолчанию
            borderWarning.Visibility = Visibility.Collapsed;
            _isTemperatureModeValid = true;

            // Получаем выбранные значения
            var selectedStorage = cmbStorage.SelectedItem;
            var selectedTemperatureMode = cmbTemperatureMode.SelectedItem;

            if (selectedStorage == null || selectedTemperatureMode == null)
                return;

            // Получаем ID температурного режима склада
            int? storageTemperatureModeId = null;
            var storageType = selectedStorage.GetType();
            var temperatureModeIdProperty = storageType.GetProperty("TemperatureModeId");
            if (temperatureModeIdProperty != null)
            {
                storageTemperatureModeId = temperatureModeIdProperty.GetValue(selectedStorage) as int?;
            }

            int componentTemperatureModeId = (int)selectedTemperatureMode.GetType().GetProperty("IdTemperatureMode").GetValue(selectedTemperatureMode);

            // Проверяем совместимость
            if (storageTemperatureModeId.HasValue && storageTemperatureModeId.Value != componentTemperatureModeId)
            {
                // Получаем информацию о температурных режимах для отображения
                var componentMode = _context.TemperatureModes
                    .FirstOrDefault(t => t.IdTemperatureMode == componentTemperatureModeId);

                var storageMode = _context.TemperatureModes
                    .FirstOrDefault(t => t.IdTemperatureMode == storageTemperatureModeId.Value);

                if (componentMode != null && storageMode != null)
                {
                    txtWarning.Text = $"ВНИМАНИЕ! Температурный режим компонента ({componentMode.NameTemperatureMode}) " +
                                     $"не соответствует температурному режиму склада ({storageMode.NameTemperatureMode}).\n" +
                                     $"Компонент: {componentMode.MinTemperature}°C - {componentMode.MaxTemperature}°C, " +
                                     $"Влажность: {componentMode.MinHumidity}% - {componentMode.MaxHumidity}%\n" +
                                     $"Склад: {storageMode.MinTemperature}°C - {storageMode.MaxTemperature}°C, " +
                                     $"Влажность: {storageMode.MinHumidity}% - {storageMode.MaxHumidity}%";

                    borderWarning.Visibility = Visibility.Visible;
                    _isTemperatureModeValid = false;
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return int.TryParse(text, out _);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(txtNameComponent.Text))
                {
                    MessageBox.Show("Введите название компонента", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbComponentType.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тип компонента", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbStorage.SelectedItem == null)
                {
                    MessageBox.Show("Выберите склад", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtComponentCount.Text))
                {
                    MessageBox.Show("Введите количество компонента на складе", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Проверка температурного режима
                if (!_isTemperatureModeValid)
                {
                    var result = MessageBox.Show(
                        "Температурный режим компонента не соответствует режиму склада.\n\n" +
                        "Вы действительно хотите продолжить создание компонента?\n" +
                        "Это может привести к порче компонента при хранении.",
                        "Предупреждение о температурном режиме",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                // Создание нового компонента
                _newComponent = new Component
                {
                    IdComponent = Guid.NewGuid().ToString(),
                    NameComponent = txtNameComponent.Text,
                    ComponentTypeId = (int)cmbComponentType.SelectedValue,
                    ManufacturerId = cmbManufacturer.SelectedValue as int?,
                    Characteristic = txtCharacteristic.Text,
                    Unit = txtUnit.Text,
                    MinStorage = string.IsNullOrWhiteSpace(txtMinStorage.Text) ? null : int.Parse(txtMinStorage.Text),
                    MaxStorage = string.IsNullOrWhiteSpace(txtMaxStorage.Text) ? null : int.Parse(txtMaxStorage.Text),
                    ExpirationDate = dpExpirationDate.SelectedDate.HasValue
                        ? DateOnly.FromDateTime(dpExpirationDate.SelectedDate.Value)
                        : null,
                    TemperatureModeId = cmbTemperatureMode.SelectedValue as int?,
                    PathToScheme = null // Можно добавить выбор файла позже
                };

                // Создание записи о количестве на складе
                _newComponentStorage = new ComponentStorage
                {
                    ComponentId = _newComponent.IdComponent,
                    StorageId = (int)cmbStorage.SelectedValue,
                    ComponentCount = int.Parse(txtComponentCount.Text),
                    Component = _newComponent
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании компонента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
