using GTA;
using GTA.UI;
using System;
using System.IO;
using System.Net.Mail;
namespace GTARadioSystem
{
    public class RadioController : Script
    {
        private bool wasInCar = false;
        private SpotifyController spotify;
        private int vehicleDelay = 0;
        private string lastSong = "";
        private string lastArtist = "";
        private int songCheckDelay = 0;

        // ============================= Constructor  =============================
        [Obsolete]
        public RadioController()
        {
            spotify = new SpotifyController();

            Tick += OnTick;
            // Notification.Show("Radio Controller Initialized");

            spotify.Test();
        }

        // ============================= Methods  =============================
        private void ShowNowPlaying(string title, string artist)
        {
            Notification.Show($"♫ {title}~n~{artist}");
        }

        [Obsolete]
        private void OnTick(object sender, EventArgs e)
        {
            bool isInCar = Game.LocalPlayerPed.IsInVehicle();

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

                    // Notification.Show("Vehicle: " + vehicle.DisplayName);
                    vehicle.RadioStation = RadioStation.SelfRadio;

                    Notification.show(vehicle.RadioStation.ToString());
                }
            }

            if (isInCar)
            {
                if (songCheckDelay > 0)
                {
                    songCheckDelay--;
                }
                else
                {
                    string title = spotify.GetCurrentSong();
                    string artist = spotify.GetCurrentArtist();
                    Vehicle vehicle = Game.LocalPlayerPed.CurrentVehicle;

                    if (title != lastSong || artist != lastArtist)
                    {
                        // ShowNowPlaying(
                        //     vehicle.RadioStation.ToString(),
                        //     "Current Radio"
                        // );

                        lastSong = title;
                        lastArtist = artist;
                    }

                    songCheckDelay = 60; // ~1 second
                }
            }

            wasInCar = isInCar;
        }
    }

}
