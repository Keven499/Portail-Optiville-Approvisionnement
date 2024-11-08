using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Portail_OptiVille.Data.Utilities
{
    public interface ICookie
    {
        Task SetValue(string key, string value, int? hours = null);
        Task<string> GetValue(string key, string def = "");
        Task destroy(string name);
    }

    public class CookieService : ICookie
    {
        const int DefaultCookieDuration = 3; // 3 hours


        private readonly IJSRuntime _jsRuntime;

        public CookieService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetValue(string key, string value, int? hours = null)
        {
            var expires = (hours != null && hours > 0)
                ? $"expires={DateToUTC(hours.Value)}"
                : $"expires={DateToUTC(3)}";
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{key}={value}; {expires}; path=/\"");
        }

        private string DateToUTC(int hours)
        {
            var expireTime = DateTime.UtcNow.AddHours(hours);
            return expireTime.ToString("R"); // Format RFC1123
        }


        public async Task<string> GetValue(string key, string def = "")
        {
            var cookies = await GetCookie();
            var cookiePairs = cookies.Split(';')
                .Select(x => x.Trim())
                .Select(x => x.Split('='))
                .ToDictionary(x => x[0], x => x.Length > 1 ? x[1] : "");

            if (cookiePairs.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                return def;
            }
        }

        public async Task<string> GetCookie()
        {
            try
            {

                return await _jsRuntime.InvokeAsync<string>("eval", "document.cookie");
            }
            catch (InvalidOperationException)
            {
                return "";
            }
        }
        public async Task destroy(string name)
        {
            await SetValue(name, "", -1); // Supprime le cookie en fixant sa dur�e de vie � -1
        }

    }
}