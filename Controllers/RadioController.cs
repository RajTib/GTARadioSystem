using GTA;
using GTA.UI;
using System;
namespace GTARadioSystem
{
    public class RadioController : Script
    {
        private bool wasInCar = false;
        private int vehicleDelay = 0;
        private SpotifyProvider spotify;
        private RadioEngine engine;

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

            Initialize();

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
