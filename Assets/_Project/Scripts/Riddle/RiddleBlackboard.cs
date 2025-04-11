using System;
using System.Collections.Generic;

namespace AE.Riddle
{
    [Serializable]
    public class RiddleBlackboard
    {
        public event Action<string, string> NewValueSet;

        private readonly Dictionary<string, string> _data = new();

        public void SetValue(string key, string value)
        {
            _data[key] = value;

            NewValueSet?.Invoke(key, value);
        }

        public string GetValue(string key)
        {
            return _data.GetValueOrDefault(key);
        }

        public bool HasKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool CheckCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return true;

            var parts = condition.Split('=');

            if (parts.Length != 2)
                return false;

            var key = parts[0].Trim();
            var expectedValue = parts[1].Trim();

            return HasKey(key) && GetValue(key) == expectedValue;
        }

        public void Reset()
        {
            _data.Clear();
        }

        public void Remove(string key)
        {
            _data.Remove(key);
        }
    }
}