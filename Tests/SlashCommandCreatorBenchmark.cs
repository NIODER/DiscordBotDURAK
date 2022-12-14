using BenchmarkDotNet.Attributes;
using DiscordBotDurak;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotDurak.Tests
{
    [MemoryDiagnoser]
    [RankColumn]
    public class SlashCommandCreatorBenchmark
    {
        [Benchmark]
        public void CreateCommandsTest()
        {
            var commands = new SlashCommandCreator().GetAllSlashCommands();
        }

        [Benchmark]
        public void CreateAndForeachCommandsTests()
        {
            var commands = new SlashCommandCreator().GetAllSlashCommands();
            int a = 0;
            foreach (var command in commands)
            {
                a += command.Options.Count;
            }
        }
    }
}
