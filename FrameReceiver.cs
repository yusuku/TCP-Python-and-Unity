using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

public class FrameReceiver : MonoBehaviour
{
    private const string apiUrl = "http://127.0.0.1:8000/array"; // FastAPI�T�[�o�[��URL
    private float requestInterval = 0.05f; // API���N�G�X�g�Ԋu�i�b�j
    private float lastRequestTime = 0f; // �Ō�Ƀ��N�G�X�g�𑗂�������

    void Update()
    {
        // ���݂̎��Ԃ����N�G�X�g�Ԋu�ȏ�o�߂��Ă�����
        if (Time.time - lastRequestTime >= requestInterval)
        {
            lastRequestTime = Time.time; // �Ō�̃��N�G�X�g���Ԃ��X�V
            StartCoroutine(GetDataFromAPI()); // �f�[�^�擾�̃R���[�`�����J�n
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
                // JSON���X�|���X���擾
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log(jsonResponse);

                // JSON���������͂��āAJsonData�I�u�W�F�N�g�ɕϊ�
                JsonData jsonData = JsonMapper.ToObject(jsonResponse);

                // "array"�L�[�̃f�[�^���擾�i2�����z��j
                JsonData arrayData = jsonData["array"];

                // arrayData�̃T�C�Y���擾�i�s���Ɨ񐔁j
                int rowCount = arrayData.Count;
                int colCount = arrayData[0].Count;

                // int[,]�^��2�����z����쐬
                int[,] intArray = new int[rowCount, colCount];

                // "array"�̊e�l��intArray�ɑ��
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        intArray[i, j] = (int)arrayData[i][j];
                    }
                }
               
                // "size"�L�[�̃f�[�^���擾�i1�����z��j
                JsonData sizeData = jsonData["size"];

                // sizeData�̒l���擾
                int width = (int)sizeData[0];
                int height = (int)sizeData[1];

                Debug.Log("Width: " + width + ", Height: " + height);
            }
        }
    }
}
