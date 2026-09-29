using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Dungeon.HexBoard.Editor
{
    public static class HexRevealTrialBuilder
    {
        public static void Create()
        {
            const string path="Assets/Scenes/HexRevealTrial.unity";
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(path)!=null)
                throw new System.InvalidOperationException("Reveal trial already exists; open it to preserve changes.");
            EditorSceneManager.SaveOpenScenes();
            var source=EditorSceneManager.OpenScene("Assets/Scenes/HexTerrainTrial.unity");
            EditorSceneManager.SaveScene(source,path,true);
            var scene=EditorSceneManager.OpenScene(path);
            var back=new Material(Shader.Find("Dungeon/Hex Card Back"));
            AssetDatabase.CreateAsset(back,"Assets/HexBoard/Art/HexCardBack.mat");
            var token=new Material(back);token.SetFloat("_Monster",1);
            AssetDatabase.CreateAsset(token,"Assets/HexBoard/Art/HexMonsterToken.mat");
            // Keep existing boards explored; the reusable prefab gains opt-in reveal support.
            const string prefabPath="Assets/HexBoard/Prefabs/BaseHexCell.prefab";
            var prefab=PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                var reveal=prefab.AddComponent<HexReveal>();
                reveal.backMaterial=back;reveal.monsterMaterial=token;reveal.explored=true;
                reveal.Setup();
                PrefabUtility.SaveAsPrefabAsset(prefab,prefabPath);
            }
            finally{PrefabUtility.UnloadPrefabContents(prefab);}
            foreach(var cell in Object.FindObjectsByType<HexCell>())
            {
                var reveal=cell.GetComponent<HexReveal>();
                var a=cell.axial;
                reveal.hasMonster=(a.x==-1&&a.y==0)||(a.x==1&&a.y==0)||(a.x==0&&a.y==2);
                reveal.explored=a.y<0&&a.x>=0;
                reveal.Setup();
                cell.GetComponent<HexCellSorting>().Refresh();
                PrefabUtility.RecordPrefabInstancePropertyModifications(reveal);
                foreach(var r in cell.GetComponentsInChildren<SpriteRenderer>(true))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
            var board=GameObject.Find("A - Modular terrain (shared base)");
            var input=board.AddComponent<HexBoardInput>();input.boardCamera=Camera.main;
            input.minimumViewSize=6.2f;input.horizontalViewSize=11;
            foreach(var label in Object.FindObjectsByType<TextMesh>())
            {
                if(label.text.StartsWith("TERRAIN"))label.text="EXPLORE / FLIP TO REVEAL";
                else if(label.text.StartsWith("A "))label.text="A   CLICK TO EXPLORE";
                else if(label.text.StartsWith("19 "))label.text="Play, then click a covered tile\nHidden encounters appear after the flip";
            }
            AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject=board;
        }
    }
}
