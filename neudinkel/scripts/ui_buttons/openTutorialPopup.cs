using Godot;

public partial class openTutorialPopup : TextureButton
{

    [Export] public PackedScene tutorialPopup { get; set; }

    [Export] public bool isPseudoBlock;


    public override void _Ready()
    {
        Pressed += OnTipsPressed;


        //Events.Instance.EmitSignal(Events.SignalName.SetTutorialIsPseudoBlock, isPseudoBlock);
    }

    private void OnTipsPressed()
    {

        if (tutorialPopup == null)
        {
            GD.PushWarning("TutorialPopup ist nicht gesetzt.");
            return;
        }
        // Popup instanziieren
        var popupInstance = tutorialPopup.Instantiate<TutorialPopup>();

        popupInstance.setTrueValuePseudoBlock(isPseudoBlock);

        if (popupInstance == null)
        {
            popupInstance = tutorialPopup.Instantiate();
            GetTree().CurrentScene.AddChild(popupInstance);
        }

        (popupInstance as TutorialPopup).PopupCentered();
    }

}