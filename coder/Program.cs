using coder;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Text;
using static coder.bundle;
using static coder.CreateRSP;


var rootCommand = new RootCommand("Root command for file bundler");

rootCommand.AddCommand(CreateBundleCommand());
rootCommand.AddCommand(CreateCreateRSPFile());

await rootCommand.InvokeAsync(args);
