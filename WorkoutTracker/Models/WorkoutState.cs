using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker.Models
{
    public class WorkoutState
    {
        public int CurrentApproach { get; set; } = 1;
        public bool IsWorkPhase { get; set; } = true;
        public int CurrentTimeSeconds { get; set; }
        public bool IsPaused { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        public void Reset(WorkoutSettings settings)
        {
            CurrentApproach = 1;
            IsWorkPhase = true;
            CurrentTimeSeconds = settings.WorkTimeSeconds;
            IsCompleted = false;
        }
    }
}
