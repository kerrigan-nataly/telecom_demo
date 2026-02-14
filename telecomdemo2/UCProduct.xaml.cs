using Microsoft.EntityFrameworkCore;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using telecomdemo2.Data;
using telecomdemo2.Models;

namespace telecomdemo2
{
    /// <summary>
    /// Логика взаимодействия для UCProduct.xaml
    /// </summary>
    public partial class UCProduct : UserControl
    {
        private readonly AppDbContext _context;
        private List<Component> _allComponents;

        public UCProduct(AppDbContext context)
        {
            InitializeComponent();
            App.Controls.Add(this);
            _context = context;
            LoadComponentTypes();
            LoadComponents();
            LoadRecentHistory();
        }

        private void LoadComponentTypes()
        {
            var types = _context.ComponentTypes.OrderBy(t => t.NameComponentType).ToList();

            // Очищаем существующие элементы
            TypeFilterComboBox.Items.Clear();

            // Добавляем элемент "Все типы"
            var allTypesItem = new ComboBoxItem
            {
                Content = "Все типы",
                Tag = "",
                IsSelected = true
            };
            TypeFilterComboBox.Items.Add(allTypesItem);

            // Добавляем разделитель
            TypeFilterComboBox.Items.Add(new Separator());

            // Добавляем типы из базы
            foreach (var type in types)
            {
                TypeFilterComboBox.Items.Add(type);
            }
        }

        private void ApplyFilter()
        {
            if (TypeFilterComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag?.ToString() == "")
            {
                // Выбран "Все типы"
                ComponentsListBox.ItemsSource = _allComponents;
            }
            else if (TypeFilterComboBox.SelectedItem is ComponentType selectedType)
            {
                // Выбран конкретный тип
                ComponentsListBox.ItemsSource = _allComponents
                    .Where(c => c.ComponentType.IdComponentType == selectedType.IdComponentType)
                    .ToList();
            }
            else
            {
                // По умолчанию показываем все
                ComponentsListBox.ItemsSource = _allComponents;
            }
        }

        private void TypeFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_allComponents == null || !_allComponents.Any())
                return;

            if (TypeFilterComboBox.SelectedItem is ComponentType ||
                (TypeFilterComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag?.ToString() == ""))
            {
                ApplyFilter();
            }
        }

        private void LoadComponents()
        {
            _allComponents = _context.Components
                .Include(c => c.ComponentType)
                .Include(c => c.Manufacturer)
                .Include(c => c.TemperatureMode)
                .Include(c => c.ComponentStorages)
                    .ThenInclude(cs => cs.Storage)
                .OrderBy(c => c.IdComponent)
                .ToList();

            ApplyFilter();
        }

        private void LoadRecentHistory()
        {
            var inserts = _context.ComponentStorageInsertsDeletes
                .Where(h => h.OperationType == "INSERT" || h.OperationType == "DELETE")
                .Include(h => h.Component)
                .Include(h => h.Storage)
                .OrderByDescending(h => h.ChangeDate)
                .Take(10)
                .ToList()
                .Select(h => new HistoryItem
                {
                    OperationType = h.OperationType == "INSERT" ? "ПОСТАВКА" : "УДАЛЕНИЕ",
                    OperationTypeColor = h.OperationType == "INSERT" ? "#4CAF50" : "#F44336",
                    ChangeDate = h.ChangeDate,
                    Description = $"{h.Component?.NameComponent}: {h.ComponentCount} шт. на склад '{h.Storage?.NameStorage}'"
                });

            var updates = _context.ComponentStorageUpdates
                .Include(u => u.Component)
                .Include(u => u.OldStorage)
                .Include(u => u.NewStorage)
                .OrderByDescending(u => u.ChangeDate)
                .Take(10)
                .ToList()
                .Select(u => new HistoryItem
                {
                    OperationType = "ПЕРЕМЕЩЕНИЕ",
                    OperationTypeColor = "#2196F3",
                    ChangeDate = u.ChangeDate,
                    Description = $"{u.Component?.NameComponent}: {u.OldCount} шт. → {u.NewCount} шт. (склад '{u.OldStorage?.NameStorage}' → '{u.NewStorage?.NameStorage}')"
                });

            var allHistory = inserts.Concat(updates)
                .OrderByDescending(h => h.ChangeDate)
                .Take(20)
                .ToList();

            HistoryListView.ItemsSource = allHistory;
        }

        private void NewButton_OnClick(object? sender, RoutedEventArgs e)
        {
            WNewProduct newwindow = new WNewProduct(_context);
            newwindow.ShowDialog();
            LoadComponents(); // перезагружаем после добавления
            LoadRecentHistory(); // обновляем историю
        }
    }

    // вспомогательный класс для истории
    public class HistoryItem
    {
        public string OperationType { get; set; }
        public string OperationTypeColor { get; set; }
        public DateTime? ChangeDate { get; set; }
        public string Description { get; set; }
    }
}
