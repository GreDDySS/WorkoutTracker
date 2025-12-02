using System.Text;
using Microsoft.Maui.Devices;
using Plugin.Maui.Audio;

namespace WorkoutTracker.Services
{
    public class WorkoutNotificationService : IWorkoutNotificationService
    {
        private readonly ISettingsService _settingsService;
        private readonly IAudioManager _audioManager;
        private readonly Lazy<Task<IAudioPlayer>> _countdownPlayer;
        private readonly object _vibrationLock = new();

        public WorkoutNotificationService(ISettingsService settingsService, IAudioManager audioManager)
        {
            _settingsService = settingsService;
            _audioManager = audioManager;
            _countdownPlayer = new Lazy<Task<IAudioPlayer>>(CreateBeepPlayerAsync);
        }

        public async Task PlayCountdownTickAsync()
        {
            await PlaySoundAsync();
            TriggerVibration(TimeSpan.FromMilliseconds(120));
        }

        public async Task PlayPhaseChangeAsync(bool isWorkPhase)
        {
            await PlaySoundAsync();
            var vibrationDuration = isWorkPhase ? 300 : 180;
            TriggerVibration(TimeSpan.FromMilliseconds(vibrationDuration));
        }

        private async Task PlaySoundAsync()
        {
            if (!_settingsService.SoundEnabled)
            {
                return;
            }

            try
            {
                var player = await _countdownPlayer.Value;
                if (player == null)
                {
                    return;
                }

                if (player.IsPlaying)
                {
                    player.Stop();
                }

                player.Seek(0);
                player.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sound playback error: {ex.Message}");
            }
        }

        private void TriggerVibration(TimeSpan duration)
        {
            if (!_settingsService.VibrationEnabled)
            {
                return;
            }

            lock (_vibrationLock)
            {
                try
                {
                    if (Vibration.Default.IsSupported)
                    {
                        Vibration.Default.Vibrate(duration);
                    }
                    else if (HapticFeedback.Default.IsSupported)
                    {
                        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                    }
                }
                catch (FeatureNotSupportedException)
                {
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Vibration error: {ex.Message}");
                }
            }
        }

        private async Task<IAudioPlayer> CreateBeepPlayerAsync()
        {
            if (_audioManager == null)
            {
                return null;
            }

            var stream = await Task.Run(CreateBeepStream);
            return _audioManager.CreatePlayer(stream);
        }

        private Stream CreateBeepStream()
        {
            const int sampleRate = 16000;
            const int frequency = 1200;
            const double durationSeconds = 0.2;
            int sampleCount = (int)(sampleRate * durationSeconds);
            int dataChunkSize = sampleCount * sizeof(short);
            int fileSize = 36 + dataChunkSize;

            var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: true);

            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(fileSize);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));

            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(sampleRate);
            writer.Write(sampleRate * sizeof(short));
            writer.Write((short)sizeof(short));
            writer.Write((short)16);

            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(dataChunkSize);

            double amplitude = short.MaxValue * 0.2;

            for (int i = 0; i < sampleCount; i++)
            {
                short sample = (short)(amplitude * Math.Sin(2 * Math.PI * frequency * i / sampleRate));
                writer.Write(sample);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
