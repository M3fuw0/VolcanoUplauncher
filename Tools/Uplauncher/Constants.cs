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
        public /*const*/ static string SiteURL = "https://pyrasis.cc/";
        public /*const*/ static string UpdateSiteURL = "https://update.sulax.vg/";
        //public /*const*/ static string UpdateSiteURL = "https://support.pyrasis.cc/";
        public /*const*/ static string SecondaryUpdateSiteURL = "https://update.sulax.vg/";
        //public static readonly Uri RSSNewsURL = new Uri("https://update.pyrasis.cc/rss/rss.xml");
        public /*const*/ static string VoteURL = "https://pyrasis.cc/vote";
        public /*const*/ static string DiscordURL = "https://discord.gg/ZYSuGp3dxY";
        public /*const*/ static string UplauncherURL = "https://update.sulax.vg/uplauncher/Uplauncher.exe";
        public /*const*/ static string UplauncherURLs = "uplauncher/Uplauncher.exe";
        public /*const*/ static string VersionURL = "https://update.sulax.vg/version/VERSION";
        public /*const*/ static string VersionURLs = "version/VERSION";

        public /*const*/ static string DofusExePath = @".\\vulcano_app\\Dofus.exe";
        public /*const*/ static string DofusRegExePath = @".\\vulcano_app\\reg\\Reg.exe";
        public /*const*/ static string LocalChecksumFile = "checksum.vulcano";
        public /*const*/ static string RemotePatchFile = "patch.xml";
        public /*const*/ static string RemotePatchURL = ".\\patch.xml";
        public /*const*/ static string UplauncherMaj = @".\majs\Uplauncher.exe";
        public /*const*/ static string RegExePath = "reg/Reg.exe";
        public /*const*/ static string Version = "VERSION";
        public /*const*/ static string VersionPath = ".\\VERSION";
        public /*const*/ static string PatchPath = ".\\UplauncherReplacer.exe";

        public /*const*/ static string GameDirPath = ".\\vulcano_app";
        public /*const*/ static string ChecksumFilePath = ".\\UplauncherReplacer.exe";
        public /*const*/ static string ConfigFile = "config.xml";
        public /*const*/ static string ConfigPath = @".\\vulcano_app\\config.xml";

        public /*const*/ static string ClientId = "1271211381873705164";
        public /*const*/ static string GameDetails = "Joue à Pyrasis 2.51 Cheat";
        public /*const*/ static string WebsiteUrl = "Allez sur https://pyrasis.cc";
        public /*const*/ static string LargeImageKey = "large"; //large
        public /*const*/ static string SmallImageKey = "small"; //small
        public /*const*/ static string LabelButton1 = "Site \ud83d\udc12";
        public /*const*/ static string UrlButton1 = "https://pyrasis.cc";
        public /*const*/ static string LabelButton2 = "Discord ";
        public /*const*/ static string UrlButton2 = "https://discord.gg/ZYSuGp3dxY";

        public /*const*/ static string ExeReplaceTempPath = "temp_upl.exe";

        public static string CurrentExePath => System.Reflection.Assembly.GetExecutingAssembly().Location;

	    public static string ApplicationVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

	    public static string ApplicationName => "Uplauncher Vulcano";
    }
}