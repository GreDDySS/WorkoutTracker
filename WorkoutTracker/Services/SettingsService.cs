namespace WorkoutTracker.Services
{
    public class SettingsService : ISettingsService
    {
        private const string KEY_WORK_TIME = "DefaultWorkTime";
        private const string KEY_REST_TIME = "DefaultRestTime";
        private const string KEY_APPROACHES = "DefaultApproaches";
        private const string KEY_SOUND = "SoundEnabled";
        private const string KEY_VIBRATION = "VibrationEnabled";
        private const string KEY_AUTO_START = "AutoStartNextExercise";
        private const string KEY_WARNING = "CountdownWarning";

        public int DefaultWorkTimeSeconds
        {
            get => Preferences.Get(KEY_WORK_TIME, 20);
            set => Preferences.Set(KEY_WORK_TIME, value);
        }

        public int DefaultRestTimeSeconds
        {
            get => Preferences.Get(KEY_REST_TIME, 10);
            set => Preferences.Set(KEY_REST_TIME, value);
        }

        public int DefaultApproaches
        {
            get => Preferences.Get(KEY_APPROACHES, 1);
            set => Preferences.Set(KEY_APPROACHES, value);
        }

        public bool SoundEnabled
        {
            get => Preferences.Get(KEY_SOUND, true);
            set => Preferences.Set(KEY_SOUND, value);
        }

        public bool VibrationEnabled
        {
            get => Preferences.Get(KEY_VIBRATION, true);
            set => Preferences.Set(KEY_VIBRATION, value);
        }

        public bool AutoStartNextExercise
        {
            get => Preferences.Get(KEY_AUTO_START, false);
            set => Preferences.Set(KEY_AUTO_START, value);
        }

        public int CountdownWarningSeconds
        {
            get => Preferences.Get(KEY_WARNING, 3);
            set => Preferences.Set(KEY_WARNING, value);
        }
    }
}
