using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using telecom_demo.Models;

namespace telecom_demo
{
    // Конвертер цветов для статусов
    public class StatusColorConverter : IValueConverter
    {
        private static readonly Dictionary<string, Color> ColorMap = new Dictionary<string, Color>
        {
            { "Новый", Color.FromRgb(66, 133, 244) },
            { "В работе", Color.FromRgb(219, 68, 55) },
            { "Выполнен", Color.FromRgb(15, 157, 88) },
            { "Отменен", Color.FromRgb(244, 180, 0) },
            { "Приостановлен", Color.FromRgb(155, 89, 182) },
            { "На проверке", Color.FromRgb(236, 112, 99) },
            { "Завершен", Color.FromRgb(0, 150, 136) },
            { "Архив", Color.FromRgb(149, 117, 205) },
            { "Без статуса", Colors.Gray }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status && status.NameStatus != null)
            {
                if (ColorMap.TryGetValue(status.NameStatus, out Color color))
                {
                    return new SolidColorBrush(color);
                }

                // Генерируем цвет на основе хеша
                int hash = Math.Abs(status.NameStatus.GetHashCode());
                byte r = (byte)((hash * 31) % 156 + 100);
                byte g = (byte)((hash * 47) % 156 + 100);
                byte b = (byte)((hash * 71) % 156 + 100);

                return new SolidColorBrush(Color.FromRgb(r, g, b));
            }

            return Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Конвертер для отображения текста на сегментах
    public class PercentageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double percentage && percentage >= 5.0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}