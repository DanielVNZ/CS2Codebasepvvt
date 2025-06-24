using System;
using Colossal.Json;
using Colossal.UI.Binding;
using UnityEngine;

namespace Game.Settings;

public struct ScreenResolution : IEquatable<ScreenResolution>, IComparable<ScreenResolution>, IJsonReadable, IJsonWritable
{
	public int width;

	public int height;

	public RefreshRate refreshRate;

	public double refreshRateDelta => Math.Abs(Math.Round(((RefreshRate)(ref refreshRate)).value) - ((RefreshRate)(ref refreshRate)).value);

	public bool isValid
	{
		get
		{
			if (width > 0 && height > 0 && refreshRate.numerator != 0)
			{
				return refreshRate.denominator != 0;
			}
			return false;
		}
	}

	private static void SupportValueTypesForAOT()
	{
		JSON.SupportTypeForAOT<ScreenResolution>();
		JSON.SupportTypeForAOT<RefreshRate>();
	}

	public ScreenResolution(Resolution resolution)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		width = ((Resolution)(ref resolution)).width;
		height = ((Resolution)(ref resolution)).height;
		refreshRate = ((Resolution)(ref resolution)).refreshRateRatio;
	}

	public bool Equals(ScreenResolution other)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		int num = width;
		int num2 = height;
		uint numerator = refreshRate.numerator;
		uint denominator = refreshRate.denominator;
		int num3 = other.width;
		int num4 = other.height;
		uint numerator2 = other.refreshRate.numerator;
		uint denominator2 = other.refreshRate.denominator;
		if (num == num3 && num2 == num4 && numerator == numerator2)
		{
			return denominator == denominator2;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is ScreenResolution other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return (width, height, refreshRate).GetHashCode();
	}

	public static bool operator ==(ScreenResolution left, ScreenResolution right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ScreenResolution left, ScreenResolution right)
	{
		return !left.Equals(right);
	}

	public void Sanitize()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (refreshRate.numerator == 0 || refreshRate.denominator == 0 || double.IsNaN(((RefreshRate)(ref refreshRate)).value))
		{
			Resolution currentResolution = Screen.currentResolution;
			refreshRate = ((Resolution)(ref currentResolution)).refreshRateRatio;
		}
	}

	public int CompareTo(ScreenResolution other)
	{
		int num = width.CompareTo(other.width);
		if (num != 0)
		{
			return num;
		}
		int num2 = height.CompareTo(other.height);
		if (num2 != 0)
		{
			return num2;
		}
		return ((RefreshRate)(ref refreshRate)).value.CompareTo(((RefreshRate)(ref other.refreshRate)).value);
	}

	public void Read(IJsonReader reader)
	{
		reader.ReadMapBegin();
		reader.ReadProperty("width");
		reader.Read(ref width);
		reader.ReadProperty("height");
		reader.Read(ref height);
		reader.ReadProperty("numerator");
		reader.Read(ref refreshRate.numerator);
		reader.ReadProperty("denominator");
		reader.Read(ref refreshRate.denominator);
		reader.ReadMapEnd();
	}

	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(typeof(ScreenResolution).FullName);
		writer.PropertyName("width");
		writer.Write(width);
		writer.PropertyName("height");
		writer.Write(height);
		writer.PropertyName("numerator");
		writer.Write(refreshRate.numerator);
		writer.PropertyName("denominator");
		writer.Write(refreshRate.denominator);
		writer.TypeEnd();
	}

	public override string ToString()
	{
		return $"{width}x{height}x{((RefreshRate)(ref refreshRate)).value}Hz";
	}
}
