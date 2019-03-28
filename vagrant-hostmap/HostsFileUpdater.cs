using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace VagrantHostmap
{
    class HostsFileUpdater
    {
        public string GetEntryForHostname(string hostname)
        {
            string[] lines = getHostsFileLines();

            for (int i = 0; i < lines.Length; i++) {
                if(lines[i].EndsWith(hostname))
                {
                    return lines[i];
                }
            }

            return null;
        }

        public string UpdateEntry(string ipAddress, string hostname, string existingEntry)
        {
            string whitespaceSepBlock = "";
            for (int i = 0; i < existingEntry.Length; i++)
            {
                if (char.IsWhiteSpace(existingEntry[i]))
                {
                    whitespaceSepBlock += existingEntry[i];
                }
            }

            var updatedEntry = ipAddress + whitespaceSepBlock + hostname;

            var curHostsLines = getHostsFileLines();
            for(int i=0; i<curHostsLines.Length; i++)
            {
                if(curHostsLines[i] == existingEntry)
                {
                    curHostsLines[i] = updatedEntry;
                }
            }

            File.WriteAllLines(getTempHostsFilePath(), curHostsLines);

            return updatedEntry;
        }

        public string AddEntry(string ipAddress, string hostname)
        {
            var newEntry = ipAddress + "     " + hostname;

            var curHostsLines = getHostsFileLines();
            List<string> newHostsLines = new List<string>(curHostsLines);
            newHostsLines.Add(newEntry);

            File.WriteAllLines(getTempHostsFilePath(), newHostsLines.ToArray());

            return newEntry;
        }

        public void CopyTempToActual()
        {
            var args = "\"" + getTempHostsFilePath() + "\" " + "\"" + getHostsFilePath() + "\" /f /h /-y";
            ExecuteAsAdmin("xcopy", args);

            File.Delete(getTempHostsFilePath());
        }

        private void ExecuteAsAdmin(string command, string arguments)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C " + command + " " + arguments,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    Verb = "runas"
                }
            };

            proc.Start();
            proc.WaitForExit();
        }

        private string getTempHostsFilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/vagrant-hostmap.hosts.tmp";
        }

        private string getHostsFilePath()
        {
            return Environment.SystemDirectory + "/drivers/etc/hosts";
        }

        private string[] getHostsFileLines()
        {
            var path = getHostsFilePath();
            string[] lines = File.ReadAllLines(path);
            return lines;
        }

    }
}
