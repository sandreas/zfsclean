using System.Text;
using System.Text.RegularExpressions;
using CliWrap;
using CliWrap.Buffered;
using Sandreas.SpectreConsoleHelpers.Commands;
using Sandreas.SpectreConsoleHelpers.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using zfsclean.Commands.Settings;

namespace zfsclean.Commands;


public partial class CleanupCommand : CancellableAsyncCommand<CleanupCommandSettings>
{
    private readonly SpectreConsoleService _console;

    /*
    [GeneratedRegex("([0-9]+[dhmsfz])", RegexOptions.IgnoreCase, "de-DE")]
    private static partial Regex TimeSpanSplitRegex();
*/

    public CleanupCommand(SpectreConsoleService console)
    {
        _console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, CleanupCommandSettings settings,
        CancellationToken cancellationToken)
    {

        var keep = ParseKeepValue(settings.Keep);
        if (keep == TimeSpan.MaxValue)
        {
            return 1;
        }
        // zfs list -t snapshot -o name,creation
        
        var result = await Cli.Wrap("zfs")
            .WithArguments(new string[]
            {
                "list", "-t", "snapshot", "-o", "name,creation"
            })
            .ExecuteBufferedAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            _console.Error.WriteLine($"zfs - could not read snapshots: {result.StandardError}{result.StandardOutput} ({result.ExitCode})");
        }
        return 0;
    }

    private static TimeSpan ParseKeepValue(string keepValue)
    {
        // System.Xml.XmlConvert.ToTimeSpan("P0DT1H0M0S")
        if (int.TryParse(keepValue, out var keepDays))
        {
            return TimeSpan.FromDays(keepDays);
        }

        var regex = new Regex("([0-9]+[dhmsfz])", RegexOptions.IgnoreCase);
        var substrings = regex.Split(keepValue.ToLowerInvariant()).Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
        return substrings.Count == 0 ? TimeSpan.MaxValue : substrings.Aggregate(TimeSpan.Zero, (current, sub) => current + ConvertToTimeSpan(sub));
    }
    
    private static TimeSpan ConvertToTimeSpan(string timeSpan)
    {
        var l = timeSpan.Length - 1;
        var value = timeSpan.Substring(0, l);
        var type = timeSpan.Substring(l, 1);

        return type switch
        {
            "d" => TimeSpan.FromDays(double.Parse(value)),
            "h" => TimeSpan.FromHours(double.Parse(value)),
            "m" => TimeSpan.FromMinutes(double.Parse(value)),
            "s" => TimeSpan.FromSeconds(double.Parse(value)),
            "f" => TimeSpan.FromMilliseconds(double.Parse(value)),
            "z" => TimeSpan.FromTicks(long.Parse(value)),
            _ => TimeSpan.FromDays(double.Parse(value))
        };
    }


}