namespace AwsCredentialsManager.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

        window.Title = "AWS Credentials Manager";

        window.Width = 1200;
        window.Height = 900;
        window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 2;
        window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2;

        return window;
    }
}
