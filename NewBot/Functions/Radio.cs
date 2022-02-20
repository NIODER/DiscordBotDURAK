using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions
{
    class RadioJSON
    {
        public string name;
        public string url;
    }

    class Radio : IFunction
    {
        /// <summary>
        /// Gets radios by key
        /// </summary>
        /// <param name="inputs">keys of radios</param>
        /// <returns>
        /// not null, if inputs is null return random radio, 
        /// if no results were found for the specified key,
        /// it returns the corresponding message
        /// </returns>
        public object Execute(params object[] inputs)
        {
            var allRadio = GetAllRadio();
            string radio = string.Empty;

            if (inputs == null)
            {
                var randomRadioJSON = allRadio.ElementAt(new Random().Next(allRadio.Count));
                return $"{randomRadioJSON.name} {randomRadioJSON.url}";
            }

            foreach (RadioJSON radioJSON in allRadio)
            {
                try
                {
                    foreach (string key in inputs)
                    {
                        if (radioJSON.name.Contains(key) || radioJSON.url.Contains(key))
                        {
                            radio += $"{radioJSON.name} {radioJSON.url}\n";
                        }
                    }
                }
                catch (InvalidCastException e)
                {
                    _ = new Log(new Discord.LogMessage(
                        Discord.LogSeverity.Error,
                        ToString(),
                        "cant cast object into string",
                        e));
                }
            }

            if (radio.Equals(string.Empty))
            {
                radio = "Таких радио не нашлось(";
            }

            return radio;
        }

        private IReadOnlyCollection<RadioJSON> GetAllRadio()
        {
            WebRequest request = WebRequest.Create("https://espradio.ru/stream_list.json");
            WebResponse response = request.GetResponse();
            List<string> resps = new();

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        resps.Add(reader.ReadLine());
                    }
                }
            }
            List<RadioJSON> jsonResps = new();

            foreach (string res in resps)
            {
                jsonResps.Add(JsonConvert.DeserializeObject<RadioJSON>(res));
            }
            return jsonResps;
        }
    }
}
