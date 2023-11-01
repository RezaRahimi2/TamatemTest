using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace GamePlayLogic
{
    // This class provides a service for generating random numbers.
    public class RandomNumberService
    {
        // The URL of the API that generates random numbers.
        private const string API_URL =
            "https://www.random.org/integers/?num=1&min=1&max=6&col=1&base=10&format=plain&rnd=new";

        // This method returns a random number.
        public async UniTask<byte> GetRandomNumber()
        {
            // If in debugging mode and UsePlayerIndexForPosition is true, generate a random number locally.
#if Debugging
            if (GameModel.Instance.UsePlayerIndexForPosition)
                return (byte)UnityEngine.Random.Range(1, 7);
#endif
            // If there is internet connectivity, make a request to the API.
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                // Log the start of the API request.
                Debug.unityLogger.Log($"<color=white>API:Starting request to {API_URL}</color>");
                using (UnityWebRequest www = UnityWebRequest.Get(API_URL))
                {
                    // Send the request and wait for the response.
                    await www.SendWebRequest();

                    // If the request fails, log the error and return 0.
                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.unityLogger.LogError("<color=red>API:Error:{0}</color>",www.error);
                        return 0;
                    }
                    else
                    {
                        // If the request is successful, log the response and return the parsed number.
                        Debug.unityLogger.Log($"<color=green>API:Response:{www.downloadHandler.text}</color>");
                        return byte.Parse(www.downloadHandler.text);
                    }
                }
            }
            else
            {
                // If there is no internet connectivity, generate a random number locally.
                return (byte)UnityEngine.Random.Range(1, 7);
            }
        }
    }

}