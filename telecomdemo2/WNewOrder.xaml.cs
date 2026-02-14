using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using telecomdemo2.Data;
using telecomdemo2.Models;

namespace telecomdemo2
{
    public partial class WNewOrder : Window
    {
        private AppDbContext _context;
        private Order _currentOrder;

        // Списки для хранения данных
        public ObservableCollection<Node> AvailableNodes { get; set; }
        public ObservableCollection<OrderNode> SelectedNodes { get; set; }
        public ObservableCollection<ComponentRequirement> RequiredComponents { get; set; }

        public class ComponentRequirement
        {
            public Component Component { get; set; }
            public string NodeTypeName { get; set; }
            public int TotalComponents { get; set; }
        }

        public WNewOrder(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            _currentOrder = new Order();

            // Инициализация списков
            AvailableNodes = new ObservableCollection<Node>();
            SelectedNodes = new ObservableCollection<OrderNode>();
            RequiredComponents = new ObservableCollection<ComponentRequirement>();

            LoadData();

            // Привязка данных
            lstAvailableNodes.ItemsSource = AvailableNodes;
            lstOrderNodes.ItemsSource = SelectedNodes;
            dgRequiredComponents.ItemsSource = RequiredComponents;

            SelectedNodes.CollectionChanged += SelectedNodes_CollectionChanged;
        }

        private void SelectedNodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateRequiredComponents();
            UpdateSummary();
        }

        private void LoadData()
        {
            try
            {
                // Загрузка статусов
                cmbStatus.ItemsSource = _context.Statuses.ToList();
                if (cmbStatus.Items.Count > 0)
                    cmbStatus.SelectedIndex = 0;

                // Загрузка доступных узлов с включением связанных данных
                var nodes = _context.Nodes
                    .Include(n => n.NodeType)
                    .ThenInclude(nt => nt.NodeTypeComponents)
                    .ThenInclude(ntc => ntc.Component)
                    .ToList();

                foreach (var node in nodes)
                {
                    AvailableNodes.Add(node);
                }

                // Установка текущей даты по умолчанию
                dpStartDate.SelectedDate = DateTime.Today;
                dpDeadlineDate.SelectedDate = DateTime.Today.AddDays(7);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик для начала перетаскивания
        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null)
            {
                var node = listBoxItem.DataContext as Node;
                if (node != null)
                {
                    DataObject dataObject = new DataObject();
                    dataObject.SetData("Node", node);
                    DragDrop.DoDragDrop(listBoxItem, dataObject, DragDropEffects.Copy);
                }
            }
        }

