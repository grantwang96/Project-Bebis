using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public interface ISceneController {
    bool LoadScene(string sceneName, bool requiresLoadingScreen = true);
    void LoadSceneInstant(string sceneName);

    event Action<bool> OnSceneLoaded;
}

public class SceneController : ISceneController {

    public event Action<bool> OnSceneLoaded;

    private string _nextScene;
    private bool _isLoading;
    private AsyncOperation _currentSceneLoadProcess;

    public SceneController() {
        // listen to scene manager events
        SceneManager.sceneLoaded += SceneLoaded;
    }

    public bool LoadScene(string sceneName, bool requiresLoadingScreen = true) {
        Debug.Log($"[{nameof(SceneController)}]: Starting load scene request for \"{sceneName}\"...");
        // if we're already loading a scene
        if (_isLoading) {
            Debug.LogWarning($"[{nameof(SceneController)}]: Already loading scene \"{_nextScene}\"!");
            OnSceneLoaded?.Invoke(false);
            return false;
        }
        // if we're already in the specified scene
        if(sceneName == SceneManager.GetActiveScene().name) {
            OnSceneLoaded?.Invoke(true);
            return true;
        }
        // if we cannot find the scene specified
        if (!ContainsScene(sceneName)) {
            Debug.LogError($"[{nameof(SceneController)}]: Could not retrieve scene with name \"{sceneName}\"!");
            OnSceneLoaded?.Invoke(false);
            return false;
        }
        // if it does not require a loading screen, immediately enter
        if (!requiresLoadingScreen) {
            LoadSceneInstant(sceneName);
            return true;
        }
        // begin loading process
        _isLoading = true;
        _nextScene = sceneName;
        _currentSceneLoadProcess = SceneManager.LoadSceneAsync(sceneName);
        return true;
    }

    public void LoadSceneInstant(string sceneName) {
        // immediately load the scene given
        Scene sceneToLoad = SceneManager.GetSceneByName(sceneName);
        if (!ContainsScene(sceneName)) {
            Debug.LogError($"[{nameof(SceneController)}]: Could not retrieve scene with name \"{sceneName}\"!");
            OnSceneLoaded?.Invoke(false);
            return;
        }
        SceneManager.LoadScene(sceneName);
        OnSceneLoaded?.Invoke(true);
        Debug.Log($"[{nameof(SceneController)}]: Loaded scene {sceneName}!");
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        Debug.Log($"[{nameof(SceneController)}]: Loaded scene {scene.name}!");
        if(scene.name == _nextScene) {
            _isLoading = false;
            _currentSceneLoadProcess = null;
            OnSceneLoaded?.Invoke(true);
        }
    }

    private bool ContainsScene(string sceneName) {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene != null;
    }
}
