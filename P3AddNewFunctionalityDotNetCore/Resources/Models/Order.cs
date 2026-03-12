using System;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace P3.Resources.Models
{
    public static class Order
    {
        private static ResourceManager resourceManager = new ResourceManager("P3.Resources.Models.Order", Assembly.GetExecutingAssembly());
        private static CultureInfo resourceCulture;

        public static string ErrorMissingName
        {
            get
            {
                return resourceManager.GetString("ErrorMissingName", resourceCulture);
            }
        }
        public static string ErrorMissingAddress
        {
            get
            {
                return resourceManager.GetString("ErrorMissingAddress", resourceCulture);
            }
        }
        public static string ErrorMissingCity
        {
            get
            {
                return resourceManager.GetString("ErrorMissingCity", resourceCulture);
            }
        }
        public static string ErrorMissingZipCode
        {
            get
            {
                return resourceManager.GetString("ErrorMissingZipCode", resourceCulture);
            }
        }
        public static string ErrorMissingCountry
        {
            get
            {
                return resourceManager.GetString("ErrorMissingCountry", resourceCulture);
            }
        }
    }
}

