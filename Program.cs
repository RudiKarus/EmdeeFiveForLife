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
        string address = "http://94.237.56.248:46467";
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


        #region GENERATE_MD5HASH
        //Register the CodePages encoding provider at application startup to enable using single and double byte encodings.
        //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //Generate the md5 hash:
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(stringToEncode);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        var md5Hash = Convert.ToHexString(hashBytes).ToLower();
        // var md5Hash = Encoding.UTF8.GetString(hashBytes);

        //Generate a file and prompt the hash on your terminal:
        Console.WriteLine($"MD5 hash: {md5Hash} \n");
        File.WriteAllText(Environment.CurrentDirectory + "/md5hash.txt", md5Hash);
        #endregion


        #region POST_md5_hash_back_to_target
        //Send data back to the input field (name="hash") of the target:
        //WebClient webClient = new WebClient();
        var data = new System.Collections.Specialized.NameValueCollection();
        data["hash"] = md5Hash;
        var response = client.UploadValues(address, "POST", data);

        //Prompt the response in your local terminal:
        string responseString = System.Text.Encoding.ASCII.GetString(response);
        Console.WriteLine(responseString);
        Console.WriteLine();
        #endregion
    }
}