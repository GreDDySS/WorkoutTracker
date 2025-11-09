namespace WorkoutTracker.Services
{
    public interface ITimeAdjustmentService
    {
        int AdjustTime(int currentTime, string direction, int step = 5, int minValue = 0);
    }
}