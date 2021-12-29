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
            WebRequest request = WebRequest.Create("https://anekdotovstreet.com/chernyy-yumor/");
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

            var jokeNodes = document.DocumentNode.SelectNodes(@"//div[contains(@class, 'anekdot-text')]/p");

            foreach (var joke in jokeNodes)
            {
                jokes.Add(joke.InnerText);
            }

            return jokes.ToArray()[new Random().Next(jokes.Count - 1)];
        }
    }
}
