using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class TCP : MonoBehaviour
{
    private TcpListener Listener;
    private Thread listenerThread;
    private bool isRunning = true;
    public RawImage rawImage;  
    private Texture2D texture;
    void Start()
    {
        // TCPリスナーを開始
        Listener = new TcpListener(IPAddress.Any, 50007);
        Listener.Start();
        Debug.Log("TCP Listener started");

        // リスナーを別スレッドで動作
        listenerThread = new Thread(ListenForClients);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    void OnApplicationQuit()
    {
        // 終了時にリスナーとスレッドを停止
        isRunning = false;
        listenerThread?.Join(); // スレッド終了を待機
        Listener?.Stop();
        Debug.Log("TCP Listener stopped");
    }

    private void ListenForClients()
    {
        while (isRunning)
        {
            try
            {
                // クライアント接続を待機
                if (Listener.Pending()) // クライアントが接続している場合のみ受け入れ
                {
                    using (TcpClient client = Listener.AcceptTcpClient())
                    {
                        Debug.Log("Client connected");
                        HandleClient(client);
                    }
                }
                else
                {
                    // クライアントが接続されるのを待つ
                    Thread.Sleep(10); // CPU負荷を軽減
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e.Message}");
                break;
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            using (NetworkStream stream = client.GetStream())
            {
                while (isRunning)
                {
                    // ヘッダー（画像データのサイズ）を受信
                    byte[] header = new byte[4];
                    int headerBytesRead = stream.Read(header, 0, header.Length);

                    if (headerBytesRead == 0)
                    {
                        Debug.Log("Client disconnected");
                        break; // クライアントが切断された場合、ループを抜ける
                    }

                    int dataSize = BitConverter.ToInt32(header, 0);
                    Debug.Log($"Data size: {dataSize}");
                    if (dataSize > 0)
                    {
                        // 画像データを受信
                        byte[] imageData = new byte[dataSize];
                        int totalRead = 0;

                        while (totalRead < dataSize)
                        {
                            int bytesRead = stream.Read(imageData, totalRead, dataSize - totalRead);
                            if (bytesRead == 0) break; // データがなくなったら終了
                            totalRead += bytesRead;
                            
                        }

                        // 画像データを処理（ここでは単純にログ出力）
                        Debug.Log($"Received image data of size {imageData.Length}");
                    }
                
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error handling client: {e.Message}");
        }
    }
    private void DisplayImage(byte[] imageData)
    {
        // 画像データをTexture2Dに変換
        texture = new Texture2D(2, 2); // 画像サイズは後で更新される
        texture.LoadImage(imageData);  // 画像データをロード

        // RawImageにテクスチャを設定
        rawImage.texture = texture;
        rawImage.SetNativeSize();  // 画像のサイズを設定
    }
}
