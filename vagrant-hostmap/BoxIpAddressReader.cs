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
            List<string> ipAddressesFound = new List<string>();

            //
            // Get interface names
            // Note, don't depend on 'eth#" interface names, see https://superuser.com/a/1086705
            //

            SshCommand netInterfaceAddrCmd = client.CreateCommand("ip -o -f inet addr");
            netInterfaceAddrCmd.Execute();

            Dictionary<string, string> interfaceToIpAddr = IpCmdOutputReader.GetInterfaceIPAddressMapFromINetAddrOneLineOutput(netInterfaceAddrCmd.Result);

            foreach(KeyValuePair<string, string> entry in interfaceToIpAddr)
            {
                string ip = entry.Value;
                if(ip == "localhost" || ip == "127.0.0.1")
                {
                    continue;
                }

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
