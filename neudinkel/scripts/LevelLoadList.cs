using Godot;
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

		if (!DirAccess.DirExistsAbsolute(path))
		{
			DirAccess.MakeDirAbsolute(path);
			return;
		}

		dir = DirAccess.Open(path);

		/* TODO: No error handling :( */
		dir.ListDirBegin();
		fname = dir.GetNext();
		while (fname != "")
		{
			/* there should be no directories in here. if there are: oops. */
			this.AddItem(fname);
			fname = dir.GetNext();
		}

	}

}
