using Catchy;
using CatchyCoreTests.Helpers;
using System.Text.Json;
using System.Xml.Linq;
using Xunit;

namespace CatchyCoreTests.Assertions.Serialization
{
    /// <summary>
    /// Integration tests for serialization assertions (JSON, XML).
    /// Covers parsing, structure validation, and element/attribute checks.
    /// </summary>
    public class SerializationAssertionsTests
    {
        private const string ValidJson = @"{ ""name"": ""John"", ""age"": 30 }";
        private const string ValidXml = @"<person><name>John</name><age>30</age></person>";

        // ===== JSON Assertions =====

        [Fact]
        public async Task JsonDocument_Parse_WithValidJson_Passes()
        {
            var json = @"{ ""key"": ""value"" }";
            var doc = JsonDocument.Parse(json);
            await Stateless.Assert.That(doc).IsNotNull();
            doc.Dispose();
        }

            [Fact]
            public async Task JsonElement_ValueKind_Is_Passes()
            {
                var doc = JsonDocument.Parse(ValidJson);
                var root = doc.RootElement;
                await Stateless.Assert.That(root.ValueKind).Is(JsonValueKind.Object);
                doc.Dispose();
            }

            [Fact]
            public async Task JsonElement_TryGetProperty_WithExistingProperty_Passes()
            {
                var doc = JsonDocument.Parse(ValidJson);
                var root = doc.RootElement;
                var hasName = root.TryGetProperty("name", out var nameElement);
                await Stateless.Assert.That(hasName).IsTrue();
                await Stateless.Assert.That(nameElement.GetString()).Is("John");
                doc.Dispose();
            }

            [Fact]
            public async Task JsonElement_GetProperty_WithExistingProperty_Passes()
            {
                var doc = JsonDocument.Parse(ValidJson);
                var root = doc.RootElement;
                var name = root.GetProperty("name");
                await Stateless.Assert.That(name.GetString()).Is("John");
                doc.Dispose();
            }

            [Fact]
            public async Task JsonElement_GetProperty_WithMissingProperty_Throws()
            {
                await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                {
                    var doc = JsonDocument.Parse(ValidJson);
                    var root = doc.RootElement;
                    _ = root.GetProperty("nonexistent");
                    doc.Dispose();
                    await Task.CompletedTask;
                });
            }

            [Fact]
            public async Task JsonElement_GetInt32_WithIntegerProperty_Passes()
            {
                var doc = JsonDocument.Parse(ValidJson);
                var age = doc.RootElement.GetProperty("age");
                await Stateless.Assert.That(age.ValueKind).Is(JsonValueKind.Number);
                await Stateless.Assert.That(age.GetInt32()).Is(30);
                doc.Dispose();
            }

            [Fact]
            public async Task JsonElement_GetArrayLength_Passes()
            {
                var json = @"[ 1, 2, 3, 4, 5 ]";
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                await Stateless.Assert.That(root.ValueKind).Is(JsonValueKind.Array);
                await Stateless.Assert.That(root.GetArrayLength()).Is(5);
                doc.Dispose();
            }

            [Fact]
            public async Task JsonElement_EnumerateArray_Passes()
            {
                var json = @"[ ""a"", ""b"", ""c"" ]";
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var items = root.EnumerateArray().ToList();
                await Stateless.Assert.That(items).HasCount(3);
                doc.Dispose();
            }

            [Fact]
            public async Task JsonElement_EnumerateObject_Passes()
            {
                var doc = JsonDocument.Parse(ValidJson);
                var root = doc.RootElement;
                var properties = root.EnumerateObject().ToList();
                await Stateless.Assert.That(properties.Count).IsGreaterThan(0);
                doc.Dispose();
            }

            [Fact]
            public async Task JsonDocument_IsEquivalentTo_SameStructure_Passes()
            {
                var actual = JsonDocument.Parse(@"{ ""name"": ""John"", ""age"": 30 }");
                var expected = JsonDocument.Parse(@"{ ""age"": 30, ""name"": ""John"" }");
                await Stateless.Assert.That(actual).IsEquivalentTo(expected);
                actual.Dispose();
                expected.Dispose();
            }

            [Fact]
            public async Task JsonDocument_IsNotEquivalentTo_DifferentStructure_Passes()
            {
                var actual = JsonDocument.Parse(@"{ ""name"": ""John"", ""age"": 30 }");
                var expected = JsonDocument.Parse(@"{ ""name"": ""Jane"", ""age"": 30 }");
                await Stateless.Assert.That(actual).IsNotEquivalentTo(expected);
                actual.Dispose();
                expected.Dispose();
            }

