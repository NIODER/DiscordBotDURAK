using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiscordBotDurak
{
    public class Constants
    {
        private readonly XDocument document;
        private readonly IEnumerable<XElement> _messages;

        public Constants()
        {
            document = XDocument.Load(Program.resourcesPath + "/XMLConstants.xml");
            _messages = document.Element("constants")
                .Element("messages")
                .Elements("message");
        }

        private XElement GetMessage(string name)
        {
            return _messages.FirstOrDefault(m => m.Attribute("name").Value == name);
        }

        public string GetOwnerMessage()
        {
            return GetMessage("firstToOwner").Value;
        }

        public string GetSpyHelpMessage()
        {
            return GetMessage("spyModeHelpMessage").Value;
        }

        public string GetSymbolsListsHelpMessage1()
        {
            return GetMessage("symbolsListsHelpMessage1").Value;
        }

        public string GetSymbolsListsHelpMessage2()
        {
            return GetMessage("symbolsListsHelpMessage2").Value;
        }

        public string GetUserHelpMessage()
        {
            return GetMessage("userHelpMessage").Value;
        }
    }
}
