using UnityEditor;
using UnityEngine;
using CriWare;

namespace MDSound
{
	/// <summary>
	/// MDSSound(ADX2)のデータを簡易再生、確認するためのEditor拡張
	/// </summary>
	public class Mds714Monitor : EditorWindow
	{
		private static int m_maxcannnels = 16;

		[MenuItem ("Window/MDSound/Mds714Monitor")]
		public static void ShowWindow () => GetWindow (typeof(Mds714Monitor), false, "Mds714Monitor");

		private bool isVisualize = false;

		private SpeakerLevelDisplay SpeakerLevelDisplay;

		private GameObject speakerLevelDisplayObj;

		private void OnGUI ()
		{

			if (GUILayout.Button ("ビジュアライズ " + isVisualize.ToString (), GUILayout.Width (200), GUILayout.Height (100)))
			{
				if (isVisualize == false)
				{
					isVisualize = true;

					speakerLevelDisplayObj = new GameObject ("Visuzlize714");
					SpeakerLevelDisplay = speakerLevelDisplayObj.AddComponent<SpeakerLevelDisplay> ();

					SpeakerLevelDisplay.mainCamera = Camera.main;

					float z = 3;
					float y = 0.0f;
					float size = 0.25f;

					// 7.1.4chスピーカーの座標を設定
					SpeakerLevelDisplay.relativeSpeakerPositions = new Vector3[]
					{
						new Vector3 (-3 * size, 1 * size+y, 3 * size + z), // フロント左
						new Vector3 (3 * size, 1 * size+y, 3 * size + z), // フロント右
						new Vector3 (0 * size, 1 * size+y, 3 * size + z), // フロントセンター
						new Vector3 (1f * size, 0 * size+y, 3 * size + z), // サブウーファー
						new Vector3 (-3 * size, 1 * size+y, 0 * size + z), // サイド左
						new Vector3 (3 * size, 1 * size+y, 0 * size + z), // サイド右
						new Vector3 (-3 * size, 1 * size+y, -3 * size + z), // リア左
						new Vector3 (3 * size, 1 * size+y, -3 * size + z), // リア右
						new Vector3 (-3 * size, 2.5f * size+y, 3 * size + z), // フロントトップ左
						new Vector3 (3 * size, 2.5f * size+y, 3 * size + z), // フロントトップ右
						new Vector3 (-3 * size, 2.5f * size+y, -3 * size + z), // リアトップ左
						new Vector3 (3 * size, 2.5f * size+y, -3 * size + z), // リアトップ右
						new Vector3 (-3 * size, -0.5f * size+y, 3 * size + z), // フロントボトム左
						new Vector3 (3 * size, -0.5f * size+y, 3 * size + z), // フロントボトム右
						new Vector3 (-3 * size, -0.5f * size+y, -3 * size + z), // リアボトム左
						new Vector3 (3 * size, -0.5f * size+y, -3 * size + z) // リアボトム右
					};

					SpeakerLevelDisplay.levels = new float[12+4]; // レベルを初期化
				}
				else
				{
					if (speakerLevelDisplayObj != null)
					{
						Destroy (speakerLevelDisplayObj);
						speakerLevelDisplayObj = null;
					}

					isVisualize = false;
				}
			}
		}

		/// <summary>
		///　Volume値からdB値を得る
		/// </summary>
		/// <param name="volume"></param>
		/// <returns></returns>
		public static float GetDb (float volume)
		{
			float retValue = Mathf.Floor (20.0f * Mathf.Log10 (volume));
			if (retValue < -115)
			{
				retValue = -115;
			}

			return retValue;
		}

		/// <summary>
		/// Update
		/// </summary>
		private void Update ()
		{
			if (!EditorApplication.isPlaying )
			{
				return;
			}

			if (isVisualize)
			{
				if (SpeakerLevelDisplay == null)
				{
					return;
				}

				var busName = "MasterOut";

				for (int channel = 0; channel < m_maxcannnels; channel++)
				{
					float level = GetDb (CriAtom.GetBusAnalyzerInfo (busName).peakLevels[channel]);
					SpeakerLevelDisplay.levels[channel] = level;
				}
			}
		}
	}
}
