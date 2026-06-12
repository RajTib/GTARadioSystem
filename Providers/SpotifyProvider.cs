using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net;

using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

using GTA;
using GTA.UI;

using GTARadioSystem.Models;

public class SpotifyProvider
{
    public async Task<List<Song>> GetPlaylist()
    {
        ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls12;
        
        try
        {
            Notification.Show("GetPlaylist Started");
            Dictionary<string, string> env = new Dictionary<string, string>();

            //Notification.Show(File.Exists(".env").ToString());
            //Notification.Show(File.Exists("Config\\.env").ToString());

            foreach (string line in File.ReadAllLines("scripts\\.env"))
            {
                string[] parts = line.Split('=');

                env[parts[0]] = parts[1];
            }

            string clientId = env["SPOTIFY_CLIENT_ID"];
            string clientSecret = env["SPOTIFY_CLIENT_SECRET"];
            string playlistId = env["PLAYLIST_ID"];

            //Notification.Show(clientId);
            //Notification.Show(clientSecret);
            //Notification.Show(playlistId);


            //Notification.Show("Env Loaded");

            List<Song> playlist = new List<Song>();

            //Notification.Show("Creating OAuth Server");
            var server = new EmbedIOAuthServer(
                new Uri("http://127.0.0.1:5000/callback"),
                5000);

            await server.Start();
            //Notification.Show("OAuth Server Started");

            server.AuthorizationCodeReceived += async (sender, response) =>
            {
                try
                {
                    //Notification.Show("AUTH CALLBACK");

                    //Notification.Show("STOPPING SERVER");


                    //Notification.Show("SERVER STOPPED");

                    //Notification.Show("REQUESTING TOKEN");

                    var token =
                        await new OAuthClient().RequestToken(
                            new AuthorizationCodeTokenRequest(
                                clientId,
                                clientSecret,
                                response.Code,
                                new Uri("http://127.0.0.1:5000/callback")));

                    await server.Stop();
                    
                    Notification.Show("TOKEN RECEIVED");

                    var spotify = new SpotifyClient(token.AccessToken);

                    Notification.Show("CLIENT CREATED");

                    var tracks =
                        await spotify.Playlists.GetPlaylistItems(
                            playlistId);

                    Notification.Show("TRACKS RECEIVED");

                    foreach (var item in tracks.Items)
                    {
                        if (item.Item is FullTrack track)
                        {
                            Song song = new Song();
                            song.Title = track.Name;
                            song.Artist = track.Artists[0].Name;
                            song.Duration = track.DurationMs / 1000;

                            playlist.Add(song);
                        }
                    }

                    Notification.Show($"SONGS={playlist.Count}");
                }
                catch (Exception ex)
                {
                    File.WriteAllText(
                        "scripts\\error.txt",
                        ex.ToString());
                }
            };

            var request = new LoginRequest(
                server.BaseUri,
                clientId,
                LoginRequest.ResponseType.Code)
            {
                Scope = new[]
                {
                Scopes.PlaylistReadPrivate,
                Scopes.PlaylistReadCollaborative
            }
            };
            //Notification.Show(request.ToUri().ToString());

            //Notification.Show("Opening Browser");
            BrowserUtil.Open(request.ToUri());

            while (playlist.Count == 0)
            {
                await Task.Delay(500);
            }

            return playlist;
        }
        catch(Exception ex)
        {
            Notification.Show("Error: " + ex.Message);
            return new List<Song>();
        }
    }
}
