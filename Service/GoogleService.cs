using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ecommerce_final.Service
{
    public class GoogleService
    {
        static async Task Main(string[] args)
        {
            string apiKey = "YOUR_API_KEY";
            string address = "Hanoi";
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
        }
    }
}
