using Microsoft.VisualStudio.TestTools.UnitTesting;
using VagrantHostmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VagrantHostmap.Tests
{
    [TestClass()]
    public class IpCmdOutputReaderTests
    {
        [TestMethod()]
        public void GetInterfaceIPAddressMapFromINetAddrOneLineOutput_ReturnsDictionaryWithInterfaceNamesMappedToIPAddresses()
        {
            string cmdOutput = "1: lo    inet 127.0.0.1/8 scope host lo\\       valid_lft forever preferred_lft forever\r\n2: eth0    inet 10.0.2.15/24 scope global eth0\\       valid_lft forever preferred_lft forever\r\n3: eth1    inet 192.168.56.96/24 scope global eth1\\       valid_lft forever preferred_lft forever";
            Dictionary<string, string> interfaceNames = IpCmdOutputReader.GetInterfaceIPAddressMapFromINetAddrOneLineOutput(cmdOutput);

            Assert.AreEqual(3, interfaceNames.Count);

            Assert.IsTrue(interfaceNames.ContainsKey("lo"));
            Assert.IsTrue(interfaceNames.ContainsKey("eth0"));
            Assert.IsTrue(interfaceNames.ContainsKey("eth1"));

            Assert.AreEqual("127.0.0.1", interfaceNames["lo"]);
            Assert.AreEqual("10.0.2.15", interfaceNames["eth0"]);
            Assert.AreEqual("192.168.56.96", interfaceNames["eth1"]);
        }
    }
}
