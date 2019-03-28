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
            if(args.Length == 0 || args[0].Length == 0)
            {
                Console.WriteLine("hostname not specified");
                return;
            }

            var desiredHostname = args[0];

            Console.WriteLine("Retrieving box SSH info...");
            var connInfoReader = new BoxConnectionInfoReader(Environment.CurrentDirectory);
            var connectionInfo = connInfoReader.GetSshConnectionInfo();
            if(connectionInfo == null)
            {
                Console.WriteLine("Failed to get SSH info.");
                Console.WriteLine("Make sure a vagrant box is setup correctly at this location and it is running.");
                return;
            }

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


            HostsFileUpdater hostFileUpdater = new HostsFileUpdater();
            var existingEntry = hostFileUpdater.GetEntryForHostname(desiredHostname);

            if (existingEntry != null)
            {
                Console.WriteLine("Existing entry found in hosts file: " + existingEntry);

                if (existingEntry.StartsWith(ipAddresses[0]))
                {
                    Console.WriteLine("Existing entry is valid, no update needed.");
                    return;
                }
                else
                {
                    var updatedEntry = hostFileUpdater.UpdateEntry(ipAddresses[0], desiredHostname, existingEntry);
                    Console.WriteLine("Updated entry: " + updatedEntry);
                }
            }
            else
            {
                var newEntry = hostFileUpdater.AddEntry(ipAddresses[0], desiredHostname);
                Console.WriteLine("Added entry: " + newEntry);
            }

            hostFileUpdater.CopyTempToActual();
            Console.WriteLine("done.");

        }
    }
}
