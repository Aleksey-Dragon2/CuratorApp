using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CuratorApp.Services
{
    public class TemplateProcessor
    {
        /// <summary>
        /// Генерирует отчёт, подставляя значения в шаблон и сохраняя в нужной папке.
        /// </summary>
        /// <param name="templatePath">Путь к шаблону относительно каталога приложения</param>
        /// <param name="replacements">Словарь замен плейсхолдеров на значения</param>
        /// <param name="outputFileName">Имя итогового файла</param>
        /// <param name="groupName">Имя группы (создаётся папка)</param>
        /// <param name="studentFolder">Если отчёт по студенту — имя поддиректории</param>
        /// <param name="tableRows">Данные для табличной части (если есть)</param>
        /// <returns>Путь сгенерированного файла</returns>
        public string GenerateReport(
            string templatePath,
            Dictionary<string, string> replacements,
            string outputFileName,
            string groupName,
            string? studentFolder = null,
            List<Dictionary<string, string>>? tableRows = null)
        {
            string fullTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath);

            if (!File.Exists(fullTemplatePath))
                throw new FileNotFoundException("Шаблон не найден", fullTemplatePath);

            string baseReportsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            string groupDir = Path.Combine(baseReportsDir, groupName);
            string finalDir = studentFolder != null
                ? Path.Combine(groupDir, studentFolder)
                : groupDir;

            Directory.CreateDirectory(finalDir);

            string outputPath = Path.Combine(finalDir, outputFileName);

            File.Copy(fullTemplatePath, outputPath, true);

            using var wordDoc = WordprocessingDocument.Open(outputPath, true);
            var body = wordDoc.MainDocumentPart?.Document.Body;

            if (body != null)
            {
                ReplacePlaceholders(body, replacements);

                if (tableRows != null && tableRows.Count > 0)
                {
                    ReplaceTableRows(body, tableRows);
                }

                wordDoc.MainDocumentPart.Document.Save();
            }

            return outputPath;
        }

        /// <summary>
        /// Заменяет все вхождения плейсхолдеров на заданные значения в тексте документа.
        /// Работает с плейсхолдерами, которые могут быть разбиты по нескольким Run/текстам.
        /// </summary>
        private void ReplacePlaceholders(Body body, Dictionary<string, string> replacements)
        {
            // Собираем все Run и связанные с ними Text элементы
            var runs = body.Descendants<Run>().ToList();
            var allTexts = runs.SelectMany(r => r.Elements<Text>().Select(t => new { Run = r, Text = t })).ToList();

            foreach (var placeholder in replacements.Keys)
            {
                string replacement = replacements[placeholder];

                int plLength = placeholder.Length;

                for (int i = 0; i < allTexts.Count; i++)
                {
                    int j = i;
                    string accum = "";

                    // Накапливаем текст из последовательных Text элементов для проверки совпадения плейсхолдера
                    while (j < allTexts.Count && accum.Length < plLength)
                    {
                        accum += allTexts[j].Text.Text;
                        j++;
                    }

                    if (accum == placeholder)
                    {
                        // Заменяем текст первого Text элемента
                        allTexts[i].Text.Text = replacement;

                        // Очищаем остальные Text элементы, входящие в плейсхолдер
                        for (int k = i + 1; k < j; k++)
                        {
                            allTexts[k].Text.Text = "";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Заменяет строку-шаблон в таблице на набор строк с данными.
        /// </summary>
        private void ReplaceTableRows(Body body, List<Dictionary<string, string>> tableRows)
        {
            var table = body.Elements<Table>()
                .FirstOrDefault(t => t.InnerText.Contains("[ФИО]")); // Ищем таблицу по наличию ключа

            if (table == null)
                return;

            var templateRow = table.Elements<TableRow>()
                .FirstOrDefault(r => r.InnerText.Contains("[ФИО]")); // Строка-шаблон с плейсхолдерами

            if (templateRow == null)
                return;

            foreach (var rowData in tableRows)
            {
                var newRow = (TableRow)templateRow.CloneNode(true);
                foreach (var cell in newRow.Elements<TableCell>())
                {
                    foreach (var text in cell.Descendants<Text>())
                    {
                        string originalText = text.Text;
                        foreach (var pair in rowData)
                        {
                            if (originalText.Contains(pair.Key))
                            {
                                text.Text = originalText.Replace(pair.Key, pair.Value);
                                originalText = text.Text; // Обновляем, чтобы заменить несколько ключей в одном тексте
                            }
                        }
                    }
                }
                table.AppendChild(newRow);
            }

            // Удаляем строку-шаблон
            table.RemoveChild(templateRow);
        }
    }
}
