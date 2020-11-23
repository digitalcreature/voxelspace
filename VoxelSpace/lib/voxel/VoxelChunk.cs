using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoxelSpace {

    using IO;

    public class VoxelChunk : IDisposable, IBinaryReadWritable {

        public const int SIZE = 32;
        public const int VOXEL_COUNT = SIZE * SIZE * SIZE;

        public VoxelVolume Volume { get; private set; }
        public Coords Coords { get; private set; }
        public VoxelChunkMesh Mesh { get; private set; }

        public IndexedVoxels Voxels { get; private set; }
        public UnmanagedArray3<VoxelData> VoxelData { get; private set; }
        public VoxelChunkLightData LightData { get; private set; }

        public VoxelTypeIndex Index { get; private set; }

        public bool WasDisposed { get; private set; }

        public VoxelChunk(VoxelVolume volume, Coords coords) {
            Volume = volume;
            Coords = coords;
            Index = Volume.Index;
            VoxelData = new UnmanagedArray3<VoxelData>();
            Voxels = new IndexedVoxels(this);
            LightData = new VoxelChunkLightData();
        }

        ~VoxelChunk() {
            Dispose();
        }

        void CheckDisposed() {
            if (WasDisposed) throw new ObjectDisposedException(GetType().Name);
        }

        public unsafe void ReadBinary(BinaryReader reader) {
            var curr = VoxelData[0];
            var end = curr + SIZE * SIZE * SIZE;
            while (curr < end) {
                ushort header = reader.ReadUInt16();
                if (header == 0) {
                    // read data packet
                    ushort data = reader.ReadUInt16();
                    while (data != 0xFFFF) {
                        curr->TypeIndex = data;
                        data = reader.ReadUInt16();
                        curr ++;
                    }
                }
                else {
                    // read rle packet
                    ushort type = reader.ReadUInt16();
                    for (int i = 0; i < header; i ++) {
                        curr->TypeIndex = type;
                        curr ++;
                    }
                }
            }
            curr = VoxelData[0];
            while (curr < end) {
                var zcount = reader.ReadUInt16();
                while (zcount > 0) {
                    curr->Data = 0;
                    curr ++;
                    zcount --;
                }
                if (curr < end) {
                    ushort data = reader.ReadUInt16();
                    while (curr < end && data != 0) {
                        curr->Data = data;
                        curr ++;
                        data = reader.ReadUInt16();
                    }
                }
            }
        }

        public unsafe void WriteBinary(BinaryWriter writer) {
            var curr = VoxelData[0];
            var end = curr + SIZE * SIZE * SIZE;
            // write type info
            while (curr < end) {
                var next = curr + 1;
                if (next == end) {
                    // if this is the last voxel, just emit an RLE of length 1
                    writer.Write((ushort) 1);
                    writer.Write((ushort) curr->TypeIndex);
                    break;
                }
                if (next->TypeIndex == curr -> TypeIndex) {
                    // if the next one is the same, emit an RLE
                    var index = curr->TypeIndex;
                    int count = 0;
                    while (curr->TypeIndex == index && curr < end) {
                        count ++;
                        curr ++;
                    }
                    writer.Write((ushort) count);
                    writer.Write((ushort) index);
                }
                else {
                    // if the next one is different, emit data until the next one matches
                    writer.Write((ushort) 0);
                    writer.Write(curr->TypeIndex);
                    while (curr->TypeIndex != next->TypeIndex && next < end) {
                        writer.Write(next->TypeIndex);
                        curr ++;
                        next ++;
                    }
                    curr = next;
                    writer.Write((ushort) 0xFFFF);
                }
            }
            // write data
            curr = VoxelData[0];
            while (curr < end) {
                ushort count = 0;
                while (curr->Data == 0 && curr < end) {
                    count ++;
                    curr ++;
                }
                writer.Write(count);
                if (curr < end) {
                    while (curr->Data != 0 && curr < end) {
                        writer.Write(curr->Data);
                        curr ++;
                    }
                    writer.Write((ushort) 0);
                }
            }
        }

        public override string ToString() {
            return $"Chunk {Coords}";
        }

        public void SetMesh(VoxelChunkMesh mesh) {
            if (WasDisposed) {
                mesh.Dispose();
            }
            if (!mesh.AreBuffersReady) {
                throw new ArgumentException($"Cannot update chunk mesh for chunk at {Coords}: Mesh buffers aren't ready!");
            }
            if (Mesh != null && Mesh != mesh) {
                Mesh.Dispose();
            }
            Mesh = mesh;
        }

        public void Dispose() {
            WasDisposed = true;
            VoxelData.Dispose();
            Mesh?.Dispose();
            LightData?.Dispose();
        }

        public static bool AreLocalCoordsInBounds(Coords c) {
            var min = 0;
            var max = SIZE - 1;
            return c.X >= min && c.X < max
                && c.Y >= min && c.Y < max
                && c.Z >= min && c.Z < max;
        }

        public Coords LocalToVolumeCoords(Coords c) 
            => Coords * SIZE + c;
        public Coords VolumeToLocalCoords(Coords c)
            => c - (Coords * SIZE);
        public Vector3 LocalToVolumeVector(Vector3 c) 
            => Coords * SIZE + c;
        public Vector3 VolumeToLocalVector(Vector3 c)
            => c - (Coords * SIZE);

        public unsafe Voxel GetVoxelIncludingNeighbors(Coords c) {
            var data = GetVoxelDataIncludingNeighbors(c);
            if (data == null) {
                return Voxel.Empty;
            }
            else {
                return new Voxel(*data, Index);
            }
        }
        public Voxel GetVoxelIncludingNeighbors(int i, int j, int k)
            => GetVoxelIncludingNeighbors(new Coords(i, j, k));

        public unsafe VoxelData* GetVoxelDataIncludingNeighbors(Coords c) {
            if (AreLocalCoordsInBounds(c)) {
                return VoxelData[c];
            }
            else {
                return Volume.GetVoxelData(LocalToVolumeCoords(c));
            }
        }
        public unsafe VoxelData* GetVoxelDataIncludingNeighbors(int i, int j, int k)
            => GetVoxelDataIncludingNeighbors(new Coords(i, j, k));

        public VoxelLight GetVoxelLightIncludingNeighbors(Coords c) {
            if (AreLocalCoordsInBounds(c)) {
                return LightData.GetVoxelLight(c);
            }
            else {
                return Volume?.GetVoxelLight(LocalToVolumeCoords(c)) ?? VoxelLight.NULL;
            }
        }
        public VoxelLight GetVoxelLightIncludingNeighbors(int i, int j, int k)
            => GetVoxelLightIncludingNeighbors(new Coords(i, j, k));
        
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(Coords c, int channel) {
            if (AreLocalCoordsInBounds(c)) {
                return LightData[channel][c];
            }
            else {
                return Volume.GetVoxelLightData(LocalToVolumeCoords(c), channel);
            }
        }
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(Coords c, VoxelLightChannel channel)
            => GetVoxelLightDataIncludingNeighbors(c, (int) channel);
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(int i, int j, int k, int channel)
            => GetVoxelLightDataIncludingNeighbors(new Coords(i, j, k), channel);
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(int i, int j, int k, VoxelLightChannel channel)
            => GetVoxelLightDataIncludingNeighbors(new Coords(i, j, k), (int) channel);

        /// <summary>
        /// Utility class to access voxels, auto translating from the stored indicies
        /// </summary>
        public class IndexedVoxels {
            
            VoxelTypeIndex _index;
            UnmanagedArray3<VoxelData> _data;

            public IndexedVoxels(VoxelChunk chunk) {
                _data = chunk.VoxelData;
                _index = chunk.Index;
            }

            public unsafe Voxel this[Coords c] {
                get => new Voxel(*_data[c], _index);
                set => *_data[c] = new VoxelData(_index.Add(value.Type), value.Data);
            }

            public unsafe Voxel this[int i, int j, int k] {
                get => new Voxel(*_data[i, j, k], _index);
                set => *_data[i, j, k] = new VoxelData(_index.Add(value.Type), value.Data);
            }


        }

        public unsafe class UnmanagedArray3<T> : IDisposable where T : unmanaged {

            public bool WasDisposed { get; private set; }

            T* data;

            public T* this[int x, int y, int z] {
                get {
                    CheckDisposed();
                    return &data[x + y * SIZE + z * SIZE * SIZE];
                }
            }
            public T* this[Coords c] {
                get {
                    CheckDisposed();
                    return &data[c.X + c.Y * SIZE + c.Z * SIZE * SIZE];
                }
            }
            public T* this[int offset] {
                get {
                    CheckDisposed();
                    return &data[offset];
                }
            }

            public UnmanagedArray3() {
                int size = Marshal.SizeOf<T>() * SIZE * SIZE * SIZE;
                data = (T*) Marshal.AllocHGlobal(size);
                while (size > 0) {
                    ((byte*) data)[--size] = 0;
                }
            }

            ~UnmanagedArray3() {
                Dispose();
            }

            public void Dispose() {
                WasDisposed = true;
                if (data != null) {
                    Marshal.FreeHGlobal((IntPtr) data);
                    data = null;
                }
            }

            void CheckDisposed() {
                if (WasDisposed) throw new ObjectDisposedException(GetType().Name);
            }

            public static int GetIndex(int x, int y, int z) => x + y * SIZE + z * SIZE * SIZE;
            public static int GetIndex(Coords c) => c.X + c.Y * SIZE + c.Z * SIZE * SIZE;

        }
    }

}