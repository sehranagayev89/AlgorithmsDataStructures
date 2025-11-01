using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    /// <summary>
    /// Utility class for backing up remote PostgreSQL databases
    /// </summary>
    public class PostgresBackup
    {
        /// <summary>
        /// Backs up a PostgreSQL database using pg_dump utility
        /// </summary>
        /// <param name="host">Database host address</param>
        /// <param name="port">Database port (default: 5432)</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Database username</param>
        /// <param name="password">Database password</param>
        /// <param name="outputPath">Path where backup file will be saved</param>
        /// <returns>True if backup was successful, false otherwise</returns>
        public static bool BackupDatabase(string host, string port, string database, string username, string password, string outputPath)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(database) || 
                    string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(outputPath))
                {
                    Console.WriteLine("Error: Host, database, username, and output path are required.");
                    return false;
                }

                // Set default port if not specified
                if (string.IsNullOrWhiteSpace(port))
                {
                    port = "5432";
                }

                // Ensure output directory exists
                string outputDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Build pg_dump command arguments
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string finalOutputPath = outputPath;
                
                // If output path is a directory, create a timestamped filename
                if (Directory.Exists(outputPath) || !Path.HasExtension(outputPath))
                {
                    finalOutputPath = Path.Combine(outputPath, $"{database}_backup_{timestamp}.sql");
                }

                Console.WriteLine($"Starting backup of database '{database}' from {host}:{port}");
                Console.WriteLine($"Backup will be saved to: {finalOutputPath}");

                // Try to use pg_dump if available
                string pgDumpPath = FindPgDump();
                
                if (!string.IsNullOrEmpty(pgDumpPath))
                {
                    return BackupWithPgDump(pgDumpPath, host, port, database, username, password, finalOutputPath);
                }
                else
                {
                    Console.WriteLine("Warning: pg_dump utility not found in PATH.");
                    Console.WriteLine("Please install PostgreSQL client tools or ensure pg_dump is in your PATH.");
                    Console.WriteLine("\nAlternatively, you can manually backup using:");
                    Console.WriteLine($"  pg_dump -h {host} -p {port} -U {username} -d {database} -f {finalOutputPath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during backup: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Finds pg_dump executable in PATH
        /// </summary>
        private static string FindPgDump()
        {
            string[] possibleNames = { "pg_dump", "pg_dump.exe" };
            
            foreach (string name in possibleNames)
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = name,
                        Arguments = "--version",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(startInfo))
                    {
                        if (process != null)
                        {
                            process.WaitForExit(2000);
                            if (process.ExitCode == 0)
                            {
                                return name;
                            }
                        }
                    }
                }
                catch
                {
                    // Continue to next possible name
                }
            }

            return null;
        }

        /// <summary>
        /// Performs backup using pg_dump utility
        /// </summary>
        private static bool BackupWithPgDump(string pgDumpPath, string host, string port, string database, 
            string username, string password, string outputPath)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = pgDumpPath,
                    Arguments = $"-h {host} -p {port} -U {username} -d {database} -f \"{outputPath}\" -F p -v",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Set password via environment variable (more secure than command line)
                if (!string.IsNullOrWhiteSpace(password))
                {
                    startInfo.EnvironmentVariables["PGPASSWORD"] = password;
                }

                Console.WriteLine("Executing pg_dump...");

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Console.WriteLine("Failed to start pg_dump process");
                        return false;
                    }

                    // Read output asynchronously
                    StringBuilder output = new StringBuilder();
                    StringBuilder error = new StringBuilder();

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            output.AppendLine(e.Data);
                            Console.WriteLine(e.Data);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            error.AppendLine(e.Data);
                            Console.WriteLine(e.Data);
                        }
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Wait for process to complete (with timeout)
                    bool completed = process.WaitForExit(300000); // 5 minutes timeout

                    if (!completed)
                    {
                        Console.WriteLine("Backup process timed out after 5 minutes");
                        process.Kill();
                        return false;
                    }

                    if (process.ExitCode == 0)
                    {
                        Console.WriteLine($"\nBackup completed successfully!");
                        Console.WriteLine($"Backup file saved to: {outputPath}");
                        
                        if (File.Exists(outputPath))
                        {
                            FileInfo fileInfo = new FileInfo(outputPath);
                            Console.WriteLine($"Backup file size: {fileInfo.Length / 1024.0:F2} KB");
                        }
                        
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"\nBackup failed with exit code: {process.ExitCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing pg_dump: {ex.Message}");
                return false;
            }
        }
    }
}
