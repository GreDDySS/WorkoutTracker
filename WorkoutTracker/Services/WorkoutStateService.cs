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
            
            if (Settings.IsProgram && Settings.ProgramExercises != null && Settings.ProgramExercises.Count > 0)
            {
                var firstExercise = Settings.ProgramExercises[0];
                CurrentState.CurrentTimeSeconds = firstExercise.WorkTimeSeconds;
            }
        }

        public void MoveToNextPhase()
        {
            if (CurrentState.IsWorkPhase)
            {
                CurrentState.IsWorkPhase = false;
                var currentExercise = GetCurrentExercise();
                CurrentState.CurrentTimeSeconds = currentExercise?.RestTimeSeconds ?? Settings.RestTimeSeconds;
            }
            else
            {
                var currentExercise = GetCurrentExercise();
                int approaches = currentExercise?.Approaches ?? Settings.Approaches;
                
                if (CurrentState.CurrentApproach < approaches)
                {
                    CurrentState.CurrentApproach++;
                    CurrentState.IsWorkPhase = true;
                    CurrentState.CurrentTimeSeconds = currentExercise?.WorkTimeSeconds ?? Settings.WorkTimeSeconds;
                }
                else
                {
                    if (Settings.IsProgram && CurrentState.CurrentExerciseIndex < Settings.ProgramExercises.Count - 1)
                    {
                        CurrentState.CurrentExerciseIndex++;
                        CurrentState.CurrentApproach = 1;
                        CurrentState.IsWorkPhase = true;
                        var nextExercise = GetCurrentExercise();
                        CurrentState.CurrentTimeSeconds = nextExercise?.WorkTimeSeconds ?? Settings.WorkTimeSeconds;
                    }
                    else
                    {
                        CurrentState.IsCompleted = true;
                    }
                }
            }
        }

        public void MoveToPreviousPhase()
        {
            if (!CurrentState.IsWorkPhase)
            {
                CurrentState.IsWorkPhase = true;
                var currentExercise = GetCurrentExercise();
                CurrentState.CurrentTimeSeconds = currentExercise?.WorkTimeSeconds ?? Settings.WorkTimeSeconds;
            }
            else if (CurrentState.CurrentApproach > 1)
            {
                CurrentState.CurrentApproach--;
                CurrentState.IsWorkPhase = false;
                var currentExercise = GetCurrentExercise();
                CurrentState.CurrentTimeSeconds = currentExercise?.RestTimeSeconds ?? Settings.RestTimeSeconds;
            }
            else if (Settings.IsProgram && CurrentState.CurrentExerciseIndex > 0)
            {
                CurrentState.CurrentExerciseIndex--;
                var previousExercise = GetCurrentExercise();
                int approaches = previousExercise?.Approaches ?? Settings.Approaches;
                CurrentState.CurrentApproach = approaches;
                CurrentState.IsWorkPhase = false;
                CurrentState.CurrentTimeSeconds = previousExercise?.RestTimeSeconds ?? Settings.RestTimeSeconds;
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

            if (Settings.IsProgram)
            {
                for (int i = CurrentState.CurrentExerciseIndex; i < Settings.ProgramExercises.Count; i++)
                {
                    var exercise = Settings.ProgramExercises[i];
                    int approaches = exercise.Approaches;
                    int workTime = exercise.WorkTimeSeconds;
                    int restTime = exercise.RestTimeSeconds;

                    if (i == CurrentState.CurrentExerciseIndex)
                    {
                        if (CurrentState.IsWorkPhase)
                        {
                            int remainingApproaches = approaches - CurrentState.CurrentApproach;
                            remainingSeconds += restTime;
                            remainingSeconds += workTime * remainingApproaches;
                            remainingSeconds += restTime * remainingApproaches;
                        }
                        else
                        {
                            int remainingApproaches = approaches - CurrentState.CurrentApproach;
                            remainingSeconds += workTime * remainingApproaches;
                            remainingSeconds += restTime * remainingApproaches;
                        }
                    }
                    else
                    {
                        remainingSeconds += workTime * approaches;
                        remainingSeconds += restTime * approaches;
                    }
                }
            }
            else
            {
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
            }

            return remainingSeconds;
        }

        public string GetProgressText()
        {
            string phaseType = CurrentState.IsWorkPhase ? "РАБОТА" : "ОТДЫХ";
            
            if (Settings.IsProgram)
            {
                var currentExercise = GetCurrentExercise();
                string exerciseName = currentExercise?.ExerciseName ?? "";
                int approaches = currentExercise?.Approaches ?? Settings.Approaches;
                return $"{exerciseName} {CurrentState.CurrentApproach}/{approaches} ({CurrentState.CurrentExerciseIndex + 1}/{Settings.ProgramExercises.Count})";
            }
            else
            {
                return $"{phaseType} {CurrentState.CurrentApproach}/{Settings.Approaches}";
            }
        }

        private ProgramExerciseItem GetCurrentExercise()
        {
            if (Settings.IsProgram && Settings.ProgramExercises != null && 
                CurrentState.CurrentExerciseIndex >= 0 && 
                CurrentState.CurrentExerciseIndex < Settings.ProgramExercises.Count)
            {
                return Settings.ProgramExercises[CurrentState.CurrentExerciseIndex];
            }
            return null;
        }
    }
}
