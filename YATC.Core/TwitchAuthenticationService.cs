using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace YATC.Core;

public interface ITwitchAuthenticationService
{
    Task<string> AuthenticateTwitchUser(CancellationToken cancellationToken);
}

public class TwitchAuthenticationService : ITwitchAuthenticationService
{
    private readonly IYatcLogger _logger;
    private readonly IConfiguration _config;

    public TwitchAuthenticationService(IYatcLogger logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task<string> AuthenticateTwitchUser(CancellationToken cancellationToken)
    {
        _logger.LogDebug(new { msg = "Opening Twitch auth URL" });

        var authServer = StartAuthCallbackServer();

        await Task.Delay(5000, cancellationToken);

        var clientId = _config["TwitchAppClientId"] ?? throw new Exception("TwitchAppClientId missing!");
        var authRedirectUrl = _config["TwitchAuthRedirectUrl"] ?? throw new Exception("TwitchAuthRedirectUrl missing!");
        var twitchAuthUrl = $"https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={clientId}&redirect_uri={authRedirectUrl}&scope=chat:read+chat:edit";

        OpenUrl(twitchAuthUrl);

        _logger.LogDebug(new { msg = "Listening for Oauth callback" });

        var listener = StartHttpListener();
        //var tokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "twitchToken.json");
        var oauthToken = await WaitForAccessToken(listener, cancellationToken);

        authServer.Close();

        return oauthToken;
    }

    private async Task<string> WaitForAccessToken(HttpListener listener, CancellationToken cancellationToken)
    {
        while (true)
        {
            var context = await listener.GetContextAsync();
            var request = context.Request;
            if (request.HttpMethod == "GET" && request.QueryString["access_token"] != null)
            {
                var accessToken = request.QueryString["access_token"];

                // Send response
                context.Response.StatusCode = 200;
                await context.Response.OutputStream.WriteAsync(Array.Empty<byte>().AsMemory(0, 0), cancellationToken);

                return accessToken ?? string.Empty;
            }
        }
    }

    private Process StartAuthCallbackServer()
    {
        var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var nodeServerPath = Path.Combine(projectDirectory, "server.js");

        var startInfo = new ProcessStartInfo("node", nodeServerPath)
        {
            UseShellExecute = false,
            CreateNoWindow = false
        };

        var process = Process.Start(startInfo);

        if (process is null)
        {
            throw new Exception("Failed to start auth callback server");
        }

        return process;
    }

    private HttpListener StartHttpListener()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:1234/");
        listener.Start();
        return listener;
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}