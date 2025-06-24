using System;
using System.Collections.Generic;
using System.Linq;
using Colossal;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Game.Input;

public abstract class ValueInputBindingComposite<T> : InputBindingComposite<T>, ICustomComposite where T : struct
{
	public bool m_IsRebindable = true;

	public bool m_IsModifiersRebindable = true;

	public bool m_AllowModifiers = true;

	public bool m_CanBeEmpty = true;

	public bool m_DeveloperOnly;

	public Platform m_Platform = (Platform)255;

	public bool m_BuiltIn = true;

	public bool m_IsDummy;

	public bool m_IsHidden;

	public OptionGroupOverride m_OptionGroupOverride;

	public BuiltInUsages m_Usages = BuiltInUsages.DefaultTool | BuiltInUsages.Overlay | BuiltInUsages.Tool | BuiltInUsages.CancelableTool;

	public long m_LinkGuid1;

	public long m_LinkGuid2;

	public bool isRebindable => m_IsRebindable;

	public bool isModifiersRebindable => m_IsModifiersRebindable;

	public bool allowModifiers => m_AllowModifiers;

	public bool canBeEmpty => m_CanBeEmpty;

	public bool developerOnly => m_DeveloperOnly;

	public Platform platform => m_Platform;

	public bool builtIn => m_BuiltIn;

	public bool isDummy => m_IsDummy;

	public bool isHidden => m_IsHidden;

	public OptionGroupOverride optionGroupOverride => m_OptionGroupOverride;

	public Guid linkedGuid
	{
		get
		{
			return CompositeUtility.GetGuid(m_LinkGuid1, m_LinkGuid2);
		}
		set
		{
			CompositeUtility.SetGuid(value, out m_LinkGuid1, out m_LinkGuid2);
		}
	}

	public virtual NameAndParameters parameters
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			NameAndParameters result = default(NameAndParameters);
			((NameAndParameters)(ref result)).name = CompositeUtility.GetCompositeTypeName(((object)this).GetType());
			((NameAndParameters)(ref result)).parameters = new ReadOnlyArray<NamedValue>(GetParameters().ToArray());
			return result;
		}
	}

	public Usages usages => new Usages(m_Usages);

	protected virtual IEnumerable<NamedValue> GetParameters()
	{
		yield return NamedValue.From<bool>("m_IsRebindable", m_IsRebindable);
		yield return NamedValue.From<bool>("m_IsModifiersRebindable", m_IsModifiersRebindable);
		yield return NamedValue.From<bool>("m_AllowModifiers", m_AllowModifiers);
		yield return NamedValue.From<bool>("m_CanBeEmpty", m_CanBeEmpty);
		yield return NamedValue.From<bool>("m_DeveloperOnly", m_DeveloperOnly);
		yield return NamedValue.From<bool>("m_BuiltIn", m_BuiltIn);
		yield return NamedValue.From<bool>("m_IsDummy", m_IsDummy);
		yield return NamedValue.From<bool>("m_IsHidden", m_IsHidden);
		yield return NamedValue.From<OptionGroupOverride>("m_OptionGroupOverride", m_OptionGroupOverride);
	}
}
