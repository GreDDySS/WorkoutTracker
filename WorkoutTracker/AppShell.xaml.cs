using WorkoutTracker.Models;
using WorkoutTracker.Views;

namespace WorkoutTracker
{
    public partial class AppShell : Shell
    {
        public static WorkoutSettings NavigationSettings { get; set; }

        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute("TimerPage", typeof(TimerPageView));
            Routing.RegisterRoute("AddExercisePage", typeof(AddExercisePageView));
            Routing.RegisterRoute("AddProgramPage", typeof(AddProgramPageView));
            Routing.RegisterRoute("SelectExercisesPage", typeof(SelectExercisesPageView));
        }
    }
}
