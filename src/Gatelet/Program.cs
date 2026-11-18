using System.CommandLine;
using Gatelet;

var root = new RootCommand("gate")
{
    LetuCommand.Create(),
    LettCommand.Create(),
    RunCommand.Create()
};

return await root.Parse(args).InvokeAsync();