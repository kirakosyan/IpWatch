using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using WatcherCore;

namespace LocalDiskRepo.Tests
{
    [TestClass]
    public class WatchRepoTests
    {
        [TestMethod]
        public async Task ReadAndWriteHosts()
        {
            var fileName = "Test-" + Guid.NewGuid().ToString() + ".json";
            var repo = new WatchRepo(fileName);
            var list = await repo.GetList();


            Assert.IsNull(list?.WatchList);

            var item = new WatchEntity
            {
                Host = Guid.NewGuid().ToString(),
                Emails = Guid.NewGuid().ToString(),
                IsEnabled = true,
                Note = "text яшерттøæå",
                PingIntervalSeconds = 1000
            };

            var b = await repo.Insert(item);

            Assert.IsTrue(b);
            list = await repo.GetList();
            Assert.IsNotNull(list.WatchList);
            Assert.AreEqual(1, list.WatchList.Count);

            Assert.AreEqual(item.Host, list.WatchList[0].Host);
            Assert.AreEqual(item.Note, list.WatchList[0].Note);

            File.Delete(fileName);
        }
    }
}
