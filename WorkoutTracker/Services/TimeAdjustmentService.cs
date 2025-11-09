namespace WorkoutTracker.Services
{
    public class TimeAdjustmentService : ITimeAdjustmentService
    {
        public int AdjustTime(int currentTime, string direction, int step = 5, int minValue = 0)
        {
            if (direction == "+")
            {
                return currentTime + step;
            }
            else if (direction == "-" && currentTime > minValue + step)
            {
                return currentTime - step;
            }
            return currentTime;
        }
    }
}