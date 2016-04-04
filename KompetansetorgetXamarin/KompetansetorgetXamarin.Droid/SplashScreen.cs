using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using KompetansetorgetXamarin.Droid;

[Activity(Label = "Kompetansetorget", MainLauncher = true, NoHistory = true, Theme = "@style/Theme.Splash",
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
public class SplashActivity : Activity
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        var intent = new Intent(this, typeof(MainActivity));

        System.Threading.Thread.Sleep(2000);
        this.StartActivity(typeof(MainActivity));
    }
}