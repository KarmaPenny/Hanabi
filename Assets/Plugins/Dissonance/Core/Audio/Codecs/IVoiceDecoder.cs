﻿using System;
using NAudio.Wave;

namespace Dissonance.Audio.Codecs
{
    internal interface IVoiceDecoder
        : IDisposable
    {
        [NotNull] WaveFormat Format { get; }

        void Reset();

        int Decode(ArraySegment<byte>? input, ArraySegment<float> output);
    }
}
