using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class video : MonoBehaviour
{
    #region VARIABLES
    [SerializeField]
    private UnityWebRequest www;
    public GameObject loading;
    public GameObject downloadFail;
    [SerializeField]
    private bool videoIsDownloaded;
    [SerializeField]
    private string videoSavePath;

    //public float progress;
    #endregion

    #region STARTMETHOD
    void Start()
    {
        //Variavel vp controlara o componente de videoplayer
        var vp = gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        StartCoroutine(GetVideo());
        downloadFail.SetActive(false);
        if (videoIsDownloaded)
        {
            //caso esteja baixado, o vp recebe o caminho do arquivo
            vp.url = videoSavePath;
            vp.playOnAwake = false;
            vp.isLooping = true;
            vp.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
            vp.targetMaterialRenderer = GetComponent<Renderer>();
            vp.targetMaterialProperty = "_MainTex";
        }
        else
        {
            //o vp recebe o url, baixa e executa o video
            vp.url = www.url;
            vp.playOnAwake = false;
            vp.isLooping = true;
            vp.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
            vp.targetMaterialRenderer = GetComponent<Renderer>();
            vp.targetMaterialProperty = "_MainTex";
            //as funcoes de pausar ou continuar o video estao em DefaultTrackableEventHandler.cs, script do gameobject ImageTarget
        }
    }
    #endregion

    #region COROUTINES
    IEnumerator GetVideo()
    {
        //Corotina principal responsavel por adquirir o video do url, fazer a comunicacao e salvar o video dinamicamente em appdata.
        string url = "https://whatsnextdigital.com.br/cdn/Shared/videos/whatsnextdigital_video.mp4";
        www = UnityWebRequest.Get(url);
        //Caminho para salvar em appdata
        videoSavePath = Path.Combine(Application.persistentDataPath, "Videos");
        videoSavePath = Path.Combine(videoSavePath, "WhatsNext.mp4");
        //Inicializando o buffer
        www.downloadHandler = new DownloadHandlerBuffer();
        //progress = www.downloadProgress;
        if (!Directory.Exists(Path.GetDirectoryName(videoSavePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(videoSavePath));
            videoIsDownloaded = true;
        }
        //HTTP GET
        www.method = UnityWebRequest.kHttpVerbGET;
        var dh = new DownloadHandlerFile(videoSavePath);
        dh.removeFileOnAbort = true;
        www.downloadHandler = dh;
        //Tempo de timeout caso ocorra um erro no download
        www.timeout = 5;
        yield return www.SendWebRequest();
        //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A));

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            downloadFail.SetActive(true);    
        }
        else
        {
            Debug.Log("Download saved to: " + videoSavePath.Replace("/", "\\") + "\r\n" + www.error);
        }
    }
    #endregion

    #region UPDATEMETHOD
    void Update()
    {
        //Se o download ainda nao foi concluido, aparece a animacao de carregamento, caso contrario, a animacao some.
        if (www.isDone)
            loading.SetActive(false);
        else
            loading.SetActive(true);
        if(downloadFail.activeInHierarchy)
        {
            //Depois do timeout da corotina, o usuario podera apertar/clicar em cima para reiniciar a tentativa.
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                downloadFail.SetActive(false);
                StopAllCoroutines();
                StartCoroutine(GetVideo());
            }
        }
    }
    #endregion
}
