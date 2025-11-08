using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkoutTracker.Views;

namespace WorkoutTracker.ViewModels
{
     public class AddExerciseViewModel : BaseViewModel
    {
        public ICommand SaveExerciseCommand { get; set; }

        public AddExerciseViewModel()
        {
            SaveExerciseCommand = new Command(SaveExercise);
        }

        public void SaveExercise()
        {
            Application.Current.MainPage = new CreateWorkoutPageView();
        }
    }
}
