using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Dalamud.Utility;
using NAudio.Wave;

namespace RightClickSearchInfo.Sound;

public class SoundEngine
{
    public void PlaySound(string path, float volume = 1.0f)
    {
        if (path.IsNullOrEmpty() || !File.Exists(path))
        {
            Shared.Log.Warning($"Invalid path: {path}");
            return;
        }

        if (Process.GetCurrentProcess().Id != ProcessUtils.GetForegroundProcessId())
        {
            Shared.Log.Warning("Current process is not in the foreground.");
            return;
        }

        var soundDevice = -1;

        Shared.Log.Information("Attempting to play sound: " + path);

        new Thread(() =>
        {
            WaveStream reader;
            try
            {
                reader = new AudioFileReader(path);
            }
            catch (Exception e)
            {
                Shared.Log.Error($"Could not play sound file: {e.Message}");
                return;
            }

            volume = Math.Max(0, Math.Min(volume, 1));

            using WaveChannel32 channel = new(reader)
            {
                Volume = 1 - (float)Math.Sqrt(1 - (volume * volume)),
                PadWithZeroes = false
            };

            using (reader)
            {
                using var output = new WaveOutEvent
                {
                    DeviceNumber = soundDevice
                };
                output.Init(channel);
                output.Play();

                while (output.PlaybackState == PlaybackState.Playing) Thread.Sleep(500);
            }
        }).Start();
    }
}
