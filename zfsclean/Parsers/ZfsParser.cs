using System.Globalization;
using System.Text.RegularExpressions;
using zfsclean.Models;

namespace zfsclean.Parsers;

public class ZfsParser
{
    public IEnumerable<ZfsSnapshot> ParseList(string output)
    {
        var reader = new StringReader(output);
        var lineNum = 0;
        string? line;
        
        while ((line = reader.ReadLine()) != null)
        {
            lineNum++;
            if (lineNum == 1)
            {
                continue;
            }
            var regex = new Regex("\\s{2,}");
            var cols = regex.Split(line);
            Console.WriteLine("cols:" + cols.Length);

            if (cols.Length != 2)
            {
                continue;
            }

            var name = cols[0].Trim();
 
            var creationAsString = cols[1].Trim();
            var trimmedCreationAsString = creationAsString[4..];
            
            // Fri Jan 19 15:19 2024
            // --- dd HH:mm:ss yyyy 
            if (!DateTime.TryParseExact(trimmedCreationAsString, 
                    "MMM dd H:mm yyyy", 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out var creation))
            {
                continue;
            }

            yield return new ZfsSnapshot()
            {
                Name = name,
                Creation = creation
            };
        }
    }
}