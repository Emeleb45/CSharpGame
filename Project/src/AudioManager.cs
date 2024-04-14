using System;
using System.Collections.Generic;
using System.Threading;
using NAudio.Wave;
public class AudioManager
{
    private LoopStream backgroundMusic;
    private List<WaveOut> effectPlayers;


    public AudioManager()
    {
        effectPlayers = new List<WaveOut>();
    }

    public void PlayBackgroundMusic(string audioFile)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        backgroundMusic = new LoopStream(new WaveFileReader(audioFile));

        backgroundMusic.playsound(audioFile, true);
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }
    }
    public void PlayEffect(string audioFile)
    {
        WaveOut waveOut = new WaveOut();
        WaveFileReader reader = new WaveFileReader(audioFile);
        waveOut.Init(reader);
        waveOut.Play();
        effectPlayers.Add(waveOut);
    }


    public void StopEffects()
    {
        foreach (var player in effectPlayers)
        {
            player.Stop();
            player.Dispose();
        }
        effectPlayers.Clear();
    }
}