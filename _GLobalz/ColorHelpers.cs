using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VC_WATERCRAFT._GLobalz
{
    public static class ColorHelpers
    {


        /// <summary>
        /// Generates a list of distinct hues to be used as base colors.
        /// </summary>
        /// <param name="count">Number of distinct hues needed.</param>
        /// <returns>A list of hue values that are far apart.</returns>
        public static List<double> GetDistinctHues(int count) // Changed to public
        {
            List<double> hues = new List<double>();
            double step = 360.0 / count; // Distribute hues evenly across the color wheel
            for (int i = 0; i < count; i++)
            {
                hues.Add(i * step);
            }

            return hues;
        }

        /// <summary>
        /// Generates a gradient of colors from a base color by varying saturation.
        /// </summary>
        /// <param name="baseColor">The base color for the gradient.</param>
        /// <param name="steps">Number of gradient steps.</param>
        /// <returns>A list of colors forming the gradient.</returns>
        public static List<Color> PUB_used_Gen_WithListParam(Color baseColor, int steps)
        {
            List<Color> gradient = new List<Color>();

            // Generate gradient by varying the saturation from 1.0 (full) to 0.1 (10%)
            for (int i = 0; i < steps; i++)
            {
                double saturation = 1.0 - 0.9 * (i / (double)(steps - 1)); // Vary saturation from 1.0 to 0.1
                Color gradientColor = ColorFromHSV(baseColor.GetHue(), saturation, baseColor.GetBrightness());
                gradient.Add(gradientColor);
            }

            return gradient;
        }

        /// <summary>
        /// Converts HSV values to a Color object.
        /// </summary>
        /// <param name="hue">The hue (0-360).</param>
        /// <param name="saturation">The saturation (0-1).</param>
        /// <param name="value">The brightness value (0-1).</param>
        /// <returns>A Color object corresponding to the HSV values.</returns>
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0:
                    return Color.FromArgb(255, v, t, p);
                case 1:
                    return Color.FromArgb(255, q, v, p);
                case 2:
                    return Color.FromArgb(255, p, v, t);
                case 3:
                    return Color.FromArgb(255, p, q, v);
                case 4:
                    return Color.FromArgb(255, t, p, v);
                default:
                    return Color.FromArgb(255, v, p, q);
            }
        }






    }
}
