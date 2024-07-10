namespace zfsclean.Models;

public class ZfsSnapshot
{
    public string Name { get; set; } = "";
    public DateTime Creation { get; set; } = DateTime.MinValue;
    public long SizeInBytes { get; set; } = 0;

}