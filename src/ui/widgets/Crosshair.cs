using Godot;

namespace NewGame.Core.UI;

public partial class Crosshair : Control
{
	public enum CrosshairState { Default, Interactive }

	[Export] public Color DefaultColor = new Color(1, 1, 1, 0.8f);
	[Export] public Color InteractiveColor = new Color(0.3f, 0.9f, 0.4f, 1f);
	[Export] public float DefaultSize = 8f;
	[Export] public float InteractiveSize = 12f;
	[Export] public float TransitionDuration = 0.15f;

	private CrosshairState _currentState = CrosshairState.Default;
	private ColorRect _centerDot;
	private Tween _tween;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Ignore;
		_centerDot = GetNode<ColorRect>("CenterDot");
		ApplyStyle(DefaultColor, DefaultSize);
		ConnectToUIManager();
	}

	private void ApplyStyle(Color color, float size)
	{
		if (_centerDot == null) return;
		
		_centerDot.Color = color;
		_centerDot.Size = new Vector2(size, size);
		_centerDot.Position = -_centerDot.Size / 2;
	}

	private void ConnectToUIManager()
	{
		if (UIManager.Instance != null)
		{
			UIManager.Instance.InteractionTargetChanged += OnInteractionTargetChanged;
		}
	}

	private void OnInteractionTargetChanged(string actionName, string inputAction, bool hasTarget)
	{
		SetState(hasTarget ? CrosshairState.Interactive : CrosshairState.Default);
	}

	public void SetState(CrosshairState state)
	{
		if (_currentState == state) return;
		_currentState = state;

		_tween?.Kill();
		_tween = CreateTween();
		_tween.SetEase(Tween.EaseType.Out);
		_tween.SetTrans(Tween.TransitionType.Cubic);

		var targetColor = state == CrosshairState.Interactive ? InteractiveColor : DefaultColor;
		var targetSize = state == CrosshairState.Interactive ? InteractiveSize : DefaultSize;
		var targetVec = new Vector2(targetSize, targetSize);

		_tween.Parallel().TweenProperty(_centerDot, "color", targetColor, TransitionDuration);
		_tween.Parallel().TweenProperty(_centerDot, "size", targetVec, TransitionDuration);
		_tween.Parallel().TweenProperty(_centerDot, "position", -targetVec / 2, TransitionDuration);
	}

	public override void _ExitTree()
	{
		if (UIManager.Instance != null)
		{
			UIManager.Instance.InteractionTargetChanged -= OnInteractionTargetChanged;
		}
	}
}
