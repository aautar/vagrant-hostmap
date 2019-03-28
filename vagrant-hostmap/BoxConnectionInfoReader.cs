using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace VagrantHostmap
{
    class BoxConnectionInfoReader
    {
        protected string dir;

        public BoxConnectionInfoReader(string vagrantDir)
        {
            dir = vagrantDir;
        }

        public ConnectionInfo GetSshConnectionInfo()
        {
            Directory.SetCurrentDirectory(dir);

            string rawSshInfo = this.ExecuteVagrantSshInfo();

            var allLines = rawSshInfo.Split(',');
            for(int i=0; i<allLines.Length; i++)
            {
                if(allLines[i] == "Vagrant::Errors::NoEnvironmentError")
                {
                    return null;
                }
            }


            var infoLineSplit = allLines[7].Split(new string[] { "\\n" }, StringSplitOptions.None);
            for(int i=0; i<infoLineSplit.Length; i++)
            {
                infoLineSplit[i] = infoLineSplit[i].Trim();
            }

            var host = GetFieldValueFromSshInfoLines(infoLineSplit, "HostName");
            var port = int.Parse(GetFieldValueFromSshInfoLines(infoLineSplit, "Port"));
            var user = GetFieldValueFromSshInfoLines(infoLineSplit, "User");
            var identityFile = GetFieldValueFromSshInfoLines(infoLineSplit, "IdentityFile");

            return new ConnectionInfo(
                host,
                port,
                user,
                new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile(identityFile))
            );
        }

        private string GetFieldValueFromSshInfoLines(string[] sshInfoLines, string fieldName)
        {
            for (int i = 0; i < sshInfoLines.Length; i++)
            {
                if(sshInfoLines[i].StartsWith(fieldName))
                {
                    var fieldLine = sshInfoLines[i];
                    return (fieldLine.Remove(0, fieldName.Length)).Trim();
                }
            }

            return "";
        }

        private string ExecuteVagrantSshInfo()
        {
            string stdOutput = "";
            string stdError = "";

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "vagrant",
                    Arguments = "ssh-config --machine-readable",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false
                }
            };

            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                stdOutput += proc.StandardOutput.ReadLine();
            }

            while (!proc.StandardError.EndOfStream)
            {
                stdError += proc.StandardError.ReadLine();
            }

            if (stdError.Length > 0)
            {
                throw new InvalidOperationException(stdError);
            }

            return stdOutput;
        }
    }
}
