﻿using System.IO;
using OpenSage.Content;
using OpenSage.Data.StreamFS;
using OpenSage.FileFormats;

namespace OpenSage.Audio
{
    public sealed class AudioFile : IHasName
    {
        internal static AudioFile ParseAsset(BinaryReader reader, Asset asset)
        {
            return new AudioFile
            {
                Name = asset.Name,
                Subtitle = reader.ReadUInt32PrefixedAsciiStringAtOffset(),
                NumberOfSamples = reader.ReadInt32(),
                SampleRate = reader.ReadInt32(),
                HeaderDataOffset = reader.ReadInt32(),
                HeaderDataSize = reader.ReadInt32(),
                NumberOfChannels = reader.ReadByte()
            };
        }

        public string Name { get; private set; }
        public string Subtitle { get; private set; }
        public byte NumberOfChannels { get; private set; }
        public int NumberOfSamples { get; private set; }
        public int SampleRate { get; private set; }
        public int HeaderDataOffset { get; private set; }
        public int HeaderDataSize { get; private set; }
    }
}