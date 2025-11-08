using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker.Services
{
    public interface IWorkoutTimerService
    {
        event EventHandler<int> TimeElapsed;
        event EventHandler PhaseCompleted;

        void Start();
        void Pause();
        void Resume();
        void Stop();
        void Dispose();
    }
}
