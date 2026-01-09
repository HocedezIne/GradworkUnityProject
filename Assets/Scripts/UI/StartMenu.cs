using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum Window
{
    Main,
    Difficulty,
    Info
}

public class StartMenu : MonoBehaviour
{
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _infoButton;
    [SerializeField] private GameObject _quitButton;

    [SerializeField] private GameObject _easyButton;
    [SerializeField] private GameObject _hardButton;

    [SerializeField] private List<GameObject> _backButtons;

    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _difficultyMenu;
    [SerializeField] private GameObject _infoMenu;
    [SerializeField] private GameObject _LoadingScreen;

    private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        BlockInput();
        Invoke(nameof(EnableInput), 0.5f);
    }

    private void Awake()
    {
        _mainMenu.SetActive(true);
        _difficultyMenu.SetActive(false);
        _infoMenu.SetActive(false);
        _LoadingScreen.SetActive(false);

        if(_startButton.TryGetComponent(out BaseUIButton playbutton))
        {
            playbutton.Clicked += ShowDifficulyMenu;
        }
        if(_infoButton.TryGetComponent(out BaseUIButton infobutton))
        {
            infobutton.Clicked += ShowInfoMenu;
        }
        if(_quitButton.TryGetComponent(out BaseUIButton quitbutton))
        {
            quitbutton.Clicked += Quit;
        }

        if (_easyButton.TryGetComponent(out BaseUIButton easyButton))
        {
            easyButton.Clicked += StartEasyGame;
        }
        if (_hardButton.TryGetComponent(out BaseUIButton hardButton))
        {
            hardButton.Clicked += StartHardGame;
        }

        foreach(GameObject backButton in _backButtons)
        {
            if (backButton.TryGetComponent(out BaseUIButton backbutton))
            {
                backbutton.Clicked += ShowMainMenu;
            }
        }
    }

    private void OnDestroy()
    {
        if(_startButton != null && _startButton.TryGetComponent(out BaseUIButton startButton))
        {
            startButton.Clicked -= ShowDifficulyMenu;
        }
        if (_infoButton != null &&_infoButton.TryGetComponent(out BaseUIButton infobutton))
        {
            infobutton.Clicked -= ShowInfoMenu;
        }
        if (_quitButton != null && _quitButton.TryGetComponent(out BaseUIButton quitbutton))
        {
            quitbutton.Clicked -= Quit;
        }

        if (_easyButton != null && _easyButton.TryGetComponent(out BaseUIButton easyButton))
        {
            easyButton.Clicked -= StartEasyGame;
        }
        if (_hardButton != null && _hardButton.TryGetComponent(out BaseUIButton hardButton))
        {
            hardButton.Clicked -= StartHardGame;
        }

        foreach (GameObject backButton in _backButtons)
        {
            if (backButton != null && backButton.TryGetComponent(out BaseUIButton backbutton))
            {
                backbutton.Clicked -= ShowMainMenu;
            }
        }
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

    private void ShowMainMenu(object sender, EventArgs e)
    {
        _mainMenu.SetActive(true);
        _infoMenu.SetActive(false);
        _difficultyMenu.SetActive(false);
    }

    private void ShowInfoMenu(object sender, EventArgs e)
    {
        _mainMenu.SetActive(false);
        _infoMenu.SetActive(true);
        _difficultyMenu.SetActive(false);
    }

    private void ShowDifficulyMenu(object sender, EventArgs e)
    {
        _mainMenu.SetActive(false);
        _infoMenu.SetActive(false);
        _difficultyMenu.SetActive(true);
    }

    private void StartEasyGame(object sender, EventArgs e)
    {
        if(_easyButton.TryGetComponent(out BaseUIButton easyButton))
        {
            easyButton.IsInteractable = false;
        }
        _LoadingScreen.SetActive(true);

        SceneManager.LoadScene(1);
    }

    private void StartHardGame(object sender, EventArgs e)
    {
        if (_hardButton.TryGetComponent(out BaseUIButton hardButton))
        {
            hardButton.IsInteractable = false;
        }
        _LoadingScreen.SetActive(true);

        SceneManager.LoadScene(2);
    }

    private void Quit(object sender, EventArgs e)
    {
        if (_quitButton.TryGetComponent(out BaseUIButton quitButton))
        {
            quitButton.IsInteractable = false;
        }
        Application.Quit();
    }
}
