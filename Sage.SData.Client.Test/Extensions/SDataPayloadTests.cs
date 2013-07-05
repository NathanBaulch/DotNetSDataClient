﻿using System;
using System.Xml;
using NUnit.Framework;
using Sage.SData.Client.Extensions;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Extensions
{
    [TestFixture]
    public class SDataPayloadTests
    {
        [Test]
        public void Typical_Payload()
        {
            const string xml = @"<salesOrder sdata:key=""43660""
                                    xmlns=""http://schemas.sage.com/myContract""
                                    xmlns:sdata=""http://schemas.sage.com/sdata/2008/1""
                                    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                          <orderDate>2001-07-01</orderDate>
                          <shipDate xsi:nil=""true"" />
                          <contact sdata:key=""216"" 
                                   sdata:uri=""http://www.example.com/sdata/myApp/myContract/-/contacts('216')"" 
                                   sdata:lookup=""http://www.example.com/sdata/myApp/myContract/-/contacts""/>
                          <orderLines sdata:uri=""http://www.example.com/sdata/myApp/myContract/-/salesOrderLines?where=salesOrderID%20eq%2043660""/>
                        </salesOrder>";
            var payload = Utility.LoadPayload(xml);

            Assert.That(payload.ResourceName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Namespace, Is.EqualTo("http://schemas.sage.com/myContract"));
            Assert.That(payload.Key, Is.EqualTo("43660"));
            Assert.That(payload.Values.Count, Is.EqualTo(4));

            object value;
            Assert.IsTrue(payload.Values.TryGetValue("orderDate", out value));
            Assert.That(value, Is.EqualTo("2001-07-01"));

            Assert.IsTrue(payload.Values.TryGetValue("shipDate", out value));
            Assert.That(value, Is.Null);

            Assert.IsTrue(payload.Values.TryGetValue("contact", out value));
            Assert.IsInstanceOf<SDataPayload>(value);
            var obj = (SDataPayload) value;
            Assert.That(obj.Key, Is.EqualTo("216"));
            Assert.That(obj.Uri, Is.EqualTo(new Uri("http://www.example.com/sdata/myApp/myContract/-/contacts('216')")));
            Assert.That(obj.Lookup, Is.EqualTo("http://www.example.com/sdata/myApp/myContract/-/contacts"));
            CollectionAssert.IsEmpty(obj.Values);

            Assert.IsTrue(payload.Values.TryGetValue("orderLines", out value));
            Assert.IsInstanceOf<SDataPayloadCollection>(value);
            var col = (SDataPayloadCollection) value;
            Assert.That(col.Uri, Is.EqualTo(new Uri("http://www.example.com/sdata/myApp/myContract/-/salesOrderLines?where=salesOrderID%20eq%2043660")));
            CollectionAssert.IsEmpty(col);
        }

        [Test]
        public void Object_Property_Without_Attributes()
        {
            const string xml = @"<salesOrder>
                          <contact>
                            <firstName>John</firstName>
                            <lastName>Smith</lastName>
                          </contact>
                        </salesOrder>";
            var payload = Utility.LoadPayload(xml);

            Assert.That(payload.ResourceName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Values.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.Values.TryGetValue("contact", out value));
            Assert.IsInstanceOf<SDataPayload>(value);
            var obj = (SDataPayload) value;
            Assert.That(obj.Values.Count, Is.EqualTo(2));

            Assert.IsTrue(obj.Values.TryGetValue("firstName", out value));
            Assert.That(value, Is.EqualTo("John"));

            Assert.IsTrue(obj.Values.TryGetValue("lastName", out value));
            Assert.That(value, Is.EqualTo("Smith"));
        }

        [Test]
        public void Empty_Collection_Property_Without_Attributes()
        {
            const string xml = @"<salesOrder>
                          <orderLines />
                        </salesOrder>";
            var payload = Utility.LoadPayload(xml);

            Assert.That(payload.ResourceName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Values.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.Values.TryGetValue("orderLines", out value));
            Assert.IsInstanceOf<SDataPayloadCollection>(value);
            var col = (SDataPayloadCollection) value;
            CollectionAssert.IsEmpty(col);
        }

        [Test]
        public void Empty_Collection_Property_Without_Attributes_Or_Namespace()
        {
            const string xml = @"<x:salesOrder xmlns:x=""http://schemas.sage.com/dynamic/2007"">
                          <orderLines />
                        </x:salesOrder>";
            var payload = Utility.LoadPayload(xml);

            Assert.That(payload.ResourceName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Values.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.Values.TryGetValue("orderLines", out value));
            Assert.IsInstanceOf<SDataPayloadCollection>(value);
            var col = (SDataPayloadCollection) value;
            CollectionAssert.IsEmpty(col);
        }

        [Test]
        public void Collection_Of_One_Property_Without_Attributes()
        {
            const string xml = @"<salesOrder xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                          <orderLines sdata:url=""http://www.example.com/sdata/myApp/myContract/-/salesOrderLines?where=salesOrderID%20eq%2043660"">
                            <salesOrderLine sdata:key=""43660-1"" />
                          </orderLines>
                        </salesOrder>";
            var payload = Utility.LoadPayload(xml);

            Assert.That(payload.ResourceName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Values.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.Values.TryGetValue("orderLines", out value));
            Assert.IsInstanceOf<SDataPayloadCollection>(value);
            var col = (SDataPayloadCollection) value;
            Assert.That(col.Count, Is.EqualTo(1));

            var item = col[0];
            Assert.That(item.ResourceName, Is.EqualTo("salesOrderLine"));
            Assert.That(item.Key, Is.EqualTo("43660-1"));
            CollectionAssert.IsEmpty(item.Values);
        }

        [Test]
        public void Collection_Property_Without_Attributes()
        {
            const string xml = @"<salesOrder xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                          <orderLines>
                            <salesOrderLine sdata:key=""43660-1"" />
                            <salesOrderLine sdata:key=""43660-2"" />
                          </orderLines>
                        </salesOrder>";
            var payload = Utility.LoadPayload(xml);

            Assert.That(payload.ResourceName, Is.EqualTo("salesOrder"));
            Assert.That(payload.Values.Count, Is.EqualTo(1));

            object value;
            Assert.IsTrue(payload.Values.TryGetValue("orderLines", out value));
            Assert.IsInstanceOf<SDataPayloadCollection>(value);
            var col = (SDataPayloadCollection) value;
            Assert.That(col.Count, Is.EqualTo(2));

            var item = col[0];
            Assert.That(item.ResourceName, Is.EqualTo("salesOrderLine"));
            Assert.That(item.Key, Is.EqualTo("43660-1"));
            CollectionAssert.IsEmpty(item.Values);

            item = col[1];
            Assert.That(item.ResourceName, Is.EqualTo("salesOrderLine"));
            Assert.That(item.Key, Is.EqualTo("43660-2"));
            CollectionAssert.IsEmpty(item.Values);
        }

        [Test]
        public void Unnested_Collection_Items()
        {
            const string xml = @"<digest xmlns=""http://schemas.sage.com/sdata/sync/2008/1"">
                          <origin>http://www.example.com/sdata/myApp1/myContract/-/accounts</origin>
                          <digestEntry>
                            <tick>5</tick>
                          </digestEntry>
                          <digestEntry>
                            <tick>11</tick>
                          </digestEntry>
                        </digest>";
            var payload = Utility.LoadPayload(xml);

            Assert.That(payload.ResourceName, Is.EqualTo("digest"));
            Assert.That(payload.Namespace, Is.EqualTo("http://schemas.sage.com/sdata/sync/2008/1"));
            Assert.That(payload.Values.Count, Is.EqualTo(2));

            object value;
            Assert.IsTrue(payload.Values.TryGetValue("origin", out value));
            Assert.That(value, Is.EqualTo("http://www.example.com/sdata/myApp1/myContract/-/accounts"));

            Assert.IsTrue(payload.Values.TryGetValue("digestEntry", out value));
            Assert.IsInstanceOf<SDataPayloadCollection>(value);
            var col = (SDataPayloadCollection) value;
            Assert.That(col.Count, Is.EqualTo(2));

            var item = col[0];
            Assert.That(item.ResourceName, Is.EqualTo("digestEntry"));
            Assert.That(item.Values.Count, Is.EqualTo(1));
            Assert.IsTrue(item.Values.TryGetValue("tick", out value));
            Assert.That(value, Is.EqualTo("5"));

            item = col[1];
            Assert.That(item.ResourceName, Is.EqualTo("digestEntry"));
            Assert.That(item.Values.Count, Is.EqualTo(1));
            Assert.IsTrue(item.Values.TryGetValue("tick", out value));
            Assert.That(value, Is.EqualTo("11"));
        }

        [Test]
        public void Loaded_Collection_Infers_Item_Resource_Name()
        {
            const string xml = @"<salesOrder xmlns:sdata=""http://schemas.sage.com/sdata/2008/1"">
                          <orderLines>
                            <salesOrderLine sdata:key=""43660-1"" />
                            <salesOrderLine sdata:key=""43660-2"" />
                          </orderLines>
                        </salesOrder>";
            var payload = Utility.LoadPayload(xml);
            var orderLines = payload.Values["orderLines"] as SDataPayloadCollection;
            Assert.That(orderLines, Is.Not.Null);
            Assert.That(orderLines.ResourceName, Is.EqualTo("salesOrderLine"));
        }

        [Test]
        public void Written_Collection_Uses_Item_Resource_Name()
        {
            var payload = new SDataPayload
                              {
                                  ResourceName = "salesOrder",
                                  Namespace = "",
                                  Values =
                                      {
                                          {
                                              "orderLines", new SDataPayloadCollection("salesOrderLine")
                                                                {
                                                                    new SDataPayload {Key = "43660-1"}
                                                                }
                                          }
                                      }
                              };
            var nav = Utility.WritePayload(payload);
            var node = nav.SelectSingleNode("*/salesOrder/orderLines/salesOrderLine");
            Assert.That(node, Is.Not.Null);
        }

        [Test]
        public void Primitive_Values_Formatted_Appropriately()
        {
            var payload = new SDataPayload
                              {
                                  ResourceName = "salesOrder",
                                  Namespace = "",
                                  Values =
                                      {
                                          {"byte", byte.MaxValue},
                                          {"sbyte", sbyte.MaxValue},
                                          {"short", short.MaxValue},
                                          {"ushort", ushort.MaxValue},
                                          {"int", int.MaxValue},
                                          {"uint", uint.MaxValue},
                                          {"long", long.MaxValue},
                                          {"ulong", ulong.MaxValue},
                                          {"bool", true},
                                          {"char", 'z'},
                                          {"float", float.MaxValue},
                                          {"double", double.MaxValue},
                                          {"decimal", decimal.MaxValue},
                                          {"Guid", Guid.NewGuid()},
                                          {"DateTime", DateTime.Now},
                                          {"DateTimeOffset", DateTimeOffset.Now},
                                          {"TimeSpan", DateTime.Now.TimeOfDay}
                                      }
                              };
            var nav = Utility.WritePayload(payload);
            nav = nav.SelectSingleNode("*/salesOrder");

            var assertDoesNotThrow = new Action<string, Action<string>>(
                (name, action) =>
                    {
                        var node = nav.SelectSingleNode(name);
                        Assert.That(node, Is.Not.Null);
                        Assert.DoesNotThrow(() => action(node.Value));
                    });
            assertDoesNotThrow("byte", x => XmlConvert.ToByte(x));
            assertDoesNotThrow("sbyte", x => XmlConvert.ToSByte(x));
            assertDoesNotThrow("short", x => XmlConvert.ToInt16(x));
            assertDoesNotThrow("ushort", x => XmlConvert.ToUInt16(x));
            assertDoesNotThrow("int", x => XmlConvert.ToInt32(x));
            assertDoesNotThrow("uint", x => XmlConvert.ToUInt32(x));
            assertDoesNotThrow("long", x => XmlConvert.ToInt64(x));
            assertDoesNotThrow("ulong", x => XmlConvert.ToUInt64(x));
            assertDoesNotThrow("bool", x => XmlConvert.ToBoolean(x));
            assertDoesNotThrow("char", x => XmlConvert.ToChar(x));
            assertDoesNotThrow("float", x => XmlConvert.ToSingle(x));
            assertDoesNotThrow("double", x => XmlConvert.ToDouble(x));
            assertDoesNotThrow("decimal", x => XmlConvert.ToDecimal(x));
            assertDoesNotThrow("Guid", x => XmlConvert.ToGuid(x));
            assertDoesNotThrow("DateTime", x => XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.RoundtripKind));
            assertDoesNotThrow("DateTimeOffset", x => XmlConvert.ToDateTimeOffset(x));
            assertDoesNotThrow("TimeSpan", x => XmlConvert.ToTimeSpan(x));
        }

        [Test]
        public void Collection_Items_Can_Be_In_Different_Namespace()
        {
            var payload = new SDataPayload
                              {
                                  ResourceName = "tradingAccount",
                                  Namespace = "http://gcrm.com",
                                  Values =
                                      {
                                          {
                                              "emails", new SDataPayloadCollection("email")
                                                            {
                                                                new SDataPayload
                                                                    {
                                                                        Namespace = "http://common.com"
                                                                    }
                                                            }
                                          }
                                      }
                              };
            var nav = Utility.WritePayload(payload);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("g", "http://gcrm.com");
            mgr.AddNamespace("c", "http://common.com");
            var node = nav.SelectSingleNode("*/g:tradingAccount/g:emails/c:email", mgr);
            Assert.That(node, Is.Not.Null);
        }

        [Test]
        public void Object_Property_With_Single_Child_Property()
        {
            const string xml = @"<productComputeSimplePrice>
                          <response>
                            <unitPrice>100</unitPrice>
                          </response>
                        </productComputeSimplePrice>";
            var payload = Utility.LoadPayload(xml);

            object value;
            Assert.That(payload.Values.TryGetValue("response", out value), Is.True);
            Assert.That(value, Is.InstanceOf<SDataPayload>());
        }

        [Test]
        public void Uri_Property_Should_Be_Escaped_When_Written()
        {
            var payload = new SDataPayload
                              {
                                  ResourceName = "person",
                                  Namespace = "http://test.com",
                                  Uri = new Uri("http://localhost/person('`%^ []{}<>')")
                              };
            var nav = Utility.WritePayload(payload);
            var mgr = new XmlNamespaceManager(nav.NameTable);
            mgr.AddNamespace("sdata", Client.Framework.Common.SData.Namespace);
            mgr.AddNamespace("test", "http://test.com");
            var node = nav.SelectSingleNode("sdata:payload/test:person/@sdata:uri", mgr);
            Assert.That(node.Value, Is.EqualTo("http://localhost/person('%60%25%5E%20%5B%5D%7B%7D%3C%3E')"));
        }
    }
}