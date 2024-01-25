namespace EmdeeFiveForLife;
using System.IO;
using System.Text;
using System.Net;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        #region GET_target_html_and_replace_content
        //Get the html string from the target:
        WebClient client = new WebClient();
        string address = "http://94.237.53.58:36883";
        string htmlContent = client.DownloadString(address);
        Console.WriteLine(htmlContent);

        //Take the string value, from the targets html, to generate a md5 hash with:
        string toReplace = "<h3 align='center'>"; //IndexOfThisElement = 148;
        char[] stringToGenerateMd5Hash = new char[20]; //20=lengthOfTheGivenString
        char[] targetHtml = htmlContent.ToCharArray();
        int h = 0;
        for (int i = (148 + toReplace.Length); i < (148 + toReplace.Length + 20); i++) //use LastIndexOf ??
        {
            stringToGenerateMd5Hash[h] += targetHtml[i];
            h++;
        }
        string stringToEncode = new string(stringToGenerateMd5Hash);
        Console.WriteLine("String to encode: " + stringToEncode);
        #endregion

        //Register the CodePages encoding provider at application startup to enable using single and double byte encodings.
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //Generate the md5 hash:
        MD5 md5 = MD5.Create();

        // byte[] inputBytes = Encoding.ASCII.GetBytes(stringToEncode);
        // byte[] hashBytes = md5.ComputeHash(inputBytes);
        // //var md5Hash = Convert.ToHexString(hashBytes).ToLower();
        // var md5Hash = Encoding.ASCII.GetString(hashBytes);

        // var byteArray = new byte[stringToEncode.Length];
        // for (int i = 0; i < stringToEncode.Length; i++)
        // {
        //     byteArray[i] = (byte)stringToEncode[i];
        // }
        // byte[] hashBytes = md5.ComputeHash(byteArray);
        // var asciiEncoding = Encoding.GetEncoding(437);
        // string md5Hash = asciiEncoding.GetString(hashBytes);

        //var asciiEncoding = Encoding.GetEncoding(437);
        var asciiEncoding = Encoding.GetEncoding("iso-8859-1");
        byte[] inputBytes = asciiEncoding.GetBytes(stringToEncode);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        var md5Hash = asciiEncoding.GetString(hashBytes);

        //Generate a file and prompt the hash on your terminal:
        Console.WriteLine($"MD5 hash: {md5Hash} \n");
        File.WriteAllText(Environment.CurrentDirectory + "/md5hash.txt", md5Hash);

        #region POST_md5_hash_back_to_target
        //Send data back to the input field (name="hash") of the target:
        WebClient webClient = new WebClient();
        var data = new System.Collections.Specialized.NameValueCollection();
        data["hash"] = md5Hash;
        var response = webClient.UploadValues(address, "POST", data);

        //Prompt the response in your local terminal:
        string responseString = System.Text.Encoding.ASCII.GetString(response);
        Console.WriteLine(responseString);
        Console.WriteLine();
        #endregion
    }
}