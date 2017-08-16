using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.QuickForms.Items;

namespace Sledge.QuickForms
{
	/// <summary>
	/// QuickForms is input dialogs for the lazy.
	/// It allows quick composition of disposable forms.
	/// </summary>
	public sealed class QuickForm : Form
	{
        /// <summary>
        /// Change the standard height of each item in the form.
        /// </summary>
		public static int ItemHeight = 20;

        /// <summary>
        /// Change the standard padding between each item in the form.
        /// </summary>
		public static int ItemPadding = 5;

        private readonly List<QuickFormItem> _items;

        /// <summary>
        /// The current Y offset of the form.
        /// </summary>
	    public int CurrentOffset { get; private set; }

	    /// <summary>
	    /// Get or set the width of item labels. This is not used 
	    /// for a Label item, which takes the full width.
	    /// </summary>
	    public int LabelWidth { get; set; }

	    public bool UseShortcutKeys { get; set; }

        /// <summary>
        /// Create a form with the specified title.
        /// </summary>
        /// <param name="title">The title of the form</param>
	    public QuickForm(string title)
	    {
			_items = new List<QuickFormItem>();
			LabelWidth = 100;
			CurrentOffset = ItemPadding;
			Text = title;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			ShowInTaskbar = false;
			MinimizeBox = false;
			MaximizeBox = false;
            UseShortcutKeys = false;
            KeyPreview = true;
	    }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (UseShortcutKeys)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var ok = Controls.OfType<Button>().FirstOrDefault(x => x.DialogResult == DialogResult.OK || x.DialogResult == DialogResult.Yes);
                    ok?.PerformClick();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    var cancel = Controls.OfType<Button>().FirstOrDefault(x => x.DialogResult == DialogResult.Cancel || x.DialogResult == DialogResult.No);
                    cancel?.PerformClick();
                }
            }
            base.OnKeyDown(e);
        }
		
		protected override void OnLoad(EventArgs e)
		{
			ClientSize = new System.Drawing.Size(ClientSize.Width, CurrentOffset + ItemPadding);
		    var nonlabel = Controls.OfType<Control>().FirstOrDefault(x => !(x is Label));
		    nonlabel?.Focus();
		    base.OnLoad(e);
		}

	    public async Task<DialogResult> ShowDialogAsync()
	    {
	        await Task.Yield();
	        return ShowDialog();
        }
		
        /// <summary>
        /// Add an item to the form.
        /// </summary>
        /// <param name="item">The item to add</param>
		public void AddItem(QuickFormItem item)
		{
			_items.Add(item);
			var ctl = item.GetControls(this);
		    Controls.AddRange(ctl.ToArray());
			CurrentOffset += ItemHeight + ItemPadding;
		}

        /// <summary>
        /// Add an item to the form.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm Item(QuickFormItem item)
        {
            AddItem(item);
            return this;
        }

	    /// <summary>
	    /// Add a textbox to the form.
	    /// </summary>
	    /// <param name="name">The name of the textbox</param>
	    /// <param name="value">The default value of the textbox</param>
	    /// <returns>This object, for method chaining</returns>
	    public QuickForm TextBox(string name, string value = "")
        {
            AddItem(new QuickFormTextBox(name, value));
            return this;
        }

	    /// <summary>
	    /// Add a browse textbox to the form.
	    /// </summary>
	    /// <param name="name">The name of the textbox</param>
	    /// <param name="filter">The filter for the open file dialog</param>
	    /// <returns>This object, for method chaining</returns>
	    public QuickForm Browse(string name, string filter)
        {
            AddItem(new QuickFormBrowse(name, filter));
            return this;
        }

        /// <summary>
        /// Add a label to the form.
        /// </summary>
        /// <param name="text">The text of the label</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm Label(string text)
		{
            AddItem(new QuickFormLabel(text));
            return this;
		}

	    /// <summary>
	    /// Add a NumericUpDown to the form.
	    /// </summary>
	    /// <param name="name">The name of the control</param>
	    /// <param name="min">The minimum value of the control</param>
	    /// <param name="max">The maximum value of the control</param>
	    /// <param name="decimals">The number of decimals for the control</param>
	    /// <param name="value">The default value of the control</param>
	    /// <returns>This object, for method chaining</returns>
	    [Obsolete("Use the other one")]
	    public QuickForm NumericUpDown(string name, int min, int max, int decimals, decimal value = 0)
        {
            AddItem(new QuickFormNumericUpDown(name, name, min, max, decimals, value));
            return this;
        }

	    /// <summary>
	    /// Add a NumericUpDown to the form.
	    /// </summary>
	    /// <param name="key">The name of the control</param>
	    /// <param name="label">The display text of the control</param>
	    /// <param name="min">The minimum value of the control</param>
	    /// <param name="max">The maximum value of the control</param>
	    /// <param name="decimals">The number of decimals for the control</param>
	    /// <param name="value">The default value of the control</param>
	    /// <returns>This object, for method chaining</returns>
	    public QuickForm NumericUpDown(string key, string label, int min, int max, int decimals, decimal value = 0)
        {
            AddItem(new QuickFormNumericUpDown(key, label, min, max, decimals, value));
            return this;
        }

        /// <summary>
        /// Add a ComboBox to the form.
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <param name="items">The items for the control</param>
        /// <returns>This object, for method chaining</returns>
        [Obsolete]
        public QuickForm ComboBox(string name, IEnumerable<object> items)
        {
            AddItem(new QuickFormComboBox(name, name, items));
            return this;
        }

        /// <summary>
        /// Add a ComboBox to the form.
        /// </summary>
        /// <param name="key">The name of the control</param>
        /// <param name="label">The display text of the control</param>
        /// <param name="items">The items for the control</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm ComboBox(string key, string label, IEnumerable<object> items)
        {
            AddItem(new QuickFormComboBox(key, label, items));
            return this;
        }

	    /// <summary>
	    /// Add a checkbox to the form.
	    /// </summary>
	    /// <param name="name">The name of the control</param>
	    /// <param name="value">The initial value of the checkbox</param>
	    /// <returns>This object, for method chaining</returns>
	    public QuickForm CheckBox(string name, bool value = false)
		{
            AddItem(new QuickFormCheckBox(name, value));
            return this;
		}

        /// <summary>
        /// Add OK and Cancel buttons to the control
        /// </summary>
        /// <param name="ok">The action to perform when OK is clicked</param>
        /// <param name="cancel">The action to perform when cancel is clicked</param>
        /// <returns>This object, for method chaining</returns>
        [Obsolete]
        public QuickForm OkCancel(Action<QuickForm> ok = null, Action<QuickForm> cancel = null)
		{
            AddItem(new QuickFormOkCancel(ok, cancel));
            return this;
		}

        /// <summary>
        /// Add OK and Cancel buttons to the control
        /// </summary>
        /// <param name="okText"></param>
        /// <param name="cancelText"></param>
        /// <param name="ok">The action to perform when OK is clicked</param>
        /// <param name="cancel">The action to perform when cancel is clicked</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm OkCancel(string okText, string cancelText, Action<QuickForm> ok = null, Action<QuickForm> cancel = null)
		{
            AddItem(new QuickFormOkCancel(okText, cancelText, ok, cancel));
            return this;
		}

        /// <summary>
        /// Add a button to the control
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="action">The action to perform when the button is clicked</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm Button(string text, Action action)
		{
            AddItem(new QuickFormButton(text, action));
            return this;
		}

		public void Close(object sender, EventArgs e)
		{
			Close();
		}
		
        /// <summary>
        /// Get a control by name
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The control, or null if it was not found</returns>
		public Control GetControl(string name)
		{
		    return Controls.OfType<Control>().FirstOrDefault(c => c.Name == name);
		}

        /// <summary>
        /// Get a string value from a control
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The string value</returns>
	    public string String(string name)
		{
			var c = GetControl(name);
			if (c != null) return c.Text;
			throw new Exception("Control " + name + " not found!");
		}

        /// <summary>
        /// Get a decimal value from a control
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The decimal value</returns>
		public decimal Decimal(string name)
		{
			var c = GetControl(name);
			if (c != null) return ((NumericUpDown) c).Value;
			throw new Exception("Control " + name + " not found!");
		}

        /// <summary>
        /// Get a boolean value from a control
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The boolean value</returns>
		public bool Bool(string name)
		{
			var c = GetControl(name);
			if (c != null) return ((CheckBox) c).Checked;
			throw new Exception("Control " + name + " not found!");
		}

        /// <summary>
        /// Get an object from a control
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The object</returns>
        public object Object(string name)
        {
            var c = GetControl(name);
            if (c != null) return ((ComboBox)c).SelectedItem;
            throw new Exception("Control " + name + " not found!");
        }
	}
}