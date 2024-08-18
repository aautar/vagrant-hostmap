using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VagrantHostmap
{
    class Program
    {
        static void UpdateEntry(string desiredHostname)
        {
            Console.WriteLine("Desired hostname = " + desiredHostname);
            Console.WriteLine("Retrieving box SSH info from " + Environment.CurrentDirectory + "...");
            var connInfoReader = new BoxConnectionInfoReader(Environment.CurrentDirectory);
            var connectionInfo = connInfoReader.GetSshConnectionInfo();
            if (connectionInfo == null)
            {
                Console.Error.WriteLine("Failed to get SSH info.");
                Console.Error.WriteLine("Make sure a vagrant box is setup correctly at this location and it is running.");
                return;
            }

            Console.WriteLine("Connecting to box at " + connectionInfo.Host + ":" + connectionInfo.Port + "...");
            var client = new SshClient(connectionInfo);

            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to establish SSH connection to box.");
                return;
            }

            Console.WriteLine("Reading IP addresses from box...");
            BoxIpAddressReader boxIpReader = new BoxIpAddressReader(client);
            var ipAddresses = boxIpReader.GetIpAddresses();
            foreach (string ip in ipAddresses)
            {
                Console.Error.WriteLine("IP address found: " + ip);
            }

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

        static void Main(string[] args)
        {
            if(args.Length == 0 || args[0].Length == 0)
            {
                Console.Error.WriteLine("hostname not specified");
                return;
            }

            var desiredHostname = args[0];
            UpdateEntry(desiredHostname);
        }
    }
}
