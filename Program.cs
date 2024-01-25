namespace EmdeeFiveForLife;
using System.IO;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

class Program
{
    static void Main(string[] args)
    {
        string address = "http://83.136.251.235:45095";
        string stringToReplace = "<h3 align='center'>";
        int lengthOfStringToEncode = 20;
        //Register the CodePages encoding provider at application startup to enable using single and double byte encodings.
        //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        WebClient client = new WebClient();
        string htmlContent = client.DownloadString(address);
        string stringToEncode = GetStringToEncode(htmlContent, stringToReplace, lengthOfStringToEncode);
        string md5Hash = GetMd5Hash(stringToEncode);

        #region POST_md5_hash_back_to_target
        var data = new System.Collections.Specialized.NameValueCollection();
        data["hash"] = md5Hash; //Send data back to the input field (name="hash") of the target
        var response = client.UploadValues(address, "POST", data);
        #endregion


        #region LOG
        Console.WriteLine(htmlContent);
        Console.WriteLine("String to encode: " + stringToEncode);
        Console.WriteLine($"MD5 hash: {md5Hash} \n");
        File.WriteAllText(Environment.CurrentDirectory + "/md5hash.txt", md5Hash);
        //Prompt the response in your local terminal:
        string responseString = System.Text.Encoding.ASCII.GetString(response);
        Console.WriteLine(responseString);
        Console.WriteLine();
        #endregion
    }


    #region METHODS
    public static string GetStringToEncode(string htmlString, string patternToReplace, int lengthOfStringToEncode)
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

    public static string GetMd5Hash(string stringToEncode)
    {
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(stringToEncode);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        string md5Hash = Convert.ToHexString(hashBytes).ToLower();
        // var md5Hash = Encoding.UTF8.GetString(hashBytes);

        return md5Hash;
    }
    #endregion

}