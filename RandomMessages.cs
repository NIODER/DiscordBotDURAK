using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK
{
    static class RandomMessages
    {
        private static List<string> _triggers = new()
        {
            "слыш",
            "код",
            "прог",
            "хуй",
            "гов",
            "кирил",
            "пис",
            "дурак",
            "?",
            "шиз",
            "сук",
            "сос",
            "как",
            "чт",
            "по"
        };
        private static List<string> _botResponses = new()
        {
            "Слишком много стеба, слишком много мата...",
            "Не груби людям, ок?",
            "еще раз так напишешь... и.. и я обижусь!",
            "Слыш, урот, ты это.. базар фильтруй, ок?",
            "Пойдем выскочим, за такие слова",
            "понятно..",
            "(я его не знаю)"
        };
        private static List<string> _supplements = new()
        {
            "Как говорил мой дед, ",
            "У тебя к спине приклеилась записка, там написано: \n",
            "Стало быть ",
            "Переведи на китайский: ",
            "Тридцать три ",
            "ВНИМАНИЕ, ТОВАРИЩИ! ",
            "Как то раз, дурак, дебил и пидарас ",
            "42 "
        };

        public static string GetSupplement()
        {
            Random random = new();
            if (random.Next(100) % 2 == 0)
            {
                return _supplements[random.Next(0, _supplements.Count - 1)];
            }
            else
            {
                return "";
            }
        }

        public static string RandomResponse()
        {
            Random random = new();
            return _botResponses[random.Next(0, _botResponses.Count - 1)];
        }

        public static void AddResponse(string msg)
        {
            _botResponses.Add(msg);
        }
        public static void AddSupplement(string msg)
        {
            _supplements.Add(msg);
        }

        public static bool TriggerCheck(string msg)
        {
            foreach (string trigger in _triggers)
            {
                if (msg.ToLower().Contains(trigger))
                {
                    Random random = new();
                    if (random.Next(100) % 2 == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
