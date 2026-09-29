using System.Collections.Generic;
using System.Linq;
using Dungeon.Core;
using UnityEditor;
using UnityEngine;

namespace Dungeon.MapDebug.Editor
{
    // Compose the supplied artwork without baking or changing the source textures.
    internal sealed class MapArtComposition
    {
        const string Root="Assets/Arts/Hex and Tile Samples 1.0.1/";
        readonly Dictionary<CellPosition,CellDefinition> cells;
        readonly IMapTopology topology;
        readonly bool hex;
        readonly Texture2D riverStraight,riverBend,riverEnd,riverSpring,pond,ground,highlands,hills;
        readonly HashSet<CellPosition> outlets=new HashSet<CellPosition>();
        readonly Dictionary<CellPosition,Rect> smallLakes=new Dictionary<CellPosition,Rect>();
        readonly Material rockMaterial,riverMaterial,dryRockMaterial,customWallMaterial;
        readonly MapTheme theme;
        readonly IReadOnlyDictionary<CellPosition,TerrainKind> surfaces;
        readonly Dictionary<TerrainKind,Texture2D> terrainArt=new Dictionary<TerrainKind,Texture2D>();
        readonly Texture2D snowCapped,snowCovered,treeCluster;

        Texture2D Load(string path) => AssetDatabase.LoadAssetAtPath<Texture2D>(Root+path+".png");
        public MapArtComposition(MapDefinition map,IMapTopology topology,DebugGrid grid,MapTheme theme)
        {
            this.theme=theme;surfaces=map.SurfaceTerrain;
            if(theme!=null)foreach(var v in theme.visuals)terrainArt[v.terrain]=grid==DebugGrid.Hex?v.hexTexture:v.squareTexture;
            cells=map.Cells.ToDictionary(c=>c.Position);this.topology=topology;hex=grid==DebugGrid.Hex;
            string water=hex?"Hex Samples/Hex Rivers Coasts & Seas/":"Tile Samples/Tile River Coasts & Seas/";
            riverStraight=Load(water+(hex?"hexRiver010010-00":"river0101-00"));
            riverBend=Load(water+(hex?"hexRiver001010-01":"river0110-00"));
            riverEnd=Load(water+(hex?"hexRiver000001-00":"river0001-00"));
            riverSpring=hex?Load(water+"hexRiverLakeEnd010000-00"):riverEnd;
            pond=Load(water+"lakeSmall03");
            string basic=hex?"Hex Samples/Hex Basic Terrain Set/hex":"Tile Samples/Tile Basic Terrain Set/";
            ground=Load(basic+(hex?"Plains00":"plains00"));
            highlands=Load(basic+(hex?"Highlands00":"highlands00"));
            hills=Load(basic+(hex?"Hills00":"hills00"));
            snowCapped=Load(basic+(hex?"Mountain00":"mountain00"));
            snowCovered=Load(hex?"Hex Samples/Hex Cold Lands/hexMountainSnow00":"Tile Samples/Tile Cold Lands/mountainSnow00");
            treeCluster=Load("Hex Samples/Hex Medieval Fantasy Locations/treesA_cluster00");
            var groundKind=theme==null?TerrainKind.Plains:theme.land.OrderByDescending(t=>t.weight).First().terrain;
            // Small lakes use an open clearing; rivers keep their recorded surface terrain.
            if(groundKind==TerrainKind.Forest||groundKind==TerrainKind.Woodland)groundKind=TerrainKind.Plains;
            var visual=theme==null?null:theme.visuals.FirstOrDefault(v=>v.terrain==groundKind);
            if(visual!=null)ground=hex?visual.hexTexture:visual.squareTexture;
            var shader=Shader.Find("Hidden/Dungeon/Map Art Composition");
            rockMaterial=new Material(shader){hideFlags=HideFlags.HideAndDontSave};rockMaterial.SetFloat("_Rock",1);
            dryRockMaterial=new Material(shader){hideFlags=HideFlags.HideAndDontSave};dryRockMaterial.SetFloat("_Dry",1);
            customWallMaterial=new Material(shader){hideFlags=HideFlags.HideAndDontSave};
            customWallMaterial.SetFloat("_CutoutHex",hex&&theme!=null&&theme.clipWallToHex?1:0);
            riverMaterial=new Material(shader){hideFlags=HideFlags.HideAndDontSave};riverMaterial.SetFloat("_River",1);riverMaterial.SetFloat("_Hex",hex?1:0);
            var pending=new HashSet<CellPosition>(map.Cells.Where(c=>c.Terrain==TerrainKind.Lake).Select(c=>c.Position));
            while(pending.Count>0)
            {
                var part=new List<CellPosition>();var queue=new Queue<CellPosition>();var start=pending.OrderBy(p=>p.Y).ThenBy(p=>p.X).First();
                pending.Remove(start);queue.Enqueue(start);
                while(queue.Count>0){var p=queue.Dequeue();part.Add(p);foreach(var n in topology.Neighbors(p))if(pending.Remove(n))queue.Enqueue(n);}
                // Lake sprites are always individual, native-scale decorations.
                foreach(var p in part)smallLakes[p]=default;

            }
            pending=new HashSet<CellPosition>(map.Cells.Where(c=>c.Terrain==TerrainKind.River||c.Terrain==TerrainKind.Ford).Select(c=>c.Position));
            while(pending.Count>0)
            {
                var part=new List<CellPosition>();var queue=new Queue<CellPosition>();var start=pending.OrderBy(p=>p.Y).ThenBy(p=>p.X).First();
                pending.Remove(start);queue.Enqueue(start);
                while(queue.Count>0){var p=queue.Dequeue();part.Add(p);foreach(var n in topology.Neighbors(p))if(pending.Remove(n))queue.Enqueue(n);}
                var ends=part.Where(p=>topology.Neighbors(p).Count(part.Contains)==1)
                    .OrderByDescending(p=>topology.Neighbors(p).Any(n=>!cells.ContainsKey(n)))
                    .ThenBy(p=>p.Y).ThenBy(p=>p.X).ToList();
                if(ends.Count>0)outlets.Add(ends[0]);
            }
        }
        public void Dispose(){Object.DestroyImmediate(rockMaterial);Object.DestroyImmediate(riverMaterial);Object.DestroyImmediate(dryRockMaterial);Object.DestroyImmediate(customWallMaterial);}
        public string LocationName(CellDefinition cell) => cell.Kind==CellKind.Event?
            theme?.locations?.FirstOrDefault(p=>p!=null&&p.count>0&&p.contentId==cell.ContentId)?.displayName:null;
        public void SetCanvasSize(Vector2 size)
        {
            rockMaterial.SetVector("_CanvasSize",size);riverMaterial.SetVector("_CanvasSize",size);
            dryRockMaterial.SetVector("_CanvasSize",size);
            customWallMaterial.SetVector("_CanvasSize",size);
        }
        static void DrawClipped(Rect rect,Texture2D texture,Material material)
        {
            material.SetVector("_DrawRect",new Vector4(rect.x,rect.y,rect.width,rect.height));
            Graphics.DrawTexture(rect,texture,material);
        }
        Vector2 Center(CellPosition p)=>hex?new Vector2(2*p.X+p.Y,-1.5f*p.Y):new Vector2(2*p.X,-2*p.Y);
        public bool DrawGround(CellDefinition cell,Rect rect)
        {
            var location=theme?.locations?.FirstOrDefault(p=>p!=null&&p.count>0&&p.contentId==cell.ContentId);
            if(cell.Kind==CellKind.Event&&location!=null)
            {
                var texture=hex?location.hexTexture:location.squareTexture;
                if(texture!=null){GUI.DrawTexture(rect,texture,ScaleMode.StretchToFill,true);return true;}
            }
            if(cell.Kind==CellKind.Event && theme!=null && theme.peakEventCount>0 && cell.ContentId==theme.peakEventId)
            {GUI.DrawTexture(rect,snowCapped,ScaleMode.StretchToFill,true);return true;}
            var surface=cell.Terrain;
            bool river=surface==TerrainKind.River||surface==TerrainKind.Ford;
            if(river && surfaces.TryGetValue(cell.Position,out var underneath))surface=underneath;
            if(surface==TerrainKind.Mountain&&theme!=null)
            {
                var custom=hex?theme.wallHexTexture:theme.wallSquareTexture;
                if(custom!=null){DrawClipped(rect,custom,customWallMaterial);return true;}
            }
            if(surface==TerrainKind.Rock&&theme!=null&&theme.aridRocks)
            {DrawClipped(rect,highlands,dryRockMaterial);return true;}
            if(surface==TerrainKind.Mountain && (theme==null||theme.wallMountain!=MountainAppearance.ThemeTexture))
            {
                var appearance=ChooseMountain(cell.Position);
                if(appearance==MountainAppearance.Sandstone)
                {DrawClipped(rect,hills,dryRockMaterial);return true;}
                if(appearance==MountainAppearance.SnowCapped||appearance==MountainAppearance.SnowCovered)
                    GUI.DrawTexture(rect,appearance==MountainAppearance.SnowCapped?snowCapped:snowCovered,ScaleMode.StretchToFill,true);
                else
                {
                    rockMaterial.SetFloat("_Rock",appearance==MountainAppearance.BareRock?1:0);
                    DrawClipped(rect,appearance==MountainAppearance.BareRock?highlands:hills,rockMaterial);
                    if(appearance==MountainAppearance.Wooded)
                        GUI.DrawTexture(new Rect(rect.x+rect.width*.18f,rect.y+rect.height*.49f,rect.width*.58f,rect.width*.58f*treeCluster.height/treeCluster.width),treeCluster,ScaleMode.StretchToFill,true);
                }
                return true;
            }
            if(river)
            {
                var texture=terrainArt.TryGetValue(surface,out var value)&&value!=null?value:ground;
                GUI.DrawTexture(rect,texture,ScaleMode.StretchToFill,true);return true;
            }
            if(smallLakes.ContainsKey(cell.Position))
            {GUI.DrawTexture(rect,ground,ScaleMode.StretchToFill,true);return true;}
            return false;
        }
        public void DrawWater(CellDefinition cell,Vector2 center,float zoom)
        {
            if(cell.Terrain!=TerrainKind.River&&cell.Terrain!=TerrainKind.Ford)return;
            var ports=new List<int>();int count=hex?6:4;
            // Port order is clockwise in screen coordinates, starting at the right edge.
            foreach(var n in topology.Neighbors(cell.Position))
            {
                if(!cells.TryGetValue(n,out var next)||(next.Terrain!=TerrainKind.River&&next.Terrain!=TerrainKind.Ford))continue;
                var delta=Center(n)-Center(cell.Position);
                if(hex)delta.x*=.8660254f;
                int port=(Mathf.RoundToInt(Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg/(360f/count))+count)%count;
                ports.Add(port);
            }
            ports.Sort();
            if(ports.Count==1)
            {
                if(!outlets.Contains(cell.Position))
                {DrawSegment(riverSpring,center,zoom,(ports[0]-(hex?5:2))*360f/count);return;}
                var exitPorts=new List<int>();
                foreach(var n in topology.Neighbors(cell.Position))
                {
                    if(cells.ContainsKey(n))continue;
                    var delta=Center(n)-Center(cell.Position);if(hex)delta.x*=.8660254f;
                    exitPorts.Add((Mathf.RoundToInt(Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg/(360f/count))+count)%count);
                }
                if(exitPorts.Count>0)
                    ports.Add(exitPorts.OrderByDescending(p=>Mathf.Min((p-ports[0]+count)%count,(ports[0]-p+count)%count)).First());
                else
                {DrawSegment(riverEnd,center,zoom,(ports[0]-(hex?3:2))*360f/count);return;}
            }
            if(ports.Count==2)
            {
                // Native hex straight ports: 2,5; bend: 0,2; end: 3.
                int a=hex?2:0,b=hex?5:2;
                for(int turn=0;turn<count;turn++)
                    if(ports.Contains((a+turn)%count)&&ports.Contains((b+turn)%count))
                    {DrawSegment(riverStraight,center,zoom,turn*360f/count);return;}
                a=hex?0:0;b=hex?2:1;
                for(int turn=0;turn<count;turn++)
                    if(ports.Contains((a+turn)%count)&&ports.Contains((b+turn)%count))
                    {DrawSegment(riverBend,center,zoom,turn*360f/count);return;}
            }
            // No fabricated junctions: the generator supplies a simple supported chain.
        }
        internal MountainAppearance ChooseMountain(CellPosition position) => theme==null?MountainAppearance.BareRock:theme.wallMountain;
        public void DrawLakes(Vector2 pan,float zoom)
        {
            float width=Mathf.Min((theme==null?1.1f:theme.pondWidth)*zoom,pond.width/EditorGUIUtility.pixelsPerPoint);
            float height=width*pond.height/pond.width;
            foreach(var p in smallLakes.Keys)
            {
                var c=Center(p)*zoom+pan;
                GUI.DrawTexture(new Rect(c.x-width*.5f,c.y-height*.5f,width,height),pond,ScaleMode.StretchToFill,true);
            }
        }
        void Mouth(string property,int port,float offset,float width)
        {riverMaterial.SetVector(property,new Vector4(-port*Mathf.PI/3,offset,width/.2f,1));}
        void DrawSegment(Texture2D texture,Vector2 center,float zoom,float degrees)
        {
            riverMaterial.SetFloat("_Angle",degrees*Mathf.Deg2Rad);
            riverMaterial.SetVector("_MouthA",Vector4.zero);riverMaterial.SetVector("_MouthB",Vector4.zero);
            if(hex)
            {
                if(texture==riverStraight){Mouth("_MouthA",2,-.0295f,.195f);Mouth("_MouthB",5,.0295f,.187f);}
                else if(texture==riverBend){Mouth("_MouthA",0,-.0285f,.211f);Mouth("_MouthB",2,-.0295f,.195f);}
                else if(texture==riverSpring)Mouth("_MouthA",5,.0295f,.187f);
                else if(texture==riverEnd)Mouth("_MouthA",3,.0325f,.219f);
            }
            DrawClipped(new Rect(center.x-zoom,center.y-zoom,2*zoom,2*zoom),texture,riverMaterial);
        }
    }
}
