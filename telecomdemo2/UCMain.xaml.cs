using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
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
    /// Логика взаимодействия для UCMain.xaml
    /// </summary>
    public partial class UCMain : UserControl
    {
        private AppDbContext _context = new AppDbContext();
        public UCMain()
        {
            InitializeComponent();
            App.MainWindow.WindowState = WindowState.Maximized;
            App.Controls.Add(this);
            LoadData();
        }

        private void LoadData()
        {
            // Статистика изменений заказов (объединяем вставки/удаления и обновления)
            var orderChanges = new List<object>();

            // Добавляем операции вставки/удаления
            var insertDeleteOperations = _context.OrderInsertsDeletes
                .OrderByDescending(o => o.ChangeDate)
                .Take(10)
                .Select(o => new
                {
                    Id = o.IdHistory,
                    OrderName = o.Order != null ? o.Order.NameOrder : "Неизвестный заказ",
                    Operation = o.OperationType,
                    Status = o.OrderStatusNavigation != null ? o.OrderStatusNavigation.NameStatus : null,
                    Date = o.ChangeDate,
                    Type = "InsertDelete"
                })
                .ToList();

            // Добавляем операции обновления статуса
            var updateOperations = _context.OrderUpdates
                .OrderByDescending(o => o.ChangeDate)
                .Take(10)
                .Select(o => new
                {
                    Id = o.IdHistory,
                    OrderName = o.Order != null ? o.Order.NameOrder : "Неизвестный заказ",
                    Operation = "Обновление статуса",
                    Status = $"Статус изменен: {(o.OldStatusNavigation != null ? o.OldStatusNavigation.NameStatus : "неизвестно")} " +
                    $"→ {(o.NewStatusNavigation != null ? o.NewStatusNavigation.NameStatus : "неизвестно")}",
                    Date = o.ChangeDate,
                    Type = "Update"
                })
                .ToList();

            // Объединяем и сортируем по дате
            orderChanges = insertDeleteOperations
                .Concat<object>(updateOperations)
                .OrderByDescending(x => ((dynamic)x).Date)
                .Take(5) // Берем последние 5 изменений
                .ToList();

            OrderListBox.ItemsSource = orderChanges;

            // Срочные задачи (дедлайн в ближайшие 3 дня)
            var urgentOrders = _context.Orders
                .Where(o => o.DeadlineDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(3))
                    && o.DeadlineDate >= DateOnly.FromDateTime(DateTime.Now)
                    && o.Status.NameStatus != "выполнен")
                .OrderBy(o => o.DeadlineDate)
                .ToList();
            UrgentOrderListBox.ItemsSource = urgentOrders;

            var lowStockComponents = _context.Components
            .Where(c => c.ComponentStorages.Sum(cs => cs.ComponentCount) <= c.MinStorage)
            .ToList();
            ProductListBox.ItemsSource = lowStockComponents;
        }

        public void LoadChartData(List<OrderInsertsDelete> insertsDeletes, List<OrderUpdate> updates)
        {
            var newOrdersData = new Dictionary<DateTime, int>();
            var updatedOrdersData = new Dictionary<DateTime, int>();

            // Подсчет новых заказов (вставок)
            foreach (var item in insertsDeletes.Where(x => x.ChangeDate.HasValue))
            {
                var date = item.ChangeDate.Value.Date;
                if (newOrdersData.ContainsKey(date))
                    newOrdersData[date]++;
                else
                    newOrdersData[date] = 1;
            }

            // Подсчет измененных заказов (обновлений статуса)
            foreach (var item in updates.Where(x => x.ChangeDate.HasValue))
            {
                var date = item.ChangeDate.Value.Date;
                if (updatedOrdersData.ContainsKey(date))
                    updatedOrdersData[date]++;
                else
                    updatedOrdersData[date] = 1;
            }

            var allDates = newOrdersData.Keys
                .Union(updatedOrdersData.Keys)
                .OrderBy(x => x)
                .ToList();

            // Если нет данных
            if (!allDates.Any())
            {
                OrdersChartControl.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Нет данных для отображения",
                        Values = new ChartValues<int> { 0 }
                    }
                };
                return;
            }

            var newOrdersValues = new ChartValues<int>();
            var updatedOrdersValues = new ChartValues<int>();

            foreach (var date in allDates)
            {
                newOrdersValues.Add(newOrdersData.GetValueOrDefault(date, 0));
                updatedOrdersValues.Add(updatedOrdersData.GetValueOrDefault(date, 0));
            }

            // Создаем коллекцию серий
            var seriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Новые заказы",
                    Values = newOrdersValues,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = System.Windows.Media.Brushes.Blue
                },
                new LineSeries
                {
                    Title = "Измененные заказы",
                    Values = updatedOrdersValues,
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 8,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = System.Windows.Media.Brushes.Orange
                }
            };

            // Создаем ось X с метками дат
            var axisX = new Axis
            {
                Title = "Дата",
                Labels = allDates.Select(d => d.ToString("dd.MM.yyyy")).ToList(),
                Separator = new LiveCharts.Wpf.Separator
                {
                    StrokeThickness = 1,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection { 2, 2 }
                }
            };

            // Создаем ось Y
            var axisY = new Axis
            {
                Title = "Количество заказов",
                LabelFormatter = value => value.ToString("N0"),
                Separator = new LiveCharts.Wpf.Separator
                {
                    StrokeThickness = 1,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection { 2, 2 }
                }
            };

            // Применяем к графику
            OrdersChartControl.Series = seriesCollection;
            OrdersChartControl.AxisX.Clear();
            OrdersChartControl.AxisX.Add(axisX);
            OrdersChartControl.AxisY.Clear();
            OrdersChartControl.AxisY.Add(axisY);
        }
        private void OrderButton_OnClick(object? sender, RoutedEventArgs e)
        {
            App.MainWindow.MainContentControl.Content = new UCOrder();
        }
        private void ProductButton_OnClick(object? sender, RoutedEventArgs e)
        {
            App.MainWindow.MainContentControl.Content = new UCProduct(_context);
        }
        private void TestingButton_OnClick(object? sender, RoutedEventArgs e)
        {
            App.MainWindow.MainContentControl.Content = new UCTesting();
        }
        private void ReportButton_OnClick(object? sender, RoutedEventArgs e)
        {
            //App.MainWindow.MainContentControl.Content = new UCTesting();
        }
        private void SortButton_OnChecked(object? sender, RoutedEventArgs e)
        {
        }
        
    }
}
