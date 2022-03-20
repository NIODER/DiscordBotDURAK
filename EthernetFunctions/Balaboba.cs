using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EthernetFunctons.Balaboba
{
    /// <summary>
    /// Класс запроса
    /// </summary>
    class Request
    {
        public string query { get; set; }
        public int intro { get; set; }
        public int filter { get; set; }
    }

    /// <summary>
    /// Класс ответа
    /// </summary>
    class Response
    {
        public int bad_query { get; set; }
        public int error { get; set; }
        public string query { get; set; }
        public string text { get; set; }
    }


    /// <summary>
    /// Генерирует ответ нейросети на сообщение с триггером
    /// </summary>
    static class Balaboba
    {
        /// <summary>
        /// Логирует ответ балабобы в E:\log.TXT
        /// </summary>
        /// <param name="response">Класс ответа</param>
        private static void Log(Response response)
        {
            using (var writer = new StreamWriter(new FileStream(Constants.Constants.logPath, FileMode.Append, FileAccess.Write)))
            {
                Console.WriteLine($"Balaboba {DateTime.Now}");
                Console.WriteLine($"   bad_query : {response.bad_query}");
                Console.WriteLine($"   error : {response.error}");
                Console.WriteLine($"   query : {response.query}");
                Console.WriteLine($"   text : {response.text}");
            }
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

        //private static string CheckErrors(Response response)
        //{
        //    if (response.bad_query == 1 && response.error != 0)
        //    {
        //        return "Ну вот, из-за того, что ты постоянно материшься, произошла ошибка. Позвать <@274543887656419338>";
        //    }
        //    else if (response.bad_query == 1)
        //    {
        //        return DiscordBotDURAK.RandomMessages.RandomResponse();
        //    }
        //    else if (response.error != 0)
        //    {
        //        return "Произошла ошибка. Позвать <@274543887656419338>";
        //    }
        //    else
        //    {
        //        return response.text;
        //    }
        //}

        /// <summary>
        /// Вся логика ответа
        /// </summary>
        /// <param name="example"></param>
        /// <returns></returns>
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

            WebResponse webResponse;
            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch (WebException e)
            {
                DiscordBotDURAK.Program.Log(new Discord.LogMessage(Discord.LogSeverity.Error, "Balaboba", "Balaboba is not working", e));
                return null;
            }

            using (Stream stream = webResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                {
                    resp = JsonSerializer.Deserialize<Response>(reader.ReadToEnd());
                }
            }
            webResponse.Close();

            Log(resp);

            return CheckErrors(resp);
        }
    }
}
