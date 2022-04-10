using UnityEngine;

public class EnterWorktable : GoThroughDoor
{
    [SerializeField] private int greenhouseNum = 0;

    public override void Start()
    {
        base.Start();
    }

    public override void DoInteraction()
    {
        if (Player.PlayerInstance.CarriedObject == null)
        {
            Worktable.GreenhouseNum = greenhouseNum;
            base.DoInteraction();
        }
        else
        {
            Debug.LogWarning("Cannot enter worktable while carrying plant");
            // TODO: make it so you carry the plant here and it passes it to the worktable to do work (pruning) on it
        }
    }
}
