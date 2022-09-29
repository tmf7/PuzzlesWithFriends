using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerConnection : MonoBehaviour
{

    private const string _loadUserPuzzleURL = "https://dev.puzzleswithfriends.com/loaduserpuzzle.php";
    private const string _testTextureURI = "https://dev.puzzleswithfriends.com/puzzle.png";

    // TODO: the C# WebSocket protocols need to be used to establish a full-duplex connection with this URL
    // not just UnitWebRequest
    private const string _webSocketURL = "wss://dev.puzzleswithfriends.com/wss2/"; 

    public RawImage _rawImage;

    private void Start()
    {
        StartCoroutine(GetUserPuzzle());
    }

    //// remember to use StartCoroutine when calling this function!
    //IEnumerator PostScores(string name, int score)
    //{
    //    //This connects to a server side php script that will add the name and score to a MySQL DB.
    //    // Supply it with a string representing the players name and the players score.
    //    string hash = Md5Sum(name + score + secretKey);

    //    string post_url = addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash;

    //    // Post the URL to the site and create a download object to get the result.
    //    WWW hs_post = new WWW(post_url);
    //    yield return hs_post; // Wait until the download is done

    //    if (hs_post.error != null)
    //    {
    //        print("There was an error posting the high score: " + hs_post.error);
    //    }
    //}

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    IEnumerator GetUserPuzzle()
    {
        // PHP
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_loadUserPuzzleURL))
        {
               // Request and wait for the desired page.
               yield return webRequest.SendWebRequest();

            string[] pages = _loadUserPuzzleURL.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }


        // TEXTURE
        using (var uwr = UnityWebRequestTexture.GetTexture(_testTextureURI))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                _rawImage.texture = DownloadHandlerTexture.GetContent(uwr);
            }
        }
    }



    //public string Md5Sum(string strToEncrypt)
    //{
    //    System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
    //    byte[] bytes = ue.GetBytes(strToEncrypt);

    //    // encrypt bytes
    //    System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    //    byte[] hashBytes = md5.ComputeHash(bytes);

    //    // Convert the encrypted bytes back to a string (base 16)
    //    string hashString = "";

    //    for (int i = 0; i < hashBytes.Length; i++)
    //    {
    //        hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
    //    }

    //    return hashString.PadLeft(32, '0');
    //}
}