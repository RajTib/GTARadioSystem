using GTA;
using GTA.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GTARadioSystem.Models;
namespace GTARadioSystem
{
    public class RadioController : Script
    {
        private bool wasInCar = false;
        private int vehicleDelay = 0;
        private SpotifyProvider spotify;
        private RadioEngine engine;
        private int songTimer = 0;
        private Song previousSong = null;

        // ============================= Initialization  =============================
        private async Task Initialize()
        {
            List<Song> songs = await spotify.GetPlaylist();
            engine.LoadPlaylist(songs);
            engine.ShufflePlaylist();
        }

        // ============================= Constructor  =============================
        public RadioController()
        {
            spotify = new SpotifyProvider();
            engine = new RadioEngine();

            _ = Initialize();

            Tick += OnTick;
        }

        // ============================= Methods  =============================
        private void ShowNowPlaying(string title, string artist)
        {
            Notification.Show($"♫ {title}~n~{artist}");
        }

        private void OnTick(object sender, EventArgs e)
        {
            bool isInCar = Game.LocalPlayerPed.IsInVehicle();

            if (isInCar)
            {
                if (songTimer > 0)
                    songTimer--;
                else{                     
                    Song currentSong = engine.GetCurrentSong();
                    if (currentSong != null && currentSong != previousSong)
                    {
                        ShowNowPlaying(currentSong.Title, currentSong.Artist);
                        previousSong = currentSong;
                        songTimer = currentSong.Duration * 1000 / 50; // Convert to ticks
                    }
                }
            }

            // Vehicle Entered
            if (!wasInCar && isInCar)
            {
                // Notification.Show("Entered");
                vehicleDelay = 60; // ~5 seconds
            }

            // Vehicle Exited
            if (wasInCar && !isInCar)
            {
                // Notification.Show("Exited");
            }

            if (vehicleDelay > 0)
            {
                vehicleDelay--;

                if (vehicleDelay == 0)
                {
                    Vehicle vehicle = Game.LocalPlayerPed.CurrentVehicle;
                    Song currentSong = engine.GetCurrentSong();

                    if (currentSong != null)
                        ShowNowPlaying(currentSong.Title, currentSong.Artist);

                    // Notification.Show("Vehicle: " + vehicle.DisplayName);
                    vehicle.RadioStation = RadioStation.SelfRadio;

                    Notification.Show(vehicle.RadioStation.ToString());
                }
            }

            wasInCar = isInCar;
        }
    }

}
