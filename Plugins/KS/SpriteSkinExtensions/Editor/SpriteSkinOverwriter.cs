using UnityEditor;
using UnityEditor.Experimental.U2D;
using UnityEngine;

namespace KS.SpriteSkinExtensions
{
	[CreateAssetMenu(fileName = "spriteOverwriter", menuName = "Sprite Skin Data Overwriter", order = 1)]
	public class SpriteSkinOverwriter : ScriptableObject
	{
		[SerializeField] Sprite mainSprite;
		[SerializeField] Sprite[] spritesToOverwrite;

		public void Overwrite()
		{
			OverwriteMesh();
			OverwriteBones();
		}

		public void OverwriteMesh()
		{
			ISpriteEditorDataProvider mainSpriteDataProvider = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mainSprite)) as ISpriteEditorDataProvider;
			mainSpriteDataProvider.InitSpriteEditorDataProvider();
			ISpriteMeshDataProvider meshDataProvider = mainSpriteDataProvider.GetDataProvider<ISpriteMeshDataProvider>();

			for (int i = 0; i < spritesToOverwrite.Length; i++)
			{
				ISpriteEditorDataProvider spriteDataProvider = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spritesToOverwrite[i])) as ISpriteEditorDataProvider;
				spriteDataProvider.InitSpriteEditorDataProvider();
				ISpriteMeshDataProvider meshDataToOverwrite = spriteDataProvider.GetDataProvider<ISpriteMeshDataProvider>();
				GUID toOver = spritesToOverwrite[i].GetSpriteID();
				GUID mainGuid = mainSprite.GetSpriteID();

				meshDataToOverwrite.SetEdges(toOver, meshDataProvider.GetEdges(mainGuid));
				meshDataToOverwrite.SetIndices(toOver, meshDataProvider.GetIndices(mainGuid));
				meshDataToOverwrite.SetVertices(toOver, meshDataProvider.GetVertices(mainGuid));

				spriteDataProvider.Apply();

				//force SetBindPose
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(spritesToOverwrite[i]));
			}

			Debug.Log("Mesh overwritten");
		}

		public void OverwriteBones()
		{
			ISpriteEditorDataProvider mainSpriteDataProvider = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mainSprite)) as ISpriteEditorDataProvider;
			mainSpriteDataProvider.InitSpriteEditorDataProvider();

			ISpriteBoneDataProvider boneDataProvider = mainSpriteDataProvider.GetDataProvider<ISpriteBoneDataProvider>();
			for (int i = 0; i < spritesToOverwrite.Length; i++)
			{
				ISpriteEditorDataProvider spriteDataProvider = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spritesToOverwrite[i])) as ISpriteEditorDataProvider;
				spriteDataProvider.InitSpriteEditorDataProvider();
				ISpriteBoneDataProvider boneDataToOverwrite = spriteDataProvider.GetDataProvider<ISpriteBoneDataProvider>();
				GUID toOver = spritesToOverwrite[i].GetSpriteID();
				GUID mainGuid = mainSprite.GetSpriteID();

				boneDataToOverwrite.SetBones(toOver, boneDataProvider.GetBones(mainGuid));
				spriteDataProvider.Apply();

				//force SetBindPose
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(spritesToOverwrite[i]));
			}
			Debug.Log("Bones overwritten");
		}
	}

	[CustomEditor(typeof(SpriteSkinOverwriter))]
	public class ObjectBuilderEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			SpriteSkinOverwriter myScript = (SpriteSkinOverwriter)target;
			if (GUILayout.Button("Overwrite All"))
			{
				myScript.Overwrite();
			}

			if (GUILayout.Button("Overwrite Bones"))
			{
				myScript.OverwriteBones();
			}

			if (GUILayout.Button("Overwrite Mesh and Weigths"))
			{
				myScript.OverwriteMesh();
			}
		}
	}
}

