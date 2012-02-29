
using System;
using Frapes;

namespace FrapeSchedSim
{
	
	
	public class SchedPlanResult
	{
		
		private int _processid;
		private int _status;
		private int _deadline;
		
		public int ProcessId
		{
			get
			{
				return this._processid;
			}
			set
			{
				this._processid = value;
			}
		}
		
		public int Status
		{
			get
			{
				return this._status;
			}
			set
			{
				this._status = value;
			}
		}
		
		public int Deadline
		{
			get
			{
				return this._deadline;
			}
			set
			{
				this._deadline = value;
			}
		}
		
		public SchedPlanResult ()
		{
		}
		
		public SchedPlanResult (int procid, int status, int deadline)
		{
			this.ProcessId = procid;
			this.Status = status;
			this.Deadline = deadline;
		}
	}
}
