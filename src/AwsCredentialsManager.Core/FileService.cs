namespace AwsCredentialsManager.Core;

public class FileService
{
    private readonly string _credentialsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".aws\\credentials");

    public async Task<Dictionary<string, AwsAccount>> GetAccounts()
    {
        var accounts = new Dictionary<string, AwsAccount>();

        await using Stream fs = File.OpenRead(_credentialsPath);
        using var reader = new StreamReader(fs);

        AwsAccount? account = null;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith('['))
            {
                account = new(line);
                accounts.Add(account.Id, account);
            }
            else
            {
                account?.AddProperty(line);
            }
        }

        return accounts;
    }

    public async Task SaveAccounts(Dictionary<string, AwsAccount> accounts)
    {
        var backupPath = $"{_credentialsPath}.{DateTime.UtcNow:yyyyMMddHHmmss}.bak";
        File.Copy(_credentialsPath, backupPath);

        await using var sw = new StreamWriter(_credentialsPath);

        foreach (var account in accounts)
        {
            foreach (var line in account.Value.AsLineList())
            {
                await sw.WriteLineAsync(line);
            }
        }
    }
}
