using Godot;

public partial class TutorialPopup : PopupPanel
{
	private TextureRect textureRect;
	private RichTextLabel tutorialText;
	private Label tutorialLabel;
	private Label seitenLabel;
	private TextureButton buttonZurueck;
	private TextureButton buttonWeiter;
	private TextureButton buttonSchliessen;

	private int aktuelleSeite = 0;

	[Export]
	public TutorialPageData[] Seiten { get; set; } = new TutorialPageData[0];

	private AudioStreamPlayer audioPlayer;

	public override void _Ready()
	{
		textureRect = GetNode<TextureRect>("VBoxContainer/TextureBox/BilderBox");
		tutorialText = GetNode<RichTextLabel>("VBoxContainer/TextBox/Text");
		tutorialLabel = GetNode<Label>("VBoxContainer/TextBox/TutorialLabel");
		seitenLabel = GetNode<Label>("VBoxContainer/ButtonsundLabelBox/Seitenlabel");
		buttonZurueck = GetNode<TextureButton>("VBoxContainer/ButtonsundLabelBox/Backwards");
		buttonWeiter = GetNode<TextureButton>("VBoxContainer/ButtonsundLabelBox/Forwards");
		buttonSchliessen = GetNode<TextureButton>("VBoxContainer/CloseBox/CloseButton");

		audioPlayer = new AudioStreamPlayer();
		AddChild(audioPlayer);

		buttonZurueck.Pressed += OnBackwardsPressed;
		buttonWeiter.Pressed += OnForwardsPressed;
		buttonSchliessen.Pressed += OnClosePressed;

		if (Seiten.Length == 0)
		{
			GD.PushWarning("TutorialPopup: 'Seiten' Liste ist leer. Bitte im Inspector füllen.");
			return;
		}

		aktuelleSeite = 0;
		UpdatePageContent();
	}

	private void OnBackwardsPressed()
	{
		if (Seiten.Length == 0)
			return;

		aktuelleSeite--;
		if (aktuelleSeite < 0)
			aktuelleSeite = Seiten.Length - 1;

		UpdatePageContent();
	}

	private void OnForwardsPressed()
	{
		if (Seiten.Length == 0)
			return;

		aktuelleSeite++;
		if (aktuelleSeite >= Seiten.Length)
			aktuelleSeite = 0;

		UpdatePageContent();
	}

	private void OnClosePressed()
	{
		Hide();
	}

	private void UpdatePageContent()
	{
		if (Seiten.Length == 0)
			return;

		var seite = Seiten[aktuelleSeite];
		
		tutorialLabel.Text = seite.Label ?? ""; 

		tutorialText.Text = seite.Text ?? "";

		textureRect.Texture = seite.Image;

		seitenLabel.Text = $"{aktuelleSeite + 1}/{Seiten.Length}";

		PlayPageAudio(seite);

    	ResetSize();

    	PopupCentered();
	}

	private void PlayPageAudio(TutorialPageData seite)
	{
		if (audioPlayer == null)
			return;

		audioPlayer.Stop();

		if (seite.Audio != null)
		{
			audioPlayer.Stream = seite.Audio;
			audioPlayer.Play();
		}
		else
		{
			audioPlayer.Stream = null;
		}
	}
}