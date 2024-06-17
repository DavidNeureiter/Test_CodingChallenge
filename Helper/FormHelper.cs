namespace websLINE.Helper
{
    public static class FormHelper 
    {
        public static string GetString(this IFormCollection form, string key) 
        {
            string? val = form[key];

            return val ?? string.Empty;
        }

        public static bool GetBool(this IFormCollection form, string key)
        {
            string val = GetString(form, key);

            if (bool.TryParse(val, out bool retVal))
            {
                return retVal;
            }

            if (int.TryParse(val, out int tempVal) && tempVal > 0)
            {
                return true;
            }

            return false;
        }
    }
}
