import socket
import numpy as np
import cv2

PORT = 50000  # ポート番号

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client:
    client.connect(("127.0.0.1", PORT))

    # 画像サイズの受信
    data_size = int.from_bytes(client.recv(4), byteorder='big')
    # 画像データの受信
    image_bytes = b""
    while len(image_bytes) < data_size:
        packet = client.recv(4096)
        if not packet:
            break
        image_bytes += packet

    # 画像データをNumPy配列にデコード
    image_array = np.frombuffer(image_bytes, dtype=np.uint8)
    image = cv2.imdecode(image_array, cv2.IMREAD_COLOR)

    # 画像の表示
    cv2.imshow("Received Image", image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()
