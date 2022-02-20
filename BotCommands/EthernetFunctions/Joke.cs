using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace DiscordBotDURAK.EthernetFunctions
{
    public static class Joke
    {
        public static string GetJoke()
        {
            WebRequest request = WebRequest.Create("https://vse-shutochki.ru/anekdoty");
            WebResponse response = request.GetResponse();
            List<string> jokes = new();
            string html = "";

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new(stream))
                {
                    html = reader.ReadToEnd();
                }
            }

            HtmlDocument document = new();
            document.LoadHtml(html);

            var jokeNodes = document.DocumentNode.SelectNodes(@"//div[contains(@class, 'post')]");
            string str_joke = "";
            foreach (var joke in jokeNodes)
            {
                foreach (var text in document.DocumentNode.SelectNodes(joke.XPath + "/text()"))
                {
                    str_joke += text.InnerText;
                }
                if (str_joke.Trim() != "")
                {
                    jokes.Add(str_joke);
                }
                str_joke = "";
            }

            return jokes[new Random().Next(jokes.Count)];
        }
    }
}
