using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using telecomdemo2.Data;

namespace telecomdemo2
{
    public static class ComponentIndexGenerator
    {
        /// <summary>
        /// Генерирует следующий индекс компонента в формате TYPE-00001
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="componentTypeId">ID типа компонента</param>
        /// <returns>Новый индекс в формате TYPE-00001</returns>
        public static string GenerateNextIndex(AppDbContext context, int componentTypeId)
        {
            // Получаем префикс типа компонента
            string typePrefix = GetComponentTypePrefix(context, componentTypeId);

            // Получаем все существующие индексы компонентов данного типа
            var existingIndexes = context.Components
                .Where(c => c.ComponentTypeId == componentTypeId && c.IdComponent != null)
                .Select(c => c.IdComponent)
                .ToList();

            // Находим максимальный номер
            int maxNumber = 0;
            string pattern = $@"^{typePrefix}-(\d+)$";

            foreach (var index in existingIndexes)
            {
                var match = Regex.Match(index, pattern);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int number))
                    {
                        if (number > maxNumber)
                        {
                            maxNumber = number;
                        }
                    }
                }
            }

            // Увеличиваем номер на 1
            int nextNumber = maxNumber + 1;

            // Форматируем с ведущими нулями (5 цифр)
            string formattedNumber = nextNumber.ToString("D5");

            return $"{typePrefix}-{formattedNumber}";
        }

        /// <summary>
        /// Генерирует следующий индекс с возможностью указать количество цифр
        /// </summary>
        public static string GenerateNextIndex(AppDbContext context, int componentTypeId, int digitsCount = 5)
        {
            string typePrefix = GetComponentTypePrefix(context, componentTypeId);

            var existingIndexes = context.Components
                .Where(c => c.ComponentTypeId == componentTypeId && c.IdComponent != null)
                .Select(c => c.IdComponent)
                .ToList();

            int maxNumber = 0;
            string pattern = $@"^{typePrefix}-(\d+)$";

            foreach (var index in existingIndexes)
            {
                var match = Regex.Match(index, pattern);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int number))
                    {
                        if (number > maxNumber)
                        {
                            maxNumber = number;
                        }
                    }
                }
            }

            int nextNumber = maxNumber + 1;

            // Форматирование с динамическим количеством цифр
            string formattedNumber = nextNumber.ToString($"D{digitsCount}");

            return $"{typePrefix}-{formattedNumber}";
        }

        /// <summary>
        /// Получает префикс для типа компонента
        /// </summary>
        private static string GetComponentTypePrefix(AppDbContext context, int componentTypeId)
        {
            var componentType = context.ComponentTypes
                .FirstOrDefault(ct => ct.IdComponentType == componentTypeId);

            if (componentType == null || string.IsNullOrWhiteSpace(componentType.NameComponentType))
            {
                throw new InvalidOperationException("Тип компонента не найден или не имеет названия");
            }

            // Берем первую букву каждого слова в верхнем регистре
            string[] words = componentType.NameComponentType.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                // Если одно слово, берем первые 3 буквы в верхнем регистре
                string word = words[0].ToUpper();
                return word.Length >= 3 ? word.Substring(0, 3) : word.PadRight(3, 'X');
            }
            else
            {
                // Если несколько слов, берем первые буквы
                string prefix = "";
                foreach (var word in words)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        prefix += word[0].ToString().ToUpper();
                    }
                }
                // Дополняем до 3 символов если нужно
                while (prefix.Length < 3)
                {
                    prefix += "X";
                }
                return prefix.Length > 3 ? prefix.Substring(0, 3) : prefix;
            }
        }

        /// <summary>
        /// Валидирует формат индекса компонента
        /// </summary>
        public static bool IsValidComponentIndex(string index)
        {
            if (string.IsNullOrEmpty(index))
                return false;

            // Проверяем формат: XXX-00000 (где X - буквы, 0 - цифры)
            return Regex.IsMatch(index, @"^[A-Z]{3}-\d{5}$");
        }

        /// <summary>
        /// Получает номер из индекса
        /// </summary>
        public static int? GetNumberFromIndex(string index)
        {
            if (string.IsNullOrEmpty(index))
                return null;

            var match = Regex.Match(index, @"-(\d+)$");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int number))
            {
                return number;
            }
            return null;
        }
    }
}
