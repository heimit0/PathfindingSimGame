using Godot;

public partial class SlideDownMenu : Node
{
    [Export] public NodePath toggleButtonPath = "./TextureButton";
    [Export] public string slideDownMenuPath = "res://scenes/slidedownmenu.tscn";
    [Export] public float animationTime = 0.5f;
    [Export] public float fadeWaitTime = 0.02f; // Wartezeit bevor Button erscheint
    [Export] public float fadeDurationTime = 2.6f; // Einblendegeschwindigkeit 

    private TextureButton menuToggleButton;
    private Control slideDownMenu;
    private Tween _tween;

    private float slideDownMenuTopEdgeYPos;
    private float viewportTopEdge;

    public override void _Ready()
    {
        menuToggleButton = GetNodeOrNull<TextureButton>(toggleButtonPath);
        if (menuToggleButton == null)
        {
            GD.PushError($"ToggleButton konnte über Pfad '{toggleButtonPath}' nicht gefunden werden.");
            return;
        }

        menuToggleButton.Pressed += OnOpenButtonPressed;
    }

    private void OnOpenButtonPressed()
    {
        if (slideDownMenu == null)
            ShowMenu();
    }

    private void ShowMenu()
    {
        var menuScene = ResourceLoader.Load<PackedScene>(slideDownMenuPath);
        if (menuScene == null)
        {
            GD.PushError($"SlideDownMenu konnte unter Pfad '{slideDownMenuPath}' nicht geladen werden.");
            return;
        }

        slideDownMenu = menuScene.Instantiate<Control>();
        GetTree().CurrentScene.AddChild(slideDownMenu);

        // Button ausblenden
        menuToggleButton.Visible = false;
        GD.Print("Button wurde ausgeblendet");

        CallDeferred(nameof(Setup));
    }

    private void Setup()
    {
        if (slideDownMenu == null)
            return;

        slideDownMenuTopEdgeYPos = slideDownMenu.Position.Y;

        slideDownMenu.ResetSize();
        var h = slideDownMenu.Size.Y;
        if (h <= 0) h = 100;

        viewportTopEdge = slideDownMenuTopEdgeYPos + h + 10;

        // Menü startet ohne Transparenz
        var color = slideDownMenu.Modulate;
        color.A = 1f;
        slideDownMenu.Modulate = color;

        var hideButton = slideDownMenu.GetNodeOrNull<TextureButton>(
            "HBoxContainer/VBoxContainer/HBoxContainer2/Menü ausblenden"
        );
        if (hideButton != null)
        {
            hideButton.Pressed += HideMenu;
            GD.Print("Hide Button mit Hide Menu verbunden");
        }
        else
        {
            GD.PushWarning("Hide Button nicht gefunden");
            slideDownMenu.PrintTreePretty();
        }

        AnimateDown();
    }

    private void AnimateDown()
    {
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(slideDownMenu, "position:y", viewportTopEdge, animationTime)
              .SetTrans(Tween.TransitionType.Quad)
              .SetEase(Tween.EaseType.Out);
    }

    public void HideMenu()
    {
        if (slideDownMenu == null)
            return;

        GD.Print("Hide Menü wird nach oben bewegt");

        _tween?.Kill();
        _tween = CreateTween();
        
        // Menü nach oben sliden  
        _tween.TweenProperty(slideDownMenu, "position:y", slideDownMenuTopEdgeYPos, animationTime)
              .SetTrans(Tween.TransitionType.Quad)
              .SetEase(Tween.EaseType.In);
        
        // Menü entfernen
        _tween.TweenCallback(Callable.From(() =>
        {
            slideDownMenu.QueueFree();
            slideDownMenu = null;
            GD.Print("Menü entfernt");
        }));
        
        // Warten bis wir den Button wieder zeigen 
        _tween.TweenInterval(fadeWaitTime);
        
        // Button einblenden
        _tween.TweenCallback(Callable.From(() =>
        {
            FadeInButton();
        }));
    }

    private void FadeInButton()
    {
        if (menuToggleButton == null)
            return;

        // Button transparent stellen 
        menuToggleButton.Visible = true;
        var color = menuToggleButton.Modulate;
        color.A = 0f;
        menuToggleButton.Modulate = color;

        // Button Einblenden
        var buttonTween = CreateTween();
        buttonTween.TweenProperty(menuToggleButton, "modulate:a", 1f, fadeDurationTime)
                   .SetTrans(Tween.TransitionType.Linear)
                   .SetEase(Tween.EaseType.InOut);
        
    }
}