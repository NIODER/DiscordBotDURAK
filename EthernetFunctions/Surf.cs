using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace DiscordBotDURAK.EthernetFunctions
{
    public static class Surf
    {
        protected class Server
        {
            internal protected HtmlNode name { get; }
            internal protected HtmlNode address { get; }
            internal protected HtmlNode map_description { get; }

            internal Server(HtmlNode tr, HtmlDocument html)
            {
                name = html.DocumentNode.SelectSingleNode($@"{tr.XPath}/td/a[contains(@itemprop, 'name')]");
                address = html.DocumentNode.SelectSingleNode($@"{tr.XPath}/td/span[contains(@class, 'serverip')]");
                map_description = html.DocumentNode.SelectSingleNode($@"{tr.XPath}/td[contains(@class, 'maptext')]/a");
            }
        }

        public static string GetAddress()
        {
            WebRequest request = WebRequest.Create("https://playmon.ru/globaloffensive/surf");
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new(stream);
            string html = reader.ReadToEnd();
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);
            HtmlNodeCollection tr_nodes = htmlDocument.DocumentNode.SelectNodes(@"//tr[@id]");
            List<Server> servers = new();

            foreach (var td in tr_nodes)
            {
                servers.Add(new Server(td, htmlDocument));
            }

            var rand = servers.ElementAt(new Random().Next(servers.Count - 1));
            string result = $"Сервер: {rand.name.InnerText}\nКарта: {rand.map_description.InnerText}\nconnect {rand.address.InnerText}";

            return result;
        }
    }
}
