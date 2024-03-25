using System;
using System.Collections.Generic;
using System.Threading;
using NAudio.Wave;

public class AudioPlayer
{
    private Dictionary<Thread, WaveOutEvent> audioThreads = new Dictionary<Thread, WaveOutEvent>();
    private readonly object locker = new object();

    public Thread PlayAudioAsync(string audioFile, bool loop)
    {
        Thread audioThread = new Thread(() => PlayAudio(audioFile, loop));
        audioThread.Start();
        return audioThread;
    }

    public void StopAudioThread(Thread thread)
    {
        lock (locker)
        {
            if (audioThreads.TryGetValue(thread, out var waveOutEvent))
            {
                waveOutEvent.Stop();
                thread.Join();
                waveOutEvent.Dispose();
                audioThreads.Remove(thread);
            }
        }
    }

    public void StopAllAudioThreads()
    {
        lock (locker)
        {
            foreach (var kvp in audioThreads)
            {
                kvp.Value.Stop();
                kvp.Value.Dispose();
            }
            audioThreads.Clear();
        }
    }

    public void WaitForAllAudioThreads()
    {
        lock (locker)
        {
            foreach (var thread in audioThreads.Keys)
            {
                thread.Join();
            }
        }
    }

    private void PlayAudio(string audioFile, bool loop)
    {
        try
        {
            using (var audioFileReader = new AudioFileReader(audioFile))
            using (var outputDevice = new WaveOutEvent())
            {
                lock (locker)
                {
                    audioThreads[Thread.CurrentThread] = outputDevice;
                }

                outputDevice.Init(audioFileReader);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }

                if (loop)
                {
                    outputDevice.Stop();
                    outputDevice.Dispose();
                    PlayAudio(audioFile, loop);
                }
                else
                {
                    lock (locker)
                    {
                        audioThreads.Remove(Thread.CurrentThread);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing audio: {ex.Message}");
        }
    }
}