using System.Collections.Generic;
using System.Linq;
namespace Ants
{
    public static class AntRegistry
    {
        private static Dictionary<Location,Dictionary<string, object>> antData = new Dictionary<Location, Dictionary<string, object>>();

        public enum RegisterStatus
        {
            AntWasAlreadyRegistered,
            AntRegistered
        }

        public static RegisterStatus RegisterAnt(Location ant)
        {
            if (!antData.ContainsKey(ant))
            {
                antData[ant] = new Dictionary<string, object>();
                return RegisterStatus.AntRegistered;
            }

            return RegisterStatus.AntWasAlreadyRegistered;
        }

        public static void RegisterMove(Location ant, Location nextStep)
        {
            var data = antData[ant];
            antData.Remove(ant);
            antData[nextStep] = data;
        }

        public static void AddData(Location ant,string key, object data)
        {
            antData[ant][key] = data;
        }

        public static object GetData(Location ant, string key)
        {
            object value;
            antData[ant].TryGetValue(key, out value);
            return value;
        }

        public static void RemoveData(Location ant, string key)
        {
            antData[ant].Remove(key);
        }

        public static void RemoveAllData(Location location)
        {
            antData[location] = new Dictionary<string, object>();
        }

        public static void Clear()
        {
            antData = new Dictionary<Location, Dictionary<string, object>>();
        }

        public static void RemoveUnusedData(IEnumerable<Location> inactiveActs, string key)
        {
            var unusedData = antData.Where(s => !inactiveActs.Contains(s.Key));

            foreach (var keyValuePair in unusedData)
            {
                antData[keyValuePair.Key].Remove(key);
            }
        }
    }
}
