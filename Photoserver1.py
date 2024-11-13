# -*- coding: utf-8 -*-
"""
server1.pyプログラム
Pythonによるサーバソケットの利用法を示す例題プログラム(1)
50000番ポートで接続を待ち受けて、画像データを送信します
使いかた　c:\>python server1.py
"""

# モジュールのインポート
import socket
import cv2
import numpy as np

# 画像ファイルを読み込む（カラー画像として読み込む）
image_path = 'a1.png'  # 画像ファイルのパス
image_array = cv2.imread(image_path, cv2.IMREAD_COLOR)

# 画像をエンコードしてバイナリデータに変換
_, image_encoded = cv2.imencode('.png', image_array)
image_bytes = image_encoded.tobytes()

# グローバル変数
PORT = 50000  # ポート番号

# メイン実行部
# ソケットの作成
server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# アドレスの設定
server.bind(("", PORT))
# 接続の待ち受け
server.listen()

# クライアントへの対応処理
while True:
    client, addr = server.accept()  # 通信用ソケットの取得 
    print("接続要求あり", addr)
    
    # 画像のサイズ情報をクライアントに送信
    client.sendall(len(image_bytes).to_bytes(4, byteorder='big'))
    # 画像データを送信
    client.sendall(image_bytes)
    print("画像データを送信しました")
    
    client.close()  # コネクションのクローズ
