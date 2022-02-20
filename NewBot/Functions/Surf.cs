using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions
{
    class Surf : IFunction
    {
        public object Execute(params object[] inputs)
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

            Server randomServer;
            List<Server> serversByKey = new();

            if (inputs == null)
            {
                randomServer = servers.ElementAt(new Random().Next(servers.Count - 1));
            }
            else
            {
                foreach (var server in servers)
                {
                    foreach (string key in inputs)
                    {
                        if (server.name.InnerText.Contains(key) || server.map_description.InnerText.Contains(key))
                        {
                            serversByKey.Add(server);
                        }
                    }
                }
            }

            if (serversByKey.Count != 0)
            {
                randomServer = serversByKey.ElementAt(new Random().Next(serversByKey.Count - 1));
            }
            else
            {
                return "Такого сервера не нашлось";
            }


            return $"Сервер: {randomServer.name.InnerText}\nКарта: {randomServer.map_description.InnerText}\nconnect {randomServer.address.InnerText}";
        }

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

    }
}
