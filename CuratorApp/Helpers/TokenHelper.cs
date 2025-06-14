using CuratorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorApp.Helpers
{
    public static class TokenHelper
    {
        public static bool IsAccessTokenValid(Tokens tokens)
        {
            return tokens != null && tokens.AccessTokenExpiration > DateTime.UtcNow;
        }

        public static bool IsRefreshTokenValid(Tokens tokens)
        {
            return tokens != null && tokens.RefreshTokenExpiration > DateTime.UtcNow;
        }
    }
}
