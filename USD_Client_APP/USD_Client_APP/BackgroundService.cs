using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using USD_Client_APP.Models;
using Microsoft.AspNet.SignalR.Client;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using Android.Util;

namespace USD_Client_APP
{
    [Service(Name = "com.xamarin.example.BackgroundService")]
    class BackgroundService : Service
    {
        ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        private static readonly int ButtonClickNotificationId = 1000;
        private SynchronizationContext sc;

        static readonly string TAG = "X:" + typeof(BackgroundService).Name;
        static readonly int TimerWait = 5000;
        Timer timer;
        DateTime startTime;
        bool isStarted = false;

        Price newP;

        public override async void OnCreate()
        {
            base.OnCreate();

            sc = SynchronizationContext.Current;

            var hubConnection = new HubConnection(BaseInfo.Base_Url);
            var mhubProxy = hubConnection.CreateHubProxy("uSDHub");

            try
            {
                await hubConnection.Start();
            }
            catch (Exception e)
            {
                //Catch handle Errors.   
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }
            mhubProxy.On<double, double, string>("Send_Price", (mPrice, iPrice, date1) =>
            {
                MakeNotification(mPrice, iPrice, date1);
            });
        }

        private void MakeNotification(double mPrice, double iPrice, string date1)
        {
            float maxPrice1 = pref.GetFloat("maxPrice", 0);
            float minPrice1 = pref.GetFloat("minPrice", 0);

            if (mPrice != maxPrice1 || iPrice != minPrice1)
            {
                // When the user clicks the notification, SecondActivity will start up.
                Intent resultIntent = new Intent(this, typeof(MainActivity));

                // Construct a back stack for cross-task navigation:
                TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
                stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
                stackBuilder.AddNextIntent(resultIntent);

                // Create the PendingIntent with the back stack:            
                PendingIntent resultPendingIntent =
                    stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

                // Build the notification:
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                    .SetAutoCancel(true)                    // Dismiss from the notif. area when clicked
                    .SetContentIntent(resultPendingIntent)  // Start 2nd activity when the intent is clicked.
                    .SetContentTitle("تم تحديث سعر الدولار")      // Set its title
                    .SetNumber(0)                       // Display the count in the Content Info
                    .SetSmallIcon(Resource.Drawable.CurrencyExchange)  // Display this icon
                    .SetContentText(String.Format(
                        "المبيع " + mPrice + " :: الشراء " + iPrice, 0)); // The message to display.

                // Finally, publish the notification:
                NotificationManager notificationManager =
                    (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Notify(ButtonClickNotificationId, builder.Build());


                ISharedPreferencesEditor edit = pref.Edit();
                edit.PutFloat("maxPrice", (float)mPrice);
                edit.PutFloat("minPrice", (float)iPrice);
                edit.PutString("dateUpdate", date1);
                edit.Apply();
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {


            Log.Debug(TAG, $"OnStartCommand called at {startTime}, flags={flags}, startid={startId}");
            if (isStarted)
            {
                TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
                Log.Debug(TAG, $"This service was already started, it's been running for {runtime:c}.");
            }
            else
            {
                startTime = DateTime.UtcNow;
                Log.Debug(TAG, $"Starting the service, at {startTime}.");
                timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
                isStarted = true;
            }


            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }


        public override void OnDestroy()
        {
            timer.Dispose();
            timer = null;
            isStarted = false;

            TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(TAG, $"Simple Service destroyed at {DateTime.UtcNow} after running for {runtime:c}.");
            base.OnDestroy();
        }


        void HandleTimerCallback(object state)
        {
            TimeSpan runTime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(TAG, $"This service has been running for {runTime:c} (since ${state}).");
        }
    }
}