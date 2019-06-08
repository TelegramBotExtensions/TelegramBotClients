using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MakeRequestAsyncModifier
{
    class Program
    {
        static void Main()
        {
            string sourceFilePath = "../../../../TelegramBotClients/RateLimitedClient/RateLimitedTelegramBotClient.cs";

            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine("Code file does not exist");
                Console.ReadKey(true);
                Environment.Exit(1);
            }

            string backupFilePath;
            int previousBackupCount = 0;
            do
            {
                backupFilePath = "backup" + (previousBackupCount != 0 ? previousBackupCount.ToString() : "") + "_" + Path.GetFileName(sourceFilePath);
                previousBackupCount++;
            }
            while (File.Exists(backupFilePath));

            File.Copy(sourceFilePath, backupFilePath);
            string code = File.ReadAllText(sourceFilePath).Replace("\r\n", "\n");

            string pattern = @"public Task(.+?)? (\w+)\(([^\)]+)\)\n +{\n +throw new NotImplementedException\(\);\n +}";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);

            int index = code.IndexOf("// __GENERATED__");
            if (index < 0) index = 0;
            string prefix = code.Substring(0, index);
            code = code.Substring(index);

            Console.WriteLine(regex.Match(code).Success);

            code = regex.Replace(code, match =>
            {
                const string indent = "        ";
                const string indent2 = "            ";

                bool hasType = match.Groups[1].Success;

                string name = match.Groups[2].Value;
                string parameters = match.Groups[3].Value;
                string callingParams = string.Join(", ", parameters.Split(',').Select(p => p.Trim().Split('=')[0].Trim().Split(' ')[1]));
                string bucket = callingParams.Contains("chatId") ? "chatId" : (callingParams.Contains("userId") ? "userId" : "");

                Console.WriteLine("Replacing " + name);

                string newBlock = "public async Task" + (hasType ? match.Groups[1].Value : "") + " " + name + "(" + parameters + ")\n";
                newBlock += indent + "{\n";
                newBlock += indent2 + "await RequestScheduler.YieldAsync(" + bucket + ").ConfigureAwait(false);\n";
                newBlock += "\n";
                newBlock += indent2 + (hasType ? "return " : "") + "await BaseClient." + name + "(" + callingParams + ").ConfigureAwait(false);\n";
                newBlock += indent + "}";

                return newBlock;
            });

            code = prefix + code;
            
            File.WriteAllText(sourceFilePath, code);

            Console.WriteLine("Done");
            Console.ReadKey(true);
        }
    }
}
