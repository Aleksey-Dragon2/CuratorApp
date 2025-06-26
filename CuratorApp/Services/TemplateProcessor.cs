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
                foreach (var pair in replacements)
                {
                    ReplacePlaceholder(body, pair.Key, pair.Value);
                }

                if (tableRows != null && tableRows.Count > 0)
                {
                    ReplaceTableRows(body, tableRows);
                }

                wordDoc.MainDocumentPart.Document.Save();
            }

            return outputPath;
        }

        private void ReplacePlaceholder(Body body, string placeholder, string replacement)
        {
            var runs = body.Descendants<Run>().ToList();
            var allTexts = runs.SelectMany(r => r.Elements<Text>().Select(t => new { Run = r, Text = t })).ToList();

            int plLength = placeholder.Length;

            for (int i = 0; i < allTexts.Count; i++)
            {
                int j = i;
                string accum = "";
                while (j < allTexts.Count && accum.Length < plLength)
                {
                    accum += allTexts[j].Text.Text;
                    j++;
                }

                if (accum == placeholder)
                {
                    allTexts[i].Text.Text = replacement;
                    for (int k = i + 1; k < j; k++)
                        allTexts[k].Text.Text = "";
                }
            }
        }

        private void ReplaceTableRows(Body body, List<Dictionary<string, string>> tableRows)
        {
            var table = body.Elements<Table>()
                .FirstOrDefault(t => t.InnerText.Contains("[ФИО]")); // Найти таблицу с шаблонными ключами

            if (table == null)
                return;

            var templateRow = table.Elements<TableRow>()
                .FirstOrDefault(r => r.InnerText.Contains("[ФИО]")); // Строка-шаблон

            if (templateRow == null)
                return;

            var parentTable = templateRow.Parent;
            foreach (var rowData in tableRows)
            {
                var newRow = (TableRow)templateRow.CloneNode(true);
                foreach (var cell in newRow.Elements<TableCell>())
                {
                    foreach (var text in cell.Descendants<Text>())
                    {
                        string original = text.Text;
                        foreach (var pair in rowData)
                        {
                            if (original.Contains(pair.Key))
                                text.Text = original.Replace(pair.Key, pair.Value);
                        }
                    }
                }

                table.AppendChild(newRow);
            }

            table.RemoveChild(templateRow); // Удаляем строку-шаблон
        }
    }
}
