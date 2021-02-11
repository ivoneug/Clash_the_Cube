using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ImpressionDataTests
    {
        [Test]
        public void FromJsonWithNullShouldYieldEmpty()
        {
            var impData = MoPub.ImpressionData.FromJson(null);
            Assert.That(impData.AppVersion, Is.Null);
            Assert.That(impData.AdUnitId, Is.Null);
            Assert.That(impData.AdUnitName, Is.Null);
            Assert.That(impData.AdUnitFormat, Is.Null);
            Assert.That(impData.ImpressionId, Is.Null);
            Assert.That(impData.Currency, Is.Null);
            Assert.That(impData.PublisherRevenue, Is.Null);
            Assert.That(impData.AdGroupId, Is.Null);
            Assert.That(impData.AdGroupName, Is.Null);
            Assert.That(impData.AdGroupType, Is.Null);
            Assert.That(impData.AdGroupPriority, Is.Null);
            Assert.That(impData.Country, Is.Null);
            Assert.That(impData.Precision, Is.Null);
            Assert.That(impData.NetworkName, Is.Null);
            Assert.That(impData.NetworkPlacementId, Is.Null);
            Assert.That(impData.JsonRepresentation, Is.Null);
        }

        [Test]
        public void FromJsonWithEmptyShouldYieldEmpty()
        {
            var json = GetJson(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
            var impData = MoPub.ImpressionData.FromJson(json);

            Assert.That(impData.AdUnitId, Is.Null);
            Assert.That(impData.AdUnitName, Is.Null);
            Assert.That(impData.AdUnitFormat, Is.Null);
            Assert.That(impData.ImpressionId, Is.Null);
            Assert.That(impData.Currency, Is.Null);
            Assert.That(impData.PublisherRevenue, Is.Null);
            Assert.That(impData.AdGroupId, Is.Null);
            Assert.That(impData.AdGroupName, Is.Null);
            Assert.That(impData.AdGroupType, Is.Null);
            Assert.That(impData.AdGroupPriority, Is.Null);
            Assert.That(impData.Country, Is.Null);
            Assert.That(impData.Precision, Is.Null);
            Assert.That(impData.NetworkName, Is.Null);
            Assert.That(impData.NetworkPlacementId, Is.Null);
            Assert.That(impData.JsonRepresentation, Is.EqualTo(json));
        }

        [Test]
        public void FromJsonWithNullValuesShouldYieldEmpty()
        {
            var json = GetNullValuesJson();
            var impData = MoPub.ImpressionData.FromJson(json);

            Assert.That(impData.AdUnitId, Is.Null);
            Assert.That(impData.AdUnitName, Is.Null);
            Assert.That(impData.AdUnitFormat, Is.Null);
            Assert.That(impData.ImpressionId, Is.Null);
            Assert.That(impData.Currency, Is.Null);
            Assert.That(impData.PublisherRevenue, Is.Null);
            Assert.That(impData.AdGroupId, Is.Null);
            Assert.That(impData.AdGroupName, Is.Null);
            Assert.That(impData.AdGroupType, Is.Null);
            Assert.That(impData.AdGroupPriority, Is.Null);
            Assert.That(impData.Country, Is.Null);
            Assert.That(impData.Precision, Is.Null);
            Assert.That(impData.NetworkName, Is.Null);
            Assert.That(impData.NetworkPlacementId, Is.Null);
            Assert.That(impData.JsonRepresentation, Is.EqualTo(json));
        }

        [Test]
        public void FromJsonWithValuesShouldYieldValues()
        {
            const string appVersion = "1.0.0";
            const string adUnitId = "1234";
            const string adUnitName = "my awesome ad unit";
            const string adUnitFormat = "Rewarded Video";
            const string impressionId = "5678";
            const string currency = "USD";
            const double publisherRevenue = 0.00001;
            const string adGroupId = "9012";
            const string adGroupName = "my great ad group";
            const string adGroupType = "marketplace";
            const int adGroupPriority = 1;
            const string country = "USA";
            const string precision = "publisher_defined";
            const string networkName = "MoPub";
            const string networkPlacementId = "3456";

            var json = GetJson(
                appVersion,
                adUnitId,
                adUnitName,
                adUnitFormat,
                impressionId,
                currency,
                publisherRevenue,
                adGroupId,
                adGroupName,
                adGroupType,
                adGroupPriority,
                country,
                precision,
                networkName,
                networkPlacementId);
            MoPub.ImpressionData impData = MoPub.ImpressionData.FromJson(json);

            Assert.That(impData.AppVersion, Is.EqualTo(appVersion));
            Assert.That(impData.AdUnitId, Is.EqualTo(adUnitId));
            Assert.That(impData.AdUnitName, Is.EqualTo(adUnitName));
            Assert.That(impData.AdUnitFormat, Is.EqualTo(adUnitFormat));
            Assert.That(impData.ImpressionId, Is.EqualTo(impressionId));
            Assert.That(impData.Currency, Is.EqualTo(currency));
            Assert.That(impData.PublisherRevenue, Is.EqualTo(publisherRevenue));
            Assert.That(impData.AdGroupId, Is.EqualTo(adGroupId));
            Assert.That(impData.AdGroupName, Is.EqualTo(adGroupName));
            Assert.That(impData.AdGroupType, Is.EqualTo(adGroupType));
            Assert.That(impData.AdGroupPriority, Is.EqualTo(adGroupPriority));
            Assert.That(impData.Country, Is.EqualTo(country));
            Assert.That(impData.Precision, Is.EqualTo(precision));
            Assert.That(impData.NetworkName, Is.EqualTo(networkName));
            Assert.That(impData.NetworkPlacementId, Is.EqualTo(networkPlacementId));
            Assert.That(impData.JsonRepresentation, Is.EqualTo(json));
        }

        [Test]
        public void FromJsonWithSomeValuesShouldYieldSetValues()
        {
            const string adUnitId = "1234";
            const string adUnitFormat = "Rewarded Video";
            const string currency = "USD";
            const double publisherRevenue = 0.00001;
            const string adGroupName = "my great ad group";
            const int adGroupPriority = 1;
            const string precision = "publisher_defined";

            var json = GetJson(
                null,
                adUnitId,
                null,
                adUnitFormat,
                null,
                currency,
                publisherRevenue,
                null,
                adGroupName,
                null,
                adGroupPriority,
                null,
                precision,
                null,
                null);
            MoPub.ImpressionData impData = MoPub.ImpressionData.FromJson(json);

            Assert.That(impData.AppVersion, Is.Null);
            Assert.That(impData.AdUnitId, Is.EqualTo(adUnitId));
            Assert.That(impData.AdUnitName, Is.Null);
            Assert.That(impData.AdUnitFormat, Is.EqualTo(adUnitFormat));
            Assert.That(impData.ImpressionId, Is.Null);
            Assert.That(impData.Currency, Is.EqualTo(currency));
            Assert.That(impData.PublisherRevenue, Is.EqualTo(publisherRevenue));
            Assert.That(impData.AdGroupId, Is.Null);
            Assert.That(impData.AdGroupName, Is.EqualTo(adGroupName));
            Assert.That(impData.AdGroupType, Is.Null);
            Assert.That(impData.AdGroupPriority, Is.EqualTo(adGroupPriority));
            Assert.That(impData.Country, Is.Null);
            Assert.That(impData.Precision, Is.EqualTo(precision));
            Assert.That(impData.NetworkName, Is.Null);
            Assert.That(impData.NetworkPlacementId, Is.Null);
            Assert.That(impData.JsonRepresentation, Is.EqualTo(json));
        }

        [Test]
        public void FromJsonWithExtraValuesShouldYieldAllValues()
        {
            const string appVersion = "1.0.0";
            const string adUnitId = "1234";
            const string adUnitName = "my awesome ad unit";
            const string adUnitFormat = "Rewarded Video";
            const string impressionId = "5678";
            const string currency = "USD";
            const double publisherRevenue = 0.00001;
            const string adGroupId = "9012";
            const string adGroupName = "my great ad group";
            const string adGroupType = "marketplace";
            const int adGroupPriority = 1;
            const string country = "USA";
            const string precision = "publisher_defined";
            const string networkName = "MoPub";
            const string networkPlacementId = "3456";

            var json = GetJson(
                appVersion,
                adUnitId,
                adUnitName,
                adUnitFormat,
                impressionId,
                currency,
                publisherRevenue,
                adGroupId,
                adGroupName,
                adGroupType,
                adGroupPriority,
                country,
                precision,
                networkName,
                networkPlacementId);
            var jsonWithExtra = json.Substring(0, json.Length - 1)
                                + GetJsonEntry("some_extra_key", "some_extra_value") + "}";
            MoPub.ImpressionData impData = MoPub.ImpressionData.FromJson(jsonWithExtra);

            Assert.That(impData.AppVersion, Is.EqualTo(appVersion));
            Assert.That(impData.AdUnitId, Is.EqualTo(adUnitId));
            Assert.That(impData.AdUnitName, Is.EqualTo(adUnitName));
            Assert.That(impData.AdUnitFormat, Is.EqualTo(adUnitFormat));
            Assert.That(impData.ImpressionId, Is.EqualTo(impressionId));
            Assert.That(impData.Currency, Is.EqualTo(currency));
            Assert.That(impData.PublisherRevenue, Is.EqualTo(publisherRevenue));
            Assert.That(impData.AdGroupId, Is.EqualTo(adGroupId));
            Assert.That(impData.AdGroupName, Is.EqualTo(adGroupName));
            Assert.That(impData.AdGroupType, Is.EqualTo(adGroupType));
            Assert.That(impData.AdGroupPriority, Is.EqualTo(adGroupPriority));
            Assert.That(impData.Country, Is.EqualTo(country));
            Assert.That(impData.Precision, Is.EqualTo(precision));
            Assert.That(impData.NetworkName, Is.EqualTo(networkName));
            Assert.That(impData.NetworkPlacementId, Is.EqualTo(networkPlacementId));
            Assert.That(impData.JsonRepresentation, Is.EqualTo(jsonWithExtra));
        }

        [Test]
        public void FromJsonInDecimalCommaCountriesShouldParseValuesCorrectly()
        {
            RunTestWithCulture("fr-FR", () => {
                const string appVersion = "1.0.0";
                const string adUnitId = "1234";
                const string adUnitName = "my awesome ad unit";
                const string adUnitFormat = "Rewarded Video";
                const string impressionId = "5678";
                const string currency = "USD";
                const double publisherRevenue = 3.14159;
                const string adGroupId = "9012";
                const string adGroupName = "my great ad group";
                const string adGroupType = "marketplace";
                const int adGroupPriority = 1;
                const string country = "USA";
                const string precision = "publisher_defined";
                const string networkName = "MoPub";
                const string networkPlacementId = "3456";

                var json = GetJson(
                    appVersion,
                    adUnitId,
                    adUnitName,
                    adUnitFormat,
                    impressionId,
                    currency,
                    publisherRevenue,
                    adGroupId,
                    adGroupName,
                    adGroupType,
                    adGroupPriority,
                    country,
                    precision,
                    networkName,
                    networkPlacementId);

                MoPub.ImpressionData impData = MoPub.ImpressionData.FromJson(json);

                Assert.That(impData.AppVersion, Is.EqualTo(appVersion));
                Assert.That(impData.AdUnitId, Is.EqualTo(adUnitId));
                Assert.That(impData.AdUnitName, Is.EqualTo(adUnitName));
                Assert.That(impData.AdUnitFormat, Is.EqualTo(adUnitFormat));
                Assert.That(impData.ImpressionId, Is.EqualTo(impressionId));
                Assert.That(impData.Currency, Is.EqualTo(currency));
                Assert.That(impData.PublisherRevenue, Is.EqualTo(publisherRevenue));
                Assert.That(impData.AdGroupId, Is.EqualTo(adGroupId));
                Assert.That(impData.AdGroupName, Is.EqualTo(adGroupName));
                Assert.That(impData.AdGroupType, Is.EqualTo(adGroupType));
                Assert.That(impData.AdGroupPriority, Is.EqualTo(adGroupPriority));
                Assert.That(impData.Country, Is.EqualTo(country));
                Assert.That(impData.Precision, Is.EqualTo(precision));
                Assert.That(impData.NetworkName, Is.EqualTo(networkName));
                Assert.That(impData.NetworkPlacementId, Is.EqualTo(networkPlacementId));
                Assert.That(impData.JsonRepresentation, Is.EqualTo(json));
            });
        }

        private static void RunTestWithCulture(string cultureName, Action test)
        {
            var originalCulture = ChangeCurrentCulture(cultureName);
            try {
                test();
            } finally {
                ChangeCurrentCulture(originalCulture);
            }
        }

        private static string ChangeCurrentCulture(string name)
        {
            var originalCulture = Thread.CurrentThread.CurrentCulture.Name;
            var newCulture = CultureInfo.CreateSpecificCulture(name);

            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            Debug.LogFormat("Culture changed from '{0}' to '{1}'", originalCulture, newCulture.Name);

            return originalCulture;
        }

        private static string GetJsonEntry(string key, object value)
        {
            return value != null ? "\"" + key + "\":\"" + MoPubUtils.InvariantCultureToString(value) + "\"," : "";
        }

        private static string GetNullValueJsonEntry(string key)
        {
            return "\"" + key + "\":" + "null" +  ",";
        }

        private static string GetJson(
            string appVersion,
            string adUnitId,
            string adUnitName,
            string adUnitFormat,
            string impressionId,
            string currency,
            object publisherRevenue,
            string adGroupId,
            string adGroupName,
            string adGroupType,
            object adGroupPriority,
            string country,
            string precision,
            string networkName,
            string networkPlacementId)
        {
            return "{"
                   + GetJsonEntry("app_version", appVersion)
                   + GetJsonEntry("adunit_id", adUnitId)
                   + GetJsonEntry("adunit_name", adUnitName)
                   + GetJsonEntry("adunit_format", adUnitFormat)
                   + GetJsonEntry("id", impressionId)
                   + GetJsonEntry("currency", currency)
                   + GetJsonEntry("publisher_revenue", publisherRevenue)
                   + GetJsonEntry("adgroup_id", adGroupId)
                   + GetJsonEntry("adgroup_name", adGroupName)
                   + GetJsonEntry("adgroup_type", adGroupType)
                   + GetJsonEntry("adgroup_priority", adGroupPriority)
                   + GetJsonEntry("country", country)
                   + GetJsonEntry("precision", precision)
                   + GetJsonEntry("network_name", networkName)
                   + GetJsonEntry("network_placement_id", networkPlacementId)
                   + "}";
        }

        private static string GetNullValuesJson()
        {
            return "{"
                   + GetNullValueJsonEntry("app_version")
                   + GetNullValueJsonEntry("adunit_id")
                   + GetNullValueJsonEntry("adunit_name")
                   + GetNullValueJsonEntry("adunit_format")
                   + GetNullValueJsonEntry("id")
                   + GetNullValueJsonEntry("currency")
                   + GetNullValueJsonEntry("publisher_revenue")
                   + GetNullValueJsonEntry("adgroup_id")
                   + GetNullValueJsonEntry("adgroup_name")
                   + GetNullValueJsonEntry("adgroup_type")
                   + GetNullValueJsonEntry("adgroup_priority")
                   + GetNullValueJsonEntry("country")
                   + GetNullValueJsonEntry("precision")
                   + GetNullValueJsonEntry("network_name")
                   + GetNullValueJsonEntry("network_placement_id")
                   + "}";
        }
    }
}
