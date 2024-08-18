using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace VagrantHostmap
{
    public class IpCmdOutputReader
    {
        static private string[] RemoveEmptyParts(string[] parts)
        {
            List<string> updatedParts = new List<string>();

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Trim().Length > 0)
                {
                    updatedParts.Add(parts[i].Trim());
                }
            }

            return updatedParts.ToArray();
        }

        // Get interface names to IPv4 addresses from "ip -o -f inet addr" output
        static public Dictionary<string, string> GetInterfaceIPAddressMapFromINetAddrOneLineOutput(string ipInetAddrOutput)
        {
            Dictionary<string, string> interfaceToIpAddr = new Dictionary<string, string>();

            string[] lines = ipInetAddrOutput.Trim().Split('\n');
            foreach (string ln in lines)
            {
                string[] parts = RemoveEmptyParts(ln.Split(null));
                if (parts.Length < 4) // without empty parts, we expect at least 4 textual parts
                {
                    continue;
                }

                string interfaceName = parts[1].Trim(null).Trim(':');
                string[] ipAddrParts = parts[3].Trim().Split('/');

                interfaceToIpAddr.TryAdd(interfaceName, ipAddrParts[0].Trim());
            }           

            return interfaceToIpAddr;
        }
    }


}
