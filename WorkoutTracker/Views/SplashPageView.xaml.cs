namespace WorkoutTracker.Views;

public partial class SplashPageView : ContentPage
{
	public SplashPageView()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        await Task.Delay(2000);
        await this.FadeTo(0, 700, Easing.CubicOut);

        Application.Current.MainPage = new AppShell();
    }
}