using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// Effectmanager 
// fragt regelmäßig ab welche Tiles selected sind
// und hängt an diese die visuellen Effekte an oder entfernt sie
public partial class TileEffectManager : Node3D
{
    [Export] public PackedScene selectedTileChimneyEffect;  // Szene für Hexagon was nach oben fliegt im Editor auswählbar machen 
    [Export] public PackedScene selectedTileOutline;        // Szene für Selected Tiles auswählbar machen
    [Export] public float EffectHeight = 0.5f;              // Y-Offset für beide Effekte
    [Export] public float checkTime = 0.1f;                // Intervall in dem wir die Selected Tiles aus dem Gamemanger abfragen 1 Frame / checkTime

    private Node modusManager;

    // Die Selected Nodes als Property vom Manager
    private PropertyInfo selectedNodesProperty;

    // Hashmaps (Dictionary) um die aktiven Effekt für die jeweiligen Positionen zu speichern
    private readonly Dictionary<(int, int), Node3D> activeChimneys = new();
    private readonly Dictionary<(int, int), Node3D> activeOutlines = new();

    // Liste um zu schauen was schon ausgewählt war im letzen Durchlauf 
    private HashSet<(int, int)> listOfSelectedTilesLastCycle = new();

    // Initialisierung des Timer Wertes auf 0 
    private float timer = 0f;

    public override void _Ready()
    {
        modusManager = GetParent();
        if (modusManager == null)
        {
            GD.PrintErr("Kein Parent vorhanden");
            return;
        }

        var type = modusManager.GetType();

        // selectedNode Property 
        selectedNodesProperty = type.GetProperty("selectedNodes");


        if (selectedNodesProperty == null)
        {
            GD.PrintErr("Selected Nodes wurde nicht gefunden");
            return;
        }
        
        // _Process anstoßen
        SetProcess(true);
    }


    // Pro Frame des Spiels den Timer inkrementieren
    public override void _Process(double delta)
    {
        timer += (float)delta;
        if (timer < checkTime)
            return;
        
        // Timer wieder auf 0 setzen
        timer = 0f;
        UpdateFromSelection();
    }

    private void UpdateFromSelection()
    {
        var currentSelected = new HashSet<(int, int)>();
        object selectedNodesList = null;

        // Da Selected Nodes als PropertyInfo geholt wird und somit nur die Metadaten enthält müssen wir diese in ein Objekt umwandelen
        if (selectedNodesProperty != null)
            selectedNodesList = selectedNodesProperty.GetValue(modusManager);

        if (selectedNodesList is IEnumerable listEntries) // Überprüfung ob selectedNodesList iterierbar ist wenn ja dann umwandeln damit wir iterieren können 
        {
            foreach (var tile in listEntries) // durch die Selected Tiles iterieren und die Positionen abspeichern in CurrentSelected
            {
                if (tile is GraphNode node) // hier casten wir nochmals um sicherzustellen das es sich auch um GraphNodes handelt
                {
                    currentSelected.Add((node.tilePosX, node.tilePosY)); // Position des Nodes hinzufügen zu currentSelected
                }
            }
        }


    // Logik damit wir vergleichen können welche Tiles markiert sind --> Effekt anschalten 
    // Dafür vergleich der Positonen beider Listen (currentSelected / listOfSelectedTilesLastCycle)
    foreach (var pos in currentSelected)
    {
        if (!listOfSelectedTilesLastCycle.Contains(pos))
        {
            enableEffects(pos.Item1, pos.Item2); 
        }
    }

    // Logik damit wir vergleichen können welche Tiles nicht mehr markiert sind --> Effekt ausschalten 
    foreach (var pos in listOfSelectedTilesLastCycle)
    {
        if (!currentSelected.Contains(pos))
        {
            disableEffects(pos.Item1, pos.Item2);
        }
    }
    
    
    // listOfSelectedTilesLastCycle updaten damit diese aktuell bleibt
    listOfSelectedTilesLastCycle = currentSelected;

    }

    private void enableEffects(int tileX, int tileY)
    {
        // Umwandeln um mit activeChimneys vergleichen zu können
        var key = (intX: tileX, intY: tileY);

        // Falls einer der Effekt schon auf der Tile läuft (sollten eigentlich immer beide Effekte auf einer Tile sein) --> abbrechen 
        if (activeChimneys.ContainsKey(key) || activeOutlines.ContainsKey(key))
            return;


        Vector3 worldPos = GraphNode.convertTilePosToPos(tileX, tileY);
        worldPos.Y += EffectHeight;

        var chimneyNode = selectedTileChimneyEffect.Instantiate();
        AddChild(chimneyNode); // Die Szene als Kind des Nodes TileEffectManager anhängen
        var outlineNode = selectedTileOutline.Instantiate();
        AddChild(outlineNode);

        // Hier nochmal überprüfen ob chimneyNode und outlineNode Node3D sind --> dann an Position setzen
        if (chimneyNode is Node3D hexagonChimney && outlineNode is Node3D outline)
        {
            hexagonChimney.GlobalPosition = worldPos;
            outline.GlobalPosition = worldPos;
            hexagonChimney.Set("emitting", true); // Partikel einschalten 
        }

        activeChimneys[key] = chimneyNode as Node3D;
        activeOutlines[key] = outlineNode as Node3D;
    }

    private void disableEffects(int tileX, int tileY)
    {
        var key = (intX: tileX, intY: tileY);


        // Sicherheitshalber checken wir hier ob der Effekt den schon existiert 
        // (wir checken ob die Listen activeChimneys oder activeOutlines den key besitzen) 
        // --> falls nicht dann brechen wir ab 
        // --> falls doch speichern wir den 3dNode beim key in chimneyNode / outlineNode (out var chimneyNode)
        if (!activeChimneys.TryGetValue(key, out var chimneyNode) || !activeOutlines.TryGetValue(key, out var outlineNode))
            return;

        // Falls es existiert ein mal ausschalten bitte 
        if (IsInstanceValid(chimneyNode) && IsInstanceValid(outlineNode))
        {
            chimneyNode.Set("emitting", false);
            chimneyNode.QueueFree();
            outlineNode.QueueFree();
        }

        activeChimneys.Remove(key);
        activeOutlines.Remove(key);
    }
}