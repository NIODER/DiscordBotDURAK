using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EthernetFunctons.Balaboba
{
    class Request
    {
        public string query { get; set; }
        public int intro { get; set; }
        public int filter { get; set; }
    }

    class Response
    {
        public int bad_query { get; set; }
        public int error { get; set; }
        public string query { get; set; }
        public string text { get; set; }
    }

    static class Balaboba
    {
        public static string Say(string example)
        {
            Response resp;
            WebRequest webRequest = WebRequest.Create("https://zeapi.yandex.net/lab/api/yalm/text3");
            webRequest.Method = "POST";
            Request dataCl = new Request { query = example, intro = 0, filter = 0 };
            string data = JsonSerializer.Serialize(dataCl);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse webResponse = webRequest.GetResponse();
            using (Stream stream = webResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                {
                    resp = JsonSerializer.Deserialize<Response>(reader.ReadToEnd());
                    example = resp.text;
                }
            }
            webResponse.Close();
            Console.WriteLine($"Response: {{ bad_query : {resp.bad_query}, error : {resp.error}, query : \"{resp.query}\" }} gotcha.");
            if (resp.bad_query == 1)
            {
                example = RandomMessages.RandomResponse();
                return example;
            }
            if (resp.error != 0)
            {
                example = "Произошла ошибка. Позвать <@274543887656419338>";
            }
            return example;
        }
    }
}
