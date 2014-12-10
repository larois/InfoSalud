namespace Sen.HTMLParser
{
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Security.Cryptography;
    using System.Text;
    using Facebook.Client;

    public class SessionStorage
    {
        private const string AccessTokenSettingsKeyName = "fb_access_token";
        private const string AccessTokenExpirySettingsKeyName = "fb_access_token_expiry";
        private const string StateSettingsKeyName = "fb_login_state";

        public static FacebookSession Load()
        {
            // read access token
            string accessTokenValue = LoadEncryptedSettingValue(AccessTokenSettingsKeyName);

            // read expiry
            DateTime expiryValue = DateTime.MinValue;
            string expiryTicks = LoadEncryptedSettingValue(AccessTokenExpirySettingsKeyName);
            if (!string.IsNullOrWhiteSpace(expiryTicks))
            {
                long expiryTicksValue = 0;
                if (long.TryParse(expiryTicks, out expiryTicksValue))
                {
                    expiryValue = new DateTime(expiryTicksValue);
                }
            }

            // read state
            string stateValue = LoadEncryptedSettingValue(StateSettingsKeyName);

            // return true only if both values retrieved and token not stale
            if (!string.IsNullOrWhiteSpace(accessTokenValue) && expiryValue > DateTime.UtcNow)
            {
                return new FacebookSession()
                {
                    AccessToken = accessTokenValue,
                    Expires = expiryValue,
                    State = stateValue
                };
            }
            else
            {
                return null;
            }
        }

        public static void Save(FacebookSession session)
        {
            SaveEncryptedSettingValue(AccessTokenSettingsKeyName, session.AccessToken);
            SaveEncryptedSettingValue(AccessTokenExpirySettingsKeyName, session.Expires.Ticks.ToString());
            SaveEncryptedSettingValue(StateSettingsKeyName, session.State);
        }

        public static void Remove()
        {
            RemoveEncryptedSettingValue(AccessTokenSettingsKeyName);
            RemoveEncryptedSettingValue(AccessTokenExpirySettingsKeyName);
            RemoveEncryptedSettingValue(StateSettingsKeyName);
        }

        private static void RemoveEncryptedSettingValue(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        private static string LoadEncryptedSettingValue(string key)
        {
            string value = null;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                var protectedBytes = IsolatedStorageSettings.ApplicationSettings[key] as byte[];
                if (protectedBytes != null)
                {
                    byte[] valueBytes = ProtectedData.Unprotect(protectedBytes, null);
                    value = Encoding.UTF8.GetString(valueBytes, 0, valueBytes.Length);
                }
            }

            return value;
        }

        private static void SaveEncryptedSettingValue(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                byte[] valueBytes = Encoding.UTF8.GetBytes(value);

                // Encrypt the value by using the Protect() method.
                byte[] protectedBytes = ProtectedData.Protect(valueBytes, null);
                if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                {
                    IsolatedStorageSettings.ApplicationSettings[key] = protectedBytes;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(key, protectedBytes);
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }




    }
}
