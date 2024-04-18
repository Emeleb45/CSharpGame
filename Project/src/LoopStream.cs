using System;
using System.Collections.Generic;
using System.Threading;
using NAudio.Wave;


public class LoopStream : WaveStream
{
    WaveStream sourceStream;



    private WaveOut waveOut;
    public LoopStream(WaveStream sourceStream)
    {
        this.sourceStream = sourceStream;
        this.EnableLooping = false;
    }
    public void playsound(string audiopath, bool loopEnabled)
    {
        if (waveOut != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;
        }

        WaveFileReader reader = new WaveFileReader(audiopath);
        LoopStream loop = new LoopStream(reader);
        waveOut = new WaveOut();
        waveOut.Init(loop);

        if (loopEnabled)
        {
            loop.EnableLooping = true;
        }

        waveOut.Play();
    }
    public void Stop()
    {
        if (waveOut != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;
        }
    }


    public bool EnableLooping { get; set; }


    public override WaveFormat WaveFormat
    {
        get { return sourceStream.WaveFormat; }
    }


    public override long Length
    {
        get { return sourceStream.Length; }
    }


    public override long Position
    {
        get { return sourceStream.Position; }
        set { sourceStream.Position = value; }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0 || sourceStream.Position > sourceStream.Length)
            {
                if (sourceStream.Position == 0 || !EnableLooping)
                {

                    break;
                }

                sourceStream.Position = 0;
            }
            totalBytesRead += bytesRead;
        }
        return totalBytesRead;
    }
}
