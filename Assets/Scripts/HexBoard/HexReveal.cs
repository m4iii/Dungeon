using System.Collections;
using UnityEngine;

namespace Dungeon.HexBoard
{
    [ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(HexCell))]
    public sealed class HexReveal : MonoBehaviour
    {
        public bool explored;
        public bool hasMonster;
        [Min(.1f)] public float duration = .45f;
        [Tooltip("How far the parchment frontier reaches into explored terrain, in tile units.")]
        [Range(0f,.65f)] public float frontierWidth = .32f;
        public Material backMaterial;
        public Material monsterMaterial;
        public SpriteRenderer panel, underlay, monster;
        public bool IsRevealing { get; private set; }
        Coroutine animation;
        MaterialPropertyBlock paperProperties;
        bool refreshRequested;

        void UpdatePaperCoordinates()
        {
            if(panel==null||underlay==null)return;
            if(paperProperties==null)paperProperties=new MaterialPropertyBlock();
            var axial=GetComponent<HexCell>().axial;
            var anchor=HexGeometry.Position(axial.x,axial.y);
            var directions=new[]{new Vector2Int(-1,1),new Vector2Int(-1,0),new Vector2Int(0,-1),new Vector2Int(1,-1),new Vector2Int(1,0),new Vector2Int(0,1)};
            var edges=new float[6];
            foreach(var other in Object.FindObjectsByType<HexReveal>(FindObjectsSortMode.None))
            {
                if(other.explored==explored||other.transform.parent!=transform.parent)continue;
                var delta=other.GetComponent<HexCell>().axial-axial;
                for(int j=0;j<6;j++)if(delta==directions[j])edges[j]=1;
            }
            // Use the resting grid coordinates, so neighboring fragments join and
            // a fragment stays attached to its panel throughout the animation.
            foreach(var renderer in new[]{panel,underlay})
            {
                renderer.GetPropertyBlock(paperProperties);
                paperProperties.SetVector("_MapOrigin",new Vector4(anchor.x,anchor.y,0,0));
                paperProperties.SetVector("_RevealEdgesA",new Vector4(edges[0],edges[1],edges[2],edges[3]));
                paperProperties.SetVector("_RevealEdgesB",new Vector4(edges[4],edges[5],0,0));
                paperProperties.SetFloat("_Explored",explored?1:0);
                paperProperties.SetFloat("_FrontierWidth",frontierWidth);
                renderer.SetPropertyBlock(paperProperties);
            }
        }

        void OnEnable() { ApplyState(); }
        void OnValidate() { refreshRequested=true; }
        void Update()
        {
            if(!refreshRequested||IsRevealing)return;
            refreshRequested=false;
            ApplyState();
        }
        void OnDisable()
        {
            if (animation != null) StopCoroutine(animation);
            animation = null; IsRevealing = false;
            ApplyState();
        }
        public void Setup()
        {
            var top=GetComponent<SpriteRenderer>();
            panel=Make("Flip panel",panel,top.sprite,backMaterial,3);
            underlay=Make("Recess under panel",underlay,top.sprite,backMaterial,0);
            underlay.color=new Color(.4f,.4f,.4f,1);
            monster=Make("Monster token",monster,top.sprite,monsterMaterial,6);
            monster.transform.localScale=Vector3.one*.32f;
            monster.transform.localPosition=new Vector3(0,-.15f,0);
            if(monster.GetComponent<TerrainPropSorting>()==null)monster.gameObject.AddComponent<TerrainPropSorting>();
            ApplyState();
        }
        SpriteRenderer Make(string label,SpriteRenderer current,Sprite sprite,Material material,int order)
        {
            if(current==null)
            {
                var obj=new GameObject(label);obj.transform.SetParent(transform,false);
                current=obj.AddComponent<SpriteRenderer>();
            }
            current.sprite=sprite;current.sharedMaterial=material;current.sortingOrder=order;
            return current;
        }
        public void ApplyState()
        {
            if(panel==null||underlay==null||monster==null)return;
            UpdatePaperCoordinates();
            GetComponent<SpriteRenderer>().enabled=explored;
            // On revealed tiles the same panel draws only the irregular frontier.
            // The terrain remains underneath; unknown tile contents are never sampled.
            panel.enabled=true;panel.sharedMaterial=backMaterial;
            panel.transform.localPosition=Vector3.zero;panel.transform.localScale=Vector3.one;
            panel.transform.localRotation=Quaternion.identity;
            underlay.enabled=false;
            SetDetails(explored,1);
            foreach(var other in Object.FindObjectsByType<HexReveal>(FindObjectsSortMode.None))
                if(other!=this&&other.transform.parent==transform.parent)other.UpdatePaperCoordinates();
        }
        void SetDetails(bool visible,float alpha)
        {
            var props=transform.Find("Terrain decorations");
            if(props!=null)
            {
                props.gameObject.SetActive(visible);
                foreach(var r in props.GetComponentsInChildren<SpriteRenderer>(true))
                { var c=r.color;c.a=alpha;r.color=c; }
            }
            var terrain=GetComponent<HexCell>().terrainRenderer;
            if(terrain!=null)terrain.enabled=visible;
            monster.enabled=visible&&hasMonster;
            monster.color=new Color(1,1,1,alpha);
        }
        public bool Reveal()
        {
            if(explored||IsRevealing||!isActiveAndEnabled)return false;
            if(!Application.isPlaying){explored=true;ApplyState();return true;}
            animation=StartCoroutine(Flip());return true;
        }
        IEnumerator Flip()
        {
            IsRevealing=true;
            float elapsed=0;
            while(elapsed<duration)
            {
                EvaluateFrame(elapsed/Mathf.Max(.1f,duration));
                elapsed+=Time.unscaledDeltaTime;yield return null;
            }
            explored=true;IsRevealing=false;animation=null;ApplyState();
        }
        // Also used by the editor preview to inspect the actual animation frames.
        public void EvaluateFrame(float progress)
        {
            if(panel==null)return;
            float t=Mathf.Clamp01(progress);
            float eased=t*t*(3-2*t);
            var top=GetComponent<SpriteRenderer>();top.enabled=false;
            underlay.enabled=true;panel.enabled=true;
            panel.sharedMaterial=t<.5f?backMaterial:top.sharedMaterial;
            panel.transform.localScale=new Vector3(Mathf.Max(.015f,Mathf.Abs(Mathf.Cos(eased*Mathf.PI))),1,1);
            panel.transform.localPosition=Vector3.up*(Mathf.Sin(t*Mathf.PI)*.18f);
            panel.transform.localRotation=Quaternion.Euler(0,0,Mathf.Sin(t*Mathf.PI*2)*3);
            SetDetails(t>.82f,Mathf.InverseLerp(.82f,1,t));
        }
        [ContextMenu("Reset exploration")]
        public void ResetExploration()
        {
            if(animation!=null)StopCoroutine(animation);
            animation=null;IsRevealing=false;explored=false;ApplyState();
        }
        [ContextMenu("Clear monster")]
        public void ClearMonster(){hasMonster=false;if(monster!=null)monster.enabled=false;}
    }
}
