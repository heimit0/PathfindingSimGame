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

        Node popupInstance = null;
        TutorialPopup existingPopup = GetNodeOrNull<TutorialPopup>("/root/Node3D/PopupPanel"); // TODO fix
        if (existingPopup != null) popupInstance = existingPopup;

        if (popupInstance == null)
        {
            popupInstance = tutorialPopup.Instantiate();
            GetTree().CurrentScene.AddChild(popupInstance);
        }

        (popupInstance as TutorialPopup).PopupCentered();
    }
}