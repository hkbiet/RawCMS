﻿using CommandLine;

namespace RawCMS.Client.BLL.Parser
{
    [Verb("login", HelpText = "Login. Required also (-u) username, (-p) password, (-i) client id, (-t) client secret")]
    public class LoginOptions
    {
        [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('u', "username", Required = true, HelpText = "Username")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password")]
        public string Password { get; set; }

        [Option('i', "client-id", Required = true, HelpText = "Client id")]
        public string ClientId { get; set; }

        [Option('t', "client-secret", Required = true, HelpText = "Client secret")]
        public string ClientSecret { get; set; }

        [Option('s', "server-url", Required = true, HelpText = "Server URL")]
        public string ServerUrl { get; set; }




    }
}
