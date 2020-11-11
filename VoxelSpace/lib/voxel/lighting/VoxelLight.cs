using System;

namespace VoxelSpace {

    /// <summary>
    /// Light values for a single voxel.
    /// </summary>
    public readonly struct VoxelLight {

        /// <summary>
        /// The total number of channels. See <see cref="VoxelLightChannel"/>
        /// </summary>
        public const int CHANNEL_COUNT = 7;
        
        /// <summary>
        /// The amount by which to decrement light during propagation.
        /// </summary>
        public const byte LIGHT_DECREMENT = MAX_LIGHT / 16;

        /// <summary>
        /// The maximum value for all channels.
        /// </summary>
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

        /// <summary>
        /// A NULL value used in place of a Nullable. saves memory, lets us use refs and pointers
        /// </summary>
        public static readonly VoxelLight NULL = new VoxelLight(0, 0, 0, 0, 0, 0, 0, false);

        /// <summary>Sunlight from X+</summary>
        public readonly byte SunXp;
        /// <summary>Sunlight from Y+</summary>
        public readonly byte SunYp;
        /// <summary>Sunlight from Z+</summary>
        public readonly byte SunZp;
        /// <summary>Sunlight from X-</summary>
        public readonly byte SunXn;
        /// <summary>Sunlight from Y-</summary>
        public readonly byte SunYn;
        /// <summary>Sunlight from Z-</summary>
        public readonly byte SunZn;

        /// <summary>Light from point lights.</summary>
        public readonly byte Point;
        /// <summary>Is this NULL?</summary>
        public readonly bool IsNonNull;

        public VoxelLight(byte sunXp, byte sunYp, byte sunZp, byte sunXn, byte sunYn, byte sunZn, byte point, bool isNonNull) {
            SunXp = sunXp;
            SunYp = sunYp;
            SunZp = sunZp;
            SunXn = sunXn;
            SunYn = sunYn;
            SunZn = sunZn;

            Point = point;
            IsNonNull = isNonNull;
        }
    }

    /// <summary>
    /// Light data is stored in multiple channels.
    /// 6 for sunlight on all 3 axes +/-.
    /// 1 for point lights.
    /// </summary>
    public enum VoxelLightChannel {
        SunXp = 0,
        SunYp = 1,
        SunZp = 2,
        SunXn = 3,
        SunYn = 4,
        SunZn = 5,
        Point = 6,
    }

    /// <summary>
    /// Light data for an entire chunk.
    /// SoA is used here for more effecient propgation code, as each channel propagates independantly.
    /// </summary>
    public unsafe class VoxelChunkLightData : IDisposable {

        VoxelChunk.UnmanagedArray3<byte>[] data;

        /// <summary>Retrieve the data for a specific channel.</summary>
        public VoxelChunk.UnmanagedArray3<byte> this[VoxelLightChannel channel] => data[(int) channel];
        /// <summary>Retrieve the data for a specific channel.</summary>
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
            }
        }

        /// <summary>
        /// Get a struct representing the light data for a specific voxel.
        /// We need this when generating light meshes!
        /// </summary>
        /// <param name="coords">Local coords to a voxel in this chunk</param>
        /// <returns></returns>
        public VoxelLight GetVoxelLight(Coords coords) {
            int offset = VoxelChunk.UnmanagedArray3<byte>.GetIndex(coords);
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