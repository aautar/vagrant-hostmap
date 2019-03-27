using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VagrantHostmap
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Retrieving box SSH info...");
            var connInfoReader = new BoxConnectionInfoReader(Environment.CurrentDirectory);
            var connectionInfo = connInfoReader.GetSshConnectionInfo();

            Console.WriteLine("Connecting to box...");
            var client = new SshClient(connectionInfo);
            client.Connect();

            Console.WriteLine("Reading IP addresses from box...");
            BoxIpAddressReader boxIpReader = new BoxIpAddressReader(client);
            var ipAddresses = boxIpReader.GetIpAddresses();

            Console.WriteLine("Testing IP addresses...");
            ipAddresses = boxIpReader.GetConnectableIpAddresses(ipAddresses);

            client.Disconnect();

            Console.WriteLine("Connectable IP address found: " + ipAddresses[0]);


            
        }
    }
}
