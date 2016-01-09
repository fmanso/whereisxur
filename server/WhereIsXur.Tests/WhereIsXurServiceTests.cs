using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WhereIsXur.Tests
{
    [TestClass]
    public class WhereIsXurServiceTests
    {       
        [TestMethod]
        public void ParseLocation_ShouldSuccess()
        {
            var whereIsXur = new WhereIsXurService();
            var body = whereIsXur.SearchPost(new DateTime(2016, 1, 9, 13, 38, 0)).Result;
            var location = whereIsXur.ParseLocation(body);
            Assert.AreEqual("The Tower, across from the Speaker's room.", location);
        }

        [TestMethod]
        public void SearchPost_ShouldSuccess()
        {
            var whereIsXur = new WhereIsXurService();
            var url = whereIsXur.SearchPost(new DateTime(2016, 1, 9, 13, 38, 0)).Result;
            Assert.IsNotNull(url);
        }
    }
}
