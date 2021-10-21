using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsMVCTests
{
    [ExcludeFromCodeCoverage]
    public static class Utilities
    {
        public static bool AreObjectsEquivalent<T>(T actual, T expected)
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                bool isEnumerable = typeof(ICollection).IsAssignableFrom(property.PropertyType)
                    || (typeof(IEnumerable).IsAssignableFrom(property.PropertyType)
                    && !(typeof(string).IsAssignableFrom(property.PropertyType)));

                if (!isEnumerable)
                {
                    var actualValue = property.GetValue(actual);
                    var expectedValue = property.GetValue(expected);

                    if (OnlyOneValueIsNull(actualValue, expectedValue))
                        return false;

                    if (BothValuesAreNull(actualValue, expectedValue))
                        continue;

                    if (!actualValue.Equals(expectedValue))
                        return false;
                }
            }

            return true;
        }

        private static bool OnlyOneValueIsNull(object actualValue, object expectedValue)
        {
            if (null == actualValue && null != expectedValue)
                return true;

            if (null != actualValue && null == expectedValue)
                return true;

            return false;
        }

        private static bool BothValuesAreNull(object actualValue, object expectedValue)
        {
            return null == actualValue && null == expectedValue;
        }
    }
}
