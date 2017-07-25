using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Runner
{
    internal static class CommandLineArguments
    {
        /// <summary>
        /// Gets the configuration file path.
        /// </summary>
        /// <value>
        /// The configuration file path.
        /// </value>
        public static string ConfigurationFilePath { get; set; }

        /// <summary>
        /// Gets the Office365 site URL.
        /// </summary>
        /// <value>
        /// The site URL.
        /// </value>
        public static string SiteUrl { get; private set; }       

        /// <summary>
        /// Processes the command line arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool ProcessCommandLineArguments(string[] args)
        {
            CommandLineManager.ProcessArguments(args);

            // Process the '/c' argument - always required
            ConfigurationFilePath = CommandLineManager.GetArgumentValue("c", null);
            if (string.IsNullOrEmpty(ConfigurationFilePath))
            {
                Console.WriteLine("The value for argument '/c' is not specified.");
                OutputUsageNotes();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Outputs the usage notes.
        /// </summary>
        private static void OutputUsageNotes()
        {
            // Get the filename for this console application
            string filename = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Output the usage notes
            Console.WriteLine();
            Console.WriteLine("USAGE: {0}", filename);
            Console.WriteLine("       /c:<CONFIGURATION_FILE>");
            Console.WriteLine();
            Console.WriteLine(" /c    - Required. Configuration file name.");
            Console.WriteLine();

            Console.ForegroundColor = originalColor;
        }
    }
}