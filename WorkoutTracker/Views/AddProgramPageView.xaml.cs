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

            if (!_hasLoadedProgram && !AppShell.ProgramIdToEdit.HasValue && !_viewModel.IsEditMode)
            {
                _viewModel.ResetState();
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
        _hasLoadedProgram = false;
    }
}