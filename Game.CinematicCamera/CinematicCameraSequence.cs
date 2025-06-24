using System;
using System.Collections.Generic;
using Colossal.Json;
using Colossal.UI.Binding;
using Game.Rendering;
using Game.Rendering.CinematicCamera;
using UnityEngine;

namespace Game.CinematicCamera;

public class CinematicCameraSequence : IJsonWritable, IJsonReadable
{
	public enum TransformCurveKey
	{
		PositionX,
		PositionY,
		PositionZ,
		RotationX,
		RotationY,
		Count
	}

	public struct CinematicCameraCurveModifier : IJsonWritable, IJsonReadable
	{
		public string id { get; set; }

		public AnimationCurve curve { get; set; }

		public float min { get; set; }

		public float max { get; set; }

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("id");
			writer.Write(id);
			if (curve != null)
			{
				writer.PropertyName("curve");
				UnityWriters.Write(writer, curve);
			}
			writer.PropertyName("min");
			writer.Write(min);
			writer.PropertyName("max");
			writer.Write(max);
			writer.TypeEnd();
		}

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("id");
			string text = default(string);
			reader.Read(ref text);
			id = text;
			reader.ReadProperty("curve");
			AnimationCurve val = default(AnimationCurve);
			UnityReaders.Read(reader, ref val);
			curve = val;
			reader.ReadProperty("min");
			float num = default(float);
			reader.Read(ref num);
			min = num;
			reader.ReadProperty("max");
			float num2 = default(float);
			reader.Read(ref num2);
			max = num2;
			reader.ReadMapEnd();
		}

		public int AddKey(float t, float value)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			//IL_0015: Expected O, but got Unknown
			if (curve == null)
			{
				AnimationCurve val = new AnimationCurve();
				AnimationCurve val2 = val;
				curve = val;
			}
			return curve.AddKey(t, value);
		}
	}

	private bool m_Loop;

	public List<CinematicCameraCurveModifier> modifiers { get; set; } = new List<CinematicCameraCurveModifier>();

	public CinematicCameraCurveModifier[] transforms { get; set; } = new CinematicCameraCurveModifier[5];

	public float playbackDuration { get; set; } = 30f;

	public bool loop
	{
		get
		{
			return m_Loop;
		}
		set
		{
			if (m_Loop != value)
			{
				m_Loop = value;
				if (value)
				{
					AfterModifications(rotationsChanged: true);
				}
			}
		}
	}

	public float timelineLength
	{
		get
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			Keyframe val;
			for (int i = 0; i < transforms.Length; i++)
			{
				if (transforms[i].curve.length > 0)
				{
					float num2 = num;
					val = transforms[i].curve[transforms[i].curve.length - 1];
					num = Mathf.Max(num2, ((Keyframe)(ref val)).time);
				}
			}
			for (int j = 0; j < modifiers.Count; j++)
			{
				if (modifiers[j].curve.length > 0)
				{
					float num3 = num;
					val = modifiers[j].curve[modifiers[j].curve.length - 1];
					num = Mathf.Max(num3, ((Keyframe)(ref val)).time);
				}
			}
			return num;
		}
	}

	public int transformCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < transforms.Length; i++)
			{
				if (transforms[i].curve != null)
				{
					num = Mathf.Max(num, transforms[i].curve.length);
				}
			}
			return num;
		}
	}

	public CinematicCameraSequence()
	{
		Reset();
	}

	public void Reset()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		modifiers.Clear();
		for (int i = 0; i < transforms.Length; i++)
		{
			CinematicCameraCurveModifier[] array = transforms;
			int num = i;
			CinematicCameraCurveModifier cinematicCameraCurveModifier = default(CinematicCameraCurveModifier);
			TransformCurveKey transformCurveKey = (TransformCurveKey)i;
			cinematicCameraCurveModifier.id = transformCurveKey.ToString();
			cinematicCameraCurveModifier.curve = new AnimationCurve();
			array[num] = cinematicCameraCurveModifier;
		}
	}

	public void RemoveModifier(string id)
	{
		int num = modifiers.FindIndex((CinematicCameraCurveModifier m) => m.id == id);
		if (num >= 0)
		{
			modifiers.RemoveAt(num);
		}
	}

	public bool SampleTransform(IGameCameraController controller, float t, out Vector3 position, out Vector3 rotation)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (transformCount == 0)
		{
			position = Vector3.zero;
			rotation = Vector3.zero;
			return false;
		}
		position = controller.position;
		rotation = controller.rotation;
		if (transforms[0].curve.keys.Length != 0)
		{
			position.x = transforms[0].curve.Evaluate(t);
		}
		if (transforms[1].curve.keys.Length != 0)
		{
			position.y = transforms[1].curve.Evaluate(t);
		}
		if (transforms[2].curve.keys.Length != 0)
		{
			position.z = transforms[2].curve.Evaluate(t);
		}
		if (transforms[3].curve.keys.Length != 0)
		{
			rotation.x = transforms[3].curve.Evaluate(t);
		}
		if (transforms[4].curve.keys.Length != 0)
		{
			rotation.y = transforms[4].curve.Evaluate(t);
		}
		rotation.z = 0f;
		return true;
	}

	public void RemoveCameraTransform(int curveIndex, int index)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		if (curveIndex < transforms.Length && curveIndex >= 0 && index < transforms[curveIndex].curve.keys.Length && index >= 0)
		{
			if (transforms[curveIndex].curve.keys.Length == 1)
			{
				CinematicCameraCurveModifier cinematicCameraCurveModifier = default(CinematicCameraCurveModifier);
				TransformCurveKey transformCurveKey = (TransformCurveKey)curveIndex;
				cinematicCameraCurveModifier.id = transformCurveKey.ToString();
				cinematicCameraCurveModifier.curve = new AnimationCurve();
				transforms[curveIndex] = cinematicCameraCurveModifier;
			}
			else
			{
				transforms[curveIndex].curve.RemoveKey(index);
				AfterModifications(curveIndex == 4);
			}
		}
	}

	public void RemoveModifierKey(string id, int idx)
	{
		int num = modifiers.FindIndex((CinematicCameraCurveModifier m) => m.id == id);
		if (num >= 0)
		{
			if (idx < modifiers[num].curve.length)
			{
				modifiers[num].curve.RemoveKey(idx);
			}
			if (modifiers[num].curve.length == 0)
			{
				RemoveModifier(id);
			}
			AfterModifications();
		}
	}

	public int AddModifierKey(string id, float t, float value, float min, float max)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		int num = modifiers.FindIndex((CinematicCameraCurveModifier m) => m.id == id);
		if (num >= 0)
		{
			return modifiers[num].curve.AddKey(t, value);
		}
		CinematicCameraCurveModifier item = new CinematicCameraCurveModifier
		{
			curve = new AnimationCurve((Keyframe[])(object)new Keyframe[1]
			{
				new Keyframe(t, value)
			}),
			id = id,
			min = min,
			max = max
		};
		modifiers.Add(item);
		AfterModifications();
		return 0;
	}

	public int AddModifierKey(string id, float t, float value)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		int num = modifiers.FindIndex((CinematicCameraCurveModifier m) => m.id == id);
		if (num >= 0)
		{
			return modifiers[num].curve.AddKey(t, value);
		}
		modifiers.Add(new CinematicCameraCurveModifier
		{
			curve = new AnimationCurve((Keyframe[])(object)new Keyframe[1]
			{
				new Keyframe(t, value)
			}),
			id = id
		});
		AfterModifications();
		return 0;
	}

	public void Refresh(float t, IDictionary<string, PhotoModeProperty> properties, IGameCameraController controller)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		foreach (CinematicCameraCurveModifier modifier in modifiers)
		{
			if (properties.TryGetValue(modifier.id, out var value))
			{
				float num = modifier.curve.Evaluate(t);
				float num2 = value.min?.Invoke() ?? float.MinValue;
				float num3 = value.max?.Invoke() ?? float.MaxValue;
				float obj = Math.Clamp(num, num2, num3);
				value.setValue(obj);
			}
		}
		if (SampleTransform(controller, t, out var position, out var rotation))
		{
			controller.rotation = rotation;
			controller.position = position;
		}
	}

	public int AddCameraTransform(float t, Vector3 position, Vector3 rotation)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		int result = transforms[0].AddKey(t, position.x);
		transforms[1].AddKey(t, position.y);
		transforms[2].AddKey(t, position.z);
		transforms[3].AddKey(t, (rotation.x > 90f) ? (rotation.x - 360f) : rotation.x);
		transforms[4].AddKey(t, rotation.y);
		AfterModifications(rotationsChanged: true);
		return result;
	}

	public int MoveKeyframe(CinematicCameraCurveModifier modifier, int index, Keyframe keyframe)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (modifier.curve == null)
		{
			return -1;
		}
		AnimationCurve curve = modifier.curve;
		if (modifier.min != modifier.max)
		{
			((Keyframe)(ref keyframe)).value = Mathf.Clamp(((Keyframe)(ref keyframe)).value, modifier.min, modifier.max);
		}
		((Keyframe)(ref keyframe)).weightedMode = (WeightedMode)3;
		Keyframe val = curve[index];
		if (((Keyframe)(ref val)).time != ((Keyframe)(ref keyframe)).time || ((Keyframe)(ref val)).value != ((Keyframe)(ref keyframe)).value || ((Keyframe)(ref val)).inTangent != ((Keyframe)(ref keyframe)).inTangent || ((Keyframe)(ref val)).outTangent != ((Keyframe)(ref keyframe)).outTangent || ((Keyframe)(ref val)).inWeight != ((Keyframe)(ref keyframe)).inWeight || ((Keyframe)(ref val)).outWeight != ((Keyframe)(ref keyframe)).outWeight)
		{
			index = curve.MoveKey(index, keyframe);
		}
		AfterModifications(modifier.id.StartsWith("Rotation"));
		return index;
	}

	public void AfterModifications(bool rotationsChanged = false)
	{
		bool flag = EnsureLoop();
		if (rotationsChanged || flag)
		{
			PatchRotations();
		}
	}

	private void PatchRotations()
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 1; i < transforms[4].curve.keys.Length; i++)
		{
			float time = ((Keyframe)(ref transforms[4].curve.keys[i])).time;
			float value = ((Keyframe)(ref transforms[4].curve.keys[i - 1])).value;
			float num = (((Keyframe)(ref transforms[4].curve.keys[i])).value - value + 180f) % 360f - 180f;
			float num2 = ((num < -180f) ? (num + 360f) : num);
			transforms[4].curve.MoveKey(i, new Keyframe(time, value + num2));
		}
	}

	private bool EnsureLoop()
	{
		bool flag = false;
		if (loop)
		{
			CinematicCameraCurveModifier[] array = transforms;
			foreach (CinematicCameraCurveModifier cinematicCameraCurveModifier in array)
			{
				flag |= EnsureLoop(cinematicCameraCurveModifier.curve);
			}
			foreach (CinematicCameraCurveModifier modifier in modifiers)
			{
				flag |= EnsureLoop(modifier.curve);
			}
		}
		return flag;
	}

	private bool EnsureLoop(AnimationCurve curve)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (curve.keys.Length != 0)
		{
			float num = curve.Evaluate(0f);
			if (((Keyframe)(ref curve.keys[0])).time > 0.1f)
			{
				curve.AddKey(0f, num);
				flag = true;
			}
			if (((Keyframe)(ref curve.keys[curve.keys.Length - 1])).time < playbackDuration)
			{
				flag = true;
				curve.AddKey(playbackDuration, num);
			}
			if (((Keyframe)(ref curve.keys[curve.keys.Length - 1])).time == playbackDuration)
			{
				Keyframe val = curve.keys[curve.keys.Length - 1];
				flag |= ((Keyframe)(ref val)).value != num;
				((Keyframe)(ref val)).time = playbackDuration;
				((Keyframe)(ref val)).value = num;
				curve.MoveKey(curve.keys.Length - 1, val);
			}
		}
		return flag;
	}

	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("modifiers");
		JsonWriterExtensions.Write<CinematicCameraCurveModifier>(writer, (IList<CinematicCameraCurveModifier>)modifiers);
		writer.PropertyName("transforms");
		JsonWriterExtensions.Write<CinematicCameraCurveModifier>(writer, (IList<CinematicCameraCurveModifier>)transforms);
		writer.TypeEnd();
	}

	public void Read(IJsonReader reader)
	{
		reader.ReadMapBegin();
		reader.ReadProperty("modifiers");
		ulong num = reader.ReadArrayBegin();
		modifiers = new List<CinematicCameraCurveModifier>((int)num);
		for (ulong num2 = 0uL; num2 < num; num2++)
		{
			CinematicCameraCurveModifier item = default(CinematicCameraCurveModifier);
			item.Read(reader);
			modifiers.Add(item);
		}
		reader.ReadArrayEnd();
		reader.ReadProperty("transforms");
		num = reader.ReadArrayBegin();
		transforms = new CinematicCameraCurveModifier[num];
		for (ulong num3 = 0uL; num3 < num; num3++)
		{
			CinematicCameraCurveModifier cinematicCameraCurveModifier = default(CinematicCameraCurveModifier);
			cinematicCameraCurveModifier.Read(reader);
			transforms[num3] = cinematicCameraCurveModifier;
		}
		reader.ReadArrayEnd();
	}

	private static void SupportValueTypesForAOT()
	{
		JSON.SupportTypeForAOT<CinematicCameraSequence>();
		JSON.SupportTypeForAOT<CinematicCameraCurveModifier>();
	}
}
