using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

class RadioEngine
{
    static  string workingDir = Directory.GetCurrentDirectory();
    static string filePath = Path.Combine(workingDir, "playlists.txt");
    static string[] lines = File.ReadAllLines(filePath);
    static Random rng = new Random();

    static void Main()
    {
        List<Song> playlist = new List<Song>();

        foreach (string line in lines)
        {
            playlist.Add(parseSong(line));
        }

        while (true)
        {
            ShufflePlaylist(playlist);

            foreach (Song song in playlist)
            {
                playSong(song);
            }
        }
    }

    static void playSong(Song song)
    {
        Console.Clear();

        Console.WriteLine("================================");
        Console.WriteLine("        SPOTIFY FM");
        Console.WriteLine("================================");

        Console.WriteLine();
        Console.WriteLine($"Now Playing: {song.Title}");
        Console.WriteLine($"Artist: {song.Artist}");

        Thread.Sleep(song.Duration * 1000);
    }

    static Song parseSong(string line)
    {
        string[] parts = line.Split(",");
        Song song = new Song();
        int duration = int.Parse(parts[2]);

        song.Title = parts[0];
        song.Artist = parts[1];
        song.Duration = duration;

        return song;
    }

    static void ShufflePlaylist(List<Song> playlist)
    {
        for (int i = playlist.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);

            Song temp = playlist[i];
            playlist[i] = playlist[j];
            playlist[j] = temp;
        }
    }
}
