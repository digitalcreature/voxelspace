using System;

namespace VoxelSpace {

    public struct VoxelLight {

        public const byte MAX_LIGHT = 255;
        public static readonly VoxelLight fullSun = new VoxelLight() {
            sunXp = MAX_LIGHT,
            sunXn = MAX_LIGHT,
            sunYp = MAX_LIGHT,
            sunYn = MAX_LIGHT,
            sunZp = MAX_LIGHT,
            sunZn = MAX_LIGHT,
            point = 0,
            isValid = true
        };
        public static readonly VoxelLight INVALID = new VoxelLight() {
            isValid = false,
        };

        public byte sunXp;
        public byte sunYp;
        public byte sunZp;
        public byte sunXn;
        public byte sunYn;
        public byte sunZn;

        public byte point;
        public bool isValid;

    }

    public enum VoxelLightChannel {
        SunXp = 0,
        SunYp = 1,
        SunZp = 2,
        SunXn = 3,
        SunYn = 4,
        SunZn = 5,
        Point = 6,
    }

    // oh baby its SoA time
    public unsafe class VoxelChunkLightData : IDisposable {

        VoxelChunk.UnmanagedArray3<byte>[] data;

        public VoxelChunk.UnmanagedArray3<byte> this[VoxelLightChannel channel] => data[(int) channel];
        public VoxelChunk.UnmanagedArray3<byte> this[int channel] => data[channel];

        public VoxelChunkLightData() {
            data = new VoxelChunk.UnmanagedArray3<byte>[7];
            for (int l = 0; l < 7; l ++) {
                data[l] = new VoxelChunk.UnmanagedArray3<byte>();
            }
        }

        ~VoxelChunkLightData() {
            Dispose();
        }

        public void Dispose() {
            if (data != null) {
                foreach (var channel in data) {
                    channel.Dispose();
                }
                data = null;
            }
        }

        // only used when we need to convert to AoS. pretty much only used in VoxelChunkMeshGenerator
        public VoxelLight GetVoxelLight(Coords c) {
            int offset = VoxelChunk.UnmanagedArray3<byte>.GetIndex(c);
            return new VoxelLight() {
                sunXp = *data[0][offset],
                sunYp = *data[1][offset],
                sunZp = *data[2][offset],
                sunXn = *data[3][offset],
                sunYn = *data[4][offset],
                sunZn = *data[5][offset],
                point = *data[6][offset],
                isValid = true
            };
        }

    }

}