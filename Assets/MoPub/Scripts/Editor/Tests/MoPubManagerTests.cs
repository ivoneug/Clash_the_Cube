using System;
using NUnit.Framework;
using UnityEngine;

// ReSharper disable Unity.IncorrectMonoBehaviourInstantiation

namespace Tests
{

    public class MoPubManagerTests : MoPubTest
    {
        [Test]
        public void EmitAdLoadedEventShouldTriggerOnAdLoadedEvent()
        {
            TestEmitAdLoadedEvent("1234", 320, 50.0f);
        }

        [Test]
        public void EmitAdLoadedEventShouldParseIntHeight()
        {
            TestEmitAdLoadedEvent("1234", 320, 50);
        }

        [Test]
        public void EmitAdLoadedEventShouldGracefullyHandleParseFailure()
        {
            TestEmitAdLoadedEvent("1234", 320, " ", true);
        }

        [Test]
        public void EmitRewardedVideoReceivedRewardEventShouldTriggerOnRewardedVideoReceivedRewardEvent()
        {
            TestEmitRewardedVideoReceivedRewardEvent("1234", "coins", 50.0f);
        }

        [Test]
        public void EmitRewardedVideoReceivedRewardEventShouldParseIntHeight()
        {
            TestEmitRewardedVideoReceivedRewardEvent("1234", "coins", 50);
        }

        [Test]
        public void EmitRewardedVideoReceivedRewardEventShouldGracefullyHandleParseFailure()
        {
            TestEmitRewardedVideoReceivedRewardEvent("1234", "coins", " ", true);
        }

        private static void TestEmitAdLoadedEvent(string adUnitId, object width, object height, bool shouldFail = false)
        {
            const string successMessage = "OnAdLoadedEvent triggered.";
            Action<string, float> successHandler = (_adUnitId, _height) => {
                Assert.That(_adUnitId, Is.EqualTo(adUnitId));
                Assert.That(_height, Is.EqualTo(height));
                Debug.Log(successMessage);
            };

            const string failureMessage = "OnAdFailedEvent triggered.";
            Action<string, string> failureHandler = (_adUnitId, _error) => {
                Assert.That(_adUnitId, Is.EqualTo(adUnitId));
                Debug.Log(failureMessage);
            };

            MoPubManager.OnAdLoadedEvent += successHandler;
            MoPubManager.OnAdFailedEvent += failureHandler;
            try {
                var mpm = new MoPubManager();
                mpm.EmitAdLoadedEvent(MoPubUtils.EncodeArgs(adUnitId, width.ToString(), height.ToString()));
                LogAssert.Expect(LogType.Log, shouldFail ? failureMessage : successMessage);
            } finally {
                MoPubManager.OnAdLoadedEvent -= successHandler;
                MoPubManager.OnAdFailedEvent -= failureHandler;
            }
        }

        private static void TestEmitRewardedVideoReceivedRewardEvent(string adUnitId, string label, object amount,
            bool shouldFail = false)
        {
            const string successMessage = "OnRewardedVideoReceivedRewardEvent triggered.";
            Action<string, string, float> successHandler = (_adUnitId, _label, _amount) => {
                Assert.That(_adUnitId, Is.EqualTo(adUnitId));
                Assert.That(_label, Is.EqualTo(label));
                Assert.That(_amount, Is.EqualTo(amount));
                Debug.Log(successMessage);
            };

            const string failureMessage = "OnRewardedVideoFailedEvent triggered.";
            Action<string, string> failureHandler = (_adUnitId, _error) => {
                Assert.That(_adUnitId, Is.EqualTo(adUnitId));
                Debug.Log(failureMessage);
            };

            MoPubManager.OnRewardedVideoReceivedRewardEvent += successHandler;
            MoPubManager.OnRewardedVideoFailedEvent += failureHandler;
            try {
                var mpm = new MoPubManager();
                mpm.EmitRewardedVideoReceivedRewardEvent(MoPubUtils.EncodeArgs(adUnitId, label, amount.ToString()));
                LogAssert.Expect(LogType.Log, shouldFail ? failureMessage : successMessage);
            } finally {
                MoPubManager.OnRewardedVideoReceivedRewardEvent -= successHandler;
                MoPubManager.OnRewardedVideoFailedEvent -= failureHandler;
            }
        }
    }
}
