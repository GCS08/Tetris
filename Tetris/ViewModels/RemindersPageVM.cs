using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    /// <summary>
    /// ViewModel for the Reminders page.
    /// Handles scheduling notifications and navigation back to the main page.
    /// </summary>
    public class RemindersPageVM
    {
        #region Fields
        private readonly Notifications notifications;
        #endregion

        #region ICommands
        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand ScheduleReminderCommand => new Command(ScheduleReminder);
        #endregion

        #region Properties
        public DateTime SelectedDate { get; set; } = DateTime.Now.Date;
        public TimeSpan SelectedTime { get; set; } = DateTime.Now.TimeOfDay;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RemindersPageVM"/> 
        /// and sets up notification handling.
        /// </summary>
        public RemindersPageVM()
        {
            notifications = new Notifications();
            notifications.NotificationReceived += OnNotificationReceived;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Schedules a reminder using the <see cref="Notifications"/> service 
        /// and displays a toast indicating success or failure.
        /// </summary>
        private void ScheduleReminder()
        {
            IToast toast = notifications.ScheduleReminder(SelectedDate,
                SelectedTime, TechnicalConsts.ZeroSignString)
                ? Toast.Make(Strings.NotificationSuccess, CommunityToolkit.
                Maui.Core.ToastDuration.Long, ConstData.ToastFontSize)
                : Toast.Make(Strings.NotificationFail, CommunityToolkit.Maui
                .Core.ToastDuration.Long, ConstData.ToastFontSize);

            toast.Show();
        }

        /// <summary>
        /// Handles a notification being received and navigates back to the main page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Notification event arguments.</param>
        private void OnNotificationReceived(object? sender, NotificationEventArgs e)
        {
            NavHome();
        }

        /// <summary>
        /// Navigates back to the main page.
        /// </summary>
        private void NavHome()
        {
            Shell.Current.Navigation.PushAsync(new MainPage());
        }

        #endregion
    }
}