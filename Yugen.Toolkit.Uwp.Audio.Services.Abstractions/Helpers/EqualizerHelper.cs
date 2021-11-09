using System.Collections.Generic;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions.Helpers
{
    public class EqualizerHelper
    {
        public static readonly float[] EqDefaultValues = {
            50f,
            200f,
            800f,
            3200f,
            12800f
        };

        public static IEnumerable<EqualizerBand> ListBands()
        {
            var list = new List<EqualizerBand>();

            for (int i = 0; i < EqDefaultValues.Length; i++)
            {
                var band = new EqualizerBand(i, EqDefaultValues[i]);

                list.Add(band);
            }

            return list;
        }

        public static IEnumerable<EqualizerConfig> ListConfigs = new EqualizerConfig[]
        {
            _custom,
            _flat,
            _rock,
            _classical,
            _club,
            _dance,
            _fullBass,
            _fullBassAndTreble,
            _fulltreble,
            _headphones,
            _largehall,
            _live,
            _party,
            _pop
        };

        private static readonly EqualizerConfig _flat = new EqualizerConfig
        {
            Name = "Flat",
            Values = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        private static readonly EqualizerConfig _classical = new EqualizerConfig
        {
            Name = "Classical",
            Values = new float[] { 0, 0, 0, 0, 0, 0, -18, -18, -18, -24 }
        };

        private static readonly EqualizerConfig _club = new EqualizerConfig
        {
            Name = "Club",
            Values = new float[] { 0, 0, 20, 14, 14, 14, 8, 0, 0, 0 }
        };

        private static readonly EqualizerConfig _dance = new EqualizerConfig
        {
            Name = "Dance",
            Values = new float[] { 24, 18, 6, 0, 0, -14, -18, -18, 0, 0 }
        };

        private static readonly EqualizerConfig _fullBass = new EqualizerConfig
        {
            Name = "Full Bass",
            Values = new[] { -20, 24, 24, 14, -4, -1, -20, -25.75f, -28, -28 }
        };

        private static readonly EqualizerConfig _fullBassAndTreble = new EqualizerConfig
        {
            Name = "Full B&T",
            Values = new float[] { 18, 14, 0, -18, -12, 4, 20, 28, 30, 30 }
        };

        private static readonly EqualizerConfig _fulltreble = new EqualizerConfig
        {
            Name = "Full Treble",
            Values = new float[] { -24, -24, -24, -10, 6, 28, 40, 40, 40, 41.75F }
        };

        private static readonly EqualizerConfig _headphones = new EqualizerConfig
        {
            Name = "Headphones",
            Values = new float[] { 12, 28, 14, -8, -6, 4, 12, 24, 32, 36 }
        };

        private static readonly EqualizerConfig _largehall = new EqualizerConfig
        {
            Name = "Large Hall",
            Values = new float[] { 25.75F, 25.75F, 14, 14, 0, -12, -12, -12, 0, 0 }
        };

        private static readonly EqualizerConfig _live = new EqualizerConfig
        {
            Name = "Live",
            Values = new float[] { -12, 0, 10, 14, 14, 14, 10, 6, 6, 6 }
        };

        private static readonly EqualizerConfig _party = new EqualizerConfig
        {
            Name = "Party",
            Values = new float[] { 18, 18, 0, 0, 0, 0, 0, 0, 18, 18 }
        };

        private static readonly EqualizerConfig _pop = new EqualizerConfig
        {
            Name = "Pop",
            Values = new float[] { -4, 12, 18, 20, 14, 0, -6, -6, -4, -4 }
        };

        private static readonly EqualizerConfig _rock = new EqualizerConfig
        {
            Name = "Rock",
            Values = new float[] { 20, 12, -14, -20, -8, -10, 22, 28, 28, 28 }
        };

        private static readonly EqualizerConfig _custom = new EqualizerConfig
        {
            Name = "Custom",
            Values = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };
    }
}