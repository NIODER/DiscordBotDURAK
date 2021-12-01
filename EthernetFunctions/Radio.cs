using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBotDURAK.EthernetFunctions
{
    class Radio
    {
        public string name;
        public string url;
    }

    public static class RadioReferences
    {
        private static IReadOnlyCollection<Radio> GetAllRadio()
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
            List<Radio> jsonResps = new();

            foreach (string res in resps)
            {
                jsonResps.Add(JsonConvert.DeserializeObject<Radio>(res));
            }
            return jsonResps;
        }

        public static string GetRadio(string key)
        {
            IReadOnlyCollection<Radio> allRadio = GetAllRadio();
            string ret = "";
            foreach (Radio radio in allRadio)
            {
                if (radio.name.Contains(key) || radio.url.Contains(key))
                {
                    ret += $"{radio.name} {radio.url}\n";
                }
            }
            return ret;
        }
    }
}
