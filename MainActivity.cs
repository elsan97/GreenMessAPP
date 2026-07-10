using Android.App;
using Android.OS;
using Android.Webkit;
using Android.Content.PM;
using AndroidX.AppCompat.App;
using Android.Content;
using Android.Views;
using System;

namespace Green_Mess2
{
    [Activity(
    Name = "com.Green_Mess2.MainActivity",
    Label = "Green Mess",
    Theme = "@style/Theme.AppCompat.NoActionBar",
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
    public class MainActivity : AppCompatActivity

    {
        private WebView webView;
        private IValueCallback filePathCallback;
        private int FILE_CHOOSER_RESULT = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#0b1a0e"));

            webView = new WebView(this);
            SetContentView(webView);

            var s = webView.Settings;
            s.JavaScriptEnabled = true;
            s.DomStorageEnabled = true;
            s.AllowFileAccess = true;
            s.MediaPlaybackRequiresUserGesture = false;
            s.MixedContentMode = MixedContentHandling.AlwaysAllow;

            webView.SetWebViewClient(new WebViewClient());
            webView.SetWebChromeClient(new MyWebChromeClient(this));

            RequestPermissions(new string[] {
                Android.Manifest.Permission.RecordAudio,
                Android.Manifest.Permission.Camera,
                Android.Manifest.Permission.ReadExternalStorage
            }, 0);

            webView.LoadUrl("http://y93938hg.beget.tech"); 
        }

        public class MyWebChromeClient : WebChromeClient
        {
            MainActivity activity;
            public MyWebChromeClient(MainActivity act) { activity = act; }

            public override void OnPermissionRequest(PermissionRequest request)
            {
                request.Grant(request.GetResources());
            }

            public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
            {
                activity.filePathCallback = filePathCallback;
                var intent = new Intent(Intent.ActionGetContent);
                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType("image/*");
                activity.StartActivityForResult(Intent.CreateChooser(intent, "Выберите фото"), activity.FILE_CHOOSER_RESULT);
                return true;
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == FILE_CHOOSER_RESULT)
            {
                if (filePathCallback == null) return;
                var result = WebChromeClient.FileChooserParams.ParseResult((int)resultCode, data);
                filePathCallback.OnReceiveValue(result);
                filePathCallback = null;
            }
        }
    }
}
