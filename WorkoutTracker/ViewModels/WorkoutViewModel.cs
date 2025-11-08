using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WorkoutTracker.Views;

namespace WorkoutTracker.ViewModels
{
    public class WorkoutViewModel : BaseViewModel
    {

        public ICommand CreateWorkoutCommand { get; set; }

        public WorkoutViewModel()
        {
            CreateWorkoutCommand = new Command(CreateWorkout);

        }
        
        private void CreateWorkout()
        {
            Application.Current.MainPage = new CreateWorkoutPageView();
        }

    }
}
