namespace EmdeeFiveForLife;
using System.IO;
using System.Text;
using System.Net;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        //SetProperties
        WebClient webClient = new WebClient();
        webClient.Headers.Add(HttpRequestHeader.Cookie, "PHPSESSID=vl38gua0b7idt8g2lgdt0oilp7");//from BurpSuite ;)
        string address = "http://94.237.62.195:30254";
        string stringToReplace = "<h3 align='center'>";
        int lengthOfStringToEncode = 20;

        //GetDataFromTarget
        string htmlContent = webClient.DownloadString(address);
        string stringToEncode = GetStringToEncode(htmlContent, stringToReplace, lengthOfStringToEncode);
        string md5Hash = GetMd5Hash(stringToEncode);

        //PostMd5HashBackToTarget
        var data = new System.Collections.Specialized.NameValueCollection();
        data["hash"] = md5Hash;
        var response = webClient.UploadValues(address, "POST", data);

        //ConsoleLogging
        Console.WriteLine(htmlContent);
        Console.WriteLine("String to encode: " + stringToEncode);
        Console.WriteLine($"Generated MD5 hash: {md5Hash} \n");
        File.WriteAllText(Environment.CurrentDirectory + "/md5hash.txt", md5Hash);
        string responseString = System.Text.Encoding.ASCII.GetString(response);
        Console.WriteLine(responseString);
    }

    #region CrappyServices
    private static string GetStringToEncode(string htmlString, string patternToReplace, int lengthOfStringToEncode)
    {
        int startIndexOfPattern = htmlString.IndexOf(patternToReplace);
        int patternSpan = patternToReplace.Length;
        char[] stringToGenerateMd5Hash = new char[lengthOfStringToEncode];
        char[] targetHtmlString = htmlString.ToCharArray();

        int h = 0;
        for (int i = (startIndexOfPattern + patternSpan); i < (startIndexOfPattern + patternSpan + lengthOfStringToEncode); i++)
        {
            stringToGenerateMd5Hash[h] += targetHtmlString[i];
            h++;
        }
        return new string(stringToGenerateMd5Hash);
    }

    private static string GetMd5Hash(string stringToEncode)
    {
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(stringToEncode);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        string md5Hash = Convert.ToHexString(hashBytes).ToLower();

        return md5Hash;
    }
    #endregion

}