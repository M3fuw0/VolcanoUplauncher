#region License GNU GPL
// Constants.cs
// 
// Copyright (C) 2013 - BehaviorIsManaged
// 
// This program is free software; you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the Free Software Foundation;
// either version 2 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details. 
// You should have received a copy of the GNU General Public License along with this program; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
#endregion

using System;

namespace Uplauncher
{
    public static class Constants
    {
        public const string SiteURL = "https://pyrasis.cc/";
        public const string UpdateSiteURL = "https://update.pyrasis.cc/";
        //public const string UpdateSiteURL = "https://support.pyrasis.cc/";
        public const string SecondaryUpdateSiteURL = "https://support.pyrasis.cc/";
        //public static readonly Uri RSSNewsURL = new Uri("https://update.pyrasis.cc/rss/rss.xml");
        public const string VoteURL = "https://pyrasis.cc/vote";
        public const string DiscordURL = "https://discord.gg/ZYSuGp3dxY";
        public const string UplauncherURL = "https://update.pyrasis.cc/uplauncher/Uplauncher.exe";
        public const string UplauncherURLs = "uplauncher/Uplauncher.exe";
        public const string VersionURL = "https://update.pyrasis.cc/version/VERSION";
        public const string VersionURLs = "version/VERSION";

        public const string DofusExePath = "pyrasis_app\\Dofus.exe";
        public const string DofusRegExePath = "pyrasis_app\\reg\\Reg.exe";
        public const string LocalChecksumFile = "checksum.pyrasis";
        public const string RemotePatchFile = "patch.xml";
        public const string RemotePatchURL = "/patch.xml";
        public const string UplauncherMaj = ".\\majs\\Uplauncher.exe";
        public const string RegExePath = "reg/Reg.exe";
        public const string Version = "VERSION";
        public const string VersionPath = ".\\VERSION";
        public const string PatchPath = ".\\UplauncherReplacer.exe";

        public const string GameDirPath = ".\\pyrasis_app";
        public const string ChecksumFilePath = ".\\UplauncherReplacer.exe";
        public const string ConfigFile = "config.xml";
        public const string ConfigPath = "pyrasis_app/config.xml";

        public const string ClientId = "1271211381873705164";
        public const string GameDetails = "Joue à Pyrasis 2.51 Cheat";
        public const string WebsiteUrl = "Allez sur https://pyrasis.cc";
        public const string LargeImageKey = "large"; //large
        public const string SmallImageKey = "small"; //small
        public const string LabelButton1 = "Site \ud83d\udc12";
        public const string UrlButton1 = "https://pyrasis.cc";
        public const string LabelButton2 = "Discord ";
        public const string UrlButton2 = "https://discord.gg/ZYSuGp3dxY";

        public const string ExeReplaceTempPath = "temp_upl.exe";

        public static string CurrentExePath => System.Reflection.Assembly.GetExecutingAssembly().Location;

	    public static string ApplicationVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

	    public static string ApplicationName => "Uplauncher Pyrasis";
    }
}