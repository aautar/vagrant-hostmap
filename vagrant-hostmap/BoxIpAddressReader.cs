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
            List<string> result = new List<string>();

            for (int i = 0; i < 25; i++)
            {
                SshCommand netData = client.CreateCommand("ifconfig eth" + i.ToString() + " | grep 'inet addr:' | cut -d: -f2 | awk '{ print $1}'");
                netData.Execute();
                string ip = netData.Result.Trim();

                if (ip.Length == 0)
                {
                    break;
                }

                result.Add(ip);
            }

            return result;
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
