namespace AwsCredentialsManager;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var windows = base.CreateWindow(activationState);

		windows.Width = 1200;

		return windows;
	}
}
