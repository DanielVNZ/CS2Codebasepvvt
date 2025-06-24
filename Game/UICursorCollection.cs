using System;
using System.Collections.Generic;
using cohtml.Net;
using UnityEngine;

namespace Game;

[CreateAssetMenu(menuName = "Colossal/UI/UICursorCollection", order = 1)]
public class UICursorCollection : ScriptableObject
{
	[Serializable]
	public class CursorInfo
	{
		public Texture2D m_Texture;

		public Vector2 m_Hotspot;

		public void Apply()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Cursor.SetCursor(m_Texture, m_Hotspot, (CursorMode)0);
		}
	}

	[Serializable]
	public class NamedCursorInfo : CursorInfo
	{
		public string m_Name;
	}

	public CursorInfo m_Pointer;

	public CursorInfo m_Text;

	public CursorInfo m_Move;

	public NamedCursorInfo[] m_NamedCursors;

	private Dictionary<string, CursorInfo> m_NamedCursorsDict;

	private void OnEnable()
	{
		if (m_NamedCursors == null)
		{
			m_NamedCursors = new NamedCursorInfo[0];
		}
		m_NamedCursorsDict = new Dictionary<string, CursorInfo>();
		RefreshNamedCursorsDict();
	}

	public void SetCursor(Cursors cursor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		if ((int)cursor == 24)
		{
			m_Pointer.Apply();
		}
		else if ((int)cursor == 30)
		{
			m_Text.Apply();
		}
		else if ((int)cursor == 14)
		{
			m_Move.Apply();
		}
		else
		{
			ResetCursor();
		}
	}

	public void SetCursor(string cursorName)
	{
		if (m_NamedCursorsDict.TryGetValue(cursorName, out var value))
		{
			value.Apply();
		}
		else
		{
			ResetCursor();
		}
	}

	public static void ResetCursor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Cursor.SetCursor((Texture2D)null, Vector2.zero, (CursorMode)0);
	}

	private void RefreshNamedCursorsDict()
	{
		m_NamedCursorsDict.Clear();
		NamedCursorInfo[] namedCursors = m_NamedCursors;
		foreach (NamedCursorInfo namedCursorInfo in namedCursors)
		{
			m_NamedCursorsDict["cursor://" + namedCursorInfo.m_Name] = namedCursorInfo;
		}
	}
}
