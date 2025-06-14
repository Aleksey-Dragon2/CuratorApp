using CuratorApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CuratorApp.Helpers
{
    public class AuthorizedHttpClient
    {
        private readonly HttpClient _client;
        private readonly TokenStorageService _tokenStorage;

        public AuthorizedHttpClient()
        {
            _client = new HttpClient();
            _tokenStorage = new TokenStorageService();
        }

        public HttpClient GetClient()
        {
            var tokens = _tokenStorage.LoadTokens();
            _client.DefaultRequestHeaders.Clear();

            if (tokens != null && tokens.AccessTokenExpiration > DateTime.UtcNow)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            }

            return _client;
        }
    }
}
