using Android.App;
using Android.Widget;
using Android.OS;
using Fgo.AndroidApp.Common;
using Android.Content;
using System;
using Newtonsoft.Json;
using Fgo.AndroidApp.Services;
using Bizkasa.Bizlunch.Business.Model;

namespace Fgo.AndroidApp
{
    [Activity(Label = "Fgo.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ProgressDialog progress;
        EditText userNametext;
        EditText passwordtext;
        AppPreferences apre;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            apre = new AppPreferences(this);

            var m_context = apre.getContextKey();


            if (!string.IsNullOrEmpty(m_context))
            {
                StartActivity(typeof(OrderListActivity));
            }
            else
            {
                SetContentView(Resource.Layout.Main);

                userNametext = FindViewById<EditText>(Resource.Id.txtuserName);
                passwordtext = FindViewById<EditText>(Resource.Id.txtpass);
                Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

                btnLogin.Click += new System.EventHandler(btnLogin_Click);
                TextView txtdangky = FindViewById<TextView>(Resource.Id.txtdangky);
                txtdangky.Click += (object sender, System.EventArgs e) =>
                {
                    Intent viewIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://Bizkasa.com/"));
                    StartActivity(viewIntent);


                };
            }
            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordtext.Text) || string.IsNullOrWhiteSpace(userNametext.Text))
            {
                Toast.MakeText(this, "Thông tin không hợp lệ !", ToastLength.Long).Show();
                return;
            }
            progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Đang đăng nhập...Xin chờ");
            progress.SetCancelable(false);
            progress.Show();
            UserService _Service = new UserService();
            LoginDTO data = new LoginDTO()
            {
                Email = userNametext.Text,
                Password = passwordtext.Text
            };
            var result = _Service.Login(data);
            if (!result.HasError)
            {
               
                apre.saveAccessKey(AppPreferences.m_workContextKey, result.Data.Token);
                StartActivity(typeof(OrderListActivity));
            }
            else
            {
                Toast.MakeText(this, result.ToErrorMsg(), ToastLength.Long).Show();
                progress.Hide();
            }


        }
    }
}

