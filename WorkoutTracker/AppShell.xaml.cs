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
            
            // Register route for TimerPage
            Routing.RegisterRoute("TimerPage", typeof(TimerPageView));
        }
    }
}
