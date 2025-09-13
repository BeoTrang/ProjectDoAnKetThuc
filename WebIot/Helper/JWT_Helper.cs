using Microsoft.Extensions.Options;
using WebIot.Models;

namespace WebIot.Helper
{
    public class JWT_Helper
    {
        private readonly ApiSettings _apiSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JWT_Helper(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> KiemTraDangNhap()
        {
            var context = _httpContextAccessor.HttpContext;
            var accessToken = context.Request.Cookies["accessToken"];
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(_apiSettings.Url + "/lay-thong-tin-nguoi-dung");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        var capLai = await CapLaiAccessToken();
                        if (capLai)
                        {
                            return true;
                        }
                        else
                        {
                            context.Response.Cookies.Delete("accessToken");
                            context.Response.Cookies.Delete("refreshToken");
                            return false;
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(refreshToken))
            {
                var client = _httpClientFactory.CreateClient();
                var payload = new
                {
                    refreshToken = refreshToken
                };
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_apiSettings.Url + "/cap-lai-access-token", content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<PhanHoiApi<JWT>>(responseBody);
                if (result.success == false || result.success == null)
                {
                    return false;
                }
                else
                {
                    context.Response.Cookies.Append("accessToken", result.data.accessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.Now.AddMinutes(15)
                    });
                    return true;
                }
            }
            context.Response.Cookies.Delete("accessToken");
            context.Response.Cookies.Delete("refreshToken");
            return false;
        }
        public async Task<bool> CapLaiAccessToken()
        {
            var context = _httpContextAccessor.HttpContext;
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }
            else
            {
                var client = _httpClientFactory.CreateClient();
                var payload = new
                {
                    refreshToken = refreshToken
                };
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_apiSettings.Url + "/cap-lai-access-token", content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<PhanHoiApi<JWT>>(responseBody);
                if (result.success == false || result.success == null)
                {
                    return false;
                }
                else
                {
                    context.Response.Cookies.Append("accessToken", result.data.accessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.Now.AddMinutes(15)
                    });
                    return true;
                }
            }
        }
    }
}
