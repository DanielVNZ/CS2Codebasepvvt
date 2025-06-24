using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Common;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class NetCompositionMeshRefSystem : GameSystemBase
{
	private struct NewMeshData
	{
		public Entity m_Entity;

		public CompositionFlags m_Flags;

		public unsafe void* m_Pieces;

		public int m_PieceCount;
	}

	[BurstCompile]
	private struct CompositionMeshRefJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<NetCompositionData> m_CompositionType;

		[ReadOnly]
		public ComponentLookup<MeshData> m_MeshData;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> m_CompositionMeshData;

		[ReadOnly]
		public BufferLookup<NetCompositionPiece> m_CompositionPieces;

		[ReadOnly]
		public BufferLookup<LodMesh> m_LodMeshes;

		[ReadOnly]
		public BufferLookup<MeshMaterial> m_MeshMaterials;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityArchetype m_MeshArchetype;

		[ReadOnly]
		public NativeParallelMultiHashMap<int, Entity> m_MeshEntities;

		public EntityCommandBuffer m_CommandBuffer;

		public unsafe void Execute()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelMultiHashMap<int, NewMeshData> newMeshes = default(NativeParallelMultiHashMap<int, NewMeshData>);
			NativeList<NetCompositionPiece> tempPieces = default(NativeList<NetCompositionPiece>);
			NativeList<NetCompositionPiece> tempPieces2 = default(NativeList<NetCompositionPiece>);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<NetCompositionData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetCompositionData>(ref m_CompositionType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val2 = nativeArray[j];
					NetCompositionData netCompositionData = nativeArray2[j];
					DynamicBuffer<NetCompositionPiece> source = m_CompositionPieces[val2];
					bool hasMesh;
					int hashCode = GetHashCode(netCompositionData.m_Flags, source.AsNativeArray(), out hasMesh);
					if (!hasMesh)
					{
						continue;
					}
					if (TryFindComposition(newMeshes, hashCode, netCompositionData.m_Flags, source.GetUnsafeReadOnlyPtr(), source.Length, ref tempPieces, out var entity, out var rotate))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<NetCompositionMeshRef>(val2, new NetCompositionMeshRef
						{
							m_Mesh = entity,
							m_Rotate = rotate
						});
						continue;
					}
					entity = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_MeshArchetype);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<NetCompositionMeshRef>(val2, new NetCompositionMeshRef
					{
						m_Mesh = entity
					});
					DynamicBuffer<NetCompositionPiece> val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<NetCompositionPiece>(entity);
					DynamicBuffer<MeshMaterial> materials = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<MeshMaterial>(entity);
					CopyMeshPieces(source, val3);
					NetCompositionMeshData netCompositionMeshData = new NetCompositionMeshData
					{
						m_Flags = netCompositionData.m_Flags,
						m_Width = netCompositionData.m_Width,
						m_MiddleOffset = netCompositionData.m_MiddleOffset,
						m_HeightRange = netCompositionData.m_HeightRange,
						m_Hash = hashCode
					};
					CalculatePieceData(val3, materials, out netCompositionMeshData.m_DefaultLayers, out netCompositionMeshData.m_State, out netCompositionMeshData.m_IndexFactor, out netCompositionMeshData.m_LodBias, out netCompositionMeshData.m_ShadowBias);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<NetCompositionMeshData>(entity, netCompositionMeshData);
					if (!newMeshes.IsCreated)
					{
						newMeshes._002Ector(50, AllocatorHandle.op_Implicit((Allocator)2));
					}
					NewMeshData newMeshData = new NewMeshData
					{
						m_Entity = entity,
						m_Flags = netCompositionData.m_Flags,
						m_Pieces = val3.GetUnsafePtr(),
						m_PieceCount = val3.Length
					};
					newMeshes.Add(hashCode, newMeshData);
					InitializeLods(entity, netCompositionMeshData, val3, newMeshes, ref tempPieces, ref tempPieces2);
				}
			}
			if (newMeshes.IsCreated)
			{
				newMeshes.Dispose();
			}
			if (tempPieces.IsCreated)
			{
				tempPieces.Dispose();
			}
			if (tempPieces2.IsCreated)
			{
				tempPieces2.Dispose();
			}
		}

		private void CopyMeshPieces(DynamicBuffer<NetCompositionPiece> source, DynamicBuffer<NetCompositionPiece> target)
		{
			int num = 0;
			for (int i = 0; i < source.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = source[i];
				num += math.select(0, 1, (netCompositionPiece.m_PieceFlags & NetPieceFlags.HasMesh) != 0 && (netCompositionPiece.m_SectionFlags & NetSectionFlags.Hidden) == 0);
			}
			target.ResizeUninitialized(num);
			num = 0;
			for (int j = 0; j < source.Length; j++)
			{
				NetCompositionPiece netCompositionPiece2 = source[j];
				if ((netCompositionPiece2.m_PieceFlags & NetPieceFlags.HasMesh) != 0 && (netCompositionPiece2.m_SectionFlags & NetSectionFlags.Hidden) == 0)
				{
					target[num++] = netCompositionPiece2;
				}
			}
		}

		private void CalculatePieceData(DynamicBuffer<NetCompositionPiece> pieces, DynamicBuffer<MeshMaterial> materials, out MeshLayer defaultLayers, out MeshFlags meshFlags, out float indexFactor, out float lodBias, out float shadowBias)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			defaultLayers = (MeshLayer)0;
			meshFlags = (MeshFlags)0u;
			indexFactor = 0f;
			lodBias = 0f;
			shadowBias = 0f;
			for (int i = 0; i < pieces.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = pieces[i];
				MeshData meshData = m_MeshData[netCompositionPiece.m_Piece];
				DynamicBuffer<MeshMaterial> val = m_MeshMaterials[netCompositionPiece.m_Piece];
				defaultLayers |= meshData.m_DefaultLayers;
				meshFlags |= meshData.m_State;
				indexFactor += (float)meshData.m_IndexCount / math.max(1f, MathUtils.Size(((Bounds3)(ref meshData.m_Bounds)).z));
				lodBias += meshData.m_LodBias;
				shadowBias += meshData.m_ShadowBias;
				for (int j = 0; j < val.Length; j++)
				{
					int materialIndex = val[j].m_MaterialIndex;
					int num = 0;
					while (true)
					{
						if (num < materials.Length)
						{
							if (materials[num].m_MaterialIndex == materialIndex)
							{
								break;
							}
							num++;
							continue;
						}
						materials.Add(new MeshMaterial
						{
							m_MaterialIndex = materialIndex
						});
						break;
					}
				}
			}
			if (pieces.Length != 0)
			{
				lodBias /= pieces.Length;
				shadowBias /= pieces.Length;
			}
		}

		private unsafe void InitializeLods(Entity mesh, NetCompositionMeshData meshData, DynamicBuffer<NetCompositionPiece> pieces, NativeParallelMultiHashMap<int, NewMeshData> newMeshes, ref NativeList<NetCompositionPiece> tempPieces, ref NativeList<NetCompositionPiece> tempPieces2)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < pieces.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = pieces[i];
				if (m_LodMeshes.HasBuffer(netCompositionPiece.m_Piece))
				{
					num = math.max(num, m_LodMeshes[netCompositionPiece.m_Piece].Length);
				}
			}
			if (num == 0)
			{
				return;
			}
			DynamicBuffer<LodMesh> val = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LodMesh>(mesh);
			val.ResizeUninitialized(num);
			DynamicBuffer<LodMesh> val2 = default(DynamicBuffer<LodMesh>);
			for (int j = 0; j < num; j++)
			{
				if (tempPieces.IsCreated)
				{
					tempPieces.Clear();
				}
				else
				{
					tempPieces = new NativeList<NetCompositionPiece>(pieces.Length, AllocatorHandle.op_Implicit((Allocator)2));
				}
				for (int k = 0; k < pieces.Length; k++)
				{
					NetCompositionPiece netCompositionPiece2 = pieces[k];
					if (m_LodMeshes.TryGetBuffer(netCompositionPiece2.m_Piece, ref val2) && val2.Length != 0)
					{
						netCompositionPiece2.m_Piece = val2[math.min(j, val2.Length - 1)].m_LodMesh;
					}
					tempPieces.Add(ref netCompositionPiece2);
				}
				meshData.m_Hash = GetHashCode(meshData.m_Flags, tempPieces.AsArray(), out var hasMesh);
				if (TryFindComposition(newMeshes, meshData.m_Hash, meshData.m_Flags, NativeListUnsafeUtility.GetUnsafeReadOnlyPtr<NetCompositionPiece>(tempPieces), tempPieces.Length, ref tempPieces2, out var entity, out hasMesh))
				{
					val[j] = new LodMesh
					{
						m_LodMesh = entity
					};
					continue;
				}
				entity = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_MeshArchetype);
				val[j] = new LodMesh
				{
					m_LodMesh = entity
				};
				DynamicBuffer<NetCompositionPiece> pieces2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<NetCompositionPiece>(entity);
				DynamicBuffer<MeshMaterial> materials = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<MeshMaterial>(entity);
				pieces2.CopyFrom(tempPieces.AsArray());
				CalculatePieceData(pieces2, materials, out meshData.m_DefaultLayers, out meshData.m_State, out meshData.m_IndexFactor, out meshData.m_LodBias, out meshData.m_ShadowBias);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<NetCompositionMeshData>(entity, meshData);
				NewMeshData newMeshData = new NewMeshData
				{
					m_Entity = entity,
					m_Flags = meshData.m_Flags,
					m_Pieces = pieces2.GetUnsafePtr(),
					m_PieceCount = pieces2.Length
				};
				newMeshes.Add(meshData.m_Hash, newMeshData);
				InitializeLods(entity, meshData, pieces2, newMeshes, ref tempPieces, ref tempPieces2);
			}
		}

		private unsafe bool TryFindComposition(NativeParallelMultiHashMap<int, NewMeshData> newMeshes, int hash, CompositionFlags flags, void* pieces, int pieceCount, ref NativeList<NetCompositionPiece> tempPieces, out Entity entity, out bool rotate)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelMultiHashMapIterator<int> val = default(NativeParallelMultiHashMapIterator<int>);
			if (m_MeshEntities.TryGetFirstValue(hash, ref entity, ref val))
			{
				do
				{
					NetCompositionMeshData netCompositionMeshData = m_CompositionMeshData[entity];
					DynamicBuffer<NetCompositionPiece> val2 = m_CompositionPieces[entity];
					if (Equals(flags, netCompositionMeshData.m_Flags, pieces, val2.GetUnsafeReadOnlyPtr(), pieceCount, val2.Length, ref tempPieces, rotate: false))
					{
						rotate = false;
						return true;
					}
					if ((flags.m_General & CompositionFlags.General.Node) == 0 && Equals(flags, netCompositionMeshData.m_Flags, pieces, val2.GetUnsafeReadOnlyPtr(), pieceCount, val2.Length, ref tempPieces, rotate: true))
					{
						rotate = true;
						return true;
					}
				}
				while (m_MeshEntities.TryGetNextValue(ref entity, ref val));
			}
			NewMeshData newMeshData = default(NewMeshData);
			if (newMeshes.IsCreated && newMeshes.TryGetFirstValue(hash, ref newMeshData, ref val))
			{
				do
				{
					if (Equals(flags, newMeshData.m_Flags, pieces, newMeshData.m_Pieces, pieceCount, newMeshData.m_PieceCount, ref tempPieces, rotate: false))
					{
						entity = newMeshData.m_Entity;
						rotate = false;
						return true;
					}
					if ((flags.m_General & CompositionFlags.General.Node) == 0 && Equals(flags, newMeshData.m_Flags, pieces, newMeshData.m_Pieces, pieceCount, newMeshData.m_PieceCount, ref tempPieces, rotate: true))
					{
						entity = newMeshData.m_Entity;
						rotate = true;
						return true;
					}
				}
				while (newMeshes.TryGetNextValue(ref newMeshData, ref val));
			}
			rotate = false;
			return false;
		}

		private unsafe int GetHashCode(CompositionFlags flags, NativeArray<NetCompositionPiece> pieces, out bool hasMesh)
		{
			int num = ((uint)GetCompositionFlagMask(flags)).GetHashCode();
			hasMesh = false;
			for (int i = 0; i < pieces.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = pieces[i];
				if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.HasMesh) != 0 && (netCompositionPiece.m_SectionFlags & NetSectionFlags.Hidden) == 0)
				{
					num += ((object)(*(Entity*)(&netCompositionPiece.m_Piece))/*cast due to .constrained prefix*/).GetHashCode();
					hasMesh = true;
				}
			}
			return num;
		}

		private unsafe bool Equals(CompositionFlags flags1, CompositionFlags flags2, void* pieces1, void* pieces2, int pieceCount1, int pieceCount2, ref NativeList<NetCompositionPiece> tempPieces, bool rotate)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			if (GetCompositionFlagMask(flags1) != GetCompositionFlagMask(flags2))
			{
				return false;
			}
			if (tempPieces.IsCreated)
			{
				tempPieces.Clear();
			}
			else
			{
				tempPieces = new NativeList<NetCompositionPiece>(pieceCount2, AllocatorHandle.op_Implicit((Allocator)2));
			}
			for (int i = 0; i < pieceCount2; i++)
			{
				NetCompositionPiece netCompositionPiece = UnsafeUtility.ReadArrayElement<NetCompositionPiece>(pieces2, i);
				if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.HasMesh) != 0 && (netCompositionPiece.m_SectionFlags & NetSectionFlags.Hidden) == 0)
				{
					tempPieces.Add(ref netCompositionPiece);
				}
			}
			bool2 val2 = default(bool2);
			for (int j = 0; j < pieceCount1; j++)
			{
				NetCompositionPiece piece = UnsafeUtility.ReadArrayElement<NetCompositionPiece>(pieces1, j);
				if ((piece.m_PieceFlags & NetPieceFlags.HasMesh) == 0 || (piece.m_SectionFlags & NetSectionFlags.Hidden) != 0)
				{
					continue;
				}
				NetSectionFlags netSectionFlags = GetSectionFlagMask(piece);
				float3 offset = piece.m_Offset;
				bool flag = false;
				if (rotate)
				{
					if ((netSectionFlags & NetSectionFlags.Left) != 0)
					{
						netSectionFlags &= ~NetSectionFlags.Left;
						netSectionFlags |= NetSectionFlags.Right;
					}
					else if ((netSectionFlags & NetSectionFlags.Right) != 0)
					{
						netSectionFlags &= ~NetSectionFlags.Right;
						netSectionFlags |= NetSectionFlags.Left;
					}
					offset.x = 0f - offset.x;
				}
				for (int k = 0; k < tempPieces.Length; k++)
				{
					NetCompositionPiece piece2 = tempPieces[k];
					if (!(piece.m_Piece != piece2.m_Piece) && netSectionFlags == GetSectionFlagMask(piece2) && !math.any(math.abs(offset - piece2.m_Offset) >= 0.1f))
					{
						NetPieceFlags netPieceFlags = piece.m_PieceFlags | piece2.m_PieceFlags;
						NetSectionFlags netSectionFlags2 = piece.m_SectionFlags ^ piece2.m_SectionFlags;
						bool2 val = new bool2((netPieceFlags & NetPieceFlags.AsymmetricMeshX) != 0, (netPieceFlags & NetPieceFlags.AsymmetricMeshZ) != 0);
						((bool2)(ref val2))._002Ector((netSectionFlags2 & NetSectionFlags.Invert) != 0, (netSectionFlags2 & NetSectionFlags.FlipMesh) != 0);
						if (!math.any(val & (val2 != rotate)))
						{
							flag = true;
							tempPieces.RemoveAtSwapBack(k);
							break;
						}
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return tempPieces.Length == 0;
		}

		private CompositionFlags.General GetCompositionFlagMask(CompositionFlags flags)
		{
			return flags.m_General & (CompositionFlags.General.Node | CompositionFlags.General.Roundabout);
		}

		private NetSectionFlags GetSectionFlagMask(NetCompositionPiece piece)
		{
			return piece.m_SectionFlags & (NetSectionFlags.Median | NetSectionFlags.Left | NetSectionFlags.Right | NetSectionFlags.AlignCenter | NetSectionFlags.HalfLength);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<MeshData> __Game_Prefabs_MeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> __Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionPiece> __Game_Prefabs_NetCompositionPiece_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LodMesh> __Game_Prefabs_LodMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshMaterial> __Game_Prefabs_MeshMaterial_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_NetCompositionData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCompositionData>(true);
			__Game_Prefabs_MeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MeshData>(true);
			__Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshData>(true);
			__Game_Prefabs_NetCompositionPiece_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionPiece>(true);
			__Game_Prefabs_LodMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LodMesh>(true);
			__Game_Prefabs_MeshMaterial_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshMaterial>(true);
		}
	}

	private NetCompositionMeshSystem m_NetCompositionMeshSystem;

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_CompositionQuery;

	private EntityArchetype m_MeshArchetype;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NetCompositionMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetCompositionMeshSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_CompositionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<NetCompositionData>(),
			ComponentType.ReadOnly<NetCompositionPiece>(),
			ComponentType.ReadOnly<NetCompositionMeshRef>(),
			ComponentType.ReadOnly<Created>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_MeshArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<NetCompositionMeshData>(),
			ComponentType.ReadWrite<NetCompositionPiece>(),
			ComponentType.ReadWrite<MeshMaterial>(),
			ComponentType.ReadWrite<BatchGroup>(),
			ComponentType.ReadWrite<Created>(),
			ComponentType.ReadWrite<Updated>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CompositionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_CompositionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle dependencies;
		JobHandle val2 = IJobExtensions.Schedule<CompositionMeshRefJob>(new CompositionMeshRefJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MeshData = InternalCompilerInterface.GetComponentLookup<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionMeshData = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshData>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionPieces = InternalCompilerInterface.GetBufferLookup<NetCompositionPiece>(ref __TypeHandle.__Game_Prefabs_NetCompositionPiece_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LodMeshes = InternalCompilerInterface.GetBufferLookup<LodMesh>(ref __TypeHandle.__Game_Prefabs_LodMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshMaterials = InternalCompilerInterface.GetBufferLookup<MeshMaterial>(ref __TypeHandle.__Game_Prefabs_MeshMaterial_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Chunks = chunks,
			m_MeshArchetype = m_MeshArchetype,
			m_MeshEntities = m_NetCompositionMeshSystem.GetMeshEntities(out dependencies),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, dependencies));
		chunks.Dispose(val2);
		m_NetCompositionMeshSystem.AddMeshEntityReader(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public NetCompositionMeshRefSystem()
	{
	}
}
