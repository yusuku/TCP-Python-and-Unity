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
        // TCP���X�i�[���J�n
        Listener = new TcpListener(IPAddress.Any, 50007);
        Listener.Start();
        Debug.Log("TCP Listener started");

        // ���X�i�[��ʃX���b�h�œ���
        listenerThread = new Thread(ListenForClients);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    void OnApplicationQuit()
    {
        // �I�����Ƀ��X�i�[�ƃX���b�h���~
        isRunning = false;
        listenerThread?.Join(); // �X���b�h�I����ҋ@
        Listener?.Stop();
        Debug.Log("TCP Listener stopped");
    }

    private void ListenForClients()
    {
        while (isRunning)
        {
            try
            {
                // �N���C�A���g�ڑ���ҋ@
                if (Listener.Pending()) // �N���C�A���g���ڑ����Ă���ꍇ�̂ݎ󂯓���
                {
                    using (TcpClient client = Listener.AcceptTcpClient())
                    {
                        Debug.Log("Client connected");
                        HandleClient(client);
                    }
                }
                else
                {
                    // �N���C�A���g���ڑ������̂�҂�
                    Thread.Sleep(10); // CPU���ׂ��y��
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
                    // �w�b�_�[�i�摜�f�[�^�̃T�C�Y�j����M
                    byte[] header = new byte[4];
                    int headerBytesRead = stream.Read(header, 0, header.Length);

                    if (headerBytesRead == 0)
                    {
                        Debug.Log("Client disconnected");
                        break; // �N���C�A���g���ؒf���ꂽ�ꍇ�A���[�v�𔲂���
                    }

                    int dataSize = BitConverter.ToInt32(header, 0);
                    Debug.Log($"Data size: {dataSize}");
                    if (dataSize > 0)
                    {
                        // �摜�f�[�^����M
                        byte[] imageData = new byte[dataSize];
                        int totalRead = 0;

                        while (totalRead < dataSize)
                        {
                            int bytesRead = stream.Read(imageData, totalRead, dataSize - totalRead);
                            if (bytesRead == 0) break; // �f�[�^���Ȃ��Ȃ�����I��
                            totalRead += bytesRead;
                            
                        }

                        // �摜�f�[�^�������i�����ł͒P���Ƀ��O�o�́j
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
        // �摜�f�[�^��Texture2D�ɕϊ�
        texture = new Texture2D(2, 2); // �摜�T�C�Y�͌�ōX�V�����
        texture.LoadImage(imageData);  // �摜�f�[�^�����[�h

        // RawImage�Ƀe�N�X�`����ݒ�
        rawImage.texture = texture;
        rawImage.SetNativeSize();  // �摜�̃T�C�Y��ݒ�
    }
}
