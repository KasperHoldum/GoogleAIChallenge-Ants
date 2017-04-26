using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ants.Wikwak
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new MyBot();
            var ants = new AntsParser(bot.GetType().Namespace);
            try
            {
                ants.PlayGame(bot);
            }
            catch (Exception ex)
            {
#if DEBUG
                var fs = new FileStream("TestBotErrors.log", FileMode.Create, FileAccess.Write);
                var sw = new StreamWriter(fs);
                sw.Write(ex.ToString());

                sw.Dispose();
                fs.Dispose();
#endif
                throw;
            }
        }
    }
}
