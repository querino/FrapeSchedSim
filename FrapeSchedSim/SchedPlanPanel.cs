
using System;
using System.Collections.Generic;
using Frapes;
using Gtk;
using Gdk;

namespace FrapeSchedSim
{
	
	
	public class SchedPlanPanel : Gtk.DrawingArea
	{
		
		private List<SchedPlan> plans = new List<SchedPlan> ();
		
		private int width = 23;
		private int height = 20;
		private int y_diff = 100;
		private int x_offset = 10;
		private int y_offset = 140;
		private int noProcessors = 1;
		
		private Color clrRed = new Color (255, 0, 0);
		private Color clrBlue = new Color(0, 0, 255);
		private Color clrDarkGreen = new Color (0, 64, 0);
		private Color clrOrange = new Color (255, 128, 0);
		private Color clrWhite = new Color (255, 255, 255);
		private Color clrBlack = new Color (0, 0, 0);
		private Color clrPink = new Color (255, 128, 255);
		
		private string[] ProcessColors = {"blue", "red", "green", "cyan", "brown", "orange", "magenta", "navy", "pink", "olive", "purple", "yellow"};
		
		private SimulatorInterface simulatorInterface;
		
		public SchedPlanPanel(SimulatorInterface simulatorInterface)
		{
			this.ModifyBg (StateType.Normal, new Color (255, 255, 255));
			this.simulatorInterface = simulatorInterface;
			this.HeightRequest = simulatorInterface.Nodes.Count * 140 + 40;
			plans = simulatorInterface.SchedPlans;
			this.WidthRequest = 60 + plans[0].Size () * this.width + 50;
			this.ExposeEvent += ExposeSchedPlanPanel;
		}
		
