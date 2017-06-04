using System.Collections.Generic;

using Android.App;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Fgo.AndroidApp.Model;
using Fgo.AndroidApp.Common;
using Fgo.AndroidApp.Adapter;
using Bizkasa.Bizlunch.Business.Model;
using Fgo.AndroidApp.Fragments;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
namespace Fgo.AndroidApp
{
    [Activity(Label = "OrderListActivity")]
    public class OrderListActivity : AppCompatActivity
    {
        private SupportToolbar mToolbar;
        private MyActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;
        private List<MenuItem> menus;
        private string m_context;
        AppPreferences apre;     

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.OrderListActivity);

            apre = new AppPreferences(this);
            m_context = apre.getContextKey();

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            // mRightDrawer = FindViewById<ListView>(Resource.Id.right_drawer);

            mLeftDrawer.Tag = 0;
            // mRightDrawer.Tag = 1;


            SetSupportActionBar(mToolbar);
            SupportActionBar.SetTitle(Resource.String.closeDrawer);
           
            menus = new List<MenuItem>();
            menus.Add(new MenuItem() { Heading = "Order", ImageResourceId = Resource.Drawable.Synchronize96 });
            menus.Add(new MenuItem() { Heading = "Friends", ImageResourceId = Resource.Drawable.Cheap });
            menus.Add(new MenuItem() { Heading = "Restaurants", ImageResourceId = Resource.Drawable.News96 });
            //menus.Add(new MenuItem() { Heading = "Khách đang ở", ImageResourceId = Resource.Drawable.UserMale96 });
            menus.Add(new MenuItem() { Heading = "Exit", ImageResourceId = Resource.Drawable.Lock96 });
            mLeftDrawer.Adapter = new LeftMenuAdapter(this, menus);
            mLeftDrawer.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(mLeftDrawer_ItemClick);



            mDrawerToggle = new MyActionBarDrawerToggle(
                this,                           //Host Activity
                mDrawerLayout,                 //DrawerLayout
                Resource.String.openDrawer,     //Opened Message
                Resource.String.closeDrawer     //Closed Message
            );

            mDrawerLayout.AddDrawerListener(mDrawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mDrawerToggle.SyncState();

            if (savedInstanceState == null)
            {
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.fragment, new OrdersFragment()).Commit();
            }



        }

        private void mLeftDrawer_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Fragment fragment = null;
            FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
            if (mDrawerLayout.IsDrawerOpen(mLeftDrawer))
            {
                //Right Drawer is already open, close it
                mDrawerLayout.CloseDrawer(mLeftDrawer);
            }
            switch (e.Position)
            {
                case 0:
                    fragment = new OrdersFragment();
                    break;
                //case 2:
                //    StartActivity(typeof(OrderInDayActivity));
                //    break;
                //case 3:
                //    StartActivity(typeof(CustomerCheckinActivity));
                //    break;
                case 3:
                    apre.DeleteAccessKey(AppPreferences.m_workContextKey);
                    StartActivity(typeof(MainActivity));
                    break;
                default:
                    break;

            }
            ft.Replace(Resource.Id.fragment, fragment).Commit();
           

        }
        

        protected override void OnSaveInstanceState(Bundle outState)
        {
            //if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            //{
            //    outState.PutString("DrawerState", "Opened");
            //}

            //else
            //{
            //    outState.PutString("DrawerState", "Closed");
            //}

            base.OnSaveInstanceState(outState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            mDrawerToggle.OnConfigurationChanged(newConfig);
        }
    }
}