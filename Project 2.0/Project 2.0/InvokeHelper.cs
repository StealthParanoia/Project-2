using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2._0
{
	public static class InvokeHelper
	{
		/// <summary>
		/// Sends the event on the thread of the subscriber.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event arguments</param>
		/// <param name="eventHandler">Event handler</param>
		public static void SendEvent(object sender, EventArgs e, Delegate eventHandler)
		{
			var handler = eventHandler;
			if (null != handler)
			{
				foreach (var singleCast in handler.GetInvocationList())
				{
					var syncInvoke =
						singleCast.Target as ISynchronizeInvoke;
					try
					{
						if ((null != syncInvoke) && (syncInvoke.InvokeRequired))
							syncInvoke.Invoke(singleCast,
											  new object[] { sender, e });
						else
						{
							singleCast.DynamicInvoke(sender, e);
							//							singleCast(sender, e);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
		}


	}
}
