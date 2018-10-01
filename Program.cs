using System;
using Substrate;
using Substrate.Core;
using Substrate.Nbt;

// This example will reset and rebuild the lighting (heightmap, block light,
// skylight) for all chunks in a map.

// Note: If it looks silly to reset the lighting, loading and saving
// all the chunks, just to load and save them again later: it's not.
// If the world lighting is not correct, it must be completely reset
// before rebuilding the light in any chunks.  That's just how the
// algorithms work, in order to limit the number of chunks that must
// be loaded at any given time.

namespace Relight
{
    class Program
    {
        static void Main (string[] args)
        {
            if (args.Length < 1) {
                Console.WriteLine("You must specify a target directory");
                return;
            }
            string dest = args[0];
            /*
            NbtVerifier.InvalidTagType += (e) => {
                throw new Exception("Invalid Tag Type: " + e.TagName + " [" + e.Tag + "]");
            };
            NbtVerifier.InvalidTagValue += (e) => {
                throw new Exception("Invalid Tag Value: " + e.TagName + " [" + e.Tag + "]");
            };
            NbtVerifier.MissingTag += (e) => {
                throw new Exception("Missing Tag: " + e.TagName);
            };
            */

            // Opening an NbtWorld will try to autodetect if a world is Alpha-style or Beta-style
            Console.WriteLine("AnvilWorld.Open(dest)");
            AnvilWorld world = AnvilWorld.Open(dest);

            // Grab a generic chunk manager reference
            Console.WriteLine("world.GetChunkManager()");
            IChunkManager cm = world.GetChunkManager();

            // Iterate through all the chunks
            foreach (ChunkRef chunk in cm) {
                // Check if Ocean chunk
                bool foundocean = false;
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        if (chunk.Biomes.GetBiome(x, z) == BiomeType.Ocean || chunk.Biomes.GetBiome(x, z) == BiomeType.DeepOcean || chunk.Biomes.GetBiome(x, z) == BiomeType.FrozenOcean) {
                            foundocean = true;
                        }
                    }
                }
                // Or if old unpopulated chunk
                //set status as liquid_carved (or similar)
                if (foundocean == true) {
                    Console.WriteLine("Found ocean in chunk {0},{1}, setting status to liquid_carved", chunk.X, chunk.Z);
                    chunk.Status = "liquid_carved";
                }

                cm.Save();

                //Console.WriteLine("Repopulated Chunk {0},{1}", chunk.X, chunk.Z);
            }
        }
    }
}
