using Godot;

public partial class TutorialPopup : PopupPanel
{
	private TextureRect textureRect;
	private HBoxContainer textureBox;
	private RichTextLabel tutorialText;
	private Label tutorialLabel;
	private Label seitenLabel;
	private TextureButton buttonZurueck;
	private TextureButton buttonWeiter;
	private TextureButton buttonSchliessen;


	private int gameModi;

	private int aktuelleSeite = 0;

	private TutorialPageData[] Seiten;

	[Export]
	public TutorialBauPageData[] BauSeiten { get; set; } = new TutorialBauPageData[0]; // Baumodus signalInt = 0

	[Export]
	public TutorialSimPageData[] SimSeiten { get; set; } = new TutorialSimPageData[0]; // Simulationsmodus signalInt = 1

	[Export]
	public TutorialPruefPageData[] PruefSeiten { get; set; } = new TutorialPruefPageData[0]; // Pruefmodus signalInt = 2

	[Export]
	public TutorialPseudoPageData[] PseudoSeiten { get; set; } = new TutorialPseudoPageData[0];


	public bool isPseudoBlockTrue = false;

	private AudioStreamPlayer audioPlayer;

	public override void _Ready()
	{
		textureBox = GetNode<HBoxContainer>("VBoxContainer/TextureBox");
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

		gameModi = (int)Globals.getGameMode(this);

		switch (gameModi)
		{
			case 0: 
				Seiten = BauSeiten;
				break;
			case 1: 
				Seiten = SimSeiten;
				if (isPseudoBlockTrue)
				{
					Seiten = PseudoSeiten;
				}
				break;
			case 2: 
				Seiten = PruefSeiten;
				if (isPseudoBlockTrue)
				{
					Seiten = PseudoSeiten;
				}
				break;
		} 

		aktuelleSeite = 0;
		UpdatePageContent();
	}

	public void setTrueValuePseudoBlock(bool pseudoBlockTrue)
    {
         isPseudoBlockTrue = pseudoBlockTrue;
    }

	// public void onSetIsPseudoBlock(bool pseudoBlockTrue)
	// {
	// 	yesPseudoBlock = pseudoBlockTrue;
	// }

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
		
		if(seite.Image == null)
		{
			textureBox.Visible = false;
			textureRect.Visible = false;
		}
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