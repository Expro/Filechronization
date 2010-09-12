/*
 *
 * User: Expro
 * Date: 2010-07-31
 * Time: 16:51
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeManagement;
using CodeManagement.Definitions;

namespace FilechronizationGUI
{
	public interface IFilechronizationMainWindow
	{
		Control AddContentToCenter(string title, Control content);
	}
}
