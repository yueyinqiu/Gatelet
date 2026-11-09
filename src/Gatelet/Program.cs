using System.CommandLine;
using Gatelet;

var root = new RootCommand("gate");
root.Add(LetuCommand.Create());
root.Add(LettCommand.Create());
root.Add(RunCommand.Create());

return await root.Parse(args).InvokeAsync(null);