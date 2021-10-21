using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManageMyNotificationsBusinessLayer
{
    public static class Helper
    {
        static object lockMerge = new object();
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Empty collection");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
            }
        }

        public static void MergeRangeToAccount(Dictionary<string, XMUserDetails> source, Dictionary<string, XMUserDetails> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Empty collection");
            }
            lock (lockMerge)
            {
                foreach (var item in collection)
                {
                    if (!source.ContainsKey(item.Key))
                    {
                        source.Add(item.Key, item.Value);
                    }
                    else
                    {
                        if (item.Value.accounts.Length > 0)
                        {
                            var accounts = source[item.Key].accounts.ToList();
                            accounts.Add(item.Value.accounts[0]);
                            source[item.Key].accounts = accounts.ToArray();
                        }
                    }
                }
            }
        }
    }
}
