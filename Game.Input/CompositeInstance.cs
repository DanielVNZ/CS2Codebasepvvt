using System;
using System.Collections.Generic;
using Colossal;
using UnityEngine.InputSystem.Utilities;

namespace Game.Input;

public class CompositeInstance : ICustomComposite
{
	private bool m_IsRebindable = true;

	private bool m_IsModifiersRebindable = true;

	private bool m_AllowModifiers = true;

	private bool m_CanBeEmpty = true;

	private bool m_DeveloperOnly;

	private Platform m_Platform = (Platform)255;

	private bool m_BuiltIn = true;

	private bool m_IsDummy;

	private bool m_IsHidden;

	public OptionGroupOverride m_OptionGroupOverride;

	private Usages m_Usages = ICustomComposite.defaultUsages;

	public string typeName { get; internal set; }

	public bool isRebindable
	{
		get
		{
			if (isDummy)
			{
				return true;
			}
			return m_IsRebindable;
		}
		set
		{
			m_IsRebindable = value;
		}
	}

	public bool isModifiersRebindable
	{
		get
		{
			if (isDummy)
			{
				return true;
			}
			if (m_IsModifiersRebindable && isRebindable)
			{
				return allowModifiers;
			}
			return false;
		}
		set
		{
			m_IsModifiersRebindable = value;
		}
	}

	public bool allowModifiers
	{
		get
		{
			if (isDummy)
			{
				return true;
			}
			return m_AllowModifiers;
		}
		set
		{
			m_AllowModifiers = value;
		}
	}

	public bool canBeEmpty
	{
		get
		{
			if (isDummy)
			{
				return true;
			}
			if (m_CanBeEmpty)
			{
				return isRebindable;
			}
			return false;
		}
		set
		{
			m_CanBeEmpty = value;
		}
	}

	public bool developerOnly
	{
		get
		{
			return m_DeveloperOnly;
		}
		set
		{
			m_DeveloperOnly = value;
		}
	}

