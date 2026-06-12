using GTA;
using GTA.UI;
using System;
using System.IO;
using System.Diagnostics;
// using Windows.Media.Control;

namespace GTARadioSystem
{
    public class SpotifyController
    {
        [Obsolete]
        public SpotifyController()
        {
            Notification.Show("Spotify Controller Initialized");
        }

        [Obsolete]
        public void Test()
        {
            Process[] spotifyProcesses = Process.GetProcessesByName("Spotify");

            Notification.Show("Spotify Processes Found: " + spotifyProcesses.Length);
        }

        public string GetCurrentSong()
        {
            string[] data = File.ReadAllLines("D:\\Noctivium\\Dev\\Projects\\Active\\gta-radio-system\\spotify-test\\current_song.txt");
            return data[0];
        }

        public string GetCurrentArtist()
        {
            // Implement Spotify API integration to get the current artist
            string[] data = File.ReadAllLines("D:\\Noctivium\\Dev\\Projects\\Active\\gta-radio-system\\spotify-test\\current_artist.txt");
            return data[0];
        }
    }
}
