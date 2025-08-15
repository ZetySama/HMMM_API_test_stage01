using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro için

public class SceneBackgroundLoader : MonoBehaviour
{
    [Header("UI Elements")]
    public RawImage backgroundImage; // Arka plan UI elementi
    public RawImage habaChestImage;  // Haba and Chest görseli
    public TMP_Text createButtonText; // "Create Btn" stringi gösterilecek TextMeshPro

    private string apiUrl = "https://api-dev.hmmmania.com/api/Scene/code/10.0.0";
    private string baseCdnUrl = "https://parent-test.hmmmania.com";

    void Start()
    {
        StartCoroutine(FetchSceneAndLoadBackground());
    }

    IEnumerator FetchSceneAndLoadBackground()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Scene verisi alınamadı: " + www.error);
                yield break;
            }

            string json = www.downloadHandler.text;
            SceneResponse response = JsonUtility.FromJson<SceneResponse>(json);

            if (response != null && response.success && response.data != null)
            {

                SceneImage bgImage = response.data.images.Find(img => img.name == "Hause Of Haba Bcg");
                if (bgImage != null)
                {
                    string fullUrl = baseCdnUrl + bgImage.link;
                    StartCoroutine(LoadImage(backgroundImage, fullUrl));
                }
                else
                {
                    Debug.LogError("Arka plan görseli bulunamadı!");
                }


                SceneImage habaChest = response.data.images.Find(img => img.name == "Haba and Chest");
                if (habaChest != null && habaChestImage != null)
                {
                    string fullUrl = baseCdnUrl + habaChest.link;
                    StartCoroutine(LoadImage(habaChestImage, fullUrl));
                }
                else
                {
                    Debug.LogWarning("Haba and Chest görseli bulunamadı!");
                }


                SceneStringItem createBtnItem = response.data.stringItems.Find(item => item.name == "Create Btn");
                if (createBtnItem != null && createButtonText != null)
                {
                    createButtonText.text = createBtnItem.content; // Ekrana yaz
                }
                else
                {
                    Debug.LogWarning("Create Btn stringi bulunamadı!");
                }
            }
        }
    }

    IEnumerator LoadImage(RawImage target, string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                target.texture = DownloadHandlerTexture.GetContent(www);
            }
            else
            {
                Debug.LogError("Görsel yüklenemedi: " + url);
            }
        }
    }
}

[System.Serializable]
public class SceneResponse
{
    public bool success;
    public string message;
    public SceneData data;
}

[System.Serializable]
public class SceneData
{
    public string id;
    public string code;
    public string name;
    public string title;
    public List<SceneImage> images;
    public List<SceneStringItem> stringItems; // Yeni eklenen alan
}

[System.Serializable]
public class SceneImage
{
    public string name;
    public string link;
}

[System.Serializable]
public class SceneStringItem
{
    public string name;
    public string content;
}
