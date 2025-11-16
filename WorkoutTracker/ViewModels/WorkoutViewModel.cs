using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutTracker.Services;

namespace WorkoutTracker.ViewModels
{
    public class WorkoutViewModel : BaseViewModel
    {
        private readonly IExerciseService _exerciseService;
        private readonly IProgramService _programService;

        private int _selectedTab = 0;
        private bool _isExercisesTabVisible = true;
        private bool _isProgramsTabVisible = false;
        private bool _isLoading = false;

        public ICommand CreateWorkoutCommand { get; set; }
        public ICommand SwitchToExercisesCommand { get; set; }
        public ICommand SwitchToProgramsCommand { get; set; }

        public ObservableCollection<object> Exercises { get; set; }
        public ObservableCollection<object> Programs { get; set; }

        public bool HasExercises => Exercises?.Count > 0;
        public bool HasPrograms => Programs?.Count > 0;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

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
            _exerciseService = GetService<IExerciseService>() ?? throw new InvalidOperationException("IExerciseService не зарегистрирован");
            _programService = GetService<IProgramService>() ?? throw new InvalidOperationException("IProgramService не зарегистрирован");

            Exercises = new ObservableCollection<object>();
            Programs = new ObservableCollection<object>();

            CreateWorkoutCommand = new Command(CreateWorkout);
            SwitchToExercisesCommand = new Command(() => SelectedTab = 0);
            SwitchToProgramsCommand = new Command(() => SelectedTab = 1);

            LoadData();
        }

        public async void LoadData()
        {
            try
            {
                IsLoading = true;

                var customExercises = await _exerciseService.GetCustomExercisesAsync();

                var program = await _programService.GetAllProgramsAsync();

                Exercises.Clear();
                foreach (var exercise in customExercises)
                {
                    Exercises.Add(exercise);
                }

                Programs.Clear();
                foreach (var prog in program)
                {
                    Programs.Add(prog);
                }

                OnPropertyChanged(nameof(HasExercises));
                OnPropertyChanged(nameof(HasPrograms));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
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
