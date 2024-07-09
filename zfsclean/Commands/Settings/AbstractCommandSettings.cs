using Spectre.Console.Cli;

namespace zfsclean.Commands.Settings;

public abstract class AbstractCommandSettings : CommandSettings
{
    [CommandOption("--debug")] public bool Debug { get; set; } = false;
    [CommandOption("--force")] public bool Force { get; set; } = false;
}