using System.Collections.Generic;
using System.Text;

namespace ManageMyNotificationsBusinessLayer
{
    public class ErrorMessageFormatter
    {
        public static string FormatElmahErrorMessage(string message, Dictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder("Error Message : " + message);
            sb.Append(System.Environment.NewLine);
            sb.Append("Parameters: ");
            foreach (var value in parameters)
            {
                sb.Append(value.Key + " : ");
                sb.Append(value.Value + ", ");
            }
            return sb.ToString().TrimEnd(',');
        }
    }
}