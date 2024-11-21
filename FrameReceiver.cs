using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

public class FrameReceiver : MonoBehaviour
{
    private const string apiUrl = "http://127.0.0.1:8000/array"; // FastAPIサーバーのURL
    private float requestInterval = 0.05f; // APIリクエスト間隔（秒）
    private float lastRequestTime = 0f; // 最後にリクエストを送った時間

    void Update()
    {
        // 現在の時間がリクエスト間隔以上経過していたら
        if (Time.time - lastRequestTime >= requestInterval)
        {
            lastRequestTime = Time.time; // 最後のリクエスト時間を更新
            StartCoroutine(GetDataFromAPI()); // データ取得のコルーチンを開始
        }
    }

    IEnumerator GetDataFromAPI()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                // JSONレスポンスを取得
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log(jsonResponse);

                // JSON文字列を解析して、JsonDataオブジェクトに変換
                JsonData jsonData = JsonMapper.ToObject(jsonResponse);

                // "array"キーのデータを取得（2次元配列）
                JsonData arrayData = jsonData["array"];

                // arrayDataのサイズを取得（行数と列数）
                int rowCount = arrayData.Count;
                int colCount = arrayData[0].Count;

                // int[,]型の2次元配列を作成
                int[,] intArray = new int[rowCount, colCount];

                // "array"の各値をintArrayに代入
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        intArray[i, j] = (int)arrayData[i][j];
                    }
                }
               
                // "size"キーのデータを取得（1次元配列）
                JsonData sizeData = jsonData["size"];

                // sizeDataの値を取得
                int width = (int)sizeData[0];
                int height = (int)sizeData[1];

                Debug.Log("Width: " + width + ", Height: " + height);
            }
        }
    }
}
