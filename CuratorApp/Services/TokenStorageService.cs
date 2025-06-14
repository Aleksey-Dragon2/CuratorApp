using CuratorApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace CuratorApp.Services
{
    public class TokenStorageService
    {
        private const string TokenFileName = "tokens.json";

        public void SaveTokens(Tokens tokens)
        {
            var json = JsonSerializer.Serialize(tokens);
            File.WriteAllText(TokenFileName, json);
        }

        public Tokens? LoadTokens()
        {
            if (!File.Exists(TokenFileName))
                return null;

            try
            {
                var json = File.ReadAllText(TokenFileName);
                return JsonSerializer.Deserialize<Tokens>(json);
            }
            catch
            {
                return null;
            }
        }

        public void ClearTokens()
        {
            if (File.Exists(TokenFileName))
                File.Delete(TokenFileName);
        }
    }
}
