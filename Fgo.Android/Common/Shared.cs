using System;
using Android.Content;
using Android.Preferences;
using Bizkasa.Bizlunch.Business.Model;
using Newtonsoft.Json;

namespace Fgo.AndroidApp.Common
{
    public class AppPreferences
    {
        private ISharedPreferences mSharedPrefs;
        private ISharedPreferencesEditor mPrefsEditor;
        private Context mContext;

        public static readonly String Domain_API = "http://friendgonow.com/API/";
        public static readonly String m_workContextKey = "FGOAPP";


        public AppPreferences(Context context)
        {
            this.mContext = context;
            mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            mPrefsEditor = mSharedPrefs.Edit();
        }

        public void saveAccessKey(string key, string value)
        {
            mPrefsEditor.PutString(key, value);
            mPrefsEditor.Commit();
        }

        public string getAccessKey(string key)
        {
            return mSharedPrefs.GetString(key, "");
        }

        public string getContextKey()
        {
            string value = mSharedPrefs.GetString(m_workContextKey, "");
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            return null;
        }
        public void DeleteAccessKey(string key)
        {
            mPrefsEditor.Remove(key);
            mPrefsEditor.Commit();
        }
    }
}