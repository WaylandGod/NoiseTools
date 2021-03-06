﻿using System;
using System.Drawing;

namespace NoiseTools.PerlinNoise
{
    /// <summary>
    /// This is just a static helper class to quickly fill a texture2D with values
    /// generated by the perlin noise function (CPU).
    /// </summary>
    public class PerlinNoiseTexture
    {
        public PerlinNoiseTexture(int seed)
        {
            this._noise = new PerlinNoise3D(new Random(seed));
        }

        public PerlinNoiseTexture(PerlinNoise3D noise)
        {
            this._noise = noise;
        }

        PerlinNoise3D _noise;

        public PerlinNoise3D Noise
        {
            get { return _noise; }
            set { _noise = value; }
        }

        int octaves = 8;
        double lacunarity = 2.85;
        double gain = 0.45;
        double offset = 1.0;

        public int Octaves
        {
            get { return octaves; }
            set { octaves = value; }
        }

        public double Lacunarity
        {
            get { return lacunarity; }
            set { lacunarity = value; }
        }

        public double Gain
        {
            get { return gain; }
            set { gain = value; }
        }

        public double Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        private Color[][] PrepareColorArray(int x, int y)
        {
            var data = new Color[x][];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Color[y];
            }
            return data;
        }

        public Color[][] FillRectangle(int width, int height, double offsetX, double offsetY, PerlinOptions options)
        {
            float red = options.RandomColor.Next(options.MinRed, options.MaxRed) / options.FactorRed;
            float green = options.RandomColor.Next(options.MinGreen, options.MaxGreen) / options.FactorGreen;
            float blue = options.RandomColor.Next(options.MinBlue, options.MaxBlue) / options.FactorBlue;

            Color[][] data = PrepareColorArray(width, height);
            double x = 0;
            double y = 0;
            double offX = x;


            for (int v = 0; v < width; v++)
            {
                y += options.PerlinNoiseStep;
                x = offX;

                for (int u = 0; u < height; u++)
                {
                    float noise = (float)Noise.RidgedMF(offsetX + x, offsetY + y, 0, options.Octaves, options.Lacunarity, options.Gain, options.Offset);
                    data[v][u] = ModifyColor(noise, options, red, green, blue);

                    x += options.PerlinNoiseStep;
                }
            }

            return data;
        }

        public Color[][] FillCircle(int width, double offsetX, double offsetY, PerlinOptions options)
        {
            float red = options.RandomColor.Next(options.MinRed, options.MaxRed) / options.FactorRed;
            float green = options.RandomColor.Next(options.MinGreen, options.MaxGreen) / options.FactorGreen;
            float blue = options.RandomColor.Next(options.MinBlue, options.MaxBlue) / options.FactorBlue;

            var data = PrepareColorArray(width, width);

            int r = width / 2; // radius
            int ox = r, oy = r; // origin

            for (int x = -r; x < r; x++)
            {
                int height = (int)Math.Sqrt(r * r - x * x);

                for (int y = -height; y < height; y++)
                {
                    float noise = (float)Noise.RidgedMF(
                        offsetX + (x + ox) * options.PerlinNoiseStep, offsetY + (y + oy) * options.PerlinNoiseStep,
                        0, options.Octaves, options.Lacunarity, options.Gain, options.Offset);
                    data[x + ox][y + oy] = ModifyColor(noise, options, red, green, blue);
                }
            }
            return data;
        }

        public Color ModifyColor(float noise, PerlinOptions options, float red, float green, float blue)
        {
            int colorRed = 0;
            int colorGreen = 0;
            int colorBlue = 0;

            colorRed = ColorFloatToInt(noise * red);
            if (options.UseCosineOnRed)
            {
                colorRed = ColorFloatToInt((float)Math.Cos(red / noise));
            }

            colorGreen = ColorFloatToInt(noise * green);
            if (options.UseCosineOnGreen)
            {
                colorGreen = ColorFloatToInt((float)Math.Cos(green / noise));
            }

            colorBlue = ColorFloatToInt(noise * blue);
            if (options.UseCosineOnBlue)
            {
                colorBlue = ColorFloatToInt((float)Math.Cos(blue / noise));
            }

            if (options.ReverseRed)
                colorRed = 255 - colorRed;

            if (options.ReverseGreen)
                colorGreen = 255 - colorGreen;

            if (options.ReverseBlue)
                colorBlue = 255 - colorBlue;

            return Color.FromArgb(255, colorRed, colorGreen, colorBlue);
        }

        private static int ColorFloatToInt(float value)
        {
            return Math.Min(Math.Max((int)(255 * value), 0), 255);
        }
    }
}
