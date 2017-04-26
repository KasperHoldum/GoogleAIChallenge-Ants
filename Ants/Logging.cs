using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Ants
{
    public class Logging
    {
        private readonly string playLogFile;
        private readonly string inputLogFile;
        private readonly StringBuilder inputCache = new StringBuilder();

        public Logging(string name)
        {
            playLogFile = name + " - log.txt";
            inputLogFile = name + " - input.txt";
#if DEBUG
            try
            {
                File.Delete(playLogFile);
            }
            catch {}

            try
            {
                File.Delete(inputLogFile);
            }
            catch {}
#endif
        }

        [Conditional("DEBUG")]
        public void Log(string msg)
        {
            using (StreamWriter writer = File.AppendText(playLogFile))
            {
                writer.WriteLine(msg);
            }
        }

        [Conditional("DEBUG")]
        public void LogInput(char c)
        {
            inputCache.Append(c);
        }
        
        [Conditional("DEBUG")]
        public void LogTurnEnd()
        {
            WriteInputCache();
        }

        private void WriteInputCache()
        {
            using (StreamWriter writer = File.AppendText(inputLogFile))
            {
                writer.Write(inputCache.ToString());
                inputCache.Length = 0; // clear does not exist in Mono
            }
        }
    }
}
