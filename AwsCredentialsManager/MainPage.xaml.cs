using System.Collections.ObjectModel;

namespace AwsCredentialsManager;

public class MainPageViewModel
{
	public AwsAccount? SelectedAccount { get; set; }

	public ObservableCollection<AwsAccount> Accounts { get; set; } = [];
}

public partial class MainPage
{
	private readonly MainPageViewModel _viewModel = new MainPageViewModel();
	private readonly string _credentialsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".aws\\credentials");

	public MainPage()
	{
		InitializeComponent();
		InitializeControls();
	}

	private void InitializeControls()
	{
		BindingContext = _viewModel;
	}

	private async void OnLoadClicked(object sender, EventArgs e)
	{
		SemanticScreenReader.Announce(LoadButton.Text);

		try
		{
			var accounts = await GetAccountsFromFile();

			if (accounts.Count == 0)
			{
				await ShowToast("No accounts found in credentials file");

				return;
			}

			_viewModel.Accounts.Clear();

			foreach (var account in accounts)
			{
				_viewModel.Accounts.Add(account);
			}

			_viewModel.SelectedAccount = _viewModel.Accounts.FirstOrDefault();

			CredentialsPicker.SelectedIndex = 0;
			CredentialsPicker.IsEnabled = true;
			InputLabel.IsEnabled = true;
			InputEditor.IsEnabled = true;
			SaveButton.IsEnabled = true;
		}
		catch (Exception ex)
		{
			await ShowToast(ex.Message);
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

			SaveButton.IsEnabled = false;
			InputEditor.Text = "";

			await ShowToast("Credentials saved");
		}
		catch (Exception ex)
		{
			await ShowToast(ex.Message);
		}
	}

	private async Task ShowToast(string message)
	{
		await DisplayAlert("", message, "Got it");

		// var cancellationTokenSource = new CancellationTokenSource();
		//
		// string text = "This is a Toast";
		// ToastDuration duration = ToastDuration.Short;
		// double fontSize = 14;
		//
		// var toast = Toast.Make(text, duration, fontSize);
		//
		// await toast.Show(cancellationTokenSource.Token);
	}
}
