using Android;

namespace Tetris.Platforms.Android
{
    /// <summary>
    /// Represents the platform-specific permission for posting notifications on Android.
    /// Handles runtime permission requirements for Android 13 (API 33) and above.
    /// </summary>
    public class NotificationPermission : Permissions.BasePlatformPermission
    {
        #region Properties

        /// <summary>
        /// Gets the list of Android permissions required for this platform permission.
        /// </summary>
        /// <remarks>
        /// Only includes <c>Manifest.Permission.PostNotifications</c> on Android 13+ (API 33+),
        /// and marks it as a runtime permission.
        /// </remarks>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                List<(string androidPermission, bool isRuntime)> result = new();

                // Android 13+ requires runtime permission for posting notifications
                if (OperatingSystem.IsAndroidVersionAtLeast(33))
                    result.Add((Manifest.Permission.PostNotifications, true));

                return result.ToArray();
            }
        }

        #endregion
    }
}