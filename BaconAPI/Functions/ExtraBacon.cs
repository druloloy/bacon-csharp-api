using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BaconAPI.Functions
{
    // enum for id sizes
    public enum BaconSize {
        SMALL = 8,
        MEDIUM = 16,
        LARGE = 32
    }
    public class HistoryType
    {
        public string Value { get; private set; }
        private HistoryType(string type) { Value = type; }

        public static HistoryType Created { get { return new HistoryType("created"); } }
        public static HistoryType Modified{ get { return new HistoryType("modified"); } }
        public static HistoryType Deleted { get { return new HistoryType("deleted"); } }
    }
    public class ExtraBacon
    {
        // source https://stackoverflow.com/questions/1054076/randomly-generated-hexadecimal-number-in-c-sharp
        public static string GenId(BaconSize size)
        {
            Random random = new Random();
            byte[] buffer = new byte[(byte)size / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("x2")));

            return (byte)size % 2 == 0 ? result : result + random.Next(16).ToString("x");
        }
        public static string DecodeBase64(string str)
        {
            byte[] buffer = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(buffer);
        }

    }
}