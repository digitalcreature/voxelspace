using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    using IO;

    /// <summary>
    /// Represents a volume of voxels as a sparse grid of chunks.
    /// </summary>
    public class VoxelVolume : IDisposable, IEnumerable<VoxelChunk>, ICollisionGrid, IBinaryReadWritable {

        Dictionary<Coords, VoxelChunk> _chunks;

        /// <summary>The count of chunks currently in the volume</summary>
        public int ChunkCount => _chunks.Count;

        public bool WasDisposed { get; private set; }

        public VoxelTypeIndex Index { get; private set; }

        Region? _chunkRegion;

        /// <summary>
        /// The chunk-space region bounding this volume. All chunks in the volume are inside this region.
        /// </summary>
        public Region ChunkRegion {
            // if the region was invalidated, such as by removal of a chunk, recalculate it
            get => _chunkRegion ?? calculateChunkRegion();
            private set => _chunkRegion = value;
        }

        /// <summary>
        /// The global-space region bounding this volume. All voxels (including empty ones) are inside this region.
        /// </summary>
        public Region VoxelRegion => ChunkRegion * VoxelChunk.SIZE;

        /// <summary>
        /// The orientation field dictating which way is "up" per voxel
        /// </summary>
        public IVoxelOrientationField OrientationField;

        /// <summary>
        /// Enumerate the coordinates of all chunks in this volume.
        /// </summary>
        public IEnumerable ChunkCoords => _chunks.Keys;

        /// <summary>
        /// Retrieve the chunk at a given set of chunk coordinates.
        /// </summary>
        public VoxelChunk this[Coords coords]
            => _chunks.TryGetValue(coords, out var chunk) ? chunk : null;

        Mutex _enumerationMutex;

        public VoxelVolume(IVoxelOrientationField orientationField = null) {
            Index = new VoxelTypeIndex();
            _chunks = new Dictionary<Coords, VoxelChunk>();
            OrientationField = orientationField;
            ChunkRegion = new Region();
            _enumerationMutex = new Mutex();
        }

        public void ReadBinary(BinaryReader reader) {
            Index.ReadBinary(reader);
            int chunkCount = reader.ReadInt32();
            for (int i = 0; i < chunkCount; i ++) {
                var coords = new Coords();
                coords.ReadBinary(reader);
                var chunk = AddChunk(coords);
                chunk.ReadBinary(reader);
            }
            calculateChunkRegion();
        }

        public void WriteBinary(BinaryWriter writer) {
            Index.WriteBinary(writer);
            writer.Write(ChunkCount);
            foreach (var chunk in _chunks.Values) {
                chunk.Coords.WriteBinary(writer);
                chunk.WriteBinary(writer);
            }
        }

        /// <summary>
        /// Add an empty chunk to this volume.
        /// Undefined if a chunk already exists.
        /// </summary>
        /// <param name="coords">The coordinates of the new chunk</param>
        /// <returns></returns>
        public VoxelChunk AddChunk(Coords coords) {
            _enumerationMutex.WaitOne();
            var chunk = new VoxelChunk(this, coords);
            _chunks.Add(coords, chunk);
            var region = ChunkRegion;
            region.ExpandToInclude(coords);
            ChunkRegion = region;
            _enumerationMutex.ReleaseMutex();
            return chunk;
        }

        /// <summary>
        /// Removes a chunk from this volume.
        /// </summary>
        /// <param name="coords">The coordinates of the chunk to remove</param>
        /// <returns>true if there was a chunk to remove, false otherwise.</returns>
        public bool RemoveChunk(Coords coords) {
            if (_chunks.TryGetValue(coords, out var chunk)) {
                _enumerationMutex.WaitOne();
                chunk.Dispose();
                _chunks.Remove(coords);
                // region isnt valid, so set it to null
                _chunkRegion = null;
                _enumerationMutex.ReleaseMutex();
                return true;
            }
            else {
                return false;
            }
        }

        // recalculate the chunk region
        Region calculateChunkRegion() {
            Region chunkRegion = new Region();
            foreach (var c in _chunks.Keys) {
                chunkRegion.ExpandToInclude(c);
            }
            _chunkRegion = chunkRegion;
            return chunkRegion;
        }

        public void Dispose() {
            WasDisposed = true;
            foreach (var chunk in _chunks.Values) {
                chunk.Dispose();
            }
        }
        
        /// <summary>
        /// Gets the coordinates of the chunk containing a set of global coordinates.
        /// </summary>
        /// <param name="coords">Global coordinates to translate</param>
        /// <returns>Translated chunk coordinates</returns>
        public Coords GlobalToChunkCoords(Coords coords) {
            Vector3 v = coords;
            v /= VoxelChunk.SIZE;
            return (Coords) v;
        }

        /// <summary>
        /// Get the voxel at a set of global coordinates.
        /// </summary>
        /// <param name="coords">Global space coordinates of the voxel to retrieve</param>
        /// <returns>The retrieved voxel. If there is no chunk in this volume containing <c>coords</c>, returns null.</returns>
        public unsafe Voxel? GetVoxel(Coords coords) {
            var data = GetVoxelData(coords);
            if (data != null) {
                return new Voxel(*data, Index);
            }
            else {
                return null;
            }
        }

        public unsafe VoxelData *GetVoxelData(Coords coords) {
            var chunk = GetChunkContainingGlobalCoords(coords);
            if (chunk != null) {
                return chunk.VoxelData[chunk.VolumeToLocalCoords(coords)];
            }
            else {
                return null;
            }
        }
        
        /// <summary>
        /// Get the voxel light data for the voxel at a set of global coordinates
        /// </summary>
        /// <param name="coords">Global space coordinates of the light data to retrieve</param>
        /// <returns>The retrieved light data. If there is no chunk in this volume containing <c>coords</c>, returns VoxelLight.NULL.</returns>
        public VoxelLight GetVoxelLight(Coords coords) {
            var chunk = GetChunkContainingGlobalCoords(coords);
            return chunk?.LightData.GetVoxelLight(chunk.VolumeToLocalCoords(coords)) ?? VoxelLight.NULL;
        }

        /// <summary>
        /// Get a pointer to the voxel light data for a specific channel at global coordinates.
        /// </summary>
        /// <param name="coords">Global space coordinates of the light data to retrieve</param>
        /// <param name="channel">The channel to retrieve light data for</param>
        /// <returns>A pointer to the requested light data. null if no chunk found.</returns>
        public unsafe byte* GetVoxelLightData(Coords coords, int channel) {
            var chunk = GetChunkContainingGlobalCoords(coords);
            if (chunk != null) {
                return chunk.LightData[channel][chunk.VolumeToLocalCoords(coords)];
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Get a pointer to the voxel light data for a specific channel at global coordinates.
        /// </summary>
        /// <param name="coords">Global space coordinates of the light data to retrieve</param>
        /// <param name="channel">The channel to retrieve light data for</param>
        /// <returns>A pointer to the requested light data. null if no chunk found.</returns>
        public unsafe byte* GetVoxelLightData(Coords c, VoxelLightChannel channel) => GetVoxelLightData(c, (int) channel);

        /// <summary>
        /// Retrieve the chunk containing a set of global coordinates.
        /// </summary>
        /// <param name="coords">The global coordinates to translate.</param>
        /// <returns>The retieved chunk. null if no chunk was found.</returns>
        public VoxelChunk GetChunkContainingGlobalCoords(Coords coords) {
            return this[GlobalToChunkCoords(coords)];
        }

        /// <summary>
        /// Call before enumerating over the volume if other threads adding and removing chunks is an issue.
        /// Call <see cref="EndThreadsafeEnumeration"/> after enumeration is complete.
        /// </summary>
        public void StartThreadsafeEnumeration() {
            _enumerationMutex.WaitOne();
        }

        /// <summary>
        /// Call after enumerating over the volume using <see cref="StartThreadsafeEnumeration"/>
        /// </summary>
        public void EndThreadsafeEnumeration() {
            _enumerationMutex.ReleaseMutex();
        }

        public IEnumerator<VoxelChunk> GetEnumerator()
            => _chunks.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool CellIsSolid(Coords c)
            => GetVoxel(c)?.IsSolid ?? false;

        /// <summary>
        /// Get the orientation of a given voxel.
        /// </summary>
        /// <param name="coords">Global voxel coordinates</param>
        /// <returns>The oriention representing the "up" direction at <c>coords</c>.</returns>
        public Orientation GetVoxelOrientation(Coords coords)
            => OrientationField?.GetVoxelOrientation(coords) ?? Orientation.Zero;

        /// <summary>
        /// Perform a raycast against the voxels in the volume.
        /// All values are in global space.
        /// </summary>
        /// <param name="origin">Point to raycast from.</param>
        /// <param name="dir">Direction to raycast.</param>
        /// <param name="range">Maximum distance to cast before giving up</param>
        /// <param name="hitPredicate">return true if the given voxel should constitute a "hit".</param>
        /// <param name="result">The result of the raycast.</param>
        /// <returns>true if the ray hit a voxel, false otherwise.</returns>
        public bool Raycast(Vector3 origin, Vector3 dir, float range, Predicate<Voxel> hitPredicate, out VoxelRaycastResult result) {
            dir.Normalize();
            Coords current = (Coords) origin;
            var voxel = GetVoxel(current) ?? Voxel.Empty;
            if (hitPredicate(voxel)) {
                result = new VoxelRaycastResult() {
                    Volume = this,
                    Chunk = GetChunkContainingGlobalCoords(current),
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
                if (distance < range && hitPredicate(voxel)) {
                    result = new VoxelRaycastResult() {
                        Voxel = voxel,
                        Coords = current,
                        Volume = this,
                        Normal = normal,
                        Chunk = GetChunkContainingGlobalCoords(current)
                    };
                    return true;
                }
            }
            result = new VoxelRaycastResult();
            return false;
        }

    }

}