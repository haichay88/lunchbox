using System.Collections.Generic;
using Java.Lang;
using Android.App;
using Android.Views;
using Android.Widget;
using Bizkasa.Bizlunch.Business.Model;
using System.Linq;

namespace Fgo.AndroidApp.Adapter
{
   public class FriendAdapter : BaseAdapter,IFilterable
    {

       public List<FriendDTO> items;
        public string[] firstNames;
        Activity context;
        ArrayFilterr filterr;
        public FriendAdapter(Activity context, List<FriendDTO> items) : base()
        {
            this.context = context;
            this.items = items;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
           
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Resource.Layout.FriendItem, parent, false);
            var item = items[position];
            TextView txtfullname = view.FindViewById<TextView>(Resource.Id.txtFullname);
            txtfullname.Text = item.FirstName + " "+item.LastName;

            TextView txtemail = view.FindViewById<TextView>(Resource.Id.txtemail);
            txtemail.Text = item.Email;

            //fill in your items
            //holder.Title.Text = "new text here";

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return 0;
            }
        }

        public Filter Filter
        {
            get
            {
                if (filterr == null)
                {
                    filterr = new ArrayFilterr();
                    filterr.OriginalData = this.items;
                    filterr.SAdapter = this;
                }
                return filterr;
            }
        }
    }

    public class ArrayFilterr : Filter

    {
      public List<FriendDTO> OriginalData { get; set; }
        //public string[] OriginalData
        //{
        //    get { return this.originalData; }
        //    set { this.originalData = value; }
        //}

      public FriendAdapter SAdapter { get; set; }
        //public SearchAdapter SAdapter
        //{
        //    get { return adapter; }
        //    set { this.adapter = value; }
        //}

        protected override Filter.FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
        {

            FilterResults results = new FilterResults();           

            if (constraint != null )
            {
              

                List<FriendDTO> matchList = new List<FriendDTO>();
              
                foreach (FriendDTO friend in this.SAdapter.items)
                {
                    if (friend.Email.ToUpper().Contains(constraint.ToString().ToUpper()))
                    {
                   
                        matchList.Add(friend);
                    }
                }

                results.Count = matchList.Count;
                results.Values =matchList.Select(a=>a.FirstName).ToArray();

            }

            return results;
        }

        protected override void PublishResults(Java.Lang.ICharSequence constraint, Filter.FilterResults results)
        {
            if (results.Count == 0)
                this.SAdapter.NotifyDataSetInvalidated();
            else
            {
                SAdapter.firstNames = (string[])results.Values;
                SAdapter.NotifyDataSetChanged();
            }
        }
    }
}