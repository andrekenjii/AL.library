using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AL.library.Utility.Validation
{
    public class ValidationResult
    {
        public List<ValidationMessages> messages { get; private set; }
        public bool isValid
        {
            get
            {
                if (messages == null) return true;
                return messages.Count == 0;
            }
        }

        public ValidationResult()
        {
            messages = new List<ValidationMessages>();
        }

        public void AddMessage(string message)
        {
            messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentEquals(object object1, object object2, string message)
        {
            if (!object1.Equals(object2))
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentFalse(bool boolValue, string message)
        {
            if (boolValue)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentLength(string stringValue, int maximum, string message)
        {
            int length = stringValue.Trim().Length;
            if (length > maximum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentLength(string stringValue, int minimum, int maximum, string message)
        {
            int length = stringValue.Trim().Length;
            if (length < minimum || length > maximum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentMatches(string pattern, string stringValue, string message)
        {
            Regex regex = new Regex(pattern);

            if (!regex.IsMatch(stringValue))
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentMinimum(int value, int minimum, string message)
        {
            if (value < minimum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentMaximum(int value, int maximum, string message)
        {
            if (value > maximum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentNotEmpty(string stringValue, string message)
        {
            if (stringValue == null || stringValue.Trim().Length == 0)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentNotEquals(object object1, object object2, string message)
        {
            if (object1.Equals(object2))
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentNotNull(object object1, string message)
        {
            if (object1 == null)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentNotNullEmpty(string object1, string message)
        {
            if (string.IsNullOrEmpty(object1))
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentRange(double value, double minimum, double maximum, string message)
        {
            if (value < minimum || value > maximum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentRange(float value, float minimum, float maximum, string message)
        {
            if (value < minimum || value > maximum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentRange(int value, int minimum, int maximum, string message)
        {
            if (value < minimum || value > maximum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentRange(long value, long minimum, long maximum, string message)
        {
            if (value < minimum || value > maximum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentRange(DateTime value, DateTime minimum, DateTime maximum, string message)
        {
            if (value <= minimum || value >= maximum)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertArgumentTrue(bool boolValue, string message)
        {
            if (!boolValue)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertStateFalse(bool boolValue, string message)
        {
            if (boolValue)
                messages.Add(new ValidationMessages { message = message });
        }

        public void AssertStateTrue(bool boolValue, string message)
        {
            if (!boolValue)
                messages.Add(new ValidationMessages { message = message });
        }
    }
}