        // Обработчик DragOver
        private void DropZone_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Node"))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        // Дроп-зона для узлов
        private void NodeDropZone_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Node"))
            {
                var node = e.Data.GetData("Node") as Node;
                if (node != null)
                {
                    AddNodeToOrder(node);
                }
            }
        }

        // Добавление узла в заказ
        private void AddNodeToOrder(Node node)
        {
            // Проверяем, не добавлен ли уже такой узел
            var existingNode = SelectedNodes.FirstOrDefault(on => on.NodeId == node.IdNode);
            if (existingNode != null)
            {
                existingNode.NodeCount++;
            }
            else
            {
                var orderNode = new OrderNode
                {
                    NodeId = node.IdNode,
                    Node = node,
                    NodeCount = 1
                };
                SelectedNodes.Add(orderNode);
            }

            // Обновляем отображение
            lstOrderNodes.Items.Refresh();
            CalculateRequiredComponents();
            UpdateSummary();
        }

        // Удаление узла
        private void RemoveNode_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderNode = button?.Tag as OrderNode;

            if (orderNode != null)
            {
                SelectedNodes.Remove(orderNode);
                CalculateRequiredComponents();
                UpdateSummary();
            }
        }

        // Изменение количества узлов
        private void NodeCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var orderNode = textBox.DataContext as OrderNode;
                if (orderNode != null)
                {
                    if (int.TryParse(textBox.Text, out int count) && count > 0)
                    {
                        orderNode.NodeCount = count;
                    }
                    else
                    {
                        orderNode.NodeCount = 1;
                        textBox.Text = "1";
                    }

                    CalculateRequiredComponents();
                    UpdateSummary();
                }
            }
        }

        // Расчет необходимых компонентов
        private void CalculateRequiredComponents()
        {
            var requirements = new Dictionary<string, ComponentRequirement>();

            foreach (var orderNode in SelectedNodes)
            {
                var node = orderNode.Node;
                var nodeType = node.NodeType;
                var nodeCount = orderNode.NodeCount ?? 1;

                if (nodeType?.NodeTypeComponents != null)
                {
                    foreach (var ntc in nodeType.NodeTypeComponents)
                    {
                        if (ntc.Component != null)
                        {
                            string key = ntc.Component.IdComponent;
                            var componentCount = ntc.ComponentCount ?? 1;

                            if (!requirements.ContainsKey(key))
                            {
                                requirements[key] = new ComponentRequirement
                                {
                                    Component = ntc.Component,
                                    NodeTypeName = nodeType.NameNodeType,
                                    TotalComponents = 0
                                };
                            }

                            requirements[key].TotalComponents += componentCount * nodeCount;
                        }
                    }
                }
            }

            RequiredComponents.Clear();
            foreach (var req in requirements.Values.OrderBy(r => r.Component.NameComponent))
            {
                RequiredComponents.Add(req);
            }
        }

        // Обновление сводки
        private void UpdateSummary()
        {
            int totalNodes = SelectedNodes.Sum(on => on.NodeCount ?? 1);
            txtTotalNodes.Text = totalNodes.ToString();

            int totalComponents = RequiredComponents.Sum(r => r.TotalComponents);
            txtTotalComponents.Text = totalComponents.ToString();
        }

        // Сохранение заказа
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(txtNameOrder.Text))
                {
                    MessageBox.Show("Введите название заказа",
                                  "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус заказа",
                                  "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!SelectedNodes.Any())
                {
                    MessageBox.Show("Добавьте хотя бы один узел в заказ",
                                  "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Заполнение данных заказа
                _currentOrder.NameOrder = txtNameOrder.Text;
                _currentOrder.StatusId = (int)cmbStatus.SelectedValue;

                if (dpStartDate.SelectedDate.HasValue)
                    _currentOrder.StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value);

                if (dpDeadlineDate.SelectedDate.HasValue)
                    _currentOrder.DeadlineDate = DateOnly.FromDateTime(dpDeadlineDate.SelectedDate.Value);

                // Сохранение в базу данных
                _context.Orders.Add(_currentOrder);
                _context.SaveChanges(); // Сохраняем заказ, чтобы получить ID

                // Добавление узлов
                foreach (var orderNode in SelectedNodes)
                {
                    orderNode.OrderId = _currentOrder.IdOrder;
                    _context.OrderNodes.Add(orderNode);
                }

                // Группировка компонентов по ComponentId и суммирование количества
                var componentGroups = RequiredComponents
                    .GroupBy(r => r.Component.IdComponent)
                    .Select(g => new
                    {
                        ComponentId = g.Key,
                        TotalCount = g.Sum(r => r.TotalComponents)
                    });

                // Создание записей о компонентах с учетом количества
                foreach (var group in componentGroups)
                {
                    var orderComponent = new OrderComponent
                    {
                        OrderId = _currentOrder.IdOrder,
                        ComponentId = group.ComponentId,
                        ComponentCount = group.TotalCount
                    };
                    _context.OrderComponents.Add(orderComponent);
                }

                _context.SaveChanges();

                MessageBox.Show($"Заказ успешно создан!\n\n" +
                              $"Узлов: {txtTotalNodes.Text}\n" +
                              $"Компонентов: {txtTotalComponents.Text}",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}\n\n{ex.InnerException?.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}