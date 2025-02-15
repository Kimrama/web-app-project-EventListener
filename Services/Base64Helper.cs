using System.Text;

namespace EventListener.Services;

public class Base64Helper
{
    public static string EncodeBase64(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")  
            .Replace("/", "_") 
            .TrimEnd('=');
    }

    public static string DecodeBase64(string input)
    {
        string base64 = input.Replace("-", "+").Replace("_", "/"); 
        while (base64.Length % 4 != 0)
        {
            base64 += "=";
        }
        
        var bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }  
}