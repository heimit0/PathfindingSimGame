using Godot;

public partial class openTutorialPopup : TextureButton
{

    [Export] public PackedScene tutorialPopup { get; set; }

    public override void _Ready()
    {
        Pressed += OnTipsPressed;
    }

    private void OnTipsPressed()
    {
        if (tutorialPopup == null)
        {
            GD.PushWarning("tutorialPopup ist nicht gesetzt.");
            return;
        }

        // Popup instanziieren
        Node popupInstance = tutorialPopup.Instantiate();

        // In aktuelle Szene einfügen
        GetTree().CurrentScene.AddChild(popupInstance);

        if (popupInstance is TutorialPopup popup) {
            popup.PopupCentered();
        }
    }
}