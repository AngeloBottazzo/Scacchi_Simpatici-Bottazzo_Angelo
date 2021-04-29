using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scacchi_Simpatici_Bottazzo_Angelo
{
    public class Scacchiera
    {
        public Grid Contenitore { get; private set; }
        Cella[,] celle;

        private Cella partenza;
        public Cella Partenza
        {
            get { return partenza; }
            set
            {
                partenza = value;
                if (partenza != null)
                {
                    partenza.Content = "P";
                    partenza.Background = Brushes.Red;
                }
            }
        }

        public void Pulisci(bool togliVicini = true)
        {
            foreach (Cella cella in CelleSrotolate)
            {
                cella.ClearValue(Button.BackgroundProperty);
                cella.ClearValue(Button.ContentProperty);
                cella.Passato = false;
                if (togliVicini)
                    cella.Vicini.Clear();
            }
        }

        public List<Cella> CelleSrotolate = new List<Cella>();


        public Scacchiera(Grid contenitore, int x, int y)
        {
            Contenitore = contenitore;
            contenitore.RowDefinitions.Clear();
            for (int i = 0; i < x; i++)
            {
                contenitore.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            contenitore.ColumnDefinitions.Clear();
            for (int i = 0; i < y; i++)
            {
                contenitore.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            celle = new Cella[x, y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Cella nuova = new Cella(this, i, j);
                    celle[i, j] = nuova;
                    CelleSrotolate.Add(nuova);
                }
            }

        }

        public async Task RisolviCavalli()
        {
            if (Partenza == null)
            {
                MessageBox.Show("Scegli una partenza dalla scacchiera");
                return;
            }

            for (int i = 0; i < celle.GetLength(0); i++)
            {
                for (int j = 0; j < celle.GetLength(1); j++)
                {
                    //creo collegamenti
                    for (int orizzontale = -1; orizzontale <= 1; orizzontale += 2)
                    {
                        for (int verticale = -1; verticale <= 1; verticale += 2)
                        {
                            for (int k = 0; k <= 1; k++)
                            {
                                int forseX = (1 + k) * orizzontale + i;
                                int forseY = (2 - k) * verticale + j;
                                if (forseX >= 0 && forseX < celle.GetLength(0) && forseY >= 0 && forseY < celle.GetLength(1))
                                {
                                    celle[i, j].Vicini.Add(celle[forseX, forseY]);
                                }
                            }
                        }
                    }
                }
            }

            int n = 1;
            bool incastrato = false;

            Cella adesso = partenza;
            partenza.Passato = true;

            CosoCheFaColori colorante = new CosoCheFaColori((double)1530 / (double)(celle.GetLength(0) * celle.GetLength(1)));

            while (CelleSrotolate.Where(x => !x.Passato).Count() > 0 && !incastrato)
            {
                adesso = adesso.Vicini.Where(x => !x.Passato/* && x.Vicini.Count(y=> !y.Passato)>0*/).OrderBy(x => (x.Vicini.Where(y => !y.Passato)).Count()).FirstOrDefault();


                if (adesso != null)
                {
                    adesso.Content = n++.ToString();
                    adesso.Passato = true;
                    adesso.Background = new SolidColorBrush(colorante.Successivo());
                    await Task.Delay(1);
                }
                else
                {
                    incastrato = true;
                    MessageBox.Show("Non sono riuscito a trovare un percorso.", "Oh no", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            if (!incastrato)
                MessageBox.Show($"Finito! Ho trovato un percorso.", "Yee", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        async public Task RisolviRegine()
        {
            Pulisci();

            CelleSrotolate.ForEach(cella =>
            {
                cella.Vicini.AddRange(PrendiDintorni(cella));
            });

            bool trovatoPercorso = false;


            for (int cambioPartenza = 0; cambioPartenza < CelleSrotolate.Count && !trovatoPercorso; cambioPartenza++)
            {
                Pulisci(false);
                Partenza = CelleSrotolate[cambioPartenza];

                //in questo caso passato indica se la cella è raggiungibile da una regina già presente, e quindi se non ci si può andare
                bool primo = true;

                int regineMesse = 0;

                while (CelleSrotolate.Where(x => !x.Passato).Count() > 0)
                {
                    Cella adesso = primo ? Partenza : CelleSrotolate.Where(x => !x.Passato).OrderBy(x => x.Vicini.Count(y => !y.Passato)).FirstOrDefault();
                    primo = false;

                    adesso.Background = Brushes.DodgerBlue;
                    adesso.Passato = true;
                    regineMesse++;

                    foreach (Cella cella in adesso.Vicini)
                    {
                        if (!cella.Passato)
                        {
                            //await Task.Delay(1);
                            cella.Passato = true;
                            cella.Background = Brushes.LightGoldenrodYellow;
                        }
                    }
                    await Task.Delay(1);
                }

                if (regineMesse >= celle.GetLength(0))
                {
                    trovatoPercorso = true;
                }
                if (trovatoPercorso && MessageBoxResult.Yes == MessageBox.Show($"Finito! Ho posizionato tutte e {celle.GetLength(0)} le regine. Vuoi cercare un'altra soluzione?", "Yee", MessageBoxButton.YesNo, MessageBoxImage.Information))
                    trovatoPercorso = false;
            }

            if (!trovatoPercorso)
                MessageBox.Show($"Non sono riuscito a mettere tutte e {celle.GetLength(0)} le regine.", "Oh no", MessageBoxButton.OK, MessageBoxImage.Warning);

            Partenza = null;
        }

        List<Cella> PrendiDintorni(Cella dove)
        {
            List<Cella> cosi = new List<Cella>();

            for (int x = 0; x < celle.GetLength(0); x++)
            {
                if (x != dove.X)
                {
                    cosi.Add(celle[x, dove.Y]);
                }
            }

            for (int y = 0; y < celle.GetLength(1); y++)
            {
                if (y != dove.Y)
                {
                    cosi.Add(celle[dove.X, y]);
                }
            }

            for (int i = 0; i < Math.Max(celle.GetLength(0), celle.GetLength(1)); i++)
            {
                for (int m1 = -1; m1 <= 1; m1 += 2)
                {
                    for (int m2 = -1; m2 <= 1; m2 += 2)
                    {
                        int forseX = dove.X + m1 * i;
                        int forseY = dove.Y + m2 * i;

                        if (forseX >= 0 && forseX < celle.GetLength(0) && forseY >= 0 && forseY < celle.GetLength(1) && forseX != dove.X && forseY != dove.Y)
                        {
                            cosi.Add(celle[forseX, forseY]);
                            //celle[forseX, forseY].Background = Brushes.Red;
                        }
                    }
                }
            }
            /*
            for (int obA = Math.Min((dove.X - dove.Y), (dove.Y - dove.X)); obA < Math.Min(celle.GetLength(0) - dove.X, celle.GetLength(1) - dove.Y); obA++)
            {
                if (obA == 0)
                {
                    cosi.Add(celle[dove.X + obA, dove.Y]);
                }
            }
            */
            return cosi;
        }
    }
}
