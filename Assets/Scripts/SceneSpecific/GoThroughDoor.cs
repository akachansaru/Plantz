using UnityEngine;

public class GoThroughDoor : Interactable
{
    public SceneLoader sceneLoader;
    public string scene;

    public override void DoInteraction()
    {
        sceneLoader.LoadScene(scene);
    }
}
