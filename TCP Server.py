import socket
import os
import cv2
import struct

HOST = "127.0.0.1"
MAINPORT = 50007

# ソケットを作成
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

try:
    # サーバーに接続
    client.connect((HOST, MAINPORT))
    print("Connected to the server")

    # カメラを初期化
    cap = cv2.VideoCapture(0)
    if not cap.isOpened():
        raise IOError("Cannot open camera")

    while True:
        ret, frame = cap.read()
        if not ret:
            print("Failed to capture frame")
            break

        # フレームを PNG にエンコード
        _, buffer = cv2.imencode('.png', frame)

        # データサイズを取得して送信
        data_size = len(buffer)
        client.sendall(struct.pack('!I', data_size))  # サイズを送信
        client.sendall(buffer)  # フレームデータを送信

        print(f"Sent frame of size {data_size} bytes")

except KeyboardInterrupt:
    print("Transmission stopped by user")

except Exception as e:
    print(f"An error occurred: {e}")

finally:
    # リソースを解放
    if 'cap' in locals() and cap.isOpened():
        cap.release()
    client.close()
    print("Connection closed")
