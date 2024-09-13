#region License GNU GPL
// ApplicationRunningHelper.cs
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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Uplauncher.Helpers
{
    public static class ApplicationRunningHelper
    {
        private const int SwRestore = 9;
        [DllImport("user32.dll")]
        private static extern
        bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern
        bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern
        bool IsIconic(IntPtr hWnd);

        /// -------------------------------------------------------------------------------------------------
        /// <summary> check if current process already running. if running, set focus to existing process and 
        ///           returns <see langword="true"/> otherwise returns <see langword="false"/>. </summary>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool AlreadyRunning()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);

            if (processes.Length <= 1)
            {
                return false;
            }

            foreach (var process in processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    IntPtr hWnd = process.MainWindowHandle;

                    // Restore the window if it is minimized.
                    if (IsIconic(hWnd))
                    {
                        ShowWindowAsync(hWnd, SwRestore);
                    }

                    // Bring the window to the foreground.
                    SetForegroundWindow(hWnd);
                    break;
                }
            }

            return true;
        }
    }
}