using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutTracker.Services;
using WorkoutTracker.Models;

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
        public ICommand EditExerciseCommand { get; set; }
        public ICommand EditProgramCommand { get; set; }
        public ICommand StartProgramCommand { get; set; }
        public ICommand DeleteExerciseCommand { get; set; }
        public ICommand DeleteProgramCommand { get; set; }

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
            EditExerciseCommand = new Command<int>(EditExercise);
            EditProgramCommand = new Command<int>(EditProgram);
            StartProgramCommand = new Command<int>(StartProgram);
            DeleteExerciseCommand = new Command<int>(DeleteExercise);
            DeleteProgramCommand = new Command<int>(DeleteProgram);

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
                AppShell.ExerciseIdToEdit = null;
                await Shell.Current.GoToAsync("AddExercisePage");
            }
            else
            {
                AppShell.ProgramIdToEdit = null;
                await Shell.Current.GoToAsync("AddProgramPage");
            }
        }

        private async void EditExercise(int exerciseId)
        {
            AppShell.ExerciseIdToEdit = exerciseId;
            await Shell.Current.GoToAsync("AddExercisePage");
        }

        private async void EditProgram(int programId)
        {
            AppShell.ProgramIdToEdit = programId;
            await Shell.Current.GoToAsync("AddProgramPage");
        }

        private async void StartProgram(int programId)
        {
            try
            {
                var program = await _programService.GetProgramByIdAsync(programId);
                if (program == null || program.Exercises == null || program.Exercises.Count == 0)
                {
                    return;
                }

                var firstExercise = program.Exercises.FirstOrDefault()?.Exercise;
                if (firstExercise == null)
                {
                    return;
                }

                var programExercises = new List<Models.ProgramExerciseItem>();
                foreach (var programExercise in program.Exercises)
                {
                    if (programExercise.Exercise != null)
                    {
                        programExercises.Add(new Models.ProgramExerciseItem
                        {
                            ExerciseId = programExercise.ExerciseId,
                            ExerciseName = programExercise.Exercise.Name,
                            WorkTimeSeconds = programExercise.Exercise.WorkTimeSeconds,
                            RestTimeSeconds = programExercise.Exercise.RestTimeSeconds,
                            Approaches = programExercise.Approaches
                        });
                    }
                }

                var settings = new Models.WorkoutSettings
                {
                    Approaches = programExercises.First().Approaches,
                    WorkTimeSeconds = firstExercise.WorkTimeSeconds,
                    RestTimeSeconds = firstExercise.RestTimeSeconds,
                    ProgramExercises = programExercises
                };

                AppShell.NavigationSettings = settings;
                await Shell.Current.GoToAsync("TimerPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при запуске программы: {ex.Message}");
            }
        }

        private async void DeleteExercise(int exerciseId)
        {
            if (exerciseId <= 0) return;

            var exerciseName = Exercises.
                OfType<Exercise>()
                .FirstOrDefault(e => e.Id == exerciseId)?.Name ?? "упражнение";

            bool confirm = await Shell.Current.DisplayAlert(
                "Удалить упражнения",
                $"Удалить \"{exerciseName}\"?",
                "Удалить",
                "Отмена");

            if (!confirm) return;

            var success = await _exerciseService.DeleteExerciseAsync(exerciseId);
            if (!success)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Не удалось удалить упражнение. Возможно, оно используется в программе.", "Ок");
                return;
            }

            LoadData();
        }

        private async void DeleteProgram(int programId)
        {
            if (programId <= 0 || Shell.Current == null)
            {
                return;
            }

            var programName = Programs
                .OfType<Program>()
                .FirstOrDefault(p => p.Id == programId)?.Name ?? "программу";

            bool confirm = await Shell.Current.DisplayAlert(
                "Удаление программы",
                $"Удалить \"{programName}\"?",
                "Удалить",
                "Отмена");

            if (!confirm)
            {
                return;
            }

            var success = await _programService.DeleteProgramAsync(programId);
            if (!success)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Не удалось удалить программу.", "Ок");
                return;
            }

            LoadData();
        }

    }
}
