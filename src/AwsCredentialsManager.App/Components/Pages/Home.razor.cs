using AwsCredentialsManager.Core;
using Microsoft.AspNetCore.Components;

namespace AwsCredentialsManager.App.Components.Pages;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class Home : ComponentBase
{
    private Dictionary<string, AwsAccount> _accounts = new Dictionary<string, AwsAccount>();
    private AwsAccount? _selectedAccount;
    private string _selectedAccountKey = "";
    private bool _saveEnabled;
    private string _statusMessage = "";

    [Inject]
    protected FileService FileService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        _accounts = await FileService.GetAccounts();

        if (_accounts.Count == 0)
        {
            SetStatus("No accounts in credentials file");
        }
        else
        {
            _selectedAccount = _accounts.First().Value;
            _selectedAccountKey = _selectedAccount.Id;

            SetStatus("Accounts loaded from credentials file");
        }
    }

    private void SelectAccount(string key)
    {
        _selectedAccountKey = key;
        _selectedAccount = _accounts[key];

        SetStatus(_selectedAccount?.Name + " selected");
    }

    private void ChangeProperty(string key, string value)
    {
      if (_selectedAccount == null)
      {
          return;
      }

      _selectedAccount.Properties[key] = value;
      _saveEnabled = true;

      SetStatus(_selectedAccount?.Name + " updated");
    }

    private async Task PasteCredentials()
    {
        var lines = await Clipboard.GetTextAsync();

        if (string.IsNullOrWhiteSpace(lines) || lines[0] != '[')
        {
            SetStatus("No account credentials in clipboard");
        }
        else
        {
            _selectedAccount?.AddProperties(lines);
            _saveEnabled = true;

            SetStatus(_selectedAccount?.Name + " updated from clipboard");
        }
    }

    private async Task SaveAccounts()
    {
        await FileService.SaveAccounts(_accounts);

        SetStatus("Credentials saved");
    }

    private void SetStatus(string status)
    {
        _statusMessage = $"{DateTime.Now:yyy-MM-dd HH:mm:ss} {status}";
    }
}
