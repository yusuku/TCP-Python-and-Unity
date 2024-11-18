using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class TCP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ConnectPython();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ConnectPython()
    {
        // �G���h�|�C���g��ݒ肷��
        IPEndPoint RemoteEP = new IPEndPoint(IPAddress.Any, 50007);

        // TcpListener���쐬����
        TcpListener Listener = new TcpListener(RemoteEP);

        // TCP�ڑ���҂��󂯂�
        Listener.Start();
        TcpClient Client = Listener.AcceptTcpClient();

        // �ڑ����ł���΁A�f�[�^������肷��X�g���[����ۑ�����
        NetworkStream Stream = Client.GetStream();

        GetPID(Stream);

        // �ڑ���؂�
        Client.Close();
    }


    void GetPID(NetworkStream Stream)
    {
        Byte[] data = new Byte[256];
        String responseData = String.Empty;

        // �ڑ��悩��f�[�^��ǂݍ���
        Int32 bytes = Stream.Read(data, 0, data.Length);

        // �ǂݍ��񂾃f�[�^�𕶎���ɕϊ�����
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        UnityEngine.Debug.Log("PID: " + responseData);

        // �󂯎����������ɕ�����t�������Ė߂�
        Byte[] buffer = System.Text.Encoding.ASCII.GetBytes("responce: " + responseData);
        Stream.Write(buffer, 0, buffer.Length);
    }
}
