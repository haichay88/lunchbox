using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.refractored.fab;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Fgo.AndroidApp.Services;
using Bizkasa.Bizlunch.Business.Model;
using Fgo.AndroidApp.Common;
using Fgo.AndroidApp.Adapter;
using Android.Support.V4.App;

namespace Fgo.AndroidApp.Fragments
{
    public class OrdersFragment : Fragment
    {
        private ListView listView;
        private FloatingActionButton fab;
        private List<OrderDTO> m_orders;
        private string m_context;
        private AppPreferences apre;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            return inflater.Inflate(Resource.Layout.order_fragment, container, false);


        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            apre = new AppPreferences(this.Activity);
            m_context = apre.getContextKey();

            this.listView = view.FindViewById<ListView>(Resource.Id.orderslv);
            this.fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
            this.fab.AttachToListView(this.listView);
            this.fab.Click += new System.EventHandler(Addorder_Click);
        }

        private void Addorder_Click(object sender, EventArgs e)
        {
            Fragment fragment = new AddOrderFragment();
            FragmentTransaction ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.fragment, fragment).Commit();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            LoadData();
            this.listView.Adapter = new OrderListItemAdapter(this.Activity, m_orders);       



        }
        /// <summary>
        /// load data from server
        /// </summary>
        private void LoadData()
        {

            UserService m_service = new UserService();
            BaseRequest model = new BaseRequest()
            {
                Token = m_context
            };

            var result = m_service.GetOrders(model);
            if (!result.HasError)
                m_orders = result.Data;

        }

    }
}