using System;
using System.Collections.Generic;
using UnityEngine;

namespace AE.Riddle
{
    [Serializable]
    public class RiddleBlackboard
    {
        public event Action<string, string> DataChanged;

        private readonly Dictionary<string, string> _data = new();

        public void SetValue(string key, string value)
        {
            if (key == null)
            {
                Debug.LogWarning("Key cannot be null");
                return;
            }

            _data[key] = value;

            DataChanged?.Invoke(key, value);
        }

        public bool TrySetResult(string result)
        {
            if (string.IsNullOrEmpty(result))
                return true;

            var parts = result.Split('=');

            if (parts.Length != 2)
                return false;

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            SetValue(key, value);
            return true;
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

            return CheckCondition(key, expectedValue);
        }

        private bool CheckCondition(string key, string expectedValue)
        {
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