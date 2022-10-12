namespace EntertainmentHub.Web.Tests
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Microsoft.Net.Http.Headers;

    public class AntiForgeryTokenExtractor
    {
        public AntiForgeryTokenExtractor()
        {
            this.Field = "__RequestVerificationToken";
            this.Cookie = "AspNetCore.Antiforgery";
        }

        public string Field { get; set; }

        public string Cookie { get; set; }

        public string ExtractCookieValue(HttpResponseMessage response)
        {
            var antiForgery = response.Headers.GetValues("Set-Cookie");
            string antiForgeryCookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains(this.Cookie));

            if (antiForgeryCookie is null)
            {
                throw new ArgumentException($"Cookie '{this.Cookie}' not found in HTTP response", nameof(response));
            }

            string antiForgeryCookieValue = SetCookieHeaderValue.Parse(antiForgeryCookie).Value.ToString();

            return antiForgeryCookieValue;
        }

        public string ExtractAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch = Regex.Match(htmlBody, $@"\<input name=""{this.Field}"" type=""hidden"" value=""([^""]+)"" \/\>");

            if (requestVerificationTokenMatch.Success)
            {
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
            }

            throw new ArgumentException($"Anti forgery token '{this.Field}' not found", nameof(htmlBody));
        }

        public async Task<(string Field, string Cookie)> ExtractAntiForgeryValues(HttpResponseMessage response)
        {
            var cookie = this.ExtractCookieValue(response);
            var token = this.ExtractAntiForgeryToken(await response.Content.ReadAsStringAsync());

            return (token, cookie);
        }
    }
}
