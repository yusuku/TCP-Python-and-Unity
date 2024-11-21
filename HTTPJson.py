import cv2
from fastapi import FastAPI

app = FastAPI()

@app.get("/array")
def get_2d_array():
    # カメラキャプチャの初期化
    cap = cv2.VideoCapture(0)  # デフォルトカメラを使用

    try:
        if not cap.isOpened():
            return {"error": "カメラが開けません"}

        # カメラからフレームを取得
        ret, frame = cap.read()
        if not ret:
            return {"error": "フレームを取得できません"}

        # 画像をグレースケールに変換
        gray_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        # 2次元配列に変換（リストに変換）
        array = gray_frame.tolist()

        # グレースケール画像のサイズを取得
        size = gray_frame.shape

        return {"array": array, "size": size}
    finally:
        # カメラキャプチャを終了
        cap.release()
