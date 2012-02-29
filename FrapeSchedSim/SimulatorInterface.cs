
using System;
using System.Collections.Generic;
using Frapes;
using Frapes.Algorithms;
using Frapes.Taxonomy.Hierarchical;

namespace FrapeSchedSim
{
	
	
	public class SimulatorInterface
	{
		
		internal ProcessList Processes = new ProcessList ();
		
	    internal NodeList Nodes = new NodeList ();
		
		internal IGlobal Strategy = null;
		
		internal List<SchedPlan> SchedPlans;
				
		public SimulatorInterface()
		{
			SchedPlans = new List<SchedPlan> ();
			
			for (int i = 0; i < Nodes.Count; i++)
			{
				SchedPlans[i] = new SchedPlan ();
			}
		}
		
		public SchedPlan GetSchedPlan (int procid)
		{
			return SchedPlans[procid];	
		}
		
		public List<SchedPlan> GetSchedPlans ()
		{
			return SchedPlans;
		}
		
		public int GetNumberOfNodes ()
		{
			return Nodes.Count;
		}
				
		public bool SetStrategyByName (string name)
		{
			if (name == "Round Robin")
			{
				Strategy = new RoundRobin ();
			}
			else if (name == "First Come First Serve")
			{
				Strategy = new FirstComeFirstServe ();
			}
			
			// Clear the results
			List<BasicNode> nodes = Nodes.GetAllNodes ();
			SchedPlans = new List<SchedPlan> ();
			for (int a = 0; a < nodes.Count; a++)
			{
				SchedPlans.Add (new SchedPlan ());
			}
			
			// Clear running processes
			Processes.RunningProcessesReset ();
			return true;
		}
		
		public void RunNextTimeUnit ()
		{
			if (Strategy == null)
			{
				throw new Exception("No scheduling algorithm selected");
			}
			
			List<BasicNode> nodes = Nodes.GetAllNodes ();
			
			// Free running processes at end of preemption
			for (int n = 0; n < nodes.Count; n++)
			{
				int actualTimeSlice = nodes[n].ActualTimeSlice;
				int timeSliceLength = nodes[n].TimeSlice;
				
				if (this.Strategy.IsPreemptive () && (actualTimeSlice == timeSliceLength) && (actualTimeSlice != 0))
				{
					// Preemptive strategy, and the end of a timeslice is reached
					if (nodes[n].LastScheduled.State == Defines.Running)
					{
						nodes[n].LastScheduled.SetReady ();
						if (Defines.DebugMode)
						{
//							Console.WriteLine ("Freed Proc.# " + nodes[n].LastScheduled.ProcessId.ToString () + " because of end of preemption");
						}
					}
				}
			}
			
			for (int n = 0; n < nodes.Count; n++)
			{
				if (Defines.DebugMode)
				{
//					Console.WriteLine ("Running Processor: " + n + 1);
				}
				
				BasicProcess result = null;
				int state = 0;
				int actualTimeSlice = nodes[n].ActualTimeSlice;
				int timeSliceLength = nodes[n].TimeSlice;
				
				if (Strategy.IsPreemptive () && (timeSliceLength != 0))
				{
					if (Defines.DebugMode)
					{
//						Console.WriteLine ("Preemptive: ActualTimeSlice: " + actualTimeSlice + " of " + timeSliceLength);
					}
					
					if ((nodes[n].LastScheduled.ProcessId == 0) || (nodes[n].LastScheduled.State == Defines.Finished) || (nodes[n].LastScheduled.State == Defines.Blocked))
					{
						// No process selected last time, or selected process is finished
						// We have to schedule a new process
						result = Strategy.Schedule (Processes.GetAllRunningProcesses ());
						nodes[n].ActualTimeSlice = 1;
						state = Defines.PreemptiveProcessFinishedOrBlocked;
					}
					else if (timeSliceLength == actualTimeSlice)
					{
						// End of timeslice: new process have to be scheduled
						result = Strategy.Schedule (Processes.GetAllRunningProcesses ());
						// Start of a new timeslice
						nodes[n].ActualTimeSlice = 1;
						state = Defines.PreemptiveTimesliceEnd;
					}
					else
					{
						// Inside a timeslice, so continue
						result = nodes[n].LastScheduled;
						if (Defines.DebugMode)
						{
//							Console.WriteLine ("Preemptive: Continue Proc.# " + result.ProcessId);
						}
						actualTimeSlice++;
						nodes[n].ActualTimeSlice = actualTimeSlice;
						state = Defines.PreemptiveTimesliceContinued;
					}
				}
				else
				{
					// Non preemptive mode
					if ((nodes[n].LastScheduled.ProcessId == 0) || (nodes[n].LastScheduled.State == Defines.Finished) || (nodes[n].LastScheduled.State == Defines.Blocked))
					{
						result = Strategy.Schedule (Processes.GetAllRunningProcesses ());
						state = Defines.NonPreemptiveProcessFinishedOrBlocked;
					}
					else
					{
						result = nodes[n].LastScheduled;
						if (Defines.DebugMode)
						{
//							Console.WriteLine ("Nonpreemptive: Continue proc.# " + result.ProcessId);
						}
						state = Defines.NonPreemptiveProcessContinued;
					}
				}
				
				if (result.ProcessId != 0)
				{
					result.Run ();
				}
				
				nodes[n].LastScheduled = result;
				
				List<BasicProcess> Proc = Processes.GetAllRunningProcesses ();
				int DeadTemp = 10;
				
				// Check if deadline was crossed
				for (int x = 0; x < Proc.Count; x++)
				{
					if ((Proc[x].DeadlineTime <= 0) && (Proc[x].ExecutionTime != 0))
					{
						DeadTemp = -1;
						break;
					}
				}
				
				SchedPlans[n].Add (result.ProcessId, state, DeadTemp);
				
				if (DeadTemp <= 0)
				{
					for (int dead = 0; dead < n; dead++)
					{
						SchedPlans[dead].GetResult (SchedPlans[dead].Size() - 1).Deadline = DeadTemp;
					}
				}
			}
			Processes.RunningProcessesUpdateStat ();
		}
		
		public void Reset ()
		{
			// Reset scheduler
			if (Strategy != null)
			{
				string sh = Strategy.ToString ();
				try
				{
					this.SetStrategyByName (sh);
				}
				catch
				{
//					Console.WriteLine ("SchedulerInterface.Reset: Exception");
				}
			}
			Processes.RunningProcessesReset ();
			
			List<BasicNode> nodes = Nodes.GetAllNodes ();

			for (int a = 0; a < Nodes.Count; a++)
			{
				nodes[a].LastScheduled = new BasicProcess ();
				nodes[a].Override = 0;
				nodes[a].Status = 0;
				
				SchedPlans[a] = new SchedPlan ();
			}
		}
	}
}
