namespace Honeylab.Utils.Prefs
{
    public interface IPrefsService
    {
        public bool HasKey(string key);
        public void DeleteKey(string key);

        public int GetInt(string key, int defaultValue = 0);
        public void SetInt(string key, int newValue);

        public string GetString(string key);
        public string GetString(string key, string defaultValue);
        public void SetString(string key, string newValue);

        public bool GetBool(string key, bool defaultValue = false);
        public void SetBool(string key, bool newValue);

        public long GetLong(string key, long defaultValue = 0L);
        public void SetLong(string key, long newValue);

        public float GetFloat(string key, float defaultValue = 0.0f);
        public void SetFloat(string key, float newValue);
    }
}
