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
        List<OrderViewDTO> items;
        Activity context;
        public OrderListItemAdapter(Activity context, List<OrderViewDTO> items) : base()
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
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, parent, false);
            var item = items[position];
            TextView txtMenutext = view.FindViewById<TextView>(Android.Resource.Id.Text1);
           // txtMenutext.Text = item.Heading;
           

            view.SetPadding(10, 20, 10, 20);
            return view;
        }
    }
}