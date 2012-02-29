
using System;
using System.Collections.Generic;
using Frapes;
using Gtk;

namespace FrapeSchedSim
{
	
	
	public partial class AddProcessDialog : Gtk.Dialog
	{

		private List<Label> labels = new List<Label> ();
		private List<SpinButton> entries = new List<SpinButton> ();
		
		private SimulatorInterface simulatorInterface;
		
		public AddProcessDialog(SimulatorInterface simulatorInterface)
		{
			this.Build ();
			this.simulatorInterface = simulatorInterface;
			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.EtchedIn;
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw.HeightRequest = 250;
			
			VBox.PackStart (sw, true, true, 0);
			Table table = new Table ((uint)(BasicProcess.ProcessLabels.Length), 2, true);
			sw.AddWithViewport (table);
			for (int i = 0; i < BasicProcess.ProcessLabels.Length; i++)
			{
				labels.Add (new Label (BasicProcess.ProcessLabels[i]));
				table.Attach (labels[i], 0, 1, (uint)(i), (uint)(i) + 1);
				entries.Add (new SpinButton (0, 80, 1));
				entries[i].ClimbRate = 1;
				entries[i].Numeric = true;
				table.Attach (entries[i], 1, 2, (uint)(i), (uint)(i) + 1);
			}			
			buttonOk.Clicked += new EventHandler (AddProcess);
			buttonCancel.Clicked += new EventHandler (Cancel);
			this.SetDefaultSize (340, 300);
			this.Modal = true;
			this.ShowAll ();			
		}
		
		private void Cancel (object o, EventArgs args)
		{
			this.Destroy ();
		}
		
		private void AddProcess (object o, EventArgs args)
		{
			BasicProcess process = new BasicProcess (Convert.ToInt32 (entries[0].Text), Convert.ToInt32 (entries[1].Text), Convert.ToInt32 (entries[2].Text), Convert.ToInt32 (entries[3].Text), Convert.ToInt32 (entries[4].Text), Convert.ToInt32 (entries[5].Text), Convert.ToInt32 (entries[6].Text));
			simulatorInterface.Processes.AddProcess (process);
			MainWindow.Store.AppendValues (process.ProcessId, process.ReadyTime, process.ExecutionTime, process.DeadlineTime, process.Priority, process.BlockingTime, process.BlockingLength);
			this.Destroy ();
		}
	}
}
