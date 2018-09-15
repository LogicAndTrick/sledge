using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly FlowLayoutPanel _flpLayout;
        private readonly Panel _layoutSizerPanel;
        private readonly List<QuickFormItem> _items;

        /// <summary>
        /// True to use shortcut keys - enter will press an ok button, escape will press a cancel button.
        /// </summary>
	    public bool UseShortcutKeys { get; set; }

        /// <summary>
        /// Create a form with the specified title.
        /// </summary>
        /// <param name="title">The title of the form</param>
	    public QuickForm(string title)
	    {
			_items = new List<QuickFormItem>();
			Text = title;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			ShowInTaskbar = false;
			MinimizeBox = false;
			MaximizeBox = false;
            UseShortcutKeys = false;
            KeyPreview = true;

	        _flpLayout = new FlowLayoutPanel
	        {
	            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
	            AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
	            FlowDirection = FlowDirection.TopDown,
	            Location = new Point(5, 5),
	            Size = new Size(ClientSize.Width - 10, 10)
	        };
	        _layoutSizerPanel = new Panel
	        {
                AutoSize = false,
                Height = 1,
                Width = ClientSize.Width - 12,
                Margin = Padding.Empty,
                Padding = Padding.Empty
	        };
            _flpLayout.Controls.Add(_layoutSizerPanel);

	        Controls.Add(_flpLayout);
        }

        protected override void OnResize(EventArgs e)
        {
            _layoutSizerPanel.Width = ClientSize.Width - 12;
            base.OnResize(e);
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
		    var ps = _flpLayout.GetPreferredSize(new Size(ClientSize.Width, 100000));
            ClientSize = new Size(ClientSize.Width, ps.Height + 10);
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
        /// <param name="name">The name of the item</param>
        /// <param name="item">The item to add</param>
        private void AddItem(string name, QuickFormItem item)
        {
            item.Name = name;
			_items.Add(item);
		    _flpLayout.Controls.Add(item);
		}

        /// <summary>
        /// Add an item to the form.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm Item(QuickFormItem item)
        {
            AddItem(item.Name, item);
            return this;
        }

        /// <summary>
        /// Add a textbox to the form.
        /// </summary>
        /// <param name="name">The name of the textbox</param>
        /// <param name="text">The display text for the textbox</param>
        /// <param name="value">The default value of the textbox</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm TextBox(string name, string text, string value = "")
        {
            AddItem(name, new QuickFormTextBox(text, value));
            return this;
        }

        /// <summary>
        /// Add a browse textbox to the form.
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <param name="text">The display text for the control</param>
        /// <param name="filter">The filter for the open file dialog</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm Browse(string name, string text, string filter)
        {
            AddItem(name, new QuickFormBrowse(text, "Browse...", filter));
            return this;
        }

        /// <summary>
        /// Add a label to the form.
        /// </summary>
        /// <param name="text">The text of the label</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm Label(string text)
		{
            AddItem(string.Empty, new QuickFormLabel(text));
            return this;
		}

        /// <summary>
        /// Add a NumericUpDown to the form.
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <param name="text">The display text of the control</param>
        /// <param name="min">The minimum value of the control</param>
        /// <param name="max">The maximum value of the control</param>
        /// <param name="decimals">The number of decimals for the control</param>
        /// <param name="value">The default value of the control</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm NumericUpDown(string name, string text, int min, int max, int decimals, decimal value = 0)
        {
            AddItem(name, new QuickFormNumericUpDown(text, min, max, decimals, value));
            return this;
        }

        /// <summary>
        /// Add a ComboBox to the form.
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <param name="text">The display text of the control</param>
        /// <param name="items">The items for the control</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm ComboBox(string name, string text, IEnumerable<object> items)
        {
            AddItem(name, new QuickFormComboBox(text, items));
            return this;
        }

        /// <summary>
        /// Add a checkbox to the form.
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <param name="text">The display text for the checkbox</param>
        /// <param name="value">The initial value of the checkbox</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm CheckBox(string name, string text, bool value = false)
		{
            AddItem(name, new QuickFormCheckBox(text, value));
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
            AddItem(string.Empty, new QuickFormButtonSet(new (string, DialogResult, Action)[]
            {
                (okText, DialogResult.OK, OKAction),
                (cancelText, DialogResult.Cancel, CancelAction)
            }));

            return this;

            void OKAction()
            {
                ok?.Invoke(this);
                DialogResult = DialogResult.OK;
                Close();
            }

            void CancelAction()
            {
                cancel?.Invoke(this);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// Add a button to the control
        /// </summary>
        /// <param name="text">The button text</param>
        /// <param name="action">The action to perform when the button is clicked</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm Button(string text, Action<QuickForm> action)
        {
            AddItem(string.Empty, new QuickFormButtonSet(new[]
            {
                (text, DialogResult.None, new Action(() => action(this)))
            }));
            return this;
		}

        /// <summary>
        /// Add a set of buttons to the control, each one closing the form
        /// with a specific dialog result.
        /// </summary>
        /// <param name="buttons">The button texts and dialog results</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm DialogButtons(params (string, DialogResult)[] buttons)
        {
            AddItem(string.Empty, new QuickFormButtonSet(
                buttons.Select(x => (x.Item1, x.Item2, new Action(() => { })))
            ));
            return this;
		}

        /// <summary>
        /// Add a set of buttons to the control, each one performing a custom action.
        /// </summary>
        /// <param name="buttons">The button texts and dialog results</param>
        /// <returns>This object, for method chaining</returns>
        public QuickForm ActionButtons(params (string, Action<QuickForm>)[] buttons)
        {
            AddItem(string.Empty, new QuickFormButtonSet(
                buttons.Select(x => (x.Item1, DialogResult.None, new Action(() => x.Item2(this))))
            ));
            return this;
		}
		
        /// <summary>
        /// Get a control by name
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The control, or null if it was not found</returns>
		public QuickFormItem GetItem(string name)
        {
            return _items.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Get a string value from a control
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The string value</returns>
	    public string String(string name)
		{
			var c = GetItem(name);
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
			var c = GetItem(name);
			if (c != null) return (decimal) c.Value;
			throw new Exception("Control " + name + " not found!");
		}

        /// <summary>
        /// Get a boolean value from a control
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The boolean value</returns>
		public bool Bool(string name)
		{
			var c = GetItem(name);
			if (c != null) return (bool) c.Value;
			throw new Exception("Control " + name + " not found!");
		}

        /// <summary>
        /// Get an object from a control
        /// </summary>
        /// <param name="name">The name of the control</param>
        /// <returns>The object</returns>
        public object Object(string name)
        {
            var c = GetItem(name);
            if (c != null) return c.Value;
            throw new Exception("Control " + name + " not found!");
        }

        /// <summary>
        /// Get all the named keys and values for this form
        /// </summary>
        /// <returns>Key/value collection</returns>
        public Dictionary<string, object> GetValues()
        {
            return _items.Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x.First().Value);
        }
    }
}