            [Fact]
            public async Task JsonDocument_IsNotEquivalentTo_EquivalentStructure_Throws()
            {
                var actual = JsonDocument.Parse(@"{ ""name"": ""John"", ""age"": 30 }");
                var expected = JsonDocument.Parse(@"{ ""age"": 30, ""name"": ""John"" }");
                var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(actual).IsNotEquivalentTo(expected));
                Assert.Contains("not to be equivalent", msg, StringComparison.OrdinalIgnoreCase);
                actual.Dispose();
                expected.Dispose();
            }

            [Fact]
            public async Task JsonDocument_RootElement_Passes()
            {
                var doc = JsonDocument.Parse(ValidJson);
                var root = Stateless.Assert.That(doc).RootElement();
                await root.IsObject();
                doc.Dispose();
            }

            [Fact]
            public async Task JsonElement_JsonPath_And_ValueChecks_Passes()
            {
                var json = @"{ ""user"": { ""name"": ""John"", ""age"": 30, ""active"": true, ""tags"": [ ""a"", ""b"" ], ""meta"": { ""level"": 2 } } }";

                var name = Stateless.Assert.That(json).AtJsonPath("$.user.name");
                await name.Exists();
                await name.HasValue("John");
                await name.DoesNotHaveValue( "Jane");

                var age = Stateless.Assert.That(json).AtJsonPath("$.user.age");
                await age.Exists();
                await age.IsNumber();
                await age.HasValue(30);

                var active = Stateless.Assert.That(json).AtJsonPath("$.user.active");
                await active.Exists();
                await active.IsBoolean();
                await active.IsTrue();

                var tags = Stateless.Assert.That(json).AtJsonPath("$.user.tags");
                await tags.Exists();
                await tags.IsArray();
                await tags.HasArrayLength(2);

                var meta = Stateless.Assert.That(json).AtJsonPath("$.user.meta");
                await meta.Exists();
                await meta.IsObject();
                var level = meta.AtPath("level");
                await level.Exists();
                await level.HasValue(2);

                var missing = Stateless.Assert.That(@"{ ""missing"": null }").AtJsonPath("$.missing");
                await missing.Exists();
                await missing.IsNull();
            }

            // ===== XML Assertions =====

            [Fact]
            public async Task XDocument_Parse_WithValidXml_Passes()
            {
                var doc = XDocument.Parse(ValidXml);
                await Stateless.Assert.That(doc).IsNotNull();
            }

            [Fact]
            public async Task XElement_Name_Is_Passes()
            {
                var doc = XDocument.Parse(ValidXml);
                var root = doc.Root;
                if (root != null)
                    await Stateless.Assert.That(root.Name.LocalName).Is("person");
            }

            [Fact]
            public async Task XElement_GetElement_WithExistingElement_Passes()
            {
                var doc = XDocument.Parse(ValidXml);
                var root = doc.Root;
                if (root != null)
                {
                    var nameElement = root.Element("name");
                    await Stateless.Assert.That(nameElement).IsNotNull();
                    if (nameElement != null)
                        await Stateless.Assert.That(nameElement.Value).Is("John");
                }
            }

            [Fact]
            public async Task XElement_GetAttribute_WithExistingAttribute_Passes()
            {
                var xml = @"<person id=""1""><name>John</name></person>";
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root != null)
                {
                    var id = root.Attribute("id");
                    await Stateless.Assert.That(id).IsNotNull();
                    if (id != null)
                        await Stateless.Assert.That(id.Value).Is("1");
                }
            }

            [Fact]
            public async Task XElement_Elements_Count_Passes()
            {
                var doc = XDocument.Parse(ValidXml);
                var root = doc.Root;
                if (root != null)
                {
                    var elements = root.Elements().ToList();
                    await Stateless.Assert.That(elements).HasCountAtLeast(1);
                }
            }

            [Fact]
            public async Task XElement_Descendants_Count_Passes()
            {
                var xml = @"<root><person><name>John</name><age>30</age></person></root>";
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root != null)
                {
                    var descendants = root.Descendants().ToList();
                    await Stateless.Assert.That(descendants).HasCountAtLeast(3);
                }
            }

            [Fact]
            public async Task XElement_Value_Is_Passes()
            {
                var xml = @"<person><age>30</age></person>";
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root != null)
                {
                    var age = root.Element("age");
                    if (age != null)
                        await Stateless.Assert.That(age.Value).Is("30");
                }
            }

            // ===== AmbientSoft Mode =====

            [Fact]
            public async Task Serialization_SoftMode_AccumulatesFailures()
            {
                var verify = Asserter.NewSoft();

                var doc = JsonDocument.Parse(ValidJson);
                var root = doc.RootElement;
                var name = root.GetProperty("name");

                await verify.That(name.GetString()).Is("John");   // Pass
                await verify.That(name.GetString()).Is("Jane");   // Fail
                await verify.That(root).ValueKind().Is(JsonValueKind.Object);  // Pass
                await verify.That(root).ValueKind().Is(JsonValueKind.Array);   // Fail

                if (verify.ErrorCount != 2) throw new AssertionException($"Expected 2 errors, got {verify.ErrorCount}");
                doc.Dispose();
            }

            // ===== JSON with Complex Structure =====

            [Fact]
            public async Task JsonElement_NestedObject_Passes()
            {
                var json = @"{ ""user"": { ""name"": ""John"", ""email"": ""john@example.com"" } }";
                var doc = JsonDocument.Parse(json);
                var user = doc.RootElement.GetProperty("user");
                var email = user.GetProperty("email");
                await Stateless.Assert.That(email.GetString()).Is("john@example.com");
                doc.Dispose();
            }

            [Fact]
            public async Task JsonElement_ArrayOfObjects_Passes()
            {
                var json = @"[ { ""id"": 1 }, { ""id"": 2 } ]";
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                await Stateless.Assert.That(root.GetArrayLength()).Is(2);
                var items = root.EnumerateArray().ToList();
                var firstId = items[0].GetProperty("id").GetInt32();
                await Stateless.Assert.That(firstId).Is(1);
                doc.Dispose();
            }

            // ===== XML with Attributes =====

            [Fact]
            public async Task XElement_MultipleAttributes_Passes()
            {
                var xml = @"<person id=""1"" name=""John"" age=""30""></person>";
                var doc = XDocument.Parse(xml);
                var root = doc.Root;
                if (root != null)
                {
                    var attributes = root.Attributes().ToList();
                    await Stateless.Assert.That(attributes).HasCount(3);
                }
            }

            [Fact]
            public async Task XElement_HasName_HasAttribute_And_Value_Passes()
            {
                var xml = @"<person id=""1"" name=""John""><details><age>30</age></details></person>";

                var person = Stateless.Assert.That(xml).AtXPath(".");
                await person.Exists();
                await person.HasName("person");
                await person.HasAttribute("id");
                await person.HasAttributeValue("name", "John");

                var details = Stateless.Assert.That(xml).AtXPath("details");
                await details.Exists();
                await details.HasChild("age");
                await Stateless.Assert.That(xml).AtXPath("details/age").HasValue("30");
            }

            [Fact]
            public async Task XElement_DoesNotHaveChild_And_IsNotEmpty_Passes()
            {
                var xml = @"<person><name>John</name><age>30</age></person>";
                var person = Stateless.Assert.That(xml).AtXPath(".");
                await person.DoesNotHaveChild( "address");
                await person.IsNotEmpty();
            }

            [Fact]
            public async Task XElement_XPath_And_ValueChecks_Passes()
            {
                var xml = @"<person xmlns=""urn:test"" id=""1"" name=""John""><details><age>30</age><tags><tag>a</tag><tag>b</tag></tags></details><empty /><text>hello world</text></person>";

                // root assertions — fresh pipeline each time to avoid shared CurrentPath state
                var person = Stateless.Assert.That(xml).AtXPath(".");
                await person.Exists();
                await person.HasName("person");
                await person.HasNamespace("urn:test");
                await person.HasAttribute("id");
                await person.HasAttributeValue("name", "John");
                await person.DoesNotHaveChild("missing");
                await person.IsNotEmpty();
                await person.ContainsText("hello");
                await person.DoesNotContainText("missing");

                // details subtree — fresh pipeline
                var details = Stateless.Assert.That(xml).AtXPath("details");
                await details.Exists();
                await details.HasChild("age");
                await details.HasChildCount(2);
                await details.DoesNotHaveAttribute("missing");
                await details.DoesNotHaveChild("missing");

                // age — fresh pipeline
                var age = Stateless.Assert.That(xml).AtXPath("details/age");
                await age.Exists();
                await age.HasValue("30");

                // tags — fresh pipeline
                var tags = Stateless.Assert.That(xml).AtXPath("details/tags");
                await tags.Exists();
                await tags.HasChildCount(2);
                await Stateless.Assert.That(xml).AtXPath("details/tags/tag").Exists();

                // empty element — fresh pipeline
                var empty = Stateless.Assert.That(xml).AtXPath("empty");
                await empty.Exists();
                await empty.IsEmpty();

                // missing element — fresh pipeline
                var missing = Stateless.Assert.That(xml).AtXPath("missing");
                await missing.DoesNotExist();
            }
        }
    }





