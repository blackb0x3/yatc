using CommandLine;

namespace YATC.CLI;

public class YatcVerb
{
    [Option('c', "channel", Required = true, HelpText = "The name of the channel to connect to.")]
    public string ChannelName { get; set; }

    [Option('u', "username", Required = true, HelpText = "The username you use to chat with.")]
    public string UserName { get; set; }
}