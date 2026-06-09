using System.Runtime.CompilerServices;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using GTARadioSystem.Models;

public class SpotifyProvider
{
    public async Task<List<Song>> GetPlaylist()
    {
        Dictionary<string, string> env = new Dictionary<string, string>();

        foreach (string line in File.ReadAllLines(".env"))
        {
            string[] parts = line.Split('=');

            env[parts[0]] = parts[1];
        }

        string clientId = env["SPOTIFY_CLIENT_ID"];
        string clientSecret = env["SPOTIFY_CLIENT_SECRET"];
        string playlistId = env["PLAYLIST_ID"];

        List<Song> playlist = new List<Song>();

        var server = new EmbedIOAuthServer(
            new Uri("http://127.0.0.1:5000/callback"),
            5000);

        await server.Start();

        server.AuthorizationCodeReceived += async (sender, response) =>
        {
            await server.Stop();

            var token =
                await new OAuthClient().RequestToken(
                    new AuthorizationCodeTokenRequest(
                        clientId,
                        clientSecret,
                        response.Code,
                        new Uri("http://127.0.0.1:5000/callback")));

            var spotify = new SpotifyClient(token.AccessToken);

            var tracks =
                await spotify.Playlists.GetPlaylistItems(
                    playlistId);

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

        BrowserUtil.Open(request.ToUri());

        while (playlist.Count == 0)
        {
            await Task.Delay(500);
        }

        return playlist;
    }
}
