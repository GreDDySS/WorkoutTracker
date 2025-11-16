using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class AddExercisePageView : ContentPage
{
    private AddExerciseViewModel _viewModel;
    private bool _hasLoadedExercise = false;

    public AddExercisePageView()
    {
        InitializeComponent();
        _viewModel = new AddExerciseViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await LoadExerciseFromNavigation();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (!_hasLoadedExercise && !_viewModel.IsEditMode)
        {
            await LoadExerciseFromNavigation();
        }
    }

    private async Task LoadExerciseFromNavigation()
    {
        try
        {
            if (AppShell.ExerciseIdToEdit.HasValue)
            {
                var exerciseId = AppShell.ExerciseIdToEdit.Value;
                AppShell.ExerciseIdToEdit = null;
                
                if (!_hasLoadedExercise || _viewModel.ExerciseId != exerciseId)
                {
                    await _viewModel.LoadExerciseAsync(exerciseId);
                    _hasLoadedExercise = true;
                    return;
                }
            }

            var currentState = Shell.Current?.CurrentState;
            if (currentState != null)
            {
                var location = currentState.Location;
                if (location != null)
                {
                    var fullPath = location.OriginalString ?? location.ToString();
                    
                    if (fullPath.Contains("exerciseId="))
                    {
                        var exerciseIdStr = ExtractQueryParameter(fullPath, "exerciseId");
                        
                        if (!string.IsNullOrEmpty(exerciseIdStr) && int.TryParse(exerciseIdStr, out int exerciseId) && exerciseId > 0)
                        {
                            if (!_hasLoadedExercise || _viewModel.ExerciseId != exerciseId)
                            {
                                await _viewModel.LoadExerciseAsync(exerciseId);
                                _hasLoadedExercise = true;
                                return;
                            }
                        }
                    }
                }
            }

            if (!_hasLoadedExercise && !AppShell.ExerciseIdToEdit.HasValue && !_viewModel.IsEditMode)
            {
                _viewModel.ResetState();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при получении параметров: {ex.Message}");
        }
    }

    private string ExtractQueryParameter(string queryString, string parameterName)
    {
        try
        {
            var index = queryString.IndexOf($"{parameterName}=");
            if (index >= 0)
            {
                var startIndex = index + parameterName.Length + 1;
                var endIndex = queryString.IndexOf("&", startIndex);
                if (endIndex < 0)
                {
                    endIndex = queryString.IndexOf("?", startIndex);
                }
                if (endIndex < 0)
                {
                    endIndex = queryString.Length;
                }
                return queryString.Substring(startIndex, endIndex - startIndex);
            }
        }
        catch
        {
        }
        return null;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _hasLoadedExercise = false;
    }
}