using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace VagrantHostmap
{
    class BoxIpAddressReader
    {
        protected SshClient client;

        public BoxIpAddressReader(SshClient sshClient)
        {
            client = sshClient;
        }

        public List<string> GetIpAddresses()
        {
            // Note, don't depend on 'eth#" interface names, see https://superuser.com/a/1086705

            SshCommand netInterfaceNamesCmd = client.CreateCommand("ifconfig -a | sed 's/[ \t].*//;/^\\(lo\\|\\)$/d'");
            netInterfaceNamesCmd.Execute();
            string[] netInterfaceNames = netInterfaceNamesCmd.Result.Trim().Split('\n');

            List<string> ipAddressesFound = new List<string>();
            foreach(string interfaceName in netInterfaceNames)
            {
                SshCommand netData = client.CreateCommand("ifconfig " + interfaceName + " | grep 'inet addr:' | cut -d: -f2 | awk '{ print $1}'");
                netData.Execute();
                string ip = netData.Result.Trim();

                ipAddressesFound.Add(ip);
            }

            return ipAddressesFound;
        }

        public List<string> GetConnectableIpAddresses(List<string> ipAddresses)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < ipAddresses.Count; i++) {
                Ping pingSender = new Ping();
                PingReply pr = pingSender.Send(ipAddresses[i]);

                if(pr.Status == IPStatus.Success)
                {
                    result.Add(ipAddresses[i]);
                }
            }

            return result;
        }

    }
}
