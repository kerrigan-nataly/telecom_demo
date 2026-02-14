using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для UCOrder.xaml
    /// </summary>
    public partial class UCOrder : UserControl
    {
        private readonly AppDbContext _context;
        public ObservableCollection<StatusGroup> OrdersGroupedByStatus { get; set; }
        public UCOrder()
        {
            InitializeComponent();
            App.Controls.Add(this);
            _context = new AppDbContext();
            LoadOrdersGroupedByStatus();
            DataContext = this;
        }
        private void LoadOrdersGroupedByStatus()
        {
            var orders = _context.Orders
                .Include(o => o.Status)
                .Include(o => o.OrderComponents)
                    .ThenInclude(oc => oc.Component)
                .Include(o => o.OrderNodes)
                    .ThenInclude(on => on.Node)
                        .ThenInclude(n => n.NodeType)
                .Include(o => o.OrderNodes)
                    .ThenInclude(on => on.Node)
                        .ThenInclude(n => n.TestingResult)
                .ToList();

            // Группировка заказов по статусам
            var groupedOrders = orders
                .GroupBy(o => o.Status)
                .Select(g => new StatusGroup
                {
                    Status = g.Key,
                    Orders = new ObservableCollection<Order>(g.OrderByDescending(o => o.DeadlineDate))
                })
                .OrderBy(g => g.Status?.IdStatus) // Сортировка статусов по ID
                .ToList();
            OrdersGroupedByStatus = new ObservableCollection<StatusGroup>(groupedOrders);
        }
        private void NewButton_OnClick(object? sender, RoutedEventArgs e)
        {
            WNewOrder newwindow = new WNewOrder(_context);
            newwindow.ShowDialog();
        }
    }
    public class StatusGroup
    {
        public Status Status { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
    }
}
