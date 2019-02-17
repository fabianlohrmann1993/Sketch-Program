using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;

        public virtual void MuisVast(SchetsControl s, Point p)
        {   startpunt = p;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {   kwast = new SolidBrush(s.PenKleur);
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);

    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        
        public override void Letter(SchetsControl s, char c)
        {
            
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                SizeF sz = gr.MeasureString(c.ToString(), font, this.startpunt, StringFormat.GenericTypographic);
                Point endpunt = new Point();
                endpunt.X = startpunt.X + (int) sz.Width;
                endpunt.Y = startpunt.Y + (int)sz.Height;

                s.figures.Add(new Figure("TekstTool", startpunt, endpunt, s.PenKleur, c.ToString()));     

                s.figures[s.figures.Count-1].DrawFigure(gr);
                
                s.Invalidate();
                startpunt.X += (int)sz.Width;
            }   
        }

    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {   Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {   base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {   s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);      
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   base.MuisLos(s, p);
            this.Compleet(s,s.MaakBitmapGraphics(), this.startpunt, p);                         //added SchetsControl as a parameter to complete, since they need the figures-list of the schetscontrol object
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(Graphics g, Point p1, Point p2);     


        public abstract void Compleet(SchetsControl s, Graphics g, Point p1, Point p2);        //added SchetsControl as a parameter to complete, since they need the figures-list of the schetscontrol object

    }

    public class RechthoekTool : TweepuntTool       
    {
        public override string ToString() { return "kader"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));      
           
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            s.figures.Add(new Figure("RechthoekTool", p1, p2, s.PenKleur, ""));
            s.figures[s.figures.Count - 1].DrawFigure(g);
        }
    }
    
    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override void Compleet(SchetsControl s,Graphics g, Point p1, Point p2)
        {   
            s.figures.Add(new Figure("VolRechthoekTool", p1, p2, s.PenKleur, ""));
            s.figures[s.figures.Count - 1].DrawFigure(g);
        }
    }

    public class CircleTool : TweepuntTool     
    {                                           
        public override string ToString() { return "ovaal"; }   

        public override void Bezig (Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2)); 
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            s.figures.Add(new Figure("CircleTool", p1, p2, s.PenKleur, ""));
            s.figures[s.figures.Count - 1].DrawFigure(g);
        }
    }

    public class VolCircleTool : TweepuntTool
    {
        public override string ToString() { return "ovaal2"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            s.figures.Add(new Figure("VolCircleTool", p1, p2, s.PenKleur, ""));
            s.figures[s.figures.Count - 1].DrawFigure(g);
        }
    }

        public class LijnTool : TweepuntTool
        {
            public override string ToString() { return "lijn"; }

            public override void Bezig(Graphics g, Point p1, Point p2)
            {
                g.DrawLine(MaakPen(this.kwast, 3), p1, p2);
            }
            public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
            {
                s.figures.Add(new Figure("LijnTool", p1, p2, s.PenKleur, ""));
                s.figures[s.figures.Count - 1].DrawFigure(g);

            }
        }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            this.MuisLos(s, p);
            this.MuisVast(s, p);
            
        }
        public override void Compleet(SchetsControl s, Graphics g, Point p1, Point p2)
        {
            s.figures.Add(new Figure("PenTool", p1, p2, s.PenKleur, ""));
            s.figures[s.figures.Count - 1].DrawFigure(s.MaakBitmapGraphics());
        }

    }
    
    //new Gum Tool
    
    public class GumTool : StartpuntTool                            // no need to be a pen or even a tweepunt tool anymore
    {
        public override string ToString() { return "gum"; }

        public override void Letter(SchetsControl s, char c) { }   //they are only defined to make the ISchetsTool Interface happy
        
        public override void MuisDrag(SchetsControl s, Point p) { } 
        
        public override void MuisVast(SchetsControl s, Point p)      
        {
            Delete(s, p, s.MaakBitmapGraphics());
        }

        public void Delete (SchetsControl s, Point p, Graphics g)
        { 

            for (int i = s.figures.Count - 1; i >= 0; i--)       //delete the clicked on figure //-1 because Count will give you the third object as 3 but its index is 2
            {
                if (raakt(s.figures[i],p))                      //if startpunt is inside the s.figure[i] 'area' we need the 'raakt' function for this
                {
                    s.figures.RemoveAt(i);
                    break;
                }
            }
            s.Schets.Schoon();
            s.Invalidate();                                  //we need to delete the bitmap and redraw it completely

            for (int i = 0; i < s.figures.Count; i++)       //draw all figures after the to be deleted has be removed
            {
                s.figures[i].DrawFigure(g);                  
            }

        }
        public bool raakt(Figure f,Point p)
        {
            bool touch = false;
            if (f.soort == "RechthoekTool")
            {
                if (f.startpunt.X < p.X && p.X < f.endpunt.X && f.startpunt.Y < p.Y && p.Y < f.endpunt.Y)
                    touch = true;
            }
                
            else if (f.soort == "VolRechthoekTool")
            {
                if (f.startpunt.X < p.X && p.X < f.endpunt.X && f.startpunt.Y < p.Y && p.Y < f.endpunt.Y)
                    touch = true;
            }
                
            else if (f.soort == "CircleTool")
            {
                if (f.startpunt.X < p.X && p.X < f.endpunt.X && f.startpunt.Y < p.Y && p.Y < f.endpunt.Y)
                    touch = true;
            }
                
            else if (f.soort == "VolCircleTool")
            {
                if (f.startpunt.X < p.X && p.X < f.endpunt.X && f.startpunt.Y < p.Y && p.Y < f.endpunt.Y)
                        touch = true;
            }
                    
            else if (f.soort == "LijnTool")
            {
                if (f.startpunt.X < p.X && p.X < f.endpunt.X && f.startpunt.Y < p.Y && p.Y < f.endpunt.Y)
                    touch = true;
            }
                
            else if (f.soort == "PenTool")
            {
                if (f.startpunt.X -5 < p.X && p.X < f.startpunt.X+5 && f.startpunt.Y -5 < p.Y && p.Y < f.startpunt.Y +5 )
                    touch = true;
            }
                
            else if (f.soort == "TekstTool")
            {
                if (f.startpunt.X < p.X && p.X < f.endpunt.X && f.startpunt.Y < p.Y && p.Y < f.endpunt.Y)
                    touch = true;

            }

            return touch;
        }
        

    }
    
}
