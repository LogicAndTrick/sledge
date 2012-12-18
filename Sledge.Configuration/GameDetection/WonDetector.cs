using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Sledge.Configuration.GameDetection
{
    public class WonDetector : IGameDetector
    {
        public string Name
        {
            get { return "WON Installed Games"; }
        }

        public void Detect()
        {
            var directoriesToScan = new List<string>();
            var software = Registry.LocalMachine.OpenSubKey("Software");
            if (software == null) return;
            var sierra = software.OpenSubKey("Sierra OnLine");
            if (sierra != null)
            {
                var setup = sierra.OpenSubKey("Setup");
                if (setup != null)
                {
                    foreach (var key in setup.GetSubKeyNames().Select(x => x.ToLower()))
                    {
                        var found = false;
                        switch (key)
                        {
                            case "halflife":
                            case "cstrike":
                            case "opforeng":
                            case "bshift":
                                found = true;
                                break;
                        }
                        if (found)
                        {
                            var gamekey = setup.OpenSubKey(key);
                            if (gamekey != null)
                            {
                                var dirkey = gamekey.GetValue("Directory");
                                if (dirkey != null)
                                {
                                    // Add
                                    directoriesToScan.Add(dirkey.ToString());
                                }
                            }
                        }
                    }
                }
            }
            var valve = software.OpenSubKey("Valve");
            if (valve != null)
            {
                var halflife = valve.OpenSubKey("Half-Life");
                if (halflife != null)
                {
                    var dirkey = halflife.GetValue("InstallPath");
                    if (dirkey != null)
                    {
                        // Add
                        directoriesToScan.Add(dirkey.ToString());
                    }
                }
            }
            foreach (var directory in directoriesToScan)
            {
                
            }
        }
    }
}
