using DG.Tweening;
using System;
using UnityEngine.SceneManagement;
using static Define;

public class SceneManagerEx
{
    private BaseScene _currentScene;

    public T GetCurrentScene<T>() where T : BaseScene
    {
        return _currentScene as T;
    }

    public void SetCurrentScene(BaseScene scene)
    {
        _currentScene = scene;
    }

    public void LoadScene(EScene type, bool isAsync = true)
    {
        Managers.Clear();
        DOTween.KillAll();

        if (isAsync)
        {
            SceneManager.LoadSceneAsync(GetSceneName(type));
        }
        else
        {
            SceneManager.LoadScene(GetSceneName(type));
        }
    }

    private string GetSceneName(EScene type)
    {
        string name = Enum.GetName(typeof(EScene), type);
        return name;
    }

    public void Clear()
    {
        _currentScene.Clear();
    }
}
