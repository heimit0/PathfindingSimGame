using Godot;

public partial class SlideDownMenuBauModus : Node
{
    [Export] public NodePath toggleButtonPath;
    [Export] public NodePath slideDownMenuPath; // Pfad zum bereits vorhandenen Menü-Control
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
        // Button beim starten suchen - falls nicht vorhanden spucken wir einen Fehler aus 
        menuToggleButton = GetNodeOrNull<TextureButton>(toggleButtonPath);
        if (menuToggleButton == null)
        {
            GD.PushError("ToggleButton konnte nicht gefunden werden.");
            return;
        }

        // Bereits in der Szene vorhandenes Menü holen
        slideDownMenu = GetNodeOrNull<Control>(slideDownMenuPath);
        if (slideDownMenu == null)
        {
            GD.PushError("SlideDownMenu konnte nicht gefunden werden.");
            return;
        }

        // Setup für das Menü durchführen
        CallDeferred("Setup");

        // Toggle-Button verbinden
        menuToggleButton.Pressed += OnOpenButtonPressed;
    }

    private void Setup()
    {
        // Falls das Menü null ist springen wir gleich wieder raus 
        if (slideDownMenu == null)
            return;

        // Oberste Kante von meinem Menü abholen 
        slideDownMenuTopEdgeYPos = slideDownMenu.Position.Y;

        // Hier setzen wir die Höhe sicherheitshalber nochmals zurück und mindestens auf 100 falls diese kleiner ist 
        slideDownMenu.ResetSize();
        var h = slideDownMenu.Size.Y;
        if (h <= 0) h = 100;

        // Position der Oberen Kante des Menü wenn dieses nach unten gefahren ist 
        viewportTopEdge = slideDownMenuTopEdgeYPos + h + 30;

        // Menü startet ohne Transparenz
        var color = slideDownMenu.Modulate;
        color.A = 1f;
        slideDownMenu.Modulate = color;

        // Menü initial versteckt (oben außerhalb des Sichtbereichs)
        slideDownMenu.Position = new Vector2(
            slideDownMenu.Position.X,
            slideDownMenuTopEdgeYPos
        );

        // Button holen der das Menü wieder nach oben fährt 
        var hideButton = slideDownMenu.GetNodeOrNull<TextureButton>(
            "HBoxContainer/VBoxContainer/HBoxContainer2/Menü ausblenden"
        );

        // Check ob wir den Button überhaupt finden sonst bissl Fehlermeldungen fürs Debuggen 
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
    }

    private void OnOpenButtonPressed()
    {
        // Menü anzeigen
        ShowMenu();
    }

    private void ShowMenu()
    {
        if (slideDownMenu == null)
        {
            GD.PushError("SlideDownMenu ist nicht gesetzt.");
            return;
        }

        // Menü sichtbar machen
        slideDownMenu.Visible = true;

        // Button ausblenden
        menuToggleButton.Visible = false;

        // Animation starten
        AnimateDown();
    }

    private void AnimateDown()
    {
        // Tween killen falls es existiert und dann neues erstellen und dieses nach unten laufen lassen 
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

        // selbes Spiel wie beim runter bewegen 
        _tween?.Kill();
        _tween = CreateTween();
        
        // Menü nach oben sliden  
        _tween.TweenProperty(slideDownMenu, "position:y", slideDownMenuTopEdgeYPos, animationTime)
              .SetTrans(Tween.TransitionType.Quad)
              .SetEase(Tween.EaseType.In);
        
        // Menü verstecken (aber nicht löschen)
        _tween.TweenCallback(Callable.From(() =>
        {
            slideDownMenu.Visible = false;
        }));
        
        // Warten bis wir den Button wieder anzeigen 
        _tween.TweenInterval(fadeWaitTime);
        
        // Button einblenden - mit TweenCallback hängen wir an das _tween eine Funktion an die automatisch aufgerufen wird sobald die Animation rdy ist 
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

        // Button Einblenden - ähnlich wie beim einblenden nur die andere Richtung 
        var buttonTween = CreateTween();
        buttonTween.TweenProperty(menuToggleButton, "modulate:a", 1f, fadeDurationTime)
                   .SetTrans(Tween.TransitionType.Linear)
                   .SetEase(Tween.EaseType.InOut);
    }
}