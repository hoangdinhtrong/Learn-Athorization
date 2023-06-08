using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var claims = User.Claims;
            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var result = await GetSecret(accessToken ?? "");
            await RefreshAccessToken();
            return View();
        }

        [Authorize]
        public IActionResult Logout()
        {
            return SignOut("Cookie", "oidc");
        }

        public async Task<string> GetSecret(string access_token)
        {
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(access_token);
            var res = await apiClient.GetAsync("https://localhost:7174/secret");
            var content = await res.Content.ReadAsStringAsync();
            return content;
        }

        private async Task RefreshAccessToken()
        {
            var serverClient = _httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:7028/");

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var refreshTokenClient = _httpClientFactory.CreateClient();

            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest()
                {
                    Address = discoveryDocument.TokenEndpoint,
                    RefreshToken = refreshToken ?? "",
                    ClientId = "client_id_mvc",
                    ClientSecret = "client_secret_mvc"
                });

            var authInfo = await HttpContext.AuthenticateAsync("Cookie");
            if (authInfo != null && tokenResponse != null)
            {
                var claims = authInfo.Principal ?? new System.Security.Claims.ClaimsPrincipal();
                authInfo.Properties?.UpdateTokenValue("access_token", tokenResponse.AccessToken ?? "");
                authInfo.Properties?.UpdateTokenValue("id_token", tokenResponse.IdentityToken ?? "");
                authInfo.Properties?.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken ?? "");

                await HttpContext.SignInAsync("Cookie", claims, authInfo.Properties);
            }
        }
    }
}