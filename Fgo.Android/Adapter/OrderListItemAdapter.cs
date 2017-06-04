using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using Fgo.AndroidApp.Model;
using Bizkasa.Bizlunch.Business.Model;

namespace Fgo.AndroidApp.Adapter
{
    public  class OrderListItemAdapter: BaseAdapter
    {
        List<OrderDTO> items;
        Activity context;
        public OrderListItemAdapter(Activity context, List<OrderDTO> items) : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            return null;
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Resource.Layout.OrderListItem, parent, false);
            var item = items[position];
            TextView txttitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
            txttitle.Text = item.Title;

            TextView txtlocation = view.FindViewById<TextView>(Resource.Id.txtlocation);
            txtlocation.Text = item.RestaurantName;


            TextView txtDate = view.FindViewById<TextView>(Resource.Id.txtDate);
            txtDate.Text = item.LunchDateText;

            view.SetPadding(10, 20, 10, 20);
            return view;
        }
    }
}