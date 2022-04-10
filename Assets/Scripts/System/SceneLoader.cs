using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        LoadSceneStatic(sceneName);
    }

    public static void LoadSceneStatic(string sceneName)
    {
        GlobalControl.Instance.Save();
        SceneManager.LoadScene(sceneName);

        if (sceneName == ConstantValues.Scenes.Worktable)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
