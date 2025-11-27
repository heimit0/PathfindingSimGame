hsing Godot;
using System;

public partial class LevelLoadList : NeudinkelItemList
{

	public override void _Ready()
	{
		this.MaxColumns = 1;
		loadLevelList();
	}

	public void loadLevelList()
	{
		string path = "user://levels";
		string fname;
		DirAccess dir;

		if (!DirAccess.DirExists(path))
		{
			DirAccess.MakeDir(path);
			return;
		}

		dir = DirAccess.Open(path);

		/* TODO: No error handling :( */
		dir.ListDirBegin();
		while (fname != "")
		{
			/* there should be no directories in here. if there are: oops. */
			this.AddItem(fname);
			fname = dir.GetNext();
		}

	}

}
