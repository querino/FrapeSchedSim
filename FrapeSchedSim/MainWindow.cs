using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Frapes;
using Gtk;
using Gdk;

namespace FrapeSchedSim
{
	public partial class MainWindow : Gtk.Window
	{	
		internal static ListStore Store;
		private TreeView treeviewProcessPool;
		private ScrolledWindow swProcessPool;
		
		internal static ListStore NodeStore;
		private TreeView treeviewNodePool;
		private ScrolledWindow swNodePool;
		
		private InitialProcessState daInitialProcess;
		private Viewport viewInitial;
		
		private Viewport viewSchedPlan;
		private SchedPlanPanel schedPlanPanel;
		
		internal static ProcessQueueView processQueue;
		
		private int TimeUnitCounter = 0;
		
		public SimulatorInterface simulatorInterface = new SimulatorInterface ();
						
		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();
			foreach (string schedName in Defines.AvailableSchedulers)
			{
				comboboxAlgorithm.AppendText(schedName);
			}
			comboboxAlgorithm.Active = 0;
			spinbuttonTimeUnits.Value = 5.0;
			PrepareProcessView ();
			PrepareNodeView ();
			processQueue = new ProcessQueueView ();
			this.swProcessQueue.Add (processQueue);
			
			// Load processes for testing
			simulatorInterface.Processes.LoadFromXml ("processes.xml");
			List<BasicProcess> list = simulatorInterface.Processes.GetAllProcesses ();
			foreach (BasicProcess process in list)
			{
				Store.AppendValues (process.ProcessId, process.ReadyTime, process.ExecutionTime, process.DeadlineTime, process.Priority, process.BlockingTime, process.BlockingLength);
			}
			
			// Load nodes for testing
			simulatorInterface.Nodes.LoadFromXml ("nodes.xml");
			List<BasicNode> nodelist = simulatorInterface.Nodes.GetAllNodes ();
			foreach (BasicNode node in nodelist)
			{
				NodeStore.AppendValues (node.NodeId, node.TimeSlice);					
			}			
			
			this.spinbuttonTimeUnits.Value = 20;
			
