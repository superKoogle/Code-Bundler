using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coder
{
    internal class bundle
    {
        public static Command CreateBundleCommand()
        {
            var bundleCommand = new Command("bundle", "Bundles code files to a single file");
            var outputOption = new Option<FileInfo>("--output", "Desired output file path and name");
            outputOption.AddAlias("-o");
            var languageOption = new Option<List<string>>("--language", "Programing languages to be included in bundle file")
            {
                AllowMultipleArgumentsPerToken = true,
                IsRequired = true
            };
            var allowedLanguages = new List<string> { "all", ".cs", ".js", ".json", ".py", ".cpp", ".c", ".java", ".sql", ".php", ".swift", ".vb", ".html", ".css" };
            languageOption.AddAlias("-l");
            var noteOption = new Option<bool>("--note", "Weather to add code source in bundle file or not.");
            noteOption.AddAlias("-n");
            var sortOption = new Option<string>("--sort-by", "sort code files by name or by language");
            sortOption.AddAlias("-s");
            sortOption.SetDefaultValue("alphabetical");//use enum
            var emptyLinesOption = new Option<bool>("--remove-empty-lines", "Weather to remove empty lines from source code or not");
            emptyLinesOption.AddAlias("-el");
            var authorOption = new Option<string>("--author", "Name of the file creator");
            authorOption.AddAlias("-a");
            bundleCommand.AddOption(outputOption);
            bundleCommand.AddOption(languageOption);
            bundleCommand.AddOption(noteOption);
            bundleCommand.AddOption(sortOption);
            bundleCommand.AddOption(emptyLinesOption);
            bundleCommand.AddOption(authorOption);
            bundleCommand.AddValidator(result =>
            {
                //validate languages option
                var usedLanguages = result.GetValueForOption(languageOption);
                foreach (var lang in usedLanguages)
                {
                    if (!allowedLanguages.Contains(lang))
                        result.ErrorMessage = "Not allowed language alias";
                }

                //validate sort option
                if (result.GetValueForOption(sortOption) != "alphabetical" &&
                result.GetValueForOption(sortOption) != "language")
                    result.ErrorMessage = "sort method not recognized";

            });
            bundleCommand.SetHandler((output, sort, languages, author, note, removeEmptyLines) =>
            {
                try
                {
                    var FilesDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
                    var files = FilesDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories)
                                          .Where(file => (languages.Contains("all") && allowedLanguages.Contains(file.Extension)) || languages.Contains(file.Extension));
                    files = from file in files
                            where !(file.FullName.ToLower().Contains("/debug/") || file.FullName.ToLower().Contains("/bin/") || file.FullName.ToLower().Contains("/nodeModules/"))
                            orderby sort == "alphabetical" ? file.Name : file.Extension
                            select file;
                    if (author is not null) File.WriteAllText(output.FullName, author + "\n");
                    foreach (var file in files)
                    {
                        if (note)
                        {
                            var relativePath = Path.GetRelativePath(output.FullName, file.FullName);
                            File.AppendAllText(output.FullName, relativePath + "\n");
                        }
                        var codeLines = File.ReadAllLines(file.FullName);
                        if (removeEmptyLines)
                            codeLines = (from line in codeLines
                                         where !line.Equals("")
                                         select line).ToArray();
                        File.AppendAllLines(output.FullName, codeLines);
                    }
                    Console.WriteLine("File was successfully created and written");
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine("File path is invalid");
                }
                catch (System.NullReferenceException)
                {
                    Console.WriteLine("File path was not provided. use option --output -o to provide output file path.");
                }
            }, outputOption, sortOption, languageOption, authorOption, noteOption, emptyLinesOption);
            return bundleCommand;
        }
       
    }
}
