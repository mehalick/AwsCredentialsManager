using System.Collections.ObjectModel;

namespace AwsCredentialsManager;

public class MainPageViewModel
{
    public AwsAccount? SelectedAccount { get; set; }

    public ObservableCollection<AwsAccount> Accounts { get; } = [];
}

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel = new MainPageViewModel();
    private readonly string _credentialsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".aws\\credentials");

    public MainPage()
    {
        InitializeComponent();

        BindingContext = _viewModel;

        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await LoadCredentials();
    }

    private async Task LoadCredentials()
    {
        try
        {
            var accounts = await GetAccountsFromFile();

            if (accounts.Count == 0)
            {
                await ShowAlert("No accounts found in credentials file");

                return;
            }

            _viewModel.Accounts.Clear();

            foreach (var account in accounts)
            {
                _viewModel.Accounts.Add(account);
            }

            _viewModel.SelectedAccount = _viewModel.Accounts.FirstOrDefault();

            CredentialsPicker.SelectedIndex = 0;

            MainLayout.IsVisible = true;
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    private async Task<List<AwsAccount>> GetAccountsFromFile()
    {
        var accounts = new List<AwsAccount>();

        await using Stream fs = File.OpenRead(_credentialsPath);
        using var reader = new StreamReader(fs);

        AwsAccount? account = null;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync() ?? "";

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith('['))
            {
                account = new(line);
                accounts.Add(account);
            }
            else
            {
                account?.SetProperty(line);
            }
        }

        return accounts;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        SemanticScreenReader.Announce(SaveButton.Text);

        try
        {
            var backupPath = $"{_credentialsPath}.{DateTime.UtcNow:yyyyMMddHHmmss}.bak";

            File.Copy(_credentialsPath, backupPath);

            _viewModel.SelectedAccount!.SetProperties(InputEditor.Text);

            await using var sw = new StreamWriter(_credentialsPath);

            foreach (var account in _viewModel.Accounts)
            {
                foreach (var line in account.AsLineList())
                {
                    await sw.WriteLineAsync(line);
                }
            }

            InputEditor.Text = "";

            SaveButton.Text = "Credentials Saved!";

            await Task.Delay(1000);

            SaveButton.Text = "Save Credentials";
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    private async Task ShowAlert(string message)
    {
        await DisplayAlert("", message, "Got it");
    }
}
