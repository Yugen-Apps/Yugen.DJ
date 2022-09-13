﻿using AudioVisualizer;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Yugen.DJ.ViewModels
{
    public class VUBarVieModel : ObservableObject
    {
        private IVisualizationSource _source;

        public IVisualizationSource Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        public Vector3 ElementShadowOffset => new Vector3(2, 2, -10);

        public float ElementShadowBlurRadius => 5;

        public Color ElementShadowColor => Colors.Red;

        public MeterBarLevel[] Levels
        {
            get
            {
                // Create bar steps with 1db steps from -86db to +6
                const int fromDb = -86;
                const int toDb = 6;
                MeterBarLevel[] levels = new MeterBarLevel[toDb - fromDb];

                for (int i = 0; i < levels.Count(); i++)
                {
                    float ratio = (float)i / (float)levels.Count();
                    levels[i].Color = Toolkit.Uwp.Helpers.ColorHelper.GradientColor(ratio);
                    levels[i].Level = i + fromDb;
                }

                return levels;
            }
        }
    }
}