using Microsoft.Extensions.Options;
using ModelLibrary;
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

        public async Task<KiemTraJWT> KiemTraDangNhap()
        {
            KiemTraJWT KetQua = new KiemTraJWT();
            var context = _httpContextAccessor.HttpContext;
            var accessToken = context.Request.Cookies["accessToken"];
            var refreshToken = context.Request.Cookies["refreshToken"];
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = null;

            switch (accessToken, refreshToken)
            {
                case (null, null):
                case (not null, null):
                    KetQua.success = false;
                    break;

                case (null, not null):
                    KetQua = await CapLaiAccessToken();
                    break;

                case (not null, not null):
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    response = await client.GetAsync(_apiSettings.Url + "/tai-khoan/thong-tin-nguoi-dung");

                    if (response.IsSuccessStatusCode)
                    {
                        KetQua.success = true;
                        KetQua.accessToken = accessToken;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        var caplai = await CapLaiAccessToken();

                        if (caplai.success)
                        {
                            KetQua.success = true;
                            KetQua.accessToken = caplai.accessToken;
                        }
                        else
                        {
                            KetQua.success = false;
                            context.Response.Cookies.Delete("accessToken");
                            context.Response.Cookies.Delete("refreshToken");
                        }
                    }
                    else
                    {
                        KetQua.success = false;
                    }
                    break;
            }

            return KetQua;
        }

        public async Task<KiemTraJWT> CapLaiAccessToken()
        {
            KiemTraJWT KetQua = new KiemTraJWT();
            var context = _httpContextAccessor.HttpContext;
            var refreshToken = context.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                KetQua.success = false;
                return KetQua;
            }

            var client = _httpClientFactory.CreateClient();

            var payload = new { refreshToken };
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiSettings.Url + "/tai-khoan/cap-lai-access-token", content);

            if (!response.IsSuccessStatusCode)
            {
                KetQua.success = false;
                return KetQua;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<Request<jwtTokens>>(responseBody);

            if (result == null || result.success != true)
            {
                KetQua.success = false;
                return KetQua;
            }

            context.Response.Cookies.Append("accessToken", result.data.accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddMinutes(15)
            });

            KetQua.success = true;
            KetQua.accessToken = result.data.accessToken;
            return KetQua;
        }
    }
}
