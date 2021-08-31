using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GetCurrentWeather
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            string city = null;
            string units = "metric";
            for(int i =0;i<args.Length;i++)
            {
                switch (args[i])
                {
                    case "-c":
                    case "--city":
                        i++;
                        if(i>=args.Length || args[i].StartsWith('-'))
                            return 1;
                        city = args[i];
                        break;
                    case "-u":
                    case "--units":
                        i++;
                        if(i>=args.Length || args[i].StartsWith('-'))
                            return 1;
                        units = args[i];
                        break;
                    default:
                        break;
                }
            }

            if(string.IsNullOrEmpty(city))
            {
                return 2;
            }

            string api = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");

            HttpClient client = new HttpClient();
            var response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={api}&units={units}");
            if(response.IsSuccessStatusCode)
            {
                var json =  await response.Content.ReadAsStringAsync();
                dynamic parsedJson = JsonConvert.DeserializeObject(json);
                int apiStatusCode = parsedJson.cod;
                if(apiStatusCode < 200 || apiStatusCode > 299)
                    return 3;
                string name = parsedJson.name;
                string temp = parsedJson.main.temp;
                string humidity = parsedJson.main.humidity;
                string pressure = parsedJson.main.pressure;
                string windSpeed = parsedJson.wind.speed;
                Console.WriteLine($"{name}|{temp}|{windSpeed}|{humidity}|{pressure}");
                return 0;
            }
            return 4;
        }
    }
}
