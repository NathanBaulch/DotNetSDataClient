﻿using NUnit.Framework;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client.Test.Framework
{
    [TestFixture]
    public class SDataUriTests
    {
        /// <summary>
        /// Appending path segments to specific service urls should append the
        /// segment to the end of the url, not in the resource segment position.
        /// </summary>
        [Test]
        public void Appending_Segments_To_Specific_Service_Urls_Test()
        {
            var uri = new SDataUri("http://test.com/sdata/-/-/-/resource/$service/name");
            uri.AppendPath("test");
            Assert.AreEqual("resource", uri.GetPathSegment(4).Text);
            Assert.AreEqual("name", uri.GetPathSegment(6).Text);
            Assert.AreEqual("sdata/-/-/-/resource/$service/name/test", uri.DirectPath);
            Assert.AreEqual("http://test.com/sdata/-/-/-/resource/$service/name/test", uri.ToString());
        }

        /// <summary>
        /// Non-standard query parameters such as IncludeContent should be
        /// prefixed with an underscore to prevent potential conflicts with
        /// future SData versions.
        /// </summary>
        [Test]
        public void Non_Standard_Parameters_Should_Have_Underscore_Prefix_Test()
        {
            var uri = new SDataUri("http://test.com/sdata/-/-/-/resource?_includeContent=true");
            Assert.IsTrue(uri["_includeContent"] == "true");
        }
    }
}