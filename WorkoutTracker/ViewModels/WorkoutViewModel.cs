using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WorkoutTracker.ViewModels
{
    public class WorkoutViewModel : BaseViewModel
    {

        private int _selectedTab = 0;
        private bool _isExercisesTabVisible = true;
        private bool _isProgramsTabVisible = false;

        public ICommand CreateWorkoutCommand { get; set; }
        public ICommand SwitchToExercisesCommand { get; set; }
        public ICommand SwitchToProgramsCommand { get; set; }

        public ObservableCollection<object> Exercises { get; set; }
        public ObservableCollection<object> Programs { get; set; }


        public bool HasExercises => Exercises?.Count > 0;
        public bool HasPrograms => Programs?.Count > 0;

        public bool IsExercisesTabActive => SelectedTab == 0;
        public bool IsProgramsTabActive => SelectedTab == 1;


        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (SetProperty(ref _selectedTab, value))
                {
                    IsExercisesTabVisible = value == 0;
                    IsProgramsTabVisible = value == 1;
                    OnPropertyChanged(nameof(IsExercisesTabVisible));
                    OnPropertyChanged(nameof(IsProgramsTabVisible));
                    OnPropertyChanged(nameof(IsExercisesTabActive));
                    OnPropertyChanged(nameof(IsProgramsTabActive));
                }
            }
        }

        public bool IsExercisesTabVisible
        {
            get => _isExercisesTabVisible;
            set => SetProperty(ref _isExercisesTabVisible, value);
        }

        public bool IsProgramsTabVisible
        {
            get => _isProgramsTabVisible;
            set => SetProperty(ref _isProgramsTabVisible, value);
        }

        public WorkoutViewModel()
        {
            Exercises = new ObservableCollection<object>();
            Programs = new ObservableCollection<object>();

            CreateWorkoutCommand = new Command(CreateWorkout);
            SwitchToExercisesCommand = new Command(() => SelectedTab = 0);
            SwitchToProgramsCommand = new Command(() => SelectedTab = 1);

            LoadData();
        }

        public void LoadData()
        {
            // TODO: Загрузить упражнения и программы из базы данных

            OnPropertyChanged(nameof(HasExercises));
            OnPropertyChanged(nameof(HasPrograms));
        }

        private async void CreateWorkout()
        {
            if (SelectedTab == 0)
            {
                await Shell.Current.GoToAsync("AddExercisePage");
            }
            else
            {
                await Shell.Current.GoToAsync("AddProgramPage");
            }
        }

    }
}
