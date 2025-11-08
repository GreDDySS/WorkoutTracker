using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public interface IWorkoutStateService
    {
        WorkoutState CurrentState { get; }
        WorkoutSettings Settings { get; }

        void Initialize(WorkoutSettings settings);
        void MoveToNextPhase();
        void MoveToPreviousPhase();
        void DecrementTime();
        bool IsWorkoutCompleted();
        int CalculateRemainingTime();
        string GetProgressText();
    }
}
