using System;

namespace VoxelSpace {

    public readonly struct VoxelLight {

        public const int CHANNEL_COUNT = 6; //7; // we are only concerned with sunlight rn

        public const byte MAX_LIGHT = 255;
        public static readonly VoxelLight fullSun = new VoxelLight(
            MAX_LIGHT,
            MAX_LIGHT,
            MAX_LIGHT,
            MAX_LIGHT,
            MAX_LIGHT,
            MAX_LIGHT,
            0,
            true
        );
        public static readonly VoxelLight INVALID = new VoxelLight(0, 0, 0, 0, 0, 0, 0, false);

        public readonly byte SunXp;
        public readonly byte SunYp;
        public readonly byte SunZp;
        public readonly byte SunXn;
        public readonly byte SunYn;
        public readonly byte SunZn;

        public readonly byte Point;
        public readonly bool IsValid;

        public VoxelLight(byte sunXp, byte sunYp, byte sunZp, byte sunXn, byte sunYn, byte sunZn, byte point, bool isValid) {
            SunXp = sunXp;
            SunYp = sunYp;
            SunZp = sunZp;
            SunXn = sunXn;
            SunYn = sunYn;
            SunZn = sunZn;

            Point = point;
            IsValid = isValid;
        }
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
            return new VoxelLight(
                *data[0][offset],
                *data[1][offset],
                *data[2][offset],
                *data[3][offset],
                *data[4][offset],
                *data[5][offset],
                *data[6][offset],
                true
            );
        }

    }

}