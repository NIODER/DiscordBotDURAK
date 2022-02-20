using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions.Balaboba
{
    class Balaboba : IFunction
    {
        public object Execute(params object[] inputs)
        {
            Response response;
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
                    response = JsonSerializer.Deserialize<Response>(reader.ReadToEnd());
                }
            }
            webResponse.Close();

            _ = new Log(new Discord.LogMessage(Discord.LogSeverity.Info,
                "balaboba",
                $"bad_query : {response.bad_query}, " +
                $"error : {response.error}, " +
                $"query : {response.query}, " +
                $"text : {response.text}"));

            return CheckErrors(response);
        }

        /// <summary>
        /// Я ебать хз, это от сюда: https://habr.com/ru/post/493484/
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string CheckErrors(Response response) =>
            (response.bad_query == 1, response.error != 0) switch
            {
                (true, true) => "Ну вот, из-за того, что ты постоянно материшься, произошла ошибка. Позвать <@274543887656419338>",
                (true, _) => DiscordBotDURAK.RandomMessages.RandomResponse(),
                (_, true) => "Произошла ошибка. Позвать <@274543887656419338>",
                _ => response.text
            };
    }
}