	public Platform platform
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Platform;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Platform = value;
		}
	}

	public bool builtIn
	{
		get
		{
			return m_BuiltIn;
		}
		set
		{
			m_BuiltIn = value;
		}
	}

	public bool isDummy
	{
		get
		{
			return m_IsDummy;
		}
		set
		{
			m_IsDummy = value;
		}
	}

	public bool isHidden
	{
		get
		{
			return m_IsHidden;
		}
		set
		{
			m_IsHidden = value;
		}
	}

	public OptionGroupOverride optionGroupOverride
	{
		get
		{
			if (isHidden)
			{
				return OptionGroupOverride.None;
			}
			return m_OptionGroupOverride;
		}
		set
		{
			m_OptionGroupOverride = value;
		}
	}

	public Usages usages
	{
		get
		{
			return m_Usages.Copy();
		}
		set
		{
			m_Usages.SetFrom(value);
		}
	}

	public Guid linkedGuid { get; set; }

	public List<NameAndParameters> processors { get; } = new List<NameAndParameters>();

	public List<NameAndParameters> interactions { get; } = new List<NameAndParameters>();

	public Mode mode { get; set; }

	public InputManager.CompositeData compositeData
	{
		get
		{
			if (!InputManager.TryGetCompositeData(typeName, out var data))
			{
				return default(InputManager.CompositeData);
			}
			return data;
		}
	}

	public NameAndParameters parameters
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			CompositeUtility.SetGuid(linkedGuid, out var part, out var part2);
			NameAndParameters result = default(NameAndParameters);
			((NameAndParameters)(ref result)).name = typeName;
			((NameAndParameters)(ref result)).parameters = new ReadOnlyArray<NamedValue>((NamedValue[])(object)new NamedValue[13]
			{
				NamedValue.From<bool>("m_IsRebindable", isRebindable),
				NamedValue.From<bool>("m_IsModifiersRebindable", isModifiersRebindable),
				NamedValue.From<bool>("m_AllowModifiers", allowModifiers),
				NamedValue.From<bool>("m_CanBeEmpty", canBeEmpty),
				NamedValue.From<bool>("m_DeveloperOnly", developerOnly),
				NamedValue.From<Platform>("m_Platform", platform),
				NamedValue.From<bool>("m_BuiltIn", builtIn),
				NamedValue.From<Mode>("m_Mode", mode),
				NamedValue.From<bool>("m_IsDummy", isDummy),
				NamedValue.From<bool>("m_IsHidden", isHidden),
				NamedValue.From<OptionGroupOverride>("m_OptionGroupOverride", optionGroupOverride),
				NamedValue.From<long>("m_LinkGuid1", part),
				NamedValue.From<long>("m_LinkGuid2", part2)
			});
			return result;
		}
		set
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			long part = 0L;
			long part2 = 0L;
			Enumerator<NamedValue> enumerator = ((NameAndParameters)(ref value)).parameters.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NamedValue current = enumerator.Current;
					PrimitiveValue value2;
					switch (((NamedValue)(ref current)).name)
					{
					case "m_IsRebindable":
						value2 = ((NamedValue)(ref current)).value;
						isRebindable = ((PrimitiveValue)(ref value2)).ToBoolean((IFormatProvider)null);
						break;
					case "m_IsModifiersRebindable":
						value2 = ((NamedValue)(ref current)).value;
						isModifiersRebindable = ((PrimitiveValue)(ref value2)).ToBoolean((IFormatProvider)null);
						break;
					case "m_AllowModifiers":
						value2 = ((NamedValue)(ref current)).value;
						allowModifiers = ((PrimitiveValue)(ref value2)).ToBoolean((IFormatProvider)null);
						break;
					case "m_CanBeEmpty":
						value2 = ((NamedValue)(ref current)).value;
						canBeEmpty = ((PrimitiveValue)(ref value2)).ToBoolean((IFormatProvider)null);
						break;
					case "m_DeveloperOnly":
						value2 = ((NamedValue)(ref current)).value;
						developerOnly = ((PrimitiveValue)(ref value2)).ToBoolean((IFormatProvider)null);
						break;
					case "m_Platform":
						value2 = ((NamedValue)(ref current)).value;
						platform = (Platform)((PrimitiveValue)(ref value2)).ToInt32((IFormatProvider)null);
						break;
					case "m_BuiltIn":
						value2 = ((NamedValue)(ref current)).value;
						builtIn = ((PrimitiveValue)(ref value2)).ToBoolean((IFormatProvider)null);
						break;
					case "m_Mode":
						value2 = ((NamedValue)(ref current)).value;
						mode = (Mode)((PrimitiveValue)(ref value2)).ToInt32((IFormatProvider)null);
						break;
					case "m_Usages":
						value2 = ((NamedValue)(ref current)).value;
						m_Usages = new Usages((BuiltInUsages)((PrimitiveValue)(ref value2)).ToInt32((IFormatProvider)null));
						break;
					case "m_IsDummy":
						value2 = ((NamedValue)(ref current)).value;
						isDummy = ((PrimitiveValue)(ref value2)).ToBoolean((IFormatProvider)null);
						break;
					case "m_IsHidden":
						value2 = ((NamedValue)(ref current)).value;
						isHidden = ((PrimitiveValue)(ref value2)).ToBoolean((IFormatProvider)null);
						break;
					case "m_OptionGroupOverride":
						value2 = ((NamedValue)(ref current)).value;
						optionGroupOverride = (OptionGroupOverride)((PrimitiveValue)(ref value2)).ToInt32((IFormatProvider)null);
						break;
					case "m_LinkGuid1":
						value2 = ((NamedValue)(ref current)).value;
						part = ((PrimitiveValue)(ref value2)).ToInt64((IFormatProvider)null);
						break;
					case "m_LinkGuid2":
						value2 = ((NamedValue)(ref current)).value;
						part2 = ((PrimitiveValue)(ref value2)).ToInt64((IFormatProvider)null);
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			linkedGuid = CompositeUtility.GetGuid(part, part2);
		}
	}

	public CompositeInstance(string typeName)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		this.typeName = typeName;
	}

	public CompositeInstance(NameAndParameters parameters)
		: this(((NameAndParameters)(ref parameters)).name)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		m_Usages = ICustomComposite.defaultUsages;
		this.parameters = parameters;
		m_Usages.MakeReadOnly();
	}

	public CompositeInstance(NameAndParameters parameters, NameAndParameters usages)
		: this(((NameAndParameters)(ref parameters)).name)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		m_Usages = new Usages(0, readOnly: false);
		this.parameters = parameters;
		m_Usages.parameters = usages;
		m_Usages.MakeReadOnly();
	}
}
