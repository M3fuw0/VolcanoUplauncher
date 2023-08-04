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
        public const string SiteURL = "https://sulax.vg/";
        public const string UpdateSiteURL = "https://update.sulax.vg/";
        public const string SecondaryUpdateSiteURL = "https://support.sulax.vg/";
        public static readonly Uri RSSNewsURL = new Uri("https://sulax.vg/releases/rss/rss.xml");
        public const string VoteURL = "https://discord.gg/nqZhMTDeBt";
        public const string UplauncherURL = "https://sulax.vg/releases/uplauncher/Uplauncher.exe";
        public const string VersionURL = "https://sulax.vg/releases/version/VERSION";

        public const string DofusExePath = "sulax_app\\Dofus.exe";
        public const string DofusRegExePath = "sulax_app\\reg\\Reg.exe";
        public const string LocalChecksumFile = "checksum.sulax";
        public const string RemotePatchFile = "patch.xml";
        public const string UplauncherMaj = ".\\majs\\Uplauncher.exe";
        public const string RegExePath = "reg/Reg.exe";
        public const string Version = "VERSION";
        public const string VersionPath = ".\\VERSION";
        public const string PatchPath = ".\\UplauncherReplacer.exe";

        public const string ConfigFile = ".\\sulax_app\\config.xml";

        public const string ClientId = "1094708076935917630";
        public const string GameDetails = "Joue à Sulax 2.51";
        public const string WebsiteUrl = "Allez sur https://sulax.vg";
        public const string LargeImageKey = "large"; //large
        public const string SmallImageKey = "small"; //small
        public const string LabelButton1 = "Site";
        public const string UrlButton1 = "https://sulax.vg";
        public const string LabelButton2 = "Discord";
        public const string UrlButton2 = "https://discord.gg/nqZhMTDeBt";

        public const string ExeReplaceTempPath = "temp_upl.exe";

        public static string CurrentExePath => System.Reflection.Assembly.GetExecutingAssembly().Location;

	    public static string ApplicationVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

	    public static string ApplicationName => "Uplauncher Sulax";
    }
}