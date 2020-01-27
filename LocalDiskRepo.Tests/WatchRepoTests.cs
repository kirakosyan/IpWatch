using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WatcherCore;

namespace LocalDiskRepo.Tests
{
    [TestClass]
    public class WatchRepoTests
    {
        static List<string> removeList;
        
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            removeList = new List<string>();
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            foreach(var s in removeList)
            {
                File.Delete(s);
            }
        }

        [TestMethod]
        public async Task ReadAndWriteHosts()
        {
            var fileName = "Test-" + Guid.NewGuid().ToString() + ".json";
            removeList.Add(fileName);
            var repo = new WatchRepo(fileName);
            var list = await repo.GetList();

            var item = new WatchEntity
            {
                Host = Guid.NewGuid().ToString(),
                Emails = Guid.NewGuid().ToString(),
                IsEnabled = true,
                Note = "text яшерттøæå=,/{}().-|_!#¤$\"'",
                PingIntervalSeconds = 1000
            };

            var b = await repo.Insert(item);

            Assert.IsTrue(b);
            list = await repo.GetList();
            Assert.IsNotNull(list.WatchList);
            Assert.AreEqual(1, list.WatchList.Count);

            Assert.AreEqual(item.Host, list.WatchList[0].Host);
            Assert.AreEqual(item.Note, list.WatchList[0].Note);;
        }

        [TestMethod]
        public async Task AddRemoveTests()
        {
            var fileName = "Test-" + Guid.NewGuid().ToString() + ".json";
            removeList.Add(fileName);
            var repo = new WatchRepo(fileName);

            var item = new WatchEntity
            {
                Host = Guid.NewGuid().ToString(),
                Emails = Guid.NewGuid().ToString(),
                IsEnabled = true,
                Note = Guid.NewGuid().ToString(),
                PingIntervalSeconds = 1000
            };

            var item2 = new WatchEntity
            {
                Host = Guid.NewGuid().ToString(),
                Emails = Guid.NewGuid().ToString(),
                IsEnabled = true,
                Note = Guid.NewGuid().ToString(),
                PingIntervalSeconds = 1000
            };

            await repo.Insert(item);
            await repo.Insert(item2);

            var list = await repo.GetList();
            Assert.AreEqual(2, list.WatchList.Count);

            await repo.Remove(item.WatchId);

            list = await repo.GetList();
            Assert.AreEqual(1, list.WatchList.Count);
            Assert.AreEqual(item2.Host, list.WatchList[0].Host);

            File.Delete(fileName);
        }
    }
}
