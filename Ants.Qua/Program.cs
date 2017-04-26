using System;
using System.IO;
using System.Linq;

namespace Ants.Qua
{
    internal class Program
    {
        public static void Main()
        {
            var bot = new AttackBot.MyBot();
            var ants = new AntsParser(bot.GetType().Namespace.Split('.').Last());
                ants.PlayGame(bot);
                //var fs = new FileStream("TestBotErrors.log", FileMode.Create, FileAccess.Write);
                //var sw = new StreamWriter(fs);
                //sw.Write(ex.ToString());

                //sw.Dispose();
                //fs.Dispose();
                //throw;
        }
    }
}