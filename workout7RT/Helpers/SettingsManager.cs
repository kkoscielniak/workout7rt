using System;
using System.Diagnostics;
using Windows.Storage;

namespace workout7RT.Helpers
{
    /// <summary>
    /// Class responsible for managing application settings.
    /// Cannot be static, because static classes couldn't possess constructors
    /// </summary>
    class SettingsManager
    {
        ApplicationDataContainer settingsManager;

        // key names 
        const string currentStreakKeyName = "currentStreak";
        const string recentDayOfWorkoutKeyName = "recentDayOfWorkout";
        
        // default values
        const int currentStreakDefaultValue = 0;
        DateTimeOffset recentDayOfWorkoutDefaultValue = DateTime.Parse("2008-05-01T07:34:42-5:00");   // for sure?

        /// <summary>
        /// In the constructor class tries to fetch settings from application storage.
        /// </summary>
        public SettingsManager()
        {
            try
            {
                // get current settings of the application 
                settingsManager = ApplicationData.Current.LocalSettings;
            }
            catch (Exception e)
            {
                // if you can get current settings show toast
#if DEBUG
                Debug.WriteLine("Exception while loading settings: " + e.ToString());
#endif
            }
        }

        /// <summary>
        /// Update application setting of given key. If setting does not exist - create one
        /// </summary>
        /// <param name="keyName">setting name (key)</param>
        /// <param name="value">setting (value) of facultative type to save</param>
        /// <returns>originally it shourd return true if the operation succeeds</returns>
        private void AddOrUpdateValue(string keyName, Object value)
        {
            // bool valueChanged = false; 

            if (settingsManager != null)
            {
                if (settingsManager.Values.ContainsKey(keyName))
                {
                    if (settingsManager.Values[keyName] != value)
                    {
                        settingsManager.Values[keyName] = value;
                        // valueChanged = true;
                    }
                }
                else
                {
                    settingsManager.Values.Add(keyName, value);
                    // valueChanged = true;
                }
            }

            // return valueChanged;
        }

        /// <summary>
        /// Get the setting value from app settings. If the value does not exist - get default one
        /// </summary>
        /// <param name="keyName">setting name (key) to get</param>
        /// <param name="defaultValue">default value of the setting</param>
        private ValueType GetValueOrDefault<ValueType>(string keyName, ValueType defaultValue)
        {
            ValueType value;

            // If key exists - retrieve the value
            if (settingsManager.Values.ContainsKey(keyName))
            {
                value = (ValueType)settingsManager.Values[keyName];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings (obsolete in Windows Store app; in WP 7/8 - arbitrary)
        /// </summary>
        private void SaveSettings() {}

        /// <summary>
        /// Gets or sets current streak count
        /// </summary>
        public int CurrentStreakSetting
        {
            get
            {
                return GetValueOrDefault<int>(currentStreakKeyName, currentStreakDefaultValue);
            }
            set
            {
                AddOrUpdateValue(currentStreakKeyName, value);
                // SaveSettings();
            }
        }

        public DateTimeOffset RecentDayOfWorkout
        {
            get
            {
                return GetValueOrDefault<DateTimeOffset>(recentDayOfWorkoutKeyName, recentDayOfWorkoutDefaultValue);
            }
            set
            {
                AddOrUpdateValue(recentDayOfWorkoutKeyName, value);
                // SaveSettings();
            }
        }

        public bool FirstRun
        {
            get
            {
                return GetValueOrDefault<bool>("firstRun", true);
            }
            set
            {
                AddOrUpdateValue("firstRun", value);
            }
        }
    }
}
