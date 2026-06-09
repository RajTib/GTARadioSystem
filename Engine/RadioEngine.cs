using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

class RadioEngine
{
    private List<Song> playlist;
    private int currentSongIndex;
    private Random rng;

    public RadioEngine()
    {
        playlist = new List<Song>();
        currentSongIndex = 0;
        rng = new Random();
    }

    public void LoadPlaylist(List<Song> songs)
    {
        playlist = songs;
        currentSongIndex = 0;
    }

    public Song GetCurrentSong()
    {
        if (playlist.Count == 0)
        {
            return null;
        }   

        return playlist[currentSongIndex];
    }

    public void NextSong()
    {
        currentSongIndex++;

        if (currentSongIndex >= playlist.Count)
        {
            currentSongIndex = 0;
        }
    }

    public void ShufflePlaylist()
    {
        for (int i = playlist.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);

            Song temp = playlist[i];
            playlist[i] = playlist[j];
            playlist[j] = temp;
        }

        currentSongIndex = 0;
    }
}