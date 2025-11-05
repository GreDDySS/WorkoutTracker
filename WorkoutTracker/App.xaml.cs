using WorkoutTracker.Views;

namespace WorkoutTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new SplashPageView();
        }
    }
}
