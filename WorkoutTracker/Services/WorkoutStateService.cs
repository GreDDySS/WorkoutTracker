using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public class WorkoutStateService : IWorkoutStateService
    {
        public WorkoutState CurrentState { get; private set; }
        public WorkoutSettings Settings { get; private set; }

        public void Initialize(WorkoutSettings settings)
        {
            Settings = settings;
            CurrentState = new WorkoutState();
            CurrentState.Reset(Settings);
        }

        public void MoveToNextPhase()
        {
            if (CurrentState.IsWorkPhase)
            {
                CurrentState.IsWorkPhase = false;
                CurrentState.CurrentTimeSeconds = Settings.RestTimeSeconds;
            }
            else
            {
                if (CurrentState.CurrentApproach < Settings.Approaches)
                {
                    CurrentState.CurrentApproach++;
                    CurrentState.IsWorkPhase = true;
                    CurrentState.CurrentTimeSeconds = Settings.WorkTimeSeconds;
                }
                else
                {
                    CurrentState.IsCompleted = true;
                }
            }
        }

        public void MoveToPreviousPhase()
        {
            if (!CurrentState.IsWorkPhase)
            {
                CurrentState.IsWorkPhase = true;
                CurrentState.CurrentTimeSeconds = Settings.WorkTimeSeconds;
            }
            else if (CurrentState.CurrentApproach > 1)
            {
                CurrentState.CurrentApproach--;
                CurrentState.IsWorkPhase = false;
                CurrentState.CurrentTimeSeconds = Settings.RestTimeSeconds;
            }
            CurrentState.IsCompleted = false;
        }

        public void DecrementTime()
        {
            if (CurrentState.CurrentTimeSeconds > 0)
            {
                CurrentState.CurrentTimeSeconds--;
            }
        }

        public bool IsWorkoutCompleted() => CurrentState.IsCompleted;

        public int CalculateRemainingTime()
        {
            int remainingSeconds = CurrentState.CurrentTimeSeconds;

            if (CurrentState.IsWorkPhase)
            {
                remainingSeconds += Settings.RestTimeSeconds;
                int remainingApproaches = Settings.Approaches - CurrentState.CurrentApproach;
                remainingSeconds += Settings.WorkTimeSeconds * remainingApproaches;
                remainingSeconds += Settings.RestTimeSeconds * remainingApproaches;
            }
            else
            {
                int remainingApproaches = Settings.Approaches - CurrentState.CurrentApproach;
                remainingSeconds += Settings.WorkTimeSeconds * remainingApproaches;
                remainingSeconds += Settings.RestTimeSeconds * remainingApproaches;
            }

            return remainingSeconds;
        }

        public string GetProgressText()
        {
            string phaseType = CurrentState.IsWorkPhase ? "РАБОТА" : "ОТДЫХ";
            return $"{phaseType} {CurrentState.CurrentApproach}/{Settings.Approaches}";
        }
    }
}
