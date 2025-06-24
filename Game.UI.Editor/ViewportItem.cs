using Colossal.UI.Binding;

namespace Game.UI.Editor;

public struct ViewportItem<T> : IJsonWritable
{
	public HierarchyItem<T> m_Item;

	public int m_ItemIndex;

	public void Write(IJsonWriter writer)
	{
		JsonWriterExtensions.Write<HierarchyItem<T>>(writer, m_Item);
	}
}
