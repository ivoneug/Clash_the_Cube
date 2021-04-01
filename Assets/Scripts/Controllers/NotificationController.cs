using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Events;
using EasyMobile;

using LocalNotification = EasyMobile.LocalNotification;

namespace ClashTheCube
{
    public class NotificationController : MonoBehaviour
    {
        [SerializeField] private LocalisationController localisationController;
        [SerializeField] private GameEvent dailyRewardEvent;

        private void Awake()
        {
            if (!RuntimeManager.IsInitialized())
            {
                RuntimeManager.Init();
            }
        }

        void OnEnable()
        {
            Notifications.LocalNotificationOpened += OnLocalNotificationOpened;
        }

        void OnDisable()
        {
            Notifications.LocalNotificationOpened -= OnLocalNotificationOpened;
        }

        private void OnLocalNotificationOpened(LocalNotification delivered)
        {
            if (dailyRewardEvent)
            {
                dailyRewardEvent.Raise();
            }
        }

        private NotificationContent PrepareNotificationContent()
        {
            string titleKey = "notificationDailyRewardTitle";
            string bodyKey = "notificationDailyRewardBody";

            NotificationContent content = new NotificationContent();
            content.title = localisationController.GetLocalizedText(titleKey);

            // You can optionally provide the notification subtitle, which is visible on iOS only.
            // content.subtitle = "Demo Subtitle";
            content.body = localisationController.GetLocalizedText(bodyKey);

            // If you want to use default small icon and large icon (on Android),
            // don't set the smallIcon and largeIcon fields of the content.
            // If you want to use custom icons instead, simply specify their names here (without file extensions). content.smallIcon = "YOUR_CUSTOM_SMALL_ICON";
            // content.largeIcon = "YOUR_CUSTOM_LARGE_ICON";

            return content;
        }

        private DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        private TimeSpan GetNextWeekdayTime(DayOfWeek day, int hour, int minute, int second)
        {
            DateTime weekday = GetNextWeekday(DateTime.Today.AddDays(1), day);
            DateTime weekdayFull = new DateTime(weekday.Year, weekday.Month, weekday.Day, hour, minute, second);

            return weekdayFull - DateTime.Now;
        }

        private TimeSpan GetNextDayTime(int hour, int minute, int second)
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);
            DateTime tomorrowFull = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, hour, minute, second);

            return tomorrowFull - DateTime.Now;
        }

        public void ScheduleNotifications()
        {
            Notifications.CancelAllPendingLocalNotifications();
            Debug.Log("All pending notifications has been cleared");

            NotificationContent content = PrepareNotificationContent();
            if (content == null)
            {
                return;
            }

            TimeSpan time = GetNextDayTime(10, 0, 0);
            NotificationRepeat repeat = NotificationRepeat.None;

            // if (settingsInfo.reminderType == SettingsInfo.ReminderType.OnceADay)
            // {
            //     time = GetNextDayTime(10, 0, 0);
            //     repeat = NotificationRepeat.EveryDay;
            // }
            // else if (settingsInfo.reminderType == SettingsInfo.ReminderType.OnceAWeek)
            // {
            //     time = GetNextWeekdayTime(DayOfWeek.Tuesday, 10, 0, 0);
            //     repeat = NotificationRepeat.EveryWeek;
            // }

            string notificationId = Notifications.ScheduleLocalNotification(time, content, repeat);
            Debug.Log("Scheduled notification: " + notificationId + "\nTime: " + time + "\nRepeatType: " + repeat);
        }

        public void ScheduleOneTimeTestNotification()
        {
            NotificationContent content = PrepareNotificationContent();
            if (content == null)
            {
                return;
            }

            TimeSpan time = new TimeSpan(0, 1, 0);
            NotificationRepeat repeat = NotificationRepeat.None;

            string notificationId = Notifications.ScheduleLocalNotification(time, content, repeat);
            Debug.Log("Scheduled notification: " + notificationId + "\nTime: " + time + "\nRepeatType: " + repeat);
        }
    }
}
