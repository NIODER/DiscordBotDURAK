using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Data.Abstractions
{
    interface CRUD
    {
        abstract void Create(params object[] parameters);
        abstract object Read(params object[] parameters);
        abstract void Update(params object[] parameters);
        abstract void Delete(params object[] parameters);
    }
}
