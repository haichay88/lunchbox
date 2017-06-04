using Android.OS;
using Android.Views;
using Fragment = Android.Support.V4.App.Fragment;
namespace Fgo.AndroidApp.Fragments
{
    public class AddOrderFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.AddOrder_fragment, container, false);

           // return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}