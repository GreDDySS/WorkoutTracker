using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Views;

public partial class AddProgramPageView : ContentPage
{
    private AddProgramViewModel _viewModel;
    private bool _hasLoadedProgram = false;

    public AddProgramPageView()
    {
        InitializeComponent();
        _viewModel = new AddProgramViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await LoadProgramFromNavigation();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Проверяем, есть ли выбранные упражнения из страницы выбора
        if (AppShell.SelectedExercises != null && AppShell.SelectedExercises.Count > 0)
        {
            _viewModel?.AddSelectedExercises(AppShell.SelectedExercises);
            AppShell.SelectedExercises = null; // Очищаем после использования
        }
        
        if (!_hasLoadedProgram && !_viewModel.IsEditMode)
        {
            await LoadProgramFromNavigation();
        }
    }

    private async Task LoadProgramFromNavigation()
    {
        try
        {
            if (AppShell.ProgramIdToEdit.HasValue)
            {
                var programId = AppShell.ProgramIdToEdit.Value;
                AppShell.ProgramIdToEdit = null;
                
                if (!_hasLoadedProgram || _viewModel.ProgramId != programId)
                {
                    await _viewModel.LoadProgramAsync(programId);
                    _hasLoadedProgram = true;
                    return;
                }
            }

            // Не сбрасываем состояние, если пользователь уже начал заполнять форму
            // (есть название программы или упражнения)
            if (!_hasLoadedProgram && !AppShell.ProgramIdToEdit.HasValue && !_viewModel.IsEditMode)
            {
                // Сбрасываем только если форма действительно пустая
                bool hasData = !string.IsNullOrWhiteSpace(_viewModel.ProgramName) || 
                              (_viewModel.ProgramExercises?.Count ?? 0) > 0;
                
                if (!hasData)
                {
                    _viewModel.ResetState();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при получении параметров: {ex.Message}");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Не сбрасываем _hasLoadedProgram, чтобы сохранить состояние при возврате
        // _hasLoadedProgram = false;
    }
}