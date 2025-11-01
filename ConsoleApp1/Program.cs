using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PostgreSQL Remote Database Backup Tool ===\n");

            // Check if arguments were provided
            if (args.Length >= 4)
            {
                // Command line mode
                string host = args[0];
                string database = args[1];
                string username = args[2];
                string outputPath = args[3];
                string port = args.Length > 4 ? args[4] : "5432";

                Console.Write("Enter database password: ");
                string password = ReadPassword();
                Console.WriteLine();

                bool success = PostgresBackup.BackupDatabase(host, port, database, username, password, outputPath);
                Environment.Exit(success ? 0 : 1);
            }
            else
            {
                // Interactive mode
                Console.WriteLine("This tool helps you backup a remote PostgreSQL database to your local computer.\n");

                Console.Write("Enter database host (e.g., localhost or remote.server.com): ");
                string host = Console.ReadLine();

                Console.Write("Enter database port (default: 5432): ");
                string port = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(port))
                {
                    port = "5432";
                }

                Console.Write("Enter database name: ");
                string database = Console.ReadLine();

                Console.Write("Enter username: ");
                string username = Console.ReadLine();

                Console.Write("Enter password: ");
                string password = ReadPassword();
                Console.WriteLine();

                Console.Write("Enter output path for backup file (e.g., C:\\backups\\mydb.sql): ");
                string outputPath = Console.ReadLine();

                // If no output path specified, use current directory
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    outputPath = Directory.GetCurrentDirectory();
                }

                Console.WriteLine();
                bool success = PostgresBackup.BackupDatabase(host, port, database, username, password, outputPath);

                if (success)
                {
                    Console.WriteLine("\n✓ Backup completed successfully!");
                }
                else
                {
                    Console.WriteLine("\n✗ Backup failed. Please check the error messages above.");
                }

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Reads password from console without displaying it
        /// </summary>
        private static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password.ToString();
        }
    }
}
