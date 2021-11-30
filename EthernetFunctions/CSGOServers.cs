using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace DiscordBotDURAK.EthernetFunctions
{
    public static class CSGOServers
    {
        public static string GetAddress()
        {
            WebRequest request = WebRequest.Create("https://playmon.ru/globaloffensive/surf");
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new(stream);
            string html = reader.ReadToEnd();
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);
            var addresses = htmlDocument.DocumentNode.SelectNodes(@"//span[contains(@class, 'serverip')]");
            List<string> stringAddresses = new();
            foreach (var address in addresses)
            {
                stringAddresses.Add(address.InnerText);
            }

            return stringAddresses.ToArray()[new Random().Next(stringAddresses.Count() - 1)];
        }
    }
}
