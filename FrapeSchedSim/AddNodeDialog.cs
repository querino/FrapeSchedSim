
using System;
using System.Collections.Generic;
using Gtk;
using Frapes;

namespace FrapeSchedSim
{
	
	
	public partial class AddNodeDialog : Gtk.Dialog
	{

		private List<Label> labels = new List<Label> ();
		private List<SpinButton> entries = new List<SpinButton> ();
		
		private SimulatorInterface simulatorInterface;
		
		public AddNodeDialog (SimulatorInterface simulatorInterface)
		{
			this.Build ();
			this.simulatorInterface = simulatorInterface;
			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.EtchedIn;
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			sw.HeightRequest = 250;
			
			VBox.PackStart (sw, true, true, 0);
			Table table = new Table ((uint)(BasicNode.NodeLabels.Length), 2, true);
			sw.AddWithViewport (table);
			for (int i = 0; i < BasicNode.NodeLabels.Length; i++)
			{
				labels.Add (new Label (BasicNode.NodeLabels[i]));
				table.Attach (labels[i], 0, 1, (uint)(i), (uint)(i) + 1);
				entries.Add (new SpinButton (0, 80, 1));
				entries[i].ClimbRate = 1;
				entries[i].Numeric = true;
				table.Attach (entries[i], 1, 2, (uint)(i), (uint)(i) + 1);
			}
			buttonOk.Clicked += new EventHandler (AddNode);
			buttonCancel.Clicked += new EventHandler (Cancel);
			this.SetDefaultSize (340, 300);
			this.Modal = true;
			this.ShowAll ();			
		}
		
		private void Cancel (object o, EventArgs args)
		{
			this.Destroy ();
		}
		
		private void AddNode (object o, EventArgs args)
		{
			BasicNode node = new BasicNode (Convert.ToInt32 (entries[0].Text), Convert.ToInt32 (entries[1].Text), 0);
			simulatorInterface.Nodes.AddNode (node);
			MainWindow.NodeStore.AppendValues (node.NodeId, node.TimeSlice);
			this.Destroy ();
		}		
		
	}
}
