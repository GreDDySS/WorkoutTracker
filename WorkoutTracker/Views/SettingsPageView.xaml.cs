using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views
{
    public partial class SettingsPageView : ContentPage
    {
        public SettingsPageView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
