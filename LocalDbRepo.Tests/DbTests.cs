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
    }
}
