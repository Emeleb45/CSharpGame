
using System;
using System.Collections.Generic;
using System.Threading;
using NAudio.Wave;

public class AudioPlayer
{
    private Dictionary<Thread, WaveOutEvent> audioThreads = new Dictionary<Thread, WaveOutEvent>();
    private bool quitRequested = false;

    public Thread PlayAudioAsync(string audioFile, bool loop)
    {
        Thread audioThread = new Thread(() => PlayAudio(audioFile, loop));
        audioThreads.Add(audioThread, new WaveOutEvent());
        audioThread.Start();
        return audioThread;
    }

    public void StopAudioThread(Thread thread)
    {
        if (audioThreads.TryGetValue(thread, out var waveOutEvent))
        {
            waveOutEvent.Stop();
            thread.Join();
            waveOutEvent.Dispose();
            audioThreads.Remove(thread);
        }
    }

    public void StopAllAudioThreads()
    {
        foreach (var kvp in audioThreads)
        {
            kvp.Value.Stop();
            kvp.Value.Dispose();
        }
        audioThreads.Clear();
    }

    public void WaitForAllAudioThreads()
    {
        foreach (var thread in audioThreads.Keys)
        {
            thread.Join();
        }
    }

    private void PlayAudio(string audioFile, bool loop)
    {
        try
        {
            while (!quitRequested)
            {
                using (var audioFileReader = new AudioFileReader(audioFile))
                using (var outputDevice = new WaveOutEvent())
                {
                    audioThreads[Thread.CurrentThread] = outputDevice;

                    outputDevice.Init(audioFileReader);

                    outputDevice.Play();

                    while (!quitRequested && (loop || outputDevice.PlaybackState == PlaybackState.Playing))
                    {
                        Thread.Sleep(100);
                    }
                }

                // If not looping or quit requested, exit the loop
                if (!loop || quitRequested)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing audio: {ex.Message}");
        }
    }


}
