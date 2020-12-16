using System.Net.Http;
using System.Text;
using System.Web;

namespace WorkerService
{
    public static class WebHookSender
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static HttpResponseMessage SendTo(string webHookUrl, string content)
        {
            var textContent = $"{{\"text\": \"{HttpUtility.JavaScriptStringEncode(content)}\"}}";
            var httpContent = new StringContent(textContent, Encoding.UTF8, "application/json");
            var httpResponseMessage = HttpClient.PostAsync(webHookUrl, httpContent).Result;
            return httpResponseMessage;
        }
    }
}