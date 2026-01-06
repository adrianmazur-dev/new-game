using Godot;

namespace NewGame.Core.UI;

public partial class UIManager : CanvasLayer
{
    public static UIManager Instance { get; private set; }

    [Signal]
    public delegate void InteractionTargetChangedEventHandler(
        string actionName,
        string inputAction,
        bool hasTarget
    );

    private Control _gameplayHUD;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void _Ready()
    {
        Layer = 100;
        _gameplayHUD = GetNodeOrNull<Control>("GameplayHUD");
    }

    public void NotifyInteractionTarget(string actionName, string inputAction)
    {
        EmitSignal(SignalName.InteractionTargetChanged, actionName, inputAction, true);
    }

    public void ClearInteractionTarget()
    {
        EmitSignal(SignalName.InteractionTargetChanged, "", "", false);
    }
}
