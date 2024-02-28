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
        var window = base.CreateWindow(activationState);

        window.Activated += Window_Activated;

        return window;
    }

    private static async void Window_Activated(object? sender, EventArgs e)
    {
#if WINDOWS
        if (sender is not Window window)
        {
            return;
        }

        // change window size.
        window.Width = 800;
        window.Height = 600;

        // give it some time to complete window resizing task.
        await window.Dispatcher.DispatchAsync(() => { });

        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

        // move to screen center
        window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 2;
        window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2;
#endif
    }
}
