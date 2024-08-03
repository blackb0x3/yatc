using System.Net;
using CommandLine;
using YATC.Core;

namespace YATC.CLI;

public class App
{
    private readonly IYatcLogger _logger;
    private readonly ITwitchAuthenticationService _twitchAuthService;

    public App(IYatcLogger logger, ITwitchAuthenticationService twitchAuthService)
    {
        _logger = logger;
        _twitchAuthService = twitchAuthService;
    }

    public async Task Run(string[] args, CancellationToken cancellationToken)
    {
        try
        {
            var parserResults = Parser.Default
                .ParseArguments<YatcVerb>(args);

            await parserResults.WithParsedAsync(
                async parsedArgs => await RunInternal(parsedArgs, cancellationToken)
            );

            parserResults.WithNotParsed(err => LogErrors(err));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task RunInternal(YatcVerb parsedArgs, CancellationToken cancellationToken)
    {
        // TODO
        var oauthToken = await _twitchAuthService.AuthenticateTwitchUser(cancellationToken);

        _logger.LogDebug(new { msg = "Authenticated through Twitch", oauthToken = new string('*', oauthToken.Length) });
        // connect to channel
        // log messages
    }

    private void LogErrors(IEnumerable<Error> errors)
    {
        _logger.LogError(new { msg = "error/s encountered", errors });
    }
}