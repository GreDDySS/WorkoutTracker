using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views
{
    public partial class MainPageView : ContentPage
    {

        public MainPageView()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

    }

}
