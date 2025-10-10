using Firebase.Auth;
using Firebase.Auth.Providers;
using Plugin.CloudFirestore;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class FbData : FbDataModel
    {
        public override async Task<bool> CreateUserWithEmailAndPWAsync(string email, string password, string userName, Func<Task, Task<bool>> OnCompleteRegister)
        {
            Task<Firebase.Auth.UserCredential> firebaseTask = facl.CreateUserWithEmailAndPasswordAsync(email, password, userName);
            bool success = false;

            try
            {
                UserCredential credential = await firebaseTask;
                Firebase.Auth.User user = credential.User;

                // Immediately sign in the new user so Firestore writes can succeed
                await facl.SignInWithEmailAndPasswordAsync(email, password);

                string userId = facl.User.Uid;
                await fdb.Collection("users").Document(userId).SetAsync(new
                {
                    userName,
                    email,
                    password,
                    dateJoined = DateTime.Now.ToString("dd/MM/yy"),
                    gamesPlayed = 0,
                    highestScore = 0,
                    settings0 = true,
                    settings1 = true,
                    settings2 = true,
                    totalLinesCleared = 0,
                });
            }
            catch (Exception ex)
            {
                TaskCompletionSource<Firebase.Auth.UserCredential> tcs = new();
                tcs.SetException(ex);
                firebaseTask = tcs.Task;
            }
            finally
            {
                success = await OnCompleteRegister(firebaseTask);
            }
            return success;
        }
        public override async Task<bool> SignInWithEmailAndPWdAsync(string email, string password, Func<Task, Task<bool>> OnCompleteLogin)
        {
            // Start Firebase sign-in
            Task<Firebase.Auth.UserCredential> firebaseTask = facl.SignInWithEmailAndPasswordAsync(email, password);
            bool success = false;

            try
            {
                // Await Firebase sign-in
                await firebaseTask;
            }
            catch (Exception ex)
            {
                // Wrap the exception in a Task to pass to the callback
                TaskCompletionSource<Firebase.Auth.UserCredential> tcs = new();
                tcs.SetException(ex);
                firebaseTask = tcs.Task;
            }
            finally
            {
                // Always invoke the callback, even if the sign-in failed
                success = await OnCompleteLogin(firebaseTask);
            }

            return success;
        }
        // 2) The actual SignInWithGoogleAsync implementation inside FbData (replace or add to FbData)


        public async Task<bool> SignInWithGoogleAsync(Func<Task, Task<bool>> OnCompleteLogin)
        {
            Task<Firebase.Auth.UserCredential> firebaseTask = null!;
            bool success = false;

            try
            {
                // Use Google provider
                var providerType = FirebaseProviderType.Google;

                // Correct redirect URI for Firebase project
                string firebaseRedirectUri = "https://tetris-gcs71.firebaseapp.com/__/auth/handler";
                string redirect = "";

                firebaseTask = facl.SignInWithRedirectAsync(providerType, async uri =>
                {
                    // Open browser/webview to start OAuth and wait for redirect
                    redirect = await OpenBrowserAndWaitForRedirectAsync(firebaseRedirectUri);
                    return redirect;
                });


                System.Diagnostics.Debug.WriteLine($"Redirect captured: {redirect}");

                // Wait for Firebase login to complete
                await firebaseTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Google login failed: {ex}");
                // Wrap exception so your existing callback receives it
                var tcs = new TaskCompletionSource<Firebase.Auth.UserCredential>();
                tcs.SetException(ex);
                firebaseTask = tcs.Task;
            }
            finally
            {
                // Call your existing callback
                success = await OnCompleteLogin(firebaseTask);
            }

            return success;
        }


        // Helper that shows a modal page with a WebView and completes when it sees
        // a navigation to the firebase auth handler URL.
        private Task<string> OpenBrowserAndWaitForRedirectAsync(string startUrl)
        {
            var tcs = new TaskCompletionSource<string>();

            // Ensure UI thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Create WebView for OAuth
                var webView = new WebView
                {
                    Source = startUrl
                };

                webView.Navigating += async (s, e) =>
                {
                    // Firebase auth handler redirect
                    var authHandlerPrefix = $"https://tetris-gcs71.firebaseapp.com/__/auth/handler";

                    if (e.Url.StartsWith(authHandlerPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        e.Cancel = true;

                        // Pass the full redirect URL to TaskCompletionSource
                        tcs.TrySetResult(e.Url);

                        // Close modal
                        await Shell.Current.CurrentPage.Navigation.PopModalAsync();
                    }
                };

                var modalPage = new ContentPage
                {
                    Title = "Google Sign In",
                    Content = webView,
                    BackgroundColor = Colors.White
                };

                // Push modal on top of current page
                await Shell.Current.CurrentPage.Navigation.PushModalAsync(modalPage);
            });

            return tcs.Task;
        }

        public override void SignOut()
        {
            if (facl != null && facl.User != null)
                facl.SignOut();
        }
        public override async Task<T> GetUserDataAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(facl.User?.Uid))
                return default!;
            IDocumentSnapshot snapshot = await fdb.Collection("users").Document(facl.User.Uid).GetAsync();
            if (!snapshot.Exists)
                return default!;

            // Firebase Cloud Firestore supports strongly-typed Get<T>
            T? value = snapshot.Get<T>(key);
            return value != null ? value : default!;
        }

    }
}
