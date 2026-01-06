using System.Linq;
using Godot;

namespace NewGame.Core.UI;

public partial class InteractionPrompt : Control
{
	[Export] public float ShowDuration = 0.2f;
	[Export] public float HideDuration = 0.15f;

	private HBoxContainer _container;
	private Label _keyLabel;
	private Label _actionLabel;
	private Tween _tween;
	private bool _isVisible;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Ignore;
		_container = GetNode<HBoxContainer>("Container");
		_keyLabel = GetNode<Label>("Container/KeyBackground/KeyLabel");
		_actionLabel = GetNode<Label>("Container/ActionLabel");
		ConnectToUIManager();
		HideImmediate();
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
		if (hasTarget)
		{
			Show(actionName, inputAction);
		}
		else
		{
			Hide();
		}
	}

	public void Show(string actionName, string inputAction)
	{
		if (_isVisible && _actionLabel.Text == actionName) return;

		var key = InputMap.ActionGetEvents(inputAction).OfType<InputEventKey>().FirstOrDefault();
		
		_keyLabel.Text = key is null ? "?" : OS.GetKeycodeString(key.PhysicalKeycode);
		_actionLabel.Text = actionName;
		_isVisible = true;

		_tween?.Kill();
		_tween = CreateTween();
		_tween.SetEase(Tween.EaseType.Out);
		_tween.SetTrans(Tween.TransitionType.Back);

		Modulate = new Color(1, 1, 1, 0);
		Visible = true;
		Scale = new Vector2(0.8f, 0.8f);

		_tween.Parallel().TweenProperty(this, "modulate:a", 1f, ShowDuration);
		_tween.Parallel().TweenProperty(this, "scale", Vector2.One, ShowDuration);
	}

	public new void Hide()
	{
		if (!_isVisible) return;
		_isVisible = false;

		_tween?.Kill();
		_tween = CreateTween();
		_tween.SetEase(Tween.EaseType.In);
		_tween.SetTrans(Tween.TransitionType.Cubic);

		_tween.Parallel().TweenProperty(this, "modulate:a", 0f, HideDuration);
		_tween.Parallel().TweenProperty(this, "scale", new Vector2(0.9f, 0.9f), HideDuration);
		_tween.TweenCallback(Callable.From(() => Visible = false));
	}

	private void HideImmediate()
	{
		Visible = false;
		Modulate = new Color(1, 1, 1, 0);
		_isVisible = false;
	}

	public override void _ExitTree()
	{
		if (UIManager.Instance != null)
		{
			UIManager.Instance.InteractionTargetChanged -= OnInteractionTargetChanged;
		}
	}
}
