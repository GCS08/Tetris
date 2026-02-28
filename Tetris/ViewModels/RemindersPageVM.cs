using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Tetris.Models;
using Tetris.ModelsLogic;
using Tetris.Views;

namespace Tetris.ViewModels
{
    public class RemindersPageVM
    {
        private readonly Notifications notifications;

        public ICommand NavBackHomeCommand => new Command(NavHome);
        public ICommand ScheduleReminderCommand => new Command(ScheduleReminder);

        public DateTime SelectedDate { get; set; } = DateTime.Now.Date;
        public TimeSpan SelectedTime { get; set; } = DateTime.Now.TimeOfDay;
        public string SelectedSeconds { get; set; } = TechnicalConsts.ZeroSignString;

        public RemindersPageVM()
        {
            notifications = new Notifications();
            notifications.NotificationReceived += OnNotificationReceived;
        }

        private void ScheduleReminder()
        {
            IToast toast = notifications.ScheduleReminder(SelectedDate, SelectedTime, SelectedSeconds)
                ? Toast.Make(Strings.NotificationSuccess, CommunityToolkit.Maui.Core.ToastDuration.Long, ConstData.ToastFontSize)
                : Toast.Make(Strings.NotificationFail, CommunityToolkit.Maui.Core.ToastDuration.Long, ConstData.ToastFontSize);
            toast.Show();
        }

        private void OnNotificationReceived(object? sender, NotificationEventArgs e)
        {
            NavHome();
        }

        private void NavHome()
        {
            Shell.Current.Navigation.PushAsync(new MainPage());
        }
    }
}