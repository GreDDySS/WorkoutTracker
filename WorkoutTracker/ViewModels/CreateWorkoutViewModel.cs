using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkoutTracker.Views;

namespace WorkoutTracker.ViewModels
{
    public class CreateWorkoutViewModel : BaseViewModel
    {
        public ICommand AddExerciseCommand { get; set; }
        public ICommand SaveWorkoutCommand { get; set; }

        public CreateWorkoutViewModel() {
            AddExerciseCommand = new Command(AddExercise);
        }

        public void AddExercise()
        {
            Application.Current.MainPage = new AddExercisePageView();
        }

        async public void SaveWorkout()
        {
            Application.Current.MainPage = new WorkoutPageView();
        }
    }
}
