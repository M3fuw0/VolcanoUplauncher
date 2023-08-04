using System;
using System.Diagnostics;
using System.IO;
using DiscordRPC.Logging;

namespace DiscordRPC.Registry
{
    internal class UnixUriSchemeCreator : IUriSchemeCreator
    {
        private readonly ILogger logger;

        public UnixUriSchemeCreator(ILogger logger)
        {
            this.logger = logger;
        }

        public bool RegisterUriScheme(UriSchemeRegister register)
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(home))
            {
                logger.Error("Failed to register because the HOME variable was not set.");
                return false;
            }

            var exe = register.ExecutablePath;
            if (string.IsNullOrEmpty(exe))
            {
                logger.Error("Failed to register because the application was not located.");
                return false;
            }

            //Prepare the command
            string command = null;
            if (register.UsingSteamApp)
                //A steam command isntead
                command = $"xdg-open steam://rungameid/{register.SteamAppID}";
            else
                //Just a regular discord command
                command = exe;


            //Prepare the file
            var desktopFileFormat =
                @"[Desktop Entry]
Name=Game {0}
Exec={1} %u
Type=Application
NoDisplay=true
Categories=Discord;Games;
MimeType=x-scheme-handler/discord-{2}";

            var file = string.Format(desktopFileFormat, register.ApplicationID, command, register.ApplicationID);

            //Prepare the path
            var filename = $"/discord-{register.ApplicationID}.desktop";
            var filepath = home + "/.local/share/applications";
            var directory = Directory.CreateDirectory(filepath);
            if (!directory.Exists)
            {
                logger.Error("Failed to register because {0} does not exist", filepath);
                return false;
            }

            //Write the file
            File.WriteAllText(filepath + filename, file);

            //Register the Mime type
            if (!RegisterMime(register.ApplicationID))
            {
                logger.Error("Failed to register because the Mime failed.");
                return false;
            }

            logger.Trace("Registered {0}, {1}, {2}", filepath + filename, file, command);
            return true;
        }

        private bool RegisterMime(string appid)
        {
            //Format the arguments
            var format = "default discord-{0}.desktop x-scheme-handler/discord-{0}";
            var arguments = string.Format(format, appid);

            //Run the process and wait for response
            var process = Process.Start("xdg-mime", arguments);
            process.WaitForExit();

            //Return if succesful
            return process.ExitCode >= 0;
        }
    }
}