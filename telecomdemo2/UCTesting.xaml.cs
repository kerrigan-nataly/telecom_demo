using Microsoft.EntityFrameworkCore;
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
using telecomdemo2.Data;
using telecomdemo2.Models;

namespace telecomdemo2
{
    /// <summary>
    /// Логика взаимодействия для UCTesting.xaml
    /// </summary>
    public partial class UCTesting : UserControl
    {
        private readonly AppDbContext _context;
        private ObservableCollection<NodeDefectSummary> _defectSummaries;
        private ObservableCollection<Node> _defectiveNodes;
        private string _selectedDefectType;

        public event PropertyChangedEventHandler? PropertyChanged;
        public UCTesting()
        {
            _context = new AppDbContext();
            InitializeComponent();
            App.Controls.Add(this);
            InitializeComponent();
            LoadData();
        }
        private void NewButton_OnClick(object? sender, RoutedEventArgs e)
        {
            WNewTesting newwindow = new WNewTesting(_context);
            newwindow.ShowDialog();
        }

        public ObservableCollection<NodeDefectSummary> DefectSummaries
        {
            get => _defectSummaries;
            set
            {
                _defectSummaries = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefectSummaries)));
            }
        }

        public ObservableCollection<Node> DefectiveNodes
        {
            get => _defectiveNodes;
            set
            {
                _defectiveNodes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefectiveNodes)));
            }
        }

        public string SelectedDefectType
        {
            get => _selectedDefectType;
            set
            {
                _selectedDefectType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDefectType)));
                LoadDefectiveNodes(value);
            }
        }

        private void LoadData()
        {
            // Сначала загружаем все необходимые данные
            var nodes = _context.Nodes
                .Include(n => n.NodeType)
                .Include(n => n.TestingResult)
                .ToList();

            // Затем выполняем группировку и агрегацию в памяти
            var summaries = nodes
                .Where(n => n.TestingResult != null &&
                           n.TestingResult.NameTestingResult != "исправен")
                .GroupBy(n => new {
                    NodeTypeName = n.NodeType?.NameNodeType ?? "Неизвестно",
                    DefectType = n.TestingResult?.NameTestingResult ?? "Неизвестно"
                })
                .Select(g => new NodeDefectSummary
                {
                    NodeTypeName = g.Key.NodeTypeName,
                    DefectType = g.Key.DefectType,
                    Count = g.Count(),
                    Nodes = g.ToList()
                })
                .OrderBy(s => s.NodeTypeName)
                .ThenBy(s => s.DefectType)
                .ToList();

            DefectSummaries = new ObservableCollection<NodeDefectSummary>(summaries);

            // Инициализируем коллекцию дефектных узлов пустой
            DefectiveNodes = new ObservableCollection<Node>();
        }

        private void LoadDefectiveNodes(string defectType)
        {
            if (string.IsNullOrEmpty(defectType))
            {
                DefectiveNodes.Clear();
                return;
            }

            // Сначала загружаем данные с Include
            var nodes = _context.Nodes
                .Include(n => n.NodeType)
                .Include(n => n.TestingResult)
                .Where(n => n.TestingResult != null &&
                           n.TestingResult.NameTestingResult == defectType)
                .ToList(); // Сначала выполняем запрос

            // Затем сортируем в памяти
            nodes = nodes
                .OrderBy(n => n.NodeType?.NameNodeType ?? string.Empty)
                .ThenBy(n => n.NameNode ?? string.Empty)
                .ToList();

            DefectiveNodes = new ObservableCollection<Node>(nodes);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void DataGridRow_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.DataContext is NodeDefectSummary summary)
            {
                SelectedDefectType = summary.DefectType;
            }
        }
    }
    public class NodeDefectSummary
    {
        public string NodeTypeName { get; set; }
        public string DefectType { get; set; }
        public int Count { get; set; }
        public List<Node> Nodes { get; set; }
    }
}
