using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static coder.bundle;

namespace coder
{
    internal class CreateRSP
    {
        public static Command CreateCreateRSPFile()
        {
            var createRSP = new Command("create-rsp", "create response file for bundle command.");
            var output = new Option<FileInfo>("--output", "path for thr rsp file to be saved.");
            createRSP.AddOption(output);
            createRSP.SetHandler((outputFile) =>
            {
                try
                {
                    File.WriteAllText(outputFile.FullName, "bundle\n");
                    Console.WriteLine("language? insert each one in a seperate line. to end hit ctrl+z");
                    var languagesOpt = new List<string> { "--language " };
                    string lang;
                    while ((lang = Console.ReadLine()) != null)
                    {
                        languagesOpt.Add(lang);
                    }
                    File.AppendAllLines(outputFile.FullName, languagesOpt);
                    var options = bundle.CreateBundleCommand().Options;
                    foreach (var option in options)
                    {
                        Console.WriteLine(option.Name + "? (" + option.Description + ")");
                        string answer;
                        if ((answer = Console.ReadLine()) != "")
                        { 
                            if(option.ValueType !=  typeof(Boolean) || bool.TryParse(answer, out bool res)==true)
                                File.AppendAllText(outputFile.FullName, "--" + option.Name + " " + answer + "\n"); 
                            else
                                Console.WriteLine("Boolean value only.");
                        }
                    }
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine("File path is invalid");
                }
                catch (System.NullReferenceException)
                {
                    Console.WriteLine("File path was not provided. use option --output to provide output file path.");
                }

            }, output);
            return createRSP;
        }
    }
}
