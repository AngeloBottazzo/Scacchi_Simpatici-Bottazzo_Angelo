using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Scacchi_Simpatici_Bottazzo_Angelo
{
    public class Cella : Button
    {
        public Scacchiera Contenitore { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public List<Cella> Vicini = new List<Cella>();
        public bool Passato { get; set; } = false;

        public Cella()
        {

        }

        public Cella(Scacchiera contenitore, int x, int y)
        {
            Click += Cella_Click;

            Contenitore = contenitore;
            X = x;
            Y = y;

            Contenitore.Contenitore.Children.Add(this);
            Grid.SetRow(this, x);
            Grid.SetColumn(this, y);

            MinWidth = 25;
            MinHeight = 25;
        }

        private void Cella_Click(object sender, RoutedEventArgs e)
        {
            Contenitore.Pulisci();
            Contenitore.Partenza = this;
        }
    }

}
