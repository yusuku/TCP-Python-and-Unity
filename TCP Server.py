import socket
import os

HOST = "127.0.0.1"
MAINPORT = 50007

def ConnectUnity():
    # TCP用のソケットを作成
    client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    # テストのため自身のpidを表示
    result = str(os.getpid())
    print(result)
   
    # クライアントに接続
    client.connect((HOST, MAINPORT))
   
    # 接続できればpidをUTF-8でエンコードした文字列を送る(バイト列にする)
    client.send(result.encode('utf-8'))

    # 受け取るデータの大きさは200でデータ受け取り待ち
    data = client.recv(200)
   
    # 受け取ったデータを表示
    print(data.decode('utf-8'))
   
    # 以降はclientを通してsendメソッドでデータを送れる
    return client

ConnectUnity()