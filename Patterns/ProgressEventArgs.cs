/*
 *
 * User: Expro
 * Date: 2010-07-30
 * Time: 05:16
 * 
 * 
 */
using System;
using System.Diagnostics.Contracts;

namespace Patterns
{
	[Serializable]
	public class ProgressEventArgs: EventArgs
	{
		private int steps;
		private int step;
		private string process;
		private string processedItem;
		
		public ProgressEventArgs(string process, string processedItem, int step, int steps)
		{
			Contract.Requires(!String.IsNullOrEmpty(process));
			Contract.Requires(!String.IsNullOrEmpty(processedItem));
			Contract.Requires(step >= 0);
			Contract.Requires(steps >= 1);
			Contract.Requires(step < steps);
			
			this.steps = steps;
			this.step = step;
			this.process = process;
			this.processedItem = processedItem;
		}
		
		public string ProcessedItem
		{
			get {return processedItem;}
		}
		
		public string Process
		{
			get {return process;}
		}
		
		public int Step
		{
			get {return step;}
		}
		
		public int Steps
		{
			get {return steps;}
		}
	}
}
