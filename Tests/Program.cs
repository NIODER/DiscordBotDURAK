using BenchmarkDotNet.Running;
using DiscordBotDurak;
using DiscordBotDurak.Data;
using System;

namespace DiscordBotDurak.Tests
{
    class Program
    {

        static void Main(string[] args)
        {
            Database db = new Database("Host=localhost;Port=5432;Database=Testing2;Username=postgres;Password=nioder125");
            //BenchmarkRunner.Run<SlashCommandCreatorBenchmark>();
        }
    }
}
