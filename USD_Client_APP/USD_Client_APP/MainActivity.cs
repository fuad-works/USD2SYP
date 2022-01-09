using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System;
using Android.Content;
using RestSharp;
using System.Collections.Generic;
using USD_Client_APP.Models;
using System.Threading;

namespace USD_Client_APP
{
    [Activity(Label = "الدولار اليوم", MainLauncher = true, Icon = "@drawable/CurrencyExchange")]
    public class MainActivity : Activity
    {
        TextView maxPriceTv, minPriceTv, dateUpdateTv, t1, t2, t3;
        Button btn, btn1_d, btn2_s;
        EditText dollars, syrian;
        float dollarPrice = 0;

        ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        Android.App.ProgressDialog progress;

        private SynchronizationContext sc;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            float maxPrice1 = pref.GetFloat("maxPrice", 0);
            float minPrice1 = pref.GetFloat("minPrice", 0);
            string dateUpdate = pref.GetString("dateUpdate", String.Empty);

            btn = FindViewById<Button>(Resource.Id.button1);
            btn1_d = FindViewById<Button>(Resource.Id.button2);
            btn2_s = FindViewById<Button>(Resource.Id.button3);
            maxPriceTv = FindViewById<TextView>(Resource.Id.maxPriceTv);
            minPriceTv = FindViewById<TextView>(Resource.Id.minPriceTv);
            dateUpdateTv = FindViewById<TextView>(Resource.Id.dateUpdateTv);

            t1 = FindViewById<TextView>(Resource.Id.textView1);
            t2 = FindViewById<TextView>(Resource.Id.textView2);
            t3 = FindViewById<TextView>(Resource.Id.textView3);
            dollars = FindViewById<EditText>(Resource.Id.editText1);
            syrian = FindViewById<EditText>(Resource.Id.editText2);

            Typeface tf = Typeface.CreateFromAsset(Assets, "Kufy.ttf");
            maxPriceTv.SetTypeface(tf, TypefaceStyle.Normal);
            minPriceTv.SetTypeface(tf, TypefaceStyle.Normal);
            dateUpdateTv.SetTypeface(tf, TypefaceStyle.Normal);
            t1.SetTypeface(tf, TypefaceStyle.Normal);
            t2.SetTypeface(tf, TypefaceStyle.Normal);
            t3.SetTypeface(tf, TypefaceStyle.Normal);
            btn.SetTypeface(tf, TypefaceStyle.Normal);

            maxPriceTv.Text = maxPrice1.ToString();
            minPriceTv.Text = minPrice1.ToString();
            dateUpdateTv.Text = dateUpdate;


            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("يتم تحديث البيانات ... يرجى الإنتظار....");
            progress.SetCancelable(false);

            sc = SynchronizationContext.Current;

            btn.Click += Btn_Click;

            StartService(new Intent(this, typeof(BackgroundService)));

            dollarPrice = maxPrice1;

            btn1_d.Click += Btn1_d_Click;
            btn2_s.Click += Bt2_s_Click;
        }

        private void Bt2_s_Click(object sender, EventArgs e)
        {
            try
            {
                float num = float.Parse(syrian.Text);
                float rst = num / dollarPrice;
                dollars.Text = rst.ToString();
            }
            catch
            {
                dollars.Text = "0";
            }
        }

        private void Btn1_d_Click(object sender, EventArgs e)
        {
            try
            {
                float num = float.Parse(dollars.Text);
                float rst = num * dollarPrice;
                syrian.Text = rst.ToString();
            }
            catch
            {
                syrian.Text = "0";
            }
        }


        private void Btn_Click(object sender, EventArgs e)
        {
            getJSON();
        }

        private async void getJSON()
        {
            progress.Show();
            IRestClient client = new RestClient(BaseInfo.Base_Url);
            IRestRequest request = new RestRequest("api/PriceAPI/", Method.GET);

            try
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    IRestResponse<List<Price>> response = client.Execute<List<Price>>(request);
                    foreach (var price in response.Data)
                    {
                        sc.Post(new SendOrPostCallback(o =>
                        {
                            maxPriceTv.Text = price.maxPrice.ToString();
                            minPriceTv.Text = price.minPrice.ToString();
                            dateUpdateTv.Text = price.UpdateDate;

                            dollarPrice = (float)price.maxPrice;

                            ISharedPreferencesEditor edit = pref.Edit();
                            edit.PutFloat("maxPrice", (float)price.maxPrice);
                            edit.PutFloat("minPrice", (float)price.minPrice);
                            edit.PutString("dateUpdate", price.UpdateDate);
                            edit.Apply();

                        }), price.UpdateDate);
                    }
                    Toast.MakeText(this, "تم التحديث بنجاح", ToastLength.Short).Show();
                    progress.Hide();
                });
            }
            catch (Exception e)
            {
                //Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                 progress.Hide();
                Console.WriteLine(e.Message);
            }
        }
    }
}

