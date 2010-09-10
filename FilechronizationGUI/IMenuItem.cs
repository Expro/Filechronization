/*
 * Created by SharpDevelop.
 * User: Expro
 * Date: 2010-09-08
 * Time: 17:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace FilechronizationGUI
{
	public interface IMenuItem
	{
		string Text {get;}
		
		event EventHandler Click;
	}
}
