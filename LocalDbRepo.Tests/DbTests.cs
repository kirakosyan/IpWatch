using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using WatcherCore;

namespace LocalDbRepo.Tests
{
    [TestClass]
    public class DbTests : TestBase
    {
        [TestMethod]
        public async Task WriteReadHostTests()
        {
            var e = new WatchEntity { 
                WatchId = Guid.NewGuid(),
                Emails = Guid.NewGuid().ToString(),
                Host = Guid.NewGuid().ToString(),
                PingIntervalSeconds = 1000
            };
            var result = await RepoContext.AddAsync(e);
            await RepoContext.SaveChangesAsync();
            Assert.AreEqual(e.WatchId, result.Entity.WatchId);
        }

        [TestMethod]
        public async Task UpdateHostTest()
        {
            var e = new WatchEntity
            {
                Emails = Guid.NewGuid().ToString(),
                Host = Guid.NewGuid().ToString(),
                PingIntervalSeconds = 5
            };
            await RepoContext.Insert(e);

            var e2 = new WatchEntity
            {
                WatchId = e.WatchId,
                Emails = Guid.NewGuid().ToString(),
                Host = Guid.NewGuid().ToString(),
                PingIntervalSeconds = 2
            };

            await RepoContext.Update(e2);

            var e3 = await RepoContext.GetItem(e.WatchId);
            Assert.AreEqual(e.WatchId, e3.WatchId);
            Assert.AreEqual(e2.Emails, e3.Emails, "email does not match");
            Assert.AreEqual(e2.Host, e3.Host);
            Assert.AreEqual(e2.PingIntervalSeconds, e3.PingIntervalSeconds);
        }

        [TestMethod]
        public async Task DeleteHostTests()
        {
            var e = new WatchEntity
            {
                WatchId = Guid.NewGuid(),
                Emails = Guid.NewGuid().ToString(),
                Host = Guid.NewGuid().ToString(),
                PingIntervalSeconds = 1000
            };

            var result = await RepoContext.AddAsync(e);
            await RepoContext.SaveChangesAsync();
            Assert.AreEqual(e.WatchId, result.Entity.WatchId);

            await RepoContext.Remove(e.WatchId);
            var e2 = await RepoContext.GetItem(e.WatchId);
            Assert.IsNull(e2);
        }
    }
}