			this.ShowAll ();			
		}
			
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected virtual void OnButtonQuitClicked (object sender, System.EventArgs e)
		{
			MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, "Do you really want to close this program?");
			msg.Modal = true;
			Gtk.ResponseType result = (ResponseType)msg.Run ();
			if (result == ResponseType.Yes)
			{
				msg.Destroy ();
				Application.Quit ();
			}
			else
			{
				msg.Destroy ();
			}
		}
		
		private void PrepareProcessView ()
		{
			swProcessPool = new ScrolledWindow ();
			swProcessPool.ShadowType = ShadowType.EtchedIn;
			swProcessPool.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			vboxProcessPool.PackEnd (swProcessPool, true, true, 0);
			
			Store = CreateModel ();
			
			treeviewProcessPool = new TreeView (Store);
			treeviewProcessPool.RulesHint = true;
			treeviewProcessPool.EnableGridLines = TreeViewGridLines.Both;
			treeviewProcessPool.WidthRequest = 370;

			swProcessPool.Add (treeviewProcessPool);

			CellRendererText renderer = new CellRendererText ();
			
			TreeViewColumn column = new TreeViewColumn ("PID", renderer, "text", Column.PID);
			treeviewProcessPool.AppendColumn (column);

			renderer = new CellRendererText ();
			column = new TreeViewColumn ("ReadyTime", renderer, "text", Column.RT);
			treeviewProcessPool.AppendColumn (column);

			renderer = new CellRendererText ();
			column = new TreeViewColumn ("ExecTime", renderer, "text", Column.ET);
			treeviewProcessPool.AppendColumn (column);

			renderer = new CellRendererText ();
			column = new TreeViewColumn ("DeadTime", renderer, "text", Column.DT);
			treeviewProcessPool.AppendColumn (column);

			renderer = new CellRendererText ();
			column = new TreeViewColumn ("Priority", renderer, "text", Column.PRI);
			treeviewProcessPool.AppendColumn (column);
			
			renderer = new CellRendererText ();
			column = new TreeViewColumn ("BlockTime", renderer, "text", Column.BT);
			treeviewProcessPool.AppendColumn (column);

			renderer = new CellRendererText ();
			column = new TreeViewColumn ("BlockLen", renderer, "text", Column.BL);
			treeviewProcessPool.AppendColumn (column);
						
			this.ShowAll ();
		}

		private void PrepareNodeView ()
		{
			swNodePool = new ScrolledWindow ();
			swNodePool.ShadowType = ShadowType.EtchedIn;
			swNodePool.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			vboxNodePool.PackEnd (swNodePool, true, true, 0);
			
			NodeStore = CreateNodeModel ();
			
			treeviewNodePool = new TreeView (NodeStore);
			treeviewNodePool.RulesHint = true;
			treeviewNodePool.EnableGridLines = TreeViewGridLines.Both;

			swNodePool.Add (treeviewNodePool);

			CellRendererText renderer = new CellRendererText ();
			
			TreeViewColumn column = new TreeViewColumn ("Node ID", renderer, "text", NodeColumn.N1);
			treeviewNodePool.AppendColumn (column);

			renderer = new CellRendererText ();
			renderer.Edited += new EditedHandler (TimeSliceEdited);
			renderer.Editable = true;
			column = new TreeViewColumn ("TimeSlice", renderer, "text", NodeColumn.N2);
			treeviewNodePool.AppendColumn (column);

			this.ShowAll ();
		}		
		
		private ListStore CreateModel ()
		{
			ListStore store = new ListStore (typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int));
			//BasicProcess proc = new BasicProcess (0, 0, 3, 6, 0);
			//simulatorInterface.Processes.AddProcess (proc);
			//store.AppendValues (proc.ProcessId, proc.ReadyTime, proc.ExecutionTime, proc.DeadlineTime, proc.Priority, proc.BlockingTime, proc.BlockingLength);
			return store;
		}
		
		private ListStore CreateNodeModel ()
		{
			ListStore store = new ListStore (typeof(int), typeof(int));
			//BasicNode node = new BasicNode (1, 4, 1);
			//simulatorInterface.Nodes.AddNode (node);
			//store.AppendValues (node.NodeId, node.TimeSlice);
			return store;
		}		
		
		protected virtual void OnButtonAddProcessClicked (object sender, System.EventArgs e)
		{
			if (simulatorInterface.Processes.Count == 12)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Maximum number of processes reached.");
				msg.Run ();
				msg.Destroy ();				
			}
			else
			{
				AddProcessDialog addProcess = new AddProcessDialog (simulatorInterface);
				addProcess.Run ();
			}
		}

		protected virtual void OnButtonRemoveProcessClicked (object sender, System.EventArgs e)
		{
			TreeIter iter;
			TreeModel model;
			
			if (treeviewProcessPool.Selection.GetSelected (out model, out iter))
			{
				int pos = Store.GetPath (iter).Indices[0];
				Store.Remove (ref iter);
				simulatorInterface.Processes.RemoveAt (pos);
			}
		}

		protected virtual void OnButtonAddNodeClicked (object sender, System.EventArgs e)
		{
			AddNodeDialog addNode = new AddNodeDialog (simulatorInterface);
			addNode.Run ();
		}

		protected virtual void OnButtonRemoveNodeClicked (object sender, System.EventArgs e)
		{
			TreeIter iter;
			TreeModel model;
			
			if (treeviewNodePool.Selection.GetSelected (out model, out iter))
			{
				int pos = NodeStore.GetPath (iter).Indices[0];
				NodeStore.Remove (ref iter);
				simulatorInterface.Nodes.RemoveAt (pos);
			}			
		}
		
		private enum Column
		{
			PID,
			RT,
			ET,
			DT,
			PRI,
			BT,
			BL
		}
		
		private enum NodeColumn
		{
			N1,
			N2,
			N3
		}
		
		private void TimeSliceEdited (object o, EditedArgs args)
		{
			TreePath path = new TreePath (args.Path);
			TreeIter iter;
			
			NodeStore.GetIter (out iter, path);
			int i = path.Indices[0];
			
			BasicNode node;
			try
			{
				List<BasicNode> nodes = simulatorInterface.Nodes.GetAllNodes ();
				node = nodes[i];
				node.TimeSlice = Convert.ToInt32(args.NewText);
			}
			catch (Exception e)
			{
				Console.WriteLine (e);
				return;
			}
			NodeStore.SetValue(iter, (int) NodeColumn.N2, node.TimeSlice);
		}

		protected virtual void OnQuitAction1Activated (object sender, System.EventArgs e)
		{
			this.Destroy ();
			Application.Quit ();
		}

		protected virtual void OnAboutThisProgramActionActivated (object sender, System.EventArgs e)
		{
			AboutDialog about = new AboutDialog ();
			about.Modal = true;
			about.Show ();
		}

		protected virtual void OnFocusActivated (object sender, System.EventArgs e)
		{
		}

		protected virtual void OnButtonRunClicked (object sender, System.EventArgs e)
		{
			if (this.comboboxAlgorithm.Active < 0)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "No algorithm selected.");
				msg.Run ();
				msg.Destroy ();
			}
			else if (this.simulatorInterface.Processes.Count <= 0)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "No processes available.");
				msg.Run ();
				msg.Destroy ();				
			}
			else if (this.simulatorInterface.Nodes.Count <= 0)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "No processing nodes available.");
				msg.Run ();
				msg.Destroy ();					
			}
			else if (spinbuttonTimeUnits.Value <= 0)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Time units must be greater than 0.");
				msg.Run ();
				msg.Destroy ();	
				spinbuttonTimeUnits.HasFocus = true;
			}
			else
			{
				simulatorInterface.SetStrategyByName (comboboxAlgorithm.ActiveText);
				this.viewInitial = new Viewport ();
				this.daInitialProcess = new InitialProcessState (simulatorInterface);
				
				this.viewInitial.Add (daInitialProcess);
				this.swInitialProcessState.Add (viewInitial);			
				
				this.viewSchedPlan = new Viewport ();
				this.schedPlanPanel = new SchedPlanPanel (simulatorInterface);
								
				this.viewSchedPlan.Add (schedPlanPanel);
				this.scrolledwindowSchedPlan.Add (viewSchedPlan);
								
				for (int i = 0; i < spinbuttonTimeUnits.ValueAsInt; i++)
				{
					RunSimulationForTimeUnit (TimeUnitCounter++);
				}
							
				this.ShowAll ();
			}
		}

		protected virtual void OnButtonResetClicked (object sender, System.EventArgs e)
		{
			simulatorInterface.Nodes = new NodeList ();
			simulatorInterface.Processes = new ProcessList ();
			NodeStore.Clear ();
			Store.Clear ();
			processQueue.Reset ();
			TimeUnitCounter = 0;
			this.swInitialProcessState.Remove (viewInitial);
			this.scrolledwindowSchedPlan.Remove (viewSchedPlan);
			this.ShowAll ();
		}
		
		private void RunSimulationForTimeUnit (int currentTimeUnit)
		{
			simulatorInterface.RunNextTimeUnit ();
		 	processQueue.AppendText ("Timeunit: " + currentTimeUnit.ToString ());
			processQueue.UpdateList (simulatorInterface.Processes.GetAllRunningProcesses());
		}
		
		protected virtual void OnButtonRunStepByStepClicked (object sender, System.EventArgs e)
		{
			if (this.comboboxAlgorithm.Active < 0)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "No algorithm selected.");
				msg.Run ();
				msg.Destroy ();
			}
			else if (this.simulatorInterface.Processes.Count <= 0)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "No processes available.");
				msg.Run ();
				msg.Destroy ();				
			}
			else if (this.simulatorInterface.Nodes.Count <= 0)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "No processing nodes available.");
				msg.Run ();
				msg.Destroy ();					
			}
			else if (spinbuttonTimeUnits.Value <= 0)
			{
				MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Time units must be greater than 0.");
				msg.Run ();
				msg.Destroy ();	
				spinbuttonTimeUnits.HasFocus = true;
			}
			else
			{
				if (this.TimeUnitCounter >= spinbuttonTimeUnits.ValueAsInt)
				{
					MessageDialog msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "End of simulation.");
					msg.Run ();
					msg.Destroy ();
				}
				else if (TimeUnitCounter == 0)
				{
					simulatorInterface.SetStrategyByName (comboboxAlgorithm.ActiveText);
					this.viewInitial = new Viewport ();
					this.daInitialProcess = new InitialProcessState (simulatorInterface);
					this.viewInitial.Add (daInitialProcess);
					this.swInitialProcessState.Add (viewInitial);			
				
					this.viewSchedPlan = new Viewport ();
					this.schedPlanPanel = new SchedPlanPanel (simulatorInterface);
					this.viewSchedPlan.Add (schedPlanPanel);
					this.scrolledwindowSchedPlan.Add (viewSchedPlan);
					
					RunSimulationForTimeUnit (this.TimeUnitCounter++);
				}
				else	
				{
					this.viewSchedPlan.Remove (schedPlanPanel);
					this.schedPlanPanel = new SchedPlanPanel (simulatorInterface);
					this.viewSchedPlan.Add (schedPlanPanel);
					RunSimulationForTimeUnit (this.TimeUnitCounter++);
				}
				this.ShowAll ();
			}
		}

		protected virtual void OnComboboxAlgorithmChanged (object sender, System.EventArgs e)
		{
			simulatorInterface.SetStrategyByName (comboboxAlgorithm.ActiveText);
			textviewInformation.Buffer.Clear ();
			textviewInformation.Buffer.Text += "Hierarchy:\n"; 
			textviewInformation.Buffer.Text += simulatorInterface.Strategy.ToString ();
			textviewInformation.Buffer.Text += "\n\n";
			textviewInformation.Buffer.Text += "Preemptive: " + simulatorInterface.Strategy.IsPreemptive().ToString ();
		}

		protected virtual void OnSaveToXmlActionActivated (object sender, System.EventArgs e)
		{
			FileChooserDialog fileSave = new FileChooserDialog ("Save Processes to Xml File", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			if (fileSave.Run () == (int) ResponseType.Accept)
			{
				simulatorInterface.Processes.SaveToXml (fileSave.Filename);
			}
			fileSave.Destroy ();
		}

		protected virtual void OnLoadFromXmlActionActivated (object sender, System.EventArgs e)
		{
			FileChooserDialog fileOpen = new FileChooserDialog ("Load Processes from Xml File", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Ok);
			if (fileOpen.Run () == (int) ResponseType.Ok)
			{
				Store.Clear ();
				simulatorInterface.Processes.RemoveAll ();
				simulatorInterface.Processes.LoadFromXml (fileOpen.Filename);
				List<BasicProcess> list = simulatorInterface.Processes.GetAllProcesses ();
				foreach (BasicProcess process in list)
				{
					Store.AppendValues (process.ProcessId, process.ReadyTime, process.ExecutionTime, process.DeadlineTime, process.Priority, process.BlockingTime, process.BlockingLength);
				}
			}
			fileOpen.Destroy ();
		}

		protected virtual void OnSaveInitialProcessStateToJPGAction1Activated (object sender, System.EventArgs e)
		{
			FileChooserDialog fileSave = new FileChooserDialog ("Save Initial Process State to PNG", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			if (fileSave.Run () == (int) ResponseType.Accept)
			{
				Gdk.Pixbuf initialPic = Gdk.Pixbuf.FromDrawable (this.daInitialProcess.GdkWindow, Gdk.Colormap.System, 0, 0, 0, 0, this.daInitialProcess.WidthRequest, this.daInitialProcess.HeightRequest);
				initialPic.Save (fileSave.Filename, "png");
			}
			fileSave.Destroy ();
		}

		protected virtual void OnSaveSchedulingPlanToPNGActionActivated (object sender, System.EventArgs e)
		{
			FileChooserDialog fileSave = new FileChooserDialog ("Save Scheduling Plan to PNG", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			if (fileSave.Run () == (int) ResponseType.Accept)
			{
				Gdk.Pixbuf schedPic = Gdk.Pixbuf.FromDrawable (schedPlanPanel.GdkWindow, Gdk.Colormap.System, 0, 0, 0, 0, schedPlanPanel.WidthRequest, schedPlanPanel.HeightRequest);
				schedPic.Save (fileSave.Filename, "png");
			}
			fileSave.Destroy ();			
		}

		protected virtual void OnSaveToXmlAction1Activated (object sender, System.EventArgs e)
		{
			FileChooserDialog fileSave = new FileChooserDialog ("Save Nodes to Xml File", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			if (fileSave.Run () == (int) ResponseType.Accept)
			{
				simulatorInterface.Nodes.SaveToXml (fileSave.Filename);
			}
			fileSave.Destroy ();
		}

		protected virtual void OnLoadFromXmlAction1Activated (object sender, System.EventArgs e)
		{
			FileChooserDialog fileOpen = new FileChooserDialog ("Load Nodes from Xml File", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Ok);
			if (fileOpen.Run () == (int) ResponseType.Ok)
			{
				NodeStore.Clear ();
				simulatorInterface.Nodes.RemoveAll ();
				simulatorInterface.Nodes.LoadFromXml (fileOpen.Filename);
				List<BasicNode> list = simulatorInterface.Nodes.GetAllNodes ();
				foreach (BasicNode node in list)
				{
					NodeStore.AppendValues (node.NodeId, node.TimeSlice);					
				}
			}
			fileOpen.Destroy ();			
		}
	}
}