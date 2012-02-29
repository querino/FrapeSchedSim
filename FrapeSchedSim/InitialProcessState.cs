
using System;
using System.Collections.Generic;
using Frapes;
using Gtk;
using Gdk;

namespace FrapeSchedSim
{
	
	
	public class InitialProcessState : Gtk.DrawingArea
	{
		
		private List<BasicProcess> ProcVec = new List<BasicProcess> ();
		
		private SimulatorInterface simulatorInterface;
		
		private int width = 20;
		private int height = 23;
		private int border = 30;
		private int y_diff = 33;
		
		private string[] ProcessColors = {"blue", "red", "green", "cyan", "brown", "orange", "magenta", "navy", "pink", "olive", "purple", "yellow"};
		
		
		public InitialProcessState (SimulatorInterface simulatorInterface)
		{
			this.simulatorInterface = simulatorInterface;
			this.ProcVec = this.simulatorInterface.Processes.GetAllProcesses ();			
			this.ModifyBg (StateType.Normal, new Color (255, 255, 255));
			this.HeightRequest = this.ProcVec.Count * 46 + 5;
			this.WidthRequest = width * 25;
			this.ExposeEvent += ExposeInitialProcessState;
		}
		
		public void ExposeInitialProcessState (object o, ExposeEventArgs args)
		{
			int size = this.ProcVec.Count;
			int maxDeadline = 0;
			int maxBlockLength = 0;
			BasicProcess currentProc = null;
			
			for (int i = 0; i < size; i++)
			{
				if (this.ProcVec[i].DeadlineTime > maxDeadline)
				{
					maxDeadline = this.ProcVec[i].DeadlineTime;
				}
				if (this.ProcVec[i].BlockingLength > maxBlockLength)
				{
					maxBlockLength = this.ProcVec[i].BlockingLength;
				}
			}
			
			int grid_height_base = size * y_diff + y_diff + border;
			int grid_width_base = this.width * maxDeadline + this.width + border;
			
			if (this.ProcVec.Count == 0)
			{
				grid_width_base = 1;
				grid_height_base = 1;
			}
			
			Gdk.GC gc_all_white = new Gdk.GC (this.GdkWindow);
			gc_all_white.RgbFgColor = new Color (255, 255, 255);
			gc_all_white.RgbBgColor = new Color (255, 255, 255);
			
			Gdk.Color color = new Gdk.Color ();
			Gdk.Color.Parse ("blue", ref color);
			
			Gdk.GC gc_blue_white = new Gdk.GC (this.GdkWindow);
			gc_blue_white.RgbFgColor = color;
			gc_blue_white.RgbBgColor = new Color (255, 255, 255);
			
			this.GdkWindow.DrawRectangle (gc_all_white, true, 0, 0, this.WidthRequest, this.HeightRequest);
			
			// Draw all the processes
			for (int i = 0; i < size; i++)
			{
				currentProc = this.ProcVec[i];
				Pango.Layout textLayout = new Pango.Layout (this.PangoContext);
				textLayout.Width = this.WidthRequest;
				Gdk.Color fontcolor = new Gdk.Color ();
				Gdk.Color.Parse (ProcessColors[i], ref fontcolor);
				Gdk.GC gc_process = new Gdk.GC (this.GdkWindow);
				gc_process.RgbBgColor = new Color (255, 255, 255);
				gc_process.RgbFgColor = fontcolor;
				textLayout.SetText ("P" + currentProc.ProcessId.ToString ());

				this.GdkWindow.DrawLayout (gc_process, 2, (height / 2) + (i + 1) * y_diff - 5, textLayout);
				
				int start = currentProc.ReadyTime;
				
				// Draw the timeline for each process
				for (int j = start; j <= maxDeadline + maxBlockLength; ++j)
				{
					// Draw blocking blocks
					if (j == start + currentProc.BlockingTime)
					{
						gc_process.RgbFgColor = new Color (10, 10, 10);
						for (int k = j; k < start + currentProc.BlockingTime + currentProc.BlockingLength; ++k, ++j)
						{
							this.GdkWindow.DrawRectangle (gc_process, true, (this.width * k) + width, (i + 1) * y_diff, this.width, this.height);
						}
					}
					
					// Draw the normal processing blocks
					if (j < start + currentProc.ExecutionTime + currentProc.BlockingLength)
					{
						
						Gdk.Color newcolor = new Gdk.Color ();
						Gdk.Color.Parse (ProcessColors[i], ref newcolor);
						gc_process.RgbFgColor = newcolor;
						this.GdkWindow.DrawRectangle (gc_process, true, (this.width * j) + width, (i + 1) * y_diff, this.width, this.height);
					}
				}				
			}
			Gdk.GC gc_black = new Gdk.GC (this.GdkWindow);
			gc_black.RgbFgColor = new Color (0, 0, 0);
			gc_black.RgbBgColor = new Color (0, 0, 0);
			
			// Draw the grid
			for (int j = 0; j < grid_width_base; j++)
			{
				if (j % 5 == 0)
				{
					gc_black.RgbFgColor = new Color (0, 0, 0);
				}
				else
				{
					gc_black.RgbFgColor = new Color (100, 100, 100);
				}
				this.GdkWindow.DrawLine (gc_black, (this.width * j) + this.width, y_diff - 5, (this.width * j) + this.width, y_diff + grid_height_base - (border + 20));
				Pango.Layout otherLayout = new Pango.Layout (this.PangoContext);
				otherLayout.Width = this.WidthRequest;
				otherLayout.SetText (j.ToString ());
				this.GdkWindow.DrawLayout (gc_black, (this.width * j) + this.width - 5, grid_height_base - border + 10, otherLayout);
			}
		}
		
	}
}
