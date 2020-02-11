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
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);

            Assert.AreEqual(item.Host, list[0].Host);
            Assert.AreEqual(item.Note, list[0].Note);;
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
            Assert.AreEqual(2, list.Count);

            await repo.Remove(item.WatchId);

            list = await repo.GetList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(item2.Host, list[0].Host);

            File.Delete(fileName);
        }

        [TestMethod]
        public async Task UpdateTests()
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

            await repo.Insert(item);

            item.Host = Guid.NewGuid().ToString();
            item.Emails = Guid.NewGuid().ToString();
            item.IsEnabled = false;
            item.Note = Guid.NewGuid().ToString();
            item.PingIntervalSeconds = 2000;

            await repo.Update(item);

            var list = await repo.GetList();
            Assert.AreEqual(1, list.Count);

            Assert.AreEqual(item.WatchId, list[0].WatchId);
            Assert.AreEqual(item.Host, list[0].Host);
            Assert.AreEqual(item.Emails, list[0].Emails);
            Assert.AreEqual(item.IsEnabled, list[0].IsEnabled);
            Assert.AreEqual(item.Note, list[0].Note);
            Assert.AreEqual(item.PingIntervalSeconds, list[0].PingIntervalSeconds);

            File.Delete(fileName);
        }
    }
}
