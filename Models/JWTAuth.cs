using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Crawler.Models
{
    public class JWTAuth
    {
        public string WpUri { get; set; }
        public string Route { get; set; }
        public string Token { get; set; }
        public HttpClient Client { get; set; }
        public JWTUser User { get; set; }
        public JWTAuth(string baseUri) { WpUri = baseUri; Client = new HttpClient(); User = new JWTUser(); }
        public async Task<HttpResponseMessage> RequestJWToken()
        {
            if (WpUri == "" || WpUri == string.Empty)
            {
                throw new Exception(nameof(WpUri));
            }
            if (User == null)
            {
                throw new Exception(nameof(JWTUser));
            }
            Client = new HttpClient();
            Route = new string("/jwt-auth/v1/token");
            var defaultUri = WpUri + Route;
            var json = JsonConvert.SerializeObject(User);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(defaultUri, content);
            try
            {
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (System.Exception)
            {

                throw new Exception(nameof(response));
            }
        }
        public async Task<bool> IsValidJWToken()
        {
            if (Token == "")
            {
                throw new Exception(nameof(Token));
            }
            Route = new string("/jwt-auth/v1/token/validate");
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            var response = await Client.PostAsync(WpUri + Route, null);
            var result = response.Content.ReadAsStringAsync().Result;
            try
            {
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public async Task<HttpResponseMessage> PostAsync(PostCreateModel p)
        {
            Route = new string("/wp/v2/posts");
            Client = new HttpClient();
            var json = JsonConvert.SerializeObject(p);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            var response = await Client.PostAsync(WpUri + Route, content);
            try
            {
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (System.Exception)
            {

                throw new Exception(nameof(response));
            }
        }
    }
    public class JWTUser
    {
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
    public class JWTResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("user_email")]
        public string Email { get; set; }
        [JsonProperty("user_nicename")]
        public string NiceName { get; set; }
        [JsonProperty("user_display_name")]
        public string DisplayName { get; set; }
    }
}