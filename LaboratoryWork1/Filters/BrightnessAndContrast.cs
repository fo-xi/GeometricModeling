using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork1
{
	public class BrightnessAndContrast : DotFilter
	{
		public const int CONTRAST_MEDIAN = 159;

		public bool Init(int b_offset, int c_offset)
		{
			int i = 0,  // transformation table color index
				t = 0,  // table index
				t_index = 0, // index of the color corresponding to the lower brightness boundary
				b_index = 0, // index of the color corresponding to the upper brightness boundary
				value_offset; // color value offset

			double value = .0;  // new color value

			// changing brightness
            for (t = 0; t < 3; t++)
            {
                for (i = 0; i < 256; i++)
                {
                    if (i + b_offset > 255)
                    {
                        BGRTransTable[t, i] = 255;
					}
                    else if (i + b_offset < 0)
                    {
                        BGRTransTable[t, i] = 0;
					}
                    else
                    {
                        BGRTransTable[t, i] = (byte)(i + b_offset);
                    }

                }
            }

            // Changing contrast
            if (c_offset < 0) // reducing contrast
            {
                for (i = 0, t = 0; t < 3; t++)
                {
                    for (i = 0; i < 256; i++)
                    {
                        if (BGRTransTable[t, i] < CONTRAST_MEDIAN)
                        {
                            // Calculating offset depending on the color's distance
                            // from the gray median from below
                            value_offset = (CONTRAST_MEDIAN - BGRTransTable[t, i]) * c_offset / 128;
                            if (BGRTransTable[t, i] - value_offset > CONTRAST_MEDIAN)
                            {
                                BGRTransTable[t, i] = CONTRAST_MEDIAN;
							}
                            else
                            {
                                BGRTransTable[t, i] = (byte)(BGRTransTable[t, i] - value_offset);
                            }
                        }
                        else
                        {
                            // Calculating offset depending on the color's distance 
                            // from the gray median from above
                            value_offset = (BGRTransTable[t, i] - CONTRAST_MEDIAN) * c_offset / 128;
                            if (BGRTransTable[t, i] + value_offset < CONTRAST_MEDIAN)
                            {
                                BGRTransTable[t, i] = CONTRAST_MEDIAN;
							}
                            else
                            {
                                BGRTransTable[t, i] = (byte)(value_offset + BGRTransTable[t, i]);
                            }
                        }
                    }
                }
            }
            else if (c_offset > 0)
			// Increasing contrast
			{
				// Calculating lower color boundary
				int offset_b = c_offset * CONTRAST_MEDIAN / 128;
				// All table values below the lower boundary will be given value 0
                for (t = 0; t < 3; t++)
                {
                    for (b_index = 0; b_index < 256; b_index++)
                    {
                        if (BGRTransTable[t, b_index] < offset_b)
                        {
                            BGRTransTable[t, b_index] = 0;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Calculating upper color boundary
				int offset_t = c_offset * 128 / CONTRAST_MEDIAN;
				// All table values above the upper boundary will be given value 255
                for (t = 0; t < 3; t++)
                {
                    for (t_index = 255; t_index >= 0; t_index--)
                    {
                        if (BGRTransTable[t, t_index] + offset_t > 255)
                        {
                            BGRTransTable[t, t_index] = 255;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Calculating color intensity change step
				double step = 256.0 / (256 - (offset_b + offset_t));
				// Stretching color intensity between the lower and upper boundaries
				// to cover the entire 0 to 255 range.
				for (t = 0; t < 3; t++)
				{
					value = .0;
					for (i = b_index; i <= t_index; i++)
					{
						if (BGRTransTable[t, i] >= offset_b || BGRTransTable[t, i] < 256 - offset_t)
						{
							value = (int)((BGRTransTable[t, i] - offset_b) * step + 0.5);
                            if (value > 255)
                            {
                                value = 255;
                            }

							BGRTransTable[t, i] = (byte)(value);
						}
					}
				}
			}

			return true;
		}
	}
}
