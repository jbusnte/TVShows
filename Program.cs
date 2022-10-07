using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TVShows
{
    class Show
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("premiered")]
        public string Premiered { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("genres")]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Genres { get; set; }
    }

    class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            return new List<T> { token.ToObject<T>() };
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            await ProcessRepositories();
        }

        private static async Task ProcessRepositories()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Enter a show name to search. Press Enter to quit the program.");

                    var showName = Console.ReadLine();

                    if (string.IsNullOrEmpty(showName))
                    {
                        break;
                    }

                    var result = await client.GetAsync("http://api.tvmaze.com/singlesearch/shows?q=" + showName.ToLower());
                    var resultRead = await result.Content.ReadAsStringAsync();

                    var show = JsonConvert.DeserializeObject<Show>(resultRead);
                    Console.WriteLine("*****************************");
                    Console.WriteLine("Show name: " + show.Name);
                    Console.WriteLine("Language: " + show.Language);
                    Console.WriteLine("Premiered on " + show.Premiered);
                    Console.WriteLine("Genre(s):");
                    foreach(string i in show.Genres) { Console.WriteLine("  *" + i); }
                    Console.WriteLine("Summary: " + show.Summary);
                    Console.WriteLine("*****************************");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error. Please enter a valid name");
            }
        }
    }
}