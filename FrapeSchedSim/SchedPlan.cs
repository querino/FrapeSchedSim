
using System;
using System.Collections.Generic;
using Frapes;

namespace FrapeSchedSim
{
	
	
	public class SchedPlan
	{
		
		private List<SchedPlanResult> Results = new List<SchedPlanResult> ();
				
		public SchedPlan()
		{
		}
		
		public List<SchedPlanResult> GetAllResults ()
		{
			return this.Results;
		}
		
		public SchedPlanResult GetResult (int index)
		{
			return this.Results[index];
		}
		
		public void SetResults (List<SchedPlanResult> results)
		{
			this.Results = results;
		}
		
		public void Add (int procid, int status, int deadline)
		{
			SchedPlanResult result = new SchedPlanResult (procid, status, deadline);
			this.Results.Add (result);
		}
		
		public void Add (SchedPlanResult result)
		{
			this.Results.Add (result);
		}
		
		public void Clear ()
		{
			this.Results.Clear ();
		}
		
		public int Size ()
		{
			return this.Results.Count;
		}
	}
}