		public void ExposeSchedPlanPanel (object o, ExposeEventArgs args)
		{
			int x_height = 25 + 20 * 23;			
			
			if (x_height < 208)
			{
				x_height = 208;
			}
			
			Gdk.GC gc_color = new Gdk.GC (this.GdkWindow);
			gc_color.RgbBgColor = clrWhite;
			gc_color.RgbFgColor = clrWhite;
			
			this.GdkWindow.DrawRectangle (gc_color, true, 0, 0, this.WidthRequest, this.HeightRequest);
			
			gc_color.RgbFgColor = clrDarkGreen;
			
			Pango.Layout text = new Pango.Layout (this.PangoContext);
			text.SetText ("non-pre. proc. finished/blocked");
			this.GdkWindow.DrawLayout (gc_color, 5, 5, text);
			
			gc_color.RgbFgColor = clrOrange;
			
			text = new Pango.Layout (this.PangoContext);
			text.SetText ("non-pre. proc. continued");
			this.GdkWindow.DrawLayout (gc_color, 5, 20, text);
			
			gc_color.RgbFgColor = clrBlue;
			
			text = new Pango.Layout (this.PangoContext);
			text.SetText ("pre. proc.      finished/blocked");
			this.GdkWindow.DrawLayout (gc_color, 5, 35, text);
			
			gc_color.RgbFgColor = clrPink;
			
			text = new Pango.Layout (this.PangoContext);
			text.SetText ("pre. proc.      timeslice continue");
			this.GdkWindow.DrawLayout (gc_color, 5, 50, text);
			
			gc_color.RgbFgColor = clrRed;
			
			text = new Pango.Layout (this.PangoContext);
			text.SetText ("pre. proc.      timeslice end");
			this.GdkWindow.DrawLayout (gc_color, 5, 65, text);
			
			gc_color.RgbFgColor = clrBlack;
			
			text = new Pango.Layout (this.PangoContext);
			text.SetText ("Strategy: " + simulatorInterface.Strategy.ToString ());
			this.GdkWindow.DrawLayout (gc_color, 5, 80, text);
			
			for (noProcessors = 0; noProcessors < plans.Count; ++noProcessors)
			{
				//bool flash = false;
				gc_color.RgbFgColor = clrBlack;
				text = new Pango.Layout (this.PangoContext);
				text.SetText ("Time: ");
				this.GdkWindow.DrawLayout (gc_color, x_offset + 10, y_offset + (noProcessors * y_diff) - 5, text);
				
				for (int i = 0; i < plans[noProcessors].Size(); ++i)
				{
					/*if ((plans[noProcessors].GetResult (i).Deadline < 0) && (!flash))
					{
						// Draw flash
						int xflash = ((i + 1) * this.width);
						int yflash = y_offset + (noProcessors * this.y_diff);
						gc_color.RgbFgColor = clrRed;
						// The peak
						this.GdkWindow.DrawLine (gc_color, xflash, yflash, xflash - 5, yflash - 10);
						this.GdkWindow.DrawLine (gc_color, xflash, yflash, xflash - 10, yflash - 5);
						this.GdkWindow.DrawLine (gc_color, xflash - 10, yflash - 5, xflash - 5, yflash - 10);
						// The flash
						this.GdkWindow.DrawLine (gc_color, xflash, yflash, xflash - 20, yflash - 20);
						this.GdkWindow.DrawLine (gc_color, xflash - 20, yflash - 20, xflash, yflash -20 + 5);
						this.GdkWindow.DrawLine (gc_color, xflash, yflash - 15, xflash - 20, yflash - 35);
						text = new Pango.Layout (this.PangoContext);
						text.SetText ("Deadline");
						this.GdkWindow.DrawLayout (gc_color, xflash - 20 + 1 - 50, yflash - 20 +5 - 20, text);
						
						Console.WriteLine ("Deadline exceeded");
						flash = true;		
					}
					*/
					
					// Draws node number
					gc_color.RgbFgColor = clrBlack;
					text = new Pango.Layout (this.PangoContext);
					text.SetText ("Node " + Convert.ToString (noProcessors + 1));
					this.GdkWindow.DrawLayout (gc_color, this.x_offset + 10, this.y_offset + (noProcessors * this.y_diff) - 20, text);
					
					// Draws time unit
					text = new Pango.Layout (this.PangoContext);
					text.SetText (i.ToString ());
					this.GdkWindow.DrawLayout (gc_color, x_offset + 57 + i * this.width, y_offset + (noProcessors * this.y_diff) - 5, text);
					
					Gdk.Color fontcolor = new Gdk.Color ();
					
					int color = plans[noProcessors].GetResult (i).ProcessId - 1;
					
					if (color < 0)
					{
						Gdk.Color.Parse ("black", ref fontcolor);
					}
					else
					{
						Gdk.Color.Parse (ProcessColors[plans[noProcessors].GetResult (i).ProcessId - 1], ref fontcolor);
					}
					gc_color.RgbFgColor = fontcolor;
					
					// Draws a rectangle for each node and process
					this.GdkWindow.DrawRectangle (gc_color, true, x_offset + 50 + i * this.width + 10, y_offset + (noProcessors * this.y_diff) + 10, this.width, this.height);
					
					// Write the process id
					if (plans[noProcessors].GetResult (i).ProcessId != 0)
					{
						text = new Pango.Layout (this.PangoContext);
						text.SetText ("P" + plans[noProcessors].GetResult (i).ProcessId.ToString ());
						this.GdkWindow.DrawLayout (gc_color, x_offset + 50 + 2 + i * this.width + 10, y_offset + (noProcessors * this.y_diff) + 30, text);
					}
					
					// Draw a line to display the process state before this time unit
					switch (plans[noProcessors].GetResult (i).Status)
					{
					case Defines.NonPreemptiveProcessFinishedOrBlocked:
						gc_color.RgbFgColor = clrDarkGreen;
						break;
						
					case Defines.NonPreemptiveProcessContinued:
						gc_color.RgbFgColor = clrOrange;
						break;
						
					case Defines.PreemptiveProcessFinishedOrBlocked:
						gc_color.RgbFgColor = clrBlue;
						break;
						
					case Defines.PreemptiveTimesliceContinued:
						gc_color.RgbFgColor = clrPink;
						break;
						
					case Defines.PreemptiveTimesliceEnd:
						gc_color.RgbFgColor = clrRed;
						break;
					}
					
					this.GdkWindow.DrawLine (gc_color, x_offset + 50 + (i * this.width) + 10, y_offset + (noProcessors * this.y_diff) + 50, x_offset + 50 + (i * this.width) + 10, y_offset + (this.noProcessors * y_diff) + this.height - 10);
				}
			}
			
			int size = 0;
			
			List<BasicProcess> processes = simulatorInterface.Processes.GetAllRunningProcesses ();
			
			for (int i = 0; i < processes.Count; i++)
			{
				if (processes[i].State == Defines.Finished)
				{
					size++;
				}
			}			
		}
	}
}
