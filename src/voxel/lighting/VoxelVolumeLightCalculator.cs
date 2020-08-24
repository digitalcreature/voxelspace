using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {
    
    // use to calculate the lights after voxel volume generation/deserialization
    public class VoxelVolumeLightCalculator : VoxelVolumeProcessor {

        public VoxelVolumeLightCalculator() : base() {}

        protected override Task StartTask() {
            return Task.Factory.StartNew(() => {
                Input.WaitForAllChunks();
                var propagator = new VoxelLightPropagator(Volume);
                Parallel.For(0, 6, (i) => CalculateSunlight(Volume, i, propagator[i]));
                EmitRemainingChunks();
            });
        }

        // seeds sunlight over an entire volume, queueing nodes for propogation
        unsafe void CalculateSunlight(VoxelVolume volume, int channel, VoxelLightPropagator.IChannel propagationChannel) {
            var cRegion = volume.ChunkRegion;
            int axis = channel % 3; // x = 0, y = 1, z = 2
            var neg = channel >= 3;
            int ai = (axis + 1) % 3;
            int bi = (axis + 2) % 3;
            int ci = axis;
            int minA, minB, minC;
            int maxA, maxB, maxC;
            minA = (&cRegion.Min.X)[ai];
            maxA = (&cRegion.Max.X)[ai];
            minB = (&cRegion.Min.X)[bi];
            maxB = (&cRegion.Max.X)[bi];
            int incr = neg ? -1 : 1;
            Coords lCoords = Coords.Zero;
            if (neg) {
                minC = (&cRegion.Max.X)[ci] - 1;
                maxC = (&cRegion.Min.X)[ci] - 1;
                (&lCoords.X)[ci] = VoxelChunk.SIZE - 1;
            }
            else {
                minC = (&cRegion.Min.X)[ci];
                maxC = (&cRegion.Max.X)[ci];
                (&lCoords.X)[ci] = 0;
            }
            Coords cCoords = Coords.Zero;
            VoxelChunk chunk;
            for (int ca = minA; ca < maxA; ca ++) {
                for (int cb = minB; cb < maxB; cb ++) {
                    (&cCoords.X)[ai] = ca;
                    (&cCoords.X)[bi] = cb;
                    for (int cc = minC; cc != maxC; cc += incr) {
                        (&cCoords.X)[ci] = cc;
                        chunk = volume[cCoords];
                        if (chunk != null) {
                            for (int la = 0; la < VoxelChunk.SIZE; la ++) {
                                for (int lb = 0; lb < VoxelChunk.SIZE; lb ++) {
                                    (&lCoords.X)[ai] = la;
                                    (&lCoords.X)[bi] = lb;
                                    *chunk.LightData[channel][lCoords] = VoxelLight.MAX_LIGHT;
                                    propagationChannel.QueueForPropagation(chunk, lCoords);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            propagationChannel.PropagateSunlight();
        }

    }

}
