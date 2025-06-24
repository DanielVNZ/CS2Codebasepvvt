using System.Collections.Generic;
using Cinemachine;
using Game.Rendering;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public static class CameraDebugUI
{
	[DebugTab("Camera", 0)]
	private static List<Widget> BuildCameraDebugUI(World world)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Invalid comparison between Unknown and I4
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		//IL_0244: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Expected O, but got Unknown
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_015e: Expected O, but got Unknown
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Expected O, but got Unknown
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Expected O, but got Unknown
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Expected O, but got Unknown
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Expected O, but got Unknown
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Expected O, but got Unknown
		//IL_03ab: Expected O, but got Unknown
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Expected O, but got Unknown
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Expected O, but got Unknown
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Expected O, but got Unknown
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Expected O, but got Unknown
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Expected O, but got Unknown
		//IL_048d: Expected O, but got Unknown
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Expected O, but got Unknown
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Expected O, but got Unknown
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Expected O, but got Unknown
		//IL_05b1: Expected O, but got Unknown
		//IL_05b6: Expected O, but got Unknown
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Expected O, but got Unknown
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Expected O, but got Unknown
		//IL_0631: Expected O, but got Unknown
		CameraUpdateSystem cameraUpdateSystem = world.GetExistingSystemManaged<CameraUpdateSystem>();
		float minClip = -50f;
		float maxClip = 1000f;
		Container val = new Container
		{
			displayName = "Game Cameras"
		};
		Camera[] allCameras = Camera.allCameras;
		for (int i = 0; i < allCameras.Length; i++)
		{
			Camera camera = allCameras[i];
			if ((int)camera.cameraType == 1)
			{
				ObservableList<Widget> children = val.children;
				Foldout val2 = new Foldout
				{
					displayName = $"#{i} {((Object)camera).name}"
				};
				((Container)val2).children.Add((Widget)new Value
				{
					displayName = "Is Main?",
					getter = () => ((Object)(object)camera == (Object)(object)Camera.main).ToString()
				});
				((Container)val2).children.Add((Widget)new Value
				{
					displayName = "Is ActiveViewer?",
					getter = () => ((Object)(object)camera == (Object)(object)cameraUpdateSystem.activeViewer?.camera).ToString()
				});
				((Container)val2).children.Add((Widget)new Value
				{
					displayName = "World Position",
					getter = delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						Camera obj2 = camera;
						return (obj2 == null) ? null : ((object)((Component)obj2).transform.position/*cast due to .constrained prefix*/).ToString();
					}
				});
				((Container)val2).children.Add((Widget)new Value
				{
					displayName = "Focus distance",
					getter = delegate
					{
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						Camera obj2 = camera;
						return (obj2 == null) ? null : ((object)((Component)obj2).transform.position/*cast due to .constrained prefix*/).ToString();
					}
				});
				children.Add((Widget)val2);
			}
		}
		Foldout val3 = new Foldout
		{
			displayName = "Active Viewer"
		};
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "World Position",
			getter = () => $"({cameraUpdateSystem.activeViewer?.position.x:F2}, {cameraUpdateSystem.activeViewer?.position.y:F2}, {cameraUpdateSystem.activeViewer?.position.z:F2})"
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Visibility distance",
			getter = () => cameraUpdateSystem.activeViewer?.visibilityDistance.ToString("F1")
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Focus distance",
			getter = () => cameraUpdateSystem.activeViewer?.viewerDistances.focus.ToString("F1")
		});
		ObservableList<Widget> children2 = ((Container)val3).children;
		BoolField val4 = new BoolField
		{
			displayName = "Shadows Adjust Start Dist"
		};
		((Field<bool>)val4).getter = () => cameraUpdateSystem.activeViewer?.shadowsAdjustStartDistance ?? false;
		((Field<bool>)val4).setter = delegate(bool value)
		{
			if (cameraUpdateSystem.activeViewer != null)
			{
				cameraUpdateSystem.activeViewer.shadowsAdjustStartDistance = value;
			}
		};
		children2.Add((Widget)val4);
		ObservableList<Widget> children3 = ((Container)val3).children;
		FloatField val5 = new FloatField
		{
			displayName = "Push culling near mult"
		};
		((Field<float>)val5).getter = () => cameraUpdateSystem.activeViewer?.pushCullingNearPlaneMultiplier ?? 0f;
		((Field<float>)val5).setter = delegate(float value)
		{
			if (cameraUpdateSystem.activeViewer != null)
			{
				cameraUpdateSystem.activeViewer.pushCullingNearPlaneMultiplier = value;
			}
		};
		val5.min = () => 0.1f;
		val5.max = () => 1f;
		val5.incStep = 0.05f;
		children3.Add((Widget)val5);
		ObservableList<Widget> children4 = ((Container)val3).children;
		FloatField val6 = new FloatField
		{
			displayName = "Push culling near value"
		};
		((Field<float>)val6).getter = () => cameraUpdateSystem.activeViewer?.pushCullingNearPlaneValue ?? 0f;
		((Field<float>)val6).setter = delegate(float value)
		{
			if (cameraUpdateSystem.activeViewer != null)
			{
				cameraUpdateSystem.activeViewer.pushCullingNearPlaneValue = value;
			}
		};
		val6.min = () => 0f;
		val6.max = () => 1000f;
		val6.incStep = 10f;
		children4.Add((Widget)val6);
		ObservableList<Widget> children5 = ((Container)val3).children;
		BoolField val7 = new BoolField
		{
			displayName = "Shadows Adjust Far Dist"
		};
		((Field<bool>)val7).getter = () => cameraUpdateSystem.activeViewer?.shadowsAdjustFarDistance ?? false;
		((Field<bool>)val7).setter = delegate(bool value)
		{
			if (cameraUpdateSystem.activeViewer != null)
			{
				cameraUpdateSystem.activeViewer.shadowsAdjustFarDistance = value;
			}
		};
		children5.Add((Widget)val7);
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Closest surface",
			getter = () => cameraUpdateSystem.activeViewer?.viewerDistances.closestSurface.ToString("F1")
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Farthest surface",
			getter = () => cameraUpdateSystem.activeViewer?.viewerDistances.farthestSurface.ToString("F1")
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Average surface",
			getter = () => cameraUpdateSystem.activeViewer?.viewerDistances.averageSurface.ToString("F1")
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Ground distance",
			getter = () => cameraUpdateSystem.activeViewer?.viewerDistances.ground.ToString("F1")
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Sea level view dist",
			getter = () => cameraUpdateSystem.activeViewer?.viewerDistances.maxDistanceToSeaLevel.ToString("F1")
		});
		Foldout val8 = val3;
		val.children.Add((Widget)(object)val8);
		Foldout val9 = new Foldout
		{
			displayName = "Cinemachine"
		};
		for (int num = 0; num < CinemachineCore.Instance.BrainCount; num++)
		{
			CinemachineCore.Instance.GetActiveBrain(num);
			for (int num2 = 0; num2 < CinemachineCore.Instance.VirtualCameraCount; num2++)
			{
				ICinemachineCamera vcam = (ICinemachineCamera)(object)CinemachineCore.Instance.GetVirtualCamera(num2);
				((Container)val9).children.Add((Widget)new Value
				{
					displayName = vcam.Name,
					getter = delegate
					{
						for (int j = 0; j < CinemachineCore.Instance.BrainCount; j++)
						{
							if (CinemachineCore.Instance.GetActiveBrain(j).ActiveVirtualCamera == vcam)
							{
								return "Active";
							}
						}
						return "Inactive";
					}
				});
			}
		}
		val.children.Add((Widget)(object)val9);
		List<Widget> obj = new List<Widget> { (Widget)(object)val };
		BoolField val10 = new BoolField
		{
			displayName = "Edge-scrolling"
		};
		((Field<bool>)val10).getter = () => CameraController.TryGet(out var cameraController) && cameraController.edgeScrolling;
		((Field<bool>)val10).setter = delegate(bool value)
		{
			if (CameraController.TryGet(out cameraController))
			{
				cameraController.edgeScrolling = value;
			}
		};
		obj.Add((Widget)val10);
		FloatField val11 = new FloatField
		{
			displayName = "Clip offset",
			min = () => minClip,
			max = () => maxClip,
			incStep = 1f,
			incStepMult = 1f,
			decimals = 0
		};
		((Field<float>)val11).getter = () => CameraController.TryGet(out cameraController) ? ((cameraController.clipDistance == float.MaxValue) ? maxClip : cameraController.clipDistance) : 0f;
		((Field<float>)val11).setter = delegate(float value)
		{
			if (CameraController.TryGet(out cameraController))
			{
				cameraController.clipDistance = math.select(value, float.MaxValue, value == maxClip);
			}
		};
		obj.Add((Widget)val11);
		return obj;
	}
}
