using Godot;

[GlobalClass] 
public partial class TutorialPageData : Resource
{
    [Export]
    public string Label { get; set; } = "";

    [Export]
    public string Text { get; set; } = "";

    [Export]
    public Texture2D Image { get; set; }

    [Export]
    public AudioStream Audio { get; set; }
}