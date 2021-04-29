using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Scacchi_Simpatici_Bottazzo_Angelo
{
    class CosoCheFaColori
    {
        double[] rgb;
        int cosaCambiare;
        public double passo;
        bool su;

        public CosoCheFaColori(double? passo = 5)
        {
            this.passo = (double)passo;
            rgb = new double[] { 255, 0, 0 };

            cosaCambiare = 1;

            su = true;
        }

        public Color Successivo()
        {
            if (su)
            {
                rgb[cosaCambiare % 3] += passo;

                if (rgb[cosaCambiare % 3] >= 255)
                {
                    rgb[cosaCambiare % 3] = 255;
                    cosaCambiare--;
                    su = false;
                }
            }
            else
            {
                rgb[cosaCambiare % 3] -= passo;


                if (rgb[cosaCambiare % 3] <= 0)
                {
                    rgb[cosaCambiare % 3] = 0;
                    cosaCambiare += 2;
                    su = true;
                }
            }

            return Color.FromRgb((byte)(int)rgb[0], (byte)(int)rgb[1], (byte)(int)rgb[2]);
        }
    }
}
