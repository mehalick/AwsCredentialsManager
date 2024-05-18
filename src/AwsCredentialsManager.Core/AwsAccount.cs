namespace AwsCredentialsManager.Core;

public class AwsAccount
{
    public string Id { get; }
    public string Name { get; }
    public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

    public AwsAccount(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name cannot be null or whitespace.", nameof(name));
        }

        if (name[0] != '[' || name[^1] != ']')
        {
            throw new ArgumentException("Invalid account name format, must match '[name]'.", nameof(name));
        }

        Id = name[1..^1];
        Name = name;
    }

    public void AddProperties(string lines)
    {
        var items = lines.Split(Environment.NewLine);

        foreach (var item in items)
        {
            AddProperty(item);
        }
    }

    public void AddProperty(string line)
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('['))
        {
            return;
        }

        var property = RemoveWhiteSpace(line).Split('=', 2);

        if (property.Length != 2)
        {
            throw new ArgumentException($"Property string '{line}' contains unexpected format.", nameof(line));
        }

        Properties[property[0]] = property[1];
    }

    private static string RemoveWhiteSpace(string s)
    {
        return new(s.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }

    public List<string> AsLineList()
    {
        var lines = new List<string>
        {
            Name
        };

        foreach (var property in Properties)
        {
            TryAdd(lines, property.Key, property.Value);
        }

        lines.Add("");

        return lines;
    }

    private static void TryAdd(List<string> lines, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        lines.Add($"{key} = {value}");
    }
}
