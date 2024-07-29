namespace MDSound
{
	using UnityEngine;

	public class SpeakerLevelDisplay : MonoBehaviour
	{
		public Camera mainCamera; // メインカメラを指定
		public Vector3[] relativeSpeakerPositions; // スピーカーの座標
		public float[] levels; // スピーカーレベル

		public static Color deepGreenColor = new Color (0, 0.15f, 0);
		public static Color darkGreenColor = new Color (0, 0.4f, 0);
		public static Color darkYellowColor = new Color (0.35f, 0.35f, 0);
		public static Color darkBlueColor = new Color (0, 0, 0.25f);
		public static Color skyBlueColor = new Color (0.25f, 0.25f, 0.5f);
		public static Color skyBlue2Color = new Color (0.35f, 0.35f, 0.65f);
		public static Color lightBlueColor = new Color (0.5f, 0.5f, 0.75f);
		public static Color lightlightBlueColor = new Color (0.75f, 0.75f, 0.95f);
		public static Color darkWhiteColor = new Color (0.75f, 0.75f, 0.75f);

		public static Color GetDbColor (float level)
		{
			if (level == -115)
			{
				return Color.black;
			}
			else if (level < -60)
			{
				return darkBlueColor;
			}
			else if (level < -36)
			{
				return skyBlueColor;
			}
			else if (level < -18)
			{
				return skyBlue2Color;
			}
			else if (level < -9)
			{
				return lightBlueColor;
			}
			else if (level < 0)
			{
				return lightlightBlueColor;
			}

			return Color.red;
		}


		private GUIStyle boxStyle;


		void OnGUI ()
		{
			if (relativeSpeakerPositions == null || levels == null || mainCamera == null)
				return;

			if (boxStyle == null)
			{
				// カスタムスタイルを作成
				boxStyle = new GUIStyle (GUI.skin.box);
				boxStyle.normal.background = Texture2D.whiteTexture; // 背景を白に設定
			}

			for (int i = 0; i < relativeSpeakerPositions.Length; i++)
			{
				if (i < levels.Length)
				{
					// カメラの位置と相対位置からワールド座標を計算
					Vector3 worldPosition = mainCamera.transform.position +
					                        mainCamera.transform.rotation * relativeSpeakerPositions[i];

					// ワールド座標をスクリーン座標に変換
					Vector3 screenPosition = mainCamera.WorldToScreenPoint (worldPosition);

					if (screenPosition.z > 0) // カメラの前にある場合のみ表示
					{
						// スクリーン座標をGUI座標に変換
						screenPosition.y = Screen.height - screenPosition.y;

						// レベルに応じた色を決定
						Color boxColor = GetDbColor (levels[i]);

						boxStyle.normal.textColor = boxColor; // スタイルの色を設定


						GUI.color = boxColor;

						// レベルメータの表示
						float levelHeight = 25 + 115 - 67 + Mathf.Clamp (levels[i], -48, 6); // レベルを適切な高さにスケール
						GUI.Box (new Rect (screenPosition.x - 25, screenPosition.y - levelHeight, 50, levelHeight),
							GUIContent.none, boxStyle);


						// GUIの色をリセット
						GUI.color = Color.white;
					}
				}
			}
		}
	}
}