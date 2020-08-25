using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolume : IDisposable, IEnumerable<VoxelChunk>, ICollisionGrid {

        Dictionary<Coords, VoxelChunk> _chunks;

        public int ChunkCount => _chunks.Count;

        Region? _chunkRegion;

        public Region ChunkRegion {
            // if the region was invalidated, such as by removal of a chunk, recalculate it
            get => _chunkRegion ?? CalculateChunkRegion();
            private set => _chunkRegion = value;
        }
        public Region VoxelRegion => ChunkRegion * VoxelChunk.SIZE;

        public IVoxelOrientationField OrientationField;

        public IEnumerable ChunkCoords => _chunks.Keys;

        public VoxelChunk this[Coords c]
            => _chunks.TryGetValue(c, out var chunk) ? chunk : null;

        public VoxelVolume(IVoxelOrientationField orientationField = null) {
            _chunks = new Dictionary<Coords, VoxelChunk>();
            OrientationField = orientationField;
            ChunkRegion = new Region();
        }

        public VoxelChunk AddChunk(Coords coords) {
            var chunk = new VoxelChunk(this, coords);
            _chunks.Add(coords, chunk);
            var region = ChunkRegion;
            region.ExpandToInclude(coords);
            ChunkRegion = region;
            return chunk;
        }

        public bool RemoveChunk(Coords coords) {
            if (_chunks.TryGetValue(coords, out var chunk)) {
                chunk.Dispose();
                _chunks.Remove(coords);
                // region isnt valid, so set it to null
                _chunkRegion = null;
                return true;
            }
            else {
                return false;
            }
        }

        // recalculate the chunk region
        Region CalculateChunkRegion() {
            Region chunkRegion = new Region();
            foreach (var c in _chunks.Keys) {
                chunkRegion.ExpandToInclude(c);
            }
            _chunkRegion = chunkRegion;
            return chunkRegion;
        }

        public void Dispose() {
            foreach (var chunk in _chunks.Values) {
                chunk.Dispose();
            }
        }

        // get the coords of the chunk containing a set of global coordsinates
        public Coords GlobalToChunkCoords(Coords c) {
            Vector3 v = c;
            v /= VoxelChunk.SIZE;
            return (Coords) v;
        }

        // get the voxel at a specific set of global coords
        public Voxel? GetVoxel(Coords c) {
            var chunk = GetChunkContainingVolumeCoords(c);
            return chunk?.Voxels[chunk.VolumeToLocalCoords(c)];
        }
        
        // get the voxel light at a specific set of global coords
        public VoxelLight GetVoxelLight(Coords c) {
            var chunk = GetChunkContainingVolumeCoords(c);
            return chunk?.LightData.GetVoxelLight(chunk.VolumeToLocalCoords(c)) ?? VoxelLight.INVALID;
        }

        public unsafe byte* GetVoxelLightData(Coords c, VoxelLightChannel channel) => GetVoxelLightData(c, (int) channel);
        public unsafe byte* GetVoxelLightData(Coords c, int channel) {
            var chunk = GetChunkContainingVolumeCoords(c);
            if (chunk != null) {
                return chunk.LightData[channel][chunk.VolumeToLocalCoords(c)];
            }
            else {
                return null;
            }
        }

        // set the voxel at a specific set of coords
        // calls the onModifyVoxel callback
        public void SetVoxel(Coords c, Voxel v) {
            var chunk = GetChunkContainingVolumeCoords(c) ?? AddChunk(GlobalToChunkCoords(c));
            var localCoords = chunk.VolumeToLocalCoords(c);
            chunk.Voxels[localCoords] = v;
        }

        // return the chunk containing the voxel at a set of coords
        public VoxelChunk GetChunkContainingVolumeCoords(Coords c) {
            return this[GlobalToChunkCoords(c)];
        }

        public IEnumerator<VoxelChunk> GetEnumerator()
            => _chunks.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool CellIsSolid(Coords c)
            => GetVoxel(c)?.IsSolid ?? false;

        public Orientation GetVoxelOrientation(Coords c)
            => OrientationField?.GetVoxelOrientation(c) ?? Orientation.Zero;

        public bool Raycast(Vector3 origin, Vector3 dir, float range, Predicate<Voxel> pred, out VoxelRaycastResult result) {            
            dir.Normalize();
            Coords current = (Coords) origin;
            var voxel = GetVoxel(current) ?? Voxel.Empty;
            if (pred(voxel)) {
                result = new VoxelRaycastResult() {
                    Volume = this,
                    Chunk = GetChunkContainingVolumeCoords(current),
                    Coords = current,
                    Normal = Vector3.Zero,
                    Voxel = voxel
                };
                return true;
            }
            Coords step = (Coords) dir.Sign();
            var tDelta = new Vector3(
                MathF.Sqrt(1 + (dir.Y * dir.Y + dir.Z * dir.Z) / (dir.X * dir.X)),
                MathF.Sqrt(1 + (dir.X * dir.X + dir.Z * dir.Z) / (dir.Y * dir.Y)),
                MathF.Sqrt(1 + (dir.X * dir.X + dir.Y * dir.Y) / (dir.Z * dir.Z))
            );
            var tMax = new Vector3(
                (step.X > 0 ? (current.X + 1 - origin.X) : origin.X - current.X) * tDelta.X,
                (step.Y > 0 ? (current.Y + 1 - origin.Y) : origin.Y - current.Y) * tDelta.Y,
                (step.Z > 0 ? (current.Z + 1 - origin.Z) : origin.Z - current.Z) * tDelta.Z
            );
            if (dir.X == 0) tMax.X = float.PositiveInfinity;
            if (dir.Y == 0) tMax.Y = float.PositiveInfinity;
            if (dir.Z == 0) tMax.Z = float.PositiveInfinity;
            float distance = 0;
            while (distance < range) {
                Vector3 normal = Vector3.Zero;
                var min = tMax.Min();
                if (min == tMax.X) {
                    current.X += step.X;
                    tMax.X += tDelta.X;
                    distance = (current.X - origin.X + (1 - step.X) / 2f) / dir.X;
                    normal.X = -step.X;
                }
                else if (min == tMax.Y) {
                    current.Y += step.Y;
                    tMax.Y += tDelta.Y;
                    distance = (current.Y - origin.Y + (1 - step.Y) / 2f) / dir.Y;
                    normal.Y = -step.Y;
                }
                else if (min == tMax.Z) {
                    current.Z += step.Z;
                    tMax.Z += tDelta.Z;
                    distance = (current.Z - origin.Z + (1 - step.Z) / 2f) / dir.Z;
                    normal = Vector3.Backward;
                    normal.Z = -step.Z;
                }
                voxel = GetVoxel(current) ?? Voxel.Empty;
                if (distance < range && pred(voxel)) {
                    result = new VoxelRaycastResult() {
                        Voxel = voxel,
                        Coords = current,
                        Volume = this,
                        Normal = normal,
                        Chunk = GetChunkContainingVolumeCoords(current)
                    };
                    return true;
                }
            }
            result = new VoxelRaycastResult();
            return false;
        }
    }

}