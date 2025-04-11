using UnityEngine;


namespace Honeylab.Utils.Prefs
{
    public class UnityPrefsService : IPrefsService
    {
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);


        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }


        public int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);


        public void SetInt(string key, int newValue)
        {
            PlayerPrefs.SetInt(key, newValue);
            PlayerPrefs.Save();
        }


        public string GetString(string key) => PlayerPrefs.GetString(key);
        public string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);


        public void SetString(string key, string newValue)
        {
            PlayerPrefs.SetString(key, newValue);
            PlayerPrefs.Save();
        }


        public bool GetBool(string key, bool defaultValue = false)
        {
            if (!HasKey(key))
            {
                return defaultValue;
            }

            return GetInt(key) > 0;
        }


        public void SetBool(string key, bool newValue)
        {
            int intValue = newValue ? 1 : 0;
            SetInt(key, intValue);
        }


        public long GetLong(string key, long defaultValue = 0L)
        {
            if (!HasKey(key))
            {
                return defaultValue;
            }

            return long.TryParse(GetString(key), out long savedValue) ? savedValue : defaultValue;
        }


        public void SetLong(string key, long newValue)
        {
            string valueStr = newValue.ToString();
            SetString(key, valueStr);
        }


        public float GetFloat(string key, float defaultValue = 0) => PlayerPrefs.GetFloat(key, defaultValue);


        public void SetFloat(string key, float newValue)
        {
            PlayerPrefs.SetFloat(key, newValue);
            PlayerPrefs.Save();
        }
    }
}
