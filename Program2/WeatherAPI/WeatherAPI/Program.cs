using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace WeatherAPI
{
    class Program
    {
        static void Main(string[] args)
        {

            const string APPID = "2deb8043802d3bfcd926b0f771b18bf8";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/");
                Console.Write("Type city name: ");
                string city = Console.ReadLine();
                var response = client.GetAsync(string.Format("weather?q={0}&appid={1}", city, APPID)).Result;
                NewMethod(response);
                string result = response.Content.ReadAsStringAsync().Result;

                Rootobject weatherDetails = JsonConvert.DeserializeObject<Rootobject>(result);

                string nation = weatherDetails.sys.country;
                Console.WriteLine("Country: {0}", nation);

                string detail = weatherDetails.weather[0].description;
                Console.WriteLine("Description: {0}", detail);

                float kelvinTemp = weatherDetails.main.temp;
                int temp = (int)((kelvinTemp - 273.15) * 1.8 + 32);
                Console.WriteLine("Current Tempature: {0}°F", temp);
                float min_Temp = weatherDetails.main.temp_min;
                int minTemp = (int)((min_Temp - 273.15) * 1.8 + 32);
                Console.WriteLine("Minimum Tempature: {0}°F", minTemp);
                float max_Temp = weatherDetails.main.temp_max;
                int maxTemp = (int)((max_Temp - 273.15) * 1.8 + 32);
                Console.WriteLine("Maximum Tempature: {0}°F", maxTemp);
                int hpa = weatherDetails.main.pressure;
                Console.WriteLine("Pressure: {0}hpa", hpa);
                int rh = weatherDetails.main.humidity;
                Console.WriteLine("Humidity: {0}%", rh);

                float speedOfWind = weatherDetails.wind.speed;
                float speedWind = speedOfWind * (float)2.23693629;
                Console.WriteLine("Wind : {0}mi/h", speedWind);

                int cloud = weatherDetails.clouds.all;
                Console.WriteLine("Cloud: {0}%", cloud);
            }
        }

        private static void NewMethod(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
        }
    }
}

public class Rootobject
{
    public Coord coord { get; set; }
    public Weather[] weather { get; set; }
    public string _base { get; set; }
    public Main main { get; set; }
    public int visibility { get; set; }
    public Wind wind { get; set; }
    public Clouds clouds { get; set; }
    public int dt { get; set; }
    public Sys sys { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int cod { get; set; }
}

public class Coord
{
    public float lon { get; set; }
    public float lat { get; set; }
}

public class Main
{
    public float temp { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
    public float temp_min { get; set; }
    public float temp_max { get; set; }
}

public class Wind
{
    public float speed { get; set; }
    public int deg { get; set; }
}

public class Clouds
{
    public int all { get; set; }
}

public class Sys
{
    public int type { get; set; }
    public int id { get; set; }
    public float message { get; set; }
    public string country { get; set; }
    public int sunrise { get; set; }
    public int sunset { get; set; }
}

public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}