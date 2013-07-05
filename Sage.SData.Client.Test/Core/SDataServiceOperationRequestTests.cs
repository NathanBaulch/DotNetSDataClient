﻿using Moq;
using NUnit.Framework;
using Sage.SData.Client.Core;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Core
{
    [TestFixture]
    public class SDataServiceOperationRequestTests : AssertionHelper
    {
        private ISDataService _service;
        private Mock<SDataService> _mock;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mock = new Mock<SDataService>(MockBehavior.Strict, "http://localhost:59213/sdata/aw/dynamic/-/", "lee", "abc123");
            _service = _mock.Object;
        }

        [Test]
        public void ServiceOperation_Verify_ToString()
        {
            var request = new SDataServiceOperationRequest(_service)
                              {
                                  ResourceKind = "employees",
                                  OperationName = "getStats"
                              };
            var url = request.ToString();
            Expect(url, Is.EqualTo("http://localhost:59213/sdata/aw/dynamic/-/employees/$service/getStats"));
        }
    }
}