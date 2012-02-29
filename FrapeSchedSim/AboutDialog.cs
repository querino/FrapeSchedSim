
using System;

namespace FrapeSchedSim
{
	
	
	public partial class AboutDialog : Gtk.Dialog
	{

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}		
		
		public AboutDialog()
		{
			this.Build();
		}
	}
}
