using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if ADX2
using CriWare;
#endif
using MyDearest;
using UnityEditor.PackageManager.UI;

namespace MDSound
{
	/// <summary>
	/// MDSSound(ADX2)のデータを簡易再生、確認するためのEditor拡張
	/// </summary>
	public class Mds714Monitor : EditorWindow
	{
		private static int m_maxcannnels = 16;
		private static float m_viewSize = 0.25f;

		[MenuItem ("Window/MDSound/Mds714Monitor")]
		public static void ShowWindow () => GetWindow (typeof(Mds714Monitor), false, "Mds714Monitor");

		private bool isVisualize = false;

		private SpeakerLevelDisplay SpeakerLevelDisplay;

		private GameObject speakerLevelDisplayObj;

		private void OnGUI ()
		{
			m_updateFrame = EditorGUILayout.IntSlider ("更新フレーム数", (int)m_updateFrame, 1, 60);
			m_maxcannnels = EditorGUILayout.IntSlider ("最大表示チャンネル数", m_maxcannnels, 1, 16);
			m_viewSize = EditorGUILayout.Slider ("UI表示サイズ", m_viewSize, 0.1f, 6f);
			if (GUI.changed)
			{
				if (speakerLevelDisplayObj != null)
				{
					SpeakerLevelDisplay.CreateSpeakers (m_maxcannnels, m_viewSize);
				}
			}


			if (EditorApplication.isPlaying)
			{
				if (GUILayout.Button ("ビジュアライズ " + isVisualize.ToString (), GUILayout.Width (200),
					    GUILayout.Height (100)))
				{
					if (isVisualize == false)
					{
						isVisualize = true;

						speakerLevelDisplayObj = new GameObject ("Visuzlize714");
						SpeakerLevelDisplay = speakerLevelDisplayObj.AddComponent<SpeakerLevelDisplay> ();

						SpeakerLevelDisplay.CreateSpeakers (m_maxcannnels, m_viewSize);
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
		}


		/// <summary>
		/// 最後に更新した時刻
		/// </summary>
		private static double m_updateLastTime = 0;

		/// <summary>
		/// 更新フレーム数
		/// </summary>
		private double m_updateFrame = 1;

		/// <summary>
		/// Update
		/// </summary>
		private void Update ()
		{
			if (!EditorApplication.isPlaying || !SoundManager.HasInstance)
			{
				return;
			}

			if ((EditorApplication.timeSinceStartup - m_updateLastTime) >= 0.01666f * m_updateFrame)
			{
				Repaint ();
				m_updateLastTime = EditorApplication.timeSinceStartup;
			}

			if (isVisualize)
			{
				if (SpeakerLevelDisplay == null)
				{
					return;
				}

				if (SoundManager.Data.AdxAcfData == null)
				{
					return;
				}


				string[] busNames = SoundManager.Data.AdxAcfData.acfData.busNames.ToArray ();
				var busName = busNames[0];

				for (int channel = 0; channel < m_maxcannnels; channel++)
				{
					SpeakerLevelDisplay.levels[channel] = -115;
				}

				for (int channel = 0; channel < m_maxcannnels; channel++)
				{
					float level = MdsEditorUtil.GetDb (CriAtom.GetBusAnalyzerInfo (busName).peakLevels[channel]);
					float peakHoldLevel =
						MdsEditorUtil.GetDb (CriAtom.GetBusAnalyzerInfo (busName).peakHoldLevels[channel]);
					SpeakerLevelDisplay.levels[channel] = level;
				}


				// L, R, C, LFE, Ls, Rs, Lb, Rb, Ltf, Rtf, Ltb, Rtb
				// 0  1  2  3    4   5   6   7   8    9    10   11

				// L R C LFE Ls Rs LTF RTF
				// 0 1 2 3   4  5  8   9
			}
		}
	}
}
