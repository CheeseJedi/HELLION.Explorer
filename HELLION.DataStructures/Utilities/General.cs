﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace HELLION.DataStructures.Utilities
{
    /// <summary>
    /// A class to hold miscellaneous static utility functions not complex enough to warrant their 
    /// own individual class.
    /// </summary>
    public static class General
    {
        // Fonts in .Forms TreeViews are BROKEN! Memory leaks aplenty.
        // public static Font fntRegular = new Font(familyName: "Segoe UI", emSize: 9.75f, style: FontStyle.Regular);
        // public static Font fntItalic = new Font(familyName: "Segoe UI", emSize: 9.75f, style: FontStyle.Italic);

        /// <summary>
        /// Returns a System.Drawing.Color object for a given string, computed from the hash of the string.
        /// </summary>
        /// <param name="sInputString">String to generate a colour for</param>
        /// <returns></returns>
        public static Color ConvertStringToColor(string sInputString)
        {
            int iHue = (Math.Abs(sInputString.GetHashCode()) % 24) * 10;
            // Debug.Print("iHue: {0}", iHue.ToString());

            HSLColor hslColor = new HSLColor(hue: iHue, saturation: 200.0, luminosity: 80.0);
            return hslColor;
        }

        /// <summary>
        /// Based on an example from https://stackoverflow.com/questions/5427020/prompt-dialog-in-windows-forms
        /// </summary>
        public static class Prompt
        {
            public static string ShowDialog(string text, string caption, bool showAdditionalCheckBox = false, Icon icon = null)
            {
                Form prompt = new Form()
                {
                    Width = 500,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    MinimizeBox = false,
                    MaximizeBox = false,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };

                // Set the form icon if one has been passed.
                if (icon != null) prompt.Icon = icon;


                Label textLabel = new Label() { Left = 50, Top = 20, Width = 400, Text = text };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                CheckBox checkBox = new CheckBox() { Left = 50, Top = 70, Width = 15 };
                Label textLabel2 = new Label() { Left = 70, Top = 75, Width = 200, Text = "Other search function" };
                Button confirmation = new Button() { Text = "OK", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                if (showAdditionalCheckBox) prompt.Controls.Add(checkBox);
                if (showAdditionalCheckBox) prompt.Controls.Add(textLabel2);

                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
            }
        }

        /// <summary>
        /// HSLColor Class by Rich Newman
        /// https://richnewman.wordpress.com/about/code-listings-and-diagrams/hslcolor-class/
        /// </summary>
        public class HSLColor
        {
            // Private data members below are on scale 0-1
            // They are scaled for use externally based on scale
            private double hue = 1.0;
            private double saturation = 1.0;
            private double luminosity = 1.0;

            private const double scale = 240.0;

            public double Hue
            {
                get { return hue * scale; }
                set { hue = CheckRange(value / scale); }
            }
            public double Saturation
            {
                get { return saturation * scale; }
                set { saturation = CheckRange(value / scale); }
            }
            public double Luminosity
            {
                get { return luminosity * scale; }
                set { luminosity = CheckRange(value / scale); }
            }

            private double CheckRange(double value)
            {
                if (value < 0.0)
                    value = 0.0;
                else if (value > 1.0)
                    value = 1.0;
                return value;
            }

            public override string ToString()
            {
                return string.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
            }

            public string ToRGBString()
            {
                Color color = (Color)this;
                return string.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
            }

            #region Casts to/from System.Drawing.Color
            public static implicit operator Color(HSLColor hslColor)
            {
                double r = 0, g = 0, b = 0;
                if (hslColor.luminosity != 0)
                {
                    if (hslColor.saturation == 0)
                        r = g = b = hslColor.luminosity;
                    else
                    {
                        double temp2 = GetTemp2(hslColor);
                        double temp1 = 2.0 * hslColor.luminosity - temp2;

                        r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
                        g = GetColorComponent(temp1, temp2, hslColor.hue);
                        b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
                    }
                }
                return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
            }

            private static double GetColorComponent(double temp1, double temp2, double temp3)
            {
                temp3 = MoveIntoRange(temp3);
                if (temp3 < 1.0 / 6.0)
                    return temp1 + (temp2 - temp1) * 6.0 * temp3;
                else if (temp3 < 0.5)
                    return temp2;
                else if (temp3 < 2.0 / 3.0)
                    return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
                else
                    return temp1;
            }
            private static double MoveIntoRange(double temp3)
            {
                if (temp3 < 0.0)
                    temp3 += 1.0;
                else if (temp3 > 1.0)
                    temp3 -= 1.0;
                return temp3;
            }
            private static double GetTemp2(HSLColor hslColor)
            {
                double temp2;
                if (hslColor.luminosity < 0.5)  //<=??
                    temp2 = hslColor.luminosity * (1.0 + hslColor.saturation);
                else
                    temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity * hslColor.saturation);
                return temp2;
            }

            public static implicit operator HSLColor(Color color)
            {
                HSLColor hslColor = new HSLColor()
                {
                    hue = color.GetHue() / 360.0, // we store hue as 0-1 as opposed to 0-360 
                    luminosity = color.GetBrightness(),
                    saturation = color.GetSaturation()
                };
                return hslColor;
            }
            #endregion

            public void SetRGB(int red, int green, int blue)
            {
                HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
                this.hue = hslColor.hue;
                this.saturation = hslColor.saturation;
                this.luminosity = hslColor.luminosity;
            }

            public HSLColor() { }
            public HSLColor(Color color)
            {
                SetRGB(color.R, color.G, color.B);
            }
            public HSLColor(int red, int green, int blue)
            {
                SetRGB(red, green, blue);
            }
            public HSLColor(double hue, double saturation, double luminosity)
            {
                this.Hue = hue;
                this.Saturation = saturation;
                this.Luminosity = luminosity;
            }

        } // End of HSLColor

    }
}
