
using System;
using System.Collections.Generic;
using Frapes;
using Gtk;

namespace FrapeSchedSim
{
	
	
	public class ProcessQueueView : Gtk.TextView
	{
		
		public ProcessQueueView()
		{
			this.Editable = false;
			this.WidthRequest = 531;
		}
		
		public void AppendList (List<BasicProcess> list)
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.Buffer.Text += list[i].DumpStatistics() + "\r\n";
				}
				this.Buffer.Text += "\r\n";
			}
		}
		
		public void UpdateList (List<BasicProcess> list)
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.Buffer.Text += list[i].DumpStatistics () + "\r\n";
				}
				this.Buffer.Text += "\r\n";
			}
		}
		
		public void AppendText (string str)
		{
			this.Buffer.Text += str + "\r\n";
		}
		
		public void Reset ()
		{
			this.Buffer.Text = "";
		}
	}
}
