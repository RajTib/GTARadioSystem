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
        private bool playlistLoaded = false;

        // ============================= Initialization  =============================
        private async Task Initialize()
        {
            //Notification.Show("Initialize Started");

            List<Song> songs = await spotify.GetPlaylist();
            //Notification.Show($"Playlist Retrieved ({songs.Count})");

            engine.LoadPlaylist(songs);
            //Notification.Show("Playlist Loaded Into Engine");

            engine.ShufflePlaylist();
            //Notification.Show("Playlist Shuffled");

            Song firstSong = engine.GetCurrentSong();

            if (firstSong != null)
            {
                previousSong = firstSong;
                songTimer = firstSong.Duration * 60;
            }

            playlistLoaded = true;
            //Notification.Show("Initialization Complete");
        }

        // ============================= Constructor  =============================
        public RadioController()
        {
            //Notification.Show("Controller Initialized");

            spotify = new SpotifyProvider();
            engine = new RadioEngine();

            //Notification.Show("Objects Created");

            _ = Initialize();

            Tick += OnTick;
            //Notification.Show("Tick Registered");
        }

        // ============================= Methods  =============================
        private void ShowNowPlaying(string title, string artist)
        {
            Notification.Show($"♫ {title}~n~{artist}");
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!playlistLoaded)
            {
                return;
            }

            bool isInCar = Game.LocalPlayerPed.IsInVehicle();

            if (isInCar)
            {
                if (songTimer > 0)
                    songTimer--;
                else
                {
                    engine.NextSong();

                    Song currentSong = engine.GetCurrentSong();

                    if (currentSong != null)
                    {
                        if (currentSong != previousSong)
                        {
                            ShowNowPlaying(currentSong.Title, currentSong.Artist);
                            previousSong = currentSong;
                        }

                        songTimer = currentSong.Duration * 60; // Convert to ticks
                    }
                }
            }

            // Vehicle Entered
            if (!wasInCar && isInCar)
            {
                //Notification.Show("Entered");
                vehicleDelay = 60; // ~5 seconds
            }

            // Vehicle Exited
            if (wasInCar && !isInCar)
            {
                //Notification.Show("Exited");
            }

            if (vehicleDelay > 0)
            {
                vehicleDelay--;

                if (vehicleDelay == 0)
                {
                    Vehicle vehicle = Game.LocalPlayerPed.CurrentVehicle;
                    Song currentSong = engine.GetCurrentSong();

                    //Notification.Show("Vehicle: " + vehicle.DisplayName);
                    vehicle.RadioStation = RadioStation.SelfRadio;

                    //Notification.Show("Self Radio");
                }
            }

            wasInCar = isInCar;
        }
    }

}
