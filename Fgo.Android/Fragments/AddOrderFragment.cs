using Android.OS;
using Android.Views;
using Android.Widget;
using Bizkasa.Bizlunch.Business.Model;
using Fgo.AndroidApp.Adapter;
using Fgo.AndroidApp.Common;
using Fgo.AndroidApp.Services;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;
namespace Fgo.AndroidApp.Fragments
{
    public class AddOrderFragment : Fragment
    {
        public AppPreferences apre;
        private string m_context;
        private AutoCompleteTextView autocomplete;
        private List<FriendDTO> m_friend;
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
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            apre = new AppPreferences(this.Activity);
            m_context = apre.getContextKey();
            LoadFriend();
            this.autocomplete = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoFriends);
            var adapter = new ArrayAdapter(this.Activity, Android.Resource.Layout.ListContent,m_friend);
            this.autocomplete.Adapter = adapter;
        }

        private void LoadFriend()
        {
            UserService m_service = new UserService();
            SearchDTO model = new SearchDTO()
            {
                Token = m_context
            };

            var result = m_service.GetFriends(model);
            m_friend = result.Data;
        }
    }
}