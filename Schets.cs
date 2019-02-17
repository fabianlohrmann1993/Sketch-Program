using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;


        public Schets()
        {
            bitmap = new Bitmap(1, 1);
            
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;

            }
        }
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);

        }
        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }
        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }
    }
    public class Figure     //defining a type "figure", of which the objects will represent the user-drawn figures and stored in the figures list and painted on the bitmap from there.
    {
        public String soort;
        public Point startpunt;
        public Point endpunt;
        public Color kleur;
        public String text;

        public Figure(String tempSoort, Point tempStartpunt, Point tempEndpunt, Color tempKleur, String tempText)
        {
            this.soort = tempSoort;
            this.startpunt = tempStartpunt;
            this.endpunt = tempEndpunt;
            this.kleur = tempKleur;
            this.text = tempText;
        }
        public void DrawFigure(Graphics g)
        {
            if (this.soort == "RechthoekTool")                                                                                    //this line might activate VolRechthoekTools as well
                g.DrawRectangle(TweepuntTool.MaakPen(new SolidBrush(this.kleur), 3), TweepuntTool.Punten2Rechthoek(this.startpunt, this.endpunt));
            else if (this.soort == "VolRechthoekTool")                                                                            //instead of giving a type we might just use a string like if this.soort == "RechthoekTool"
                g.FillRectangle(new SolidBrush(this.kleur), TweepuntTool.Punten2Rechthoek(this.startpunt, this.endpunt));
            else if (this.soort == "CircleTool")
                g.DrawEllipse(TweepuntTool.MaakPen(new SolidBrush(this.kleur), 3), TweepuntTool.Punten2Rechthoek(startpunt, endpunt));
            else if (this.soort == "VolCircleTool")
                g.FillEllipse(new SolidBrush(this.kleur), TweepuntTool.Punten2Rechthoek(this.startpunt, this.endpunt));
            else if (this.soort == "LijnTool")
                g.DrawLine(TweepuntTool.MaakPen(new SolidBrush(this.kleur), 3), this.startpunt, this.endpunt);
            else if (this.soort == "PenTool")
                g.DrawLine(TweepuntTool.MaakPen(new SolidBrush(this.kleur), 3), this.startpunt, this.endpunt);
            else if (this.soort == "TekstTool")
            {
                Font font = new Font("Tahoma", 40);
                
                SizeF sz = g.MeasureString(this.text, font, this.startpunt, StringFormat.GenericTypographic);

                g.DrawString(this.text, font, (new SolidBrush(this.kleur)), this.startpunt, StringFormat.GenericTypographic);
               

               
            }

            

        }        
    }
}
