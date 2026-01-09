using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScreen : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private int m_NextSceneInFlow;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _exitButton;

    [SerializeField] private GameObject _LoadingScreen;

    private void OnEnable()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        BlockInput();
        Invoke(nameof(EnableInput), 0.5f);
    }

    private void EnableInput()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    private void BlockInput()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void Awake()
    {
        _LoadingScreen?.SetActive(false);

        if (_continueButton != null && _continueButton.TryGetComponent(out BaseUIButton continueButton))
        {
            continueButton.Clicked += Continue;
        }
        if (_exitButton != null && _exitButton.TryGetComponent(out BaseUIButton exitButton))
        {
            exitButton.Clicked += Exit;
        }
    }

    private void OnDestroy()
    {
        if (_continueButton != null && _continueButton.TryGetComponent(out BaseUIButton continueButton))
        {
            continueButton.Clicked -= Continue;
        }
        if (_exitButton != null && _exitButton.TryGetComponent(out BaseUIButton exitButton))
        {
            exitButton.Clicked -= Exit;
        }
    }

    public void Continue(object sender, EventArgs e)
    {
        if (_continueButton.TryGetComponent(out BaseUIButton continueButton))
        {
            continueButton.IsInteractable = false;
        }
        _LoadingScreen?.SetActive(true);

        SceneManager.LoadScene(m_NextSceneInFlow);
    }

    public void Exit(object sender, EventArgs e)
    {
        if (_exitButton.TryGetComponent(out BaseUIButton exitButton))
        {
            exitButton.IsInteractable = false;
        }

        string dataFolder = Settings.Instance.m_DataPath;
        string parentFolder = Directory.GetParent(dataFolder).FullName;
        string zipFile = Path.Combine(parentFolder, "Data.zip");

        if(Directory.Exists(dataFolder))
        {
            if (File.Exists(zipFile)) File.Delete(zipFile);
            ZipFile.CreateFromDirectory(dataFolder, zipFile);
        }

        Application.OpenURL("file://" + parentFolder);

        Application.Quit();
    }
}
