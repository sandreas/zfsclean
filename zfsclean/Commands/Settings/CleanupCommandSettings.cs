using Spectre.Console.Cli;

namespace zfsclean.Commands.Settings;

public class CleanupCommandSettings: AbstractCommandSettings
{
    [CommandOption("--keep")] public string Keep { get; set; } = "14d";
}