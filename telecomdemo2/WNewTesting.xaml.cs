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
using System.Windows.Shapes;
using telecomdemo2.Data;
using telecomdemo2.Models;

namespace telecomdemo2
{
    /// <summary>
    /// Логика взаимодействия для WNewTesting.xaml
    /// </summary>
    public partial class WNewTesting : Window
    {

        private AppDbContext _context;
        private List<TestingResult> _testingResults;
        private List<OrderNode> _currentOrderNodes;
        public WNewTesting(AppDbContext context)
        {
            _context = context;
            InitializeComponent();
            LoadTestingResults();
            LoadOrders();
            
        }
        // Свойство для привязки списка результатов тестировки
        public List<TestingResult> AvailableTestingResults
        {
            get { return _testingResults; }
        }

        private void LoadTestingResults()
        {
            try
            {
                _testingResults = _context.TestingResults.ToList();
                DataContext = this; // Обновляем контекст данных
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки результатов тестировки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrders()
        {
            try
            {
                var orders = _context.Orders
                    .Include(o => o.Status)
                    .OrderBy(o => o.IdOrder)
                    .ToList();

                if (orders.Any())
                {
                    cmbOrders.ItemsSource = orders;
                    cmbOrders.Visibility = Visibility.Visible;
                    txtNoOrders.Visibility = Visibility.Collapsed;

                    if (orders.Count > 0)
                        cmbOrders.SelectedIndex = 0;
                }
                else
                {
                    cmbOrders.Visibility = Visibility.Collapsed;
                    txtNoOrders.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbOrders.SelectedItem is Order selectedOrder)
            {
                LoadOrderNodes(selectedOrder.IdOrder);
            }
        }

        private void LoadOrderNodes(int orderId)
        {
            try
            {
                _currentOrderNodes = _context.OrderNodes
                    .Include(on => on.Node)
                        .ThenInclude(n => n.NodeType)
                    .Include(on => on.Node)
                        .ThenInclude(n => n.TestingResult)
                    .Where(on => on.OrderId == orderId)
                    .ToList();

                if (_currentOrderNodes.Any())
                {
                    dgNodes.ItemsSource = _currentOrderNodes;
                    dgNodes.Visibility = Visibility.Visible;
                    panelOrderInfo.Visibility = Visibility.Visible;
                    txtNoNodes.Visibility = Visibility.Collapsed;
                }
                else
                {
                    dgNodes.ItemsSource = null;
                    dgNodes.Visibility = Visibility.Collapsed;
                    panelOrderInfo.Visibility = Visibility.Collapsed;
                    txtNoNodes.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки узлов заказа: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbTestingResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Tag is Node node)
            {
                if (comboBox.SelectedItem is TestingResult selectedResult)
                {
                    node.TestingResultId = selectedResult.IdTestingResult;
                    node.TestingResult = selectedResult;
                }
                else if (comboBox.SelectedValue is int resultId)
                {
                    node.TestingResultId = resultId;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сохраняем изменения в базе данных
                int changesCount = _context.SaveChanges();

                if (changesCount > 0)
                {
                    MessageBox.Show($"Изменения успешно сохранены. Обновлено {changesCount} записей.",
                        "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновляем список узлов
                    if (cmbOrders.SelectedItem is Order selectedOrder)
                    {
                        LoadOrderNodes(selectedOrder.IdOrder);
                    }
                }
                else
                {
                    MessageBox.Show("Нет изменений для сохранения.",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка сохранения в базу данных: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обновляем контекст
                _context.ChangeTracker.Clear();

                // Перезагружаем данные
                LoadTestingResults();
                LoadOrders();

                MessageBox.Show("Данные обновлены.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
