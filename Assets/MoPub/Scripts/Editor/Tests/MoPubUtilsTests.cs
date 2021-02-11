using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class MoPubUtilsTests : MoPubTest
    {
        [Test]
        public void CompareVersionsWithFirstSmaller()
        {
            Assert.That(MoPubUtils.CompareVersions("0", "1"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("0.9", "1.0"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("0.9.99", "1.0.0"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("0.9.99", "0.10.0"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("0.9.99", "0.9.100"), Is.EqualTo(-1));
        }

        [Test]
        public void CompareVersionsWithFirstGreater()
        {
            Assert.That(MoPubUtils.CompareVersions("1", "0"), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions("1.0", "0.9"), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions("1.0.0", "0.9.99"), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions("0.10.0", "0.9.99"), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions("0.9.100", "0.9.99"), Is.EqualTo(1));
        }

        [Test]
        public void CompareVersionsWithEqual()
        {
            Assert.That(MoPubUtils.CompareVersions("1", "1"), Is.EqualTo(0));
            Assert.That(MoPubUtils.CompareVersions("1.0", "1.0"), Is.EqualTo(0));
            Assert.That(MoPubUtils.CompareVersions("1.0.0", "1.0.0"), Is.EqualTo(0));
        }

        [Test]
        public void CompareVersionsWithEmptyValues()
        {
            Assert.That(MoPubUtils.CompareVersions("", ""), Is.EqualTo(0));
            Assert.That(MoPubUtils.CompareVersions("", "1"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("1", ""), Is.EqualTo(1));
            Assert.That(MoPubUtils.CompareVersions(null, null), Is.EqualTo(0));
            Assert.That(MoPubUtils.CompareVersions(null, "1"), Is.EqualTo(-1));
            Assert.That(MoPubUtils.CompareVersions("1", null), Is.EqualTo(1));
        }

        [Test]
        public void DecodeArgsWithNullShouldErrorAndYieldEmptyList()
        {
            var res = MoPubUtils.DecodeArgs(null, 0);

            LogAssert.Expect(LogType.Error, "Invalid JSON data: ");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(0));
        }

        [Test]
        public void DecodeArgsWithInvalidShouldErrorAndYieldEmptyList()
        {
            var res = MoPubUtils.DecodeArgs("{\"a\"]", 0);

            LogAssert.Expect(LogType.Error, "Invalid JSON data: {\"a\"]");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(0));
        }

        [Test]
        public void DecodeArgsWithValueShouldYieldListWithValue()
        {
            var res = MoPubUtils.DecodeArgs("[\"a\"]", 0);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(1));
            Assert.That(res[0], Is.EqualTo("a"));
        }

        [Test]
        public void DecodeArgsWithoutMinimumValuesShouldErrorAndYieldListWithDesiredLength()
        {
            var res = MoPubUtils.DecodeArgs("[\"a\", \"b\"]", 3);

            LogAssert.Expect(LogType.Error, "Missing one or more values: [\"a\", \"b\"] (expected 3)");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(3));
            Assert.That(res[0], Is.EqualTo("a"));
            Assert.That(res[1], Is.EqualTo("b"));
            Assert.That(res[2], Is.EqualTo(""));
        }

        [Test]
        public void DecodeArgsWithExpectedValuesShouldYieldListWithDesiredValues()
        {
            var res = MoPubUtils.DecodeArgs("[\"a\", \"b\", \"c\"]", 3);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(3));
            Assert.That(res[0], Is.EqualTo("a"));
            Assert.That(res[1], Is.EqualTo("b"));
            Assert.That(res[2], Is.EqualTo("c"));
        }
    }
}
