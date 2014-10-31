using System.Collections.Generic;
using System.Drawing;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.QuickForms.Items
{
	/// <summary>
	/// Abstract base class for all form items.
	/// </summary>
	public interface IQuickFormItem
	{
		string Name { get; }
		IControl GetControl(QuickForm qf);
	}
}
