﻿using Moq;
using NUnit.Framework;
using Sage.SData.Client.Atom;
using Sage.SData.Client.Core;
using Sage.SData.Client.Framework;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Core
{
    [TestFixture]
    public class SDataServiceTests : AssertionHelper
    {
        [Test]
        public void Service_Verify_CanConstruct()
        {
            var service = new SDataService();
            Expect(service, Is.Not.Null);
        }

        [Test]
        public void Service_Verify_CanConstructWithUrl()
        {
            var service = new SDataService("http://localhost:59213/sdata/aw/dynamic/-/employees", "lee", "abc123");
            Expect(service, Is.Not.Null);
        }

        [Test]
        public void Service_Verity_CanInitialize()
        {
            var service = new SDataService("http://localhost:59213/sdata/aw/dynamic/-/employees", "lee", "abc123");

            Expect(service.UserName, Is.Not.Null);
            Expect(service.UserName, Is.EqualTo("lee"));

            Expect(service.Password, Is.Not.Null);
            Expect(service.Password, Is.EqualTo("abc123"));

            Expect(service.Protocol, Is.Not.Null);
            Expect(service.Protocol, Is.EqualTo("http"));

            Expect(service.ServerName, Is.Not.Null);
            Expect(service.ServerName, Is.EqualTo("localhost"));

            Expect(service.Port, Is.Not.Null);
            Expect(service.Port, Is.EqualTo(59213));

            Expect(service.VirtualDirectory, Is.Not.Null);
            Expect(service.VirtualDirectory, Is.EqualTo("sdata"));

            Expect(service.ApplicationName, Is.Not.Null);
            Expect(service.ApplicationName, Is.EqualTo("aw"));

            Expect(service.ContractName, Is.Not.Null);
            Expect(service.ContractName, Is.EqualTo("dynamic"));

            Expect(service.DataSet, Is.Not.Null);
            Expect(service.DataSet, Is.EqualTo("-"));
        }

        [Test]
        public void verify_can_handle_AtomFeed_response_for_create_entry()
        {
            var mock = new Mock<ISDataResponse>(MockBehavior.Strict);
            var feed = new AtomFeed();
            feed.AddEntry(new AtomEntry());

            mock.SetupGet(r => r.Content).Returns(feed);
            mock.SetupGet(r => r.ETag).Returns(string.Empty);

            var service = new MockService(mock);
            var result = service.CreateEntry(new SDataServiceOperationRequest(service) {OperationName = "computePrice"}, new AtomEntry());
            Expect(result, Is.InstanceOf<AtomEntry>());
        }

        [Test]
        public void verify_can_handle_AtomEntry_response_for_create_entry()
        {
            var mock = new Mock<ISDataResponse>(MockBehavior.Strict);
            mock.SetupGet(r => r.Content).Returns(new AtomEntry());
            mock.SetupGet(r => r.ETag).Returns(string.Empty);

            var service = new MockService(mock);
            var result = service.CreateEntry(new SDataServiceOperationRequest(service) {OperationName = "computePrice"}, new AtomEntry());
            Expect(result, Is.InstanceOf<AtomEntry>());
        }

        private class MockService : SDataService
        {
            private readonly Mock<ISDataResponse> _mock;

            public MockService(Mock<ISDataResponse> mock)
            {
                _mock = mock;
            }

            protected override ISDataResponse ExecuteRequest(string url, RequestOperation operation, params MediaType[] accept)
            {
                return _mock.Object;
            }
        }
    }
}