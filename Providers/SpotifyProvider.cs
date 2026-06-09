using System.Runtime.CompilerServices;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

class SpotifyProvider
{
    static async Task Main()
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
        
        var server = new EmbedIOAuthServer(
            new Uri("http://127.0.0.1:5000/callback"),
            5000);

        await server.Start();

        server.AuthorizationCodeReceived += async (sender, response) =>
        {
            await server.Stop();

            Console.WriteLine("Authorization successful!");

            var token =
                await new OAuthClient().RequestToken(
                    new AuthorizationCodeTokenRequest(
                        clientId,
                        clientSecret,
                        response.Code,
                        new Uri("http://127.0.0.1:5000/callback")));

            var spotify = new SpotifyClient(token.AccessToken);

            string playlistId = "59sHwgujUkNHEIPyIlDtuP";

            var tracks =
                await spotify.Playlists.GetPlaylistItems(
                    playlistId);

            Console.WriteLine("Playlist items fetched!");

            Console.WriteLine(
                $"Items count: {tracks.Items.Count}"
            );

            foreach (var item in tracks.Items)
            {
                if (item.Item is FullTrack track)
                {
                    Console.WriteLine(track.Name);

                    Console.WriteLine(track.Artists[0].Name);

                    Console.WriteLine(track.DurationMs / 1000);

                    Console.WriteLine("----------------");
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

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}
