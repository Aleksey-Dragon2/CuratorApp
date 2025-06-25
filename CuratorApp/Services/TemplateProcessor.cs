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
        public string GenerateReport(string templatePath, Dictionary<string, string> replacements, string outputFileName, string groupName, string? studentFolder = null)
        {
            string fullTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath);

            if (!File.Exists(fullTemplatePath))
                throw new FileNotFoundException("Шаблон не найден", fullTemplatePath);

            // Базовая директория отчётов
            string baseReportsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");

            // Директория группы — по названию группы
            string groupDir = Path.Combine(baseReportsDir, groupName);

            // Если студент указан — папка внутри группы
            string finalDir = studentFolder != null
                ? Path.Combine(groupDir, studentFolder)
                : groupDir;

            Directory.CreateDirectory(finalDir);

            string outputPath = Path.Combine(finalDir, outputFileName);

            // Копируем шаблон в новый файл
            File.Copy(fullTemplatePath, outputPath, true);

            // Открываем и заменяем ключи
            using var wordDoc = WordprocessingDocument.Open(outputPath, true);
            var body = wordDoc.MainDocumentPart?.Document.Body;

            if (body != null)
            {
                foreach (var pair in replacements)
                {
                    ReplacePlaceholder(body, pair.Key, pair.Value);
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
                    {
                        allTexts[k].Text.Text = "";
                    }
                }
            }
        }
    }
}
