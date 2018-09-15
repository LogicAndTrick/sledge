using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// Abstract base class for all form items.
	/// </summary>
	public abstract class QuickFormItem : FlowLayoutPanel
	{
	    protected const int LabelWidth = 80;

        public abstract object Value { get; }

	    protected QuickFormItem()
	    {
	        FlowDirection = FlowDirection.LeftToRight;
	        AutoSize = true;
	        AutoSizeMode = AutoSizeMode.GrowAndShrink;
	        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
	        Margin = Padding.Empty;
	    }
	}
}
