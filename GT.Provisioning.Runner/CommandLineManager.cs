using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Runner
{
    internal static class CommandLineManager
    {
        /// <summary>
        /// The arguments collection
        /// </summary>
        private static readonly Dictionary<string, string> arguments = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessArguments(string[] args)
        {
            arguments.Clear();

            foreach (string arg in args)
            {
                // Find the colon or equals separating the key with the value
                string[] keyValuePair = arg.Split(new char[] { ':', '=' }, 2);

                if (keyValuePair.Length == 2)
                {
                    string key = keyValuePair[0].Trim().TrimStart('/').TrimStart('-');
                    string keyValue = keyValuePair[1].Trim();
                    keyValue = keyValue.Trim('\"');

                    arguments.Add(key, keyValue);
                }
                else if (keyValuePair.Length == 1)
                {
                    string key = keyValuePair[0].Trim().TrimStart('/').TrimStart('-');
                    arguments.Add(key, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetArgumentValue(string name, string defaultValue)
        {
            return arguments.ContainsKey(name) ? arguments[name] ?? defaultValue : defaultValue;
        }
    }
}
