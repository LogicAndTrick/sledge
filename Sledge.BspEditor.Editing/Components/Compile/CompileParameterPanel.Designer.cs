namespace Sledge.BspEditor.Editing.Components.Compile
{
    partial class CompileParameterPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.GeneratedParametersTextbox = new System.Windows.Forms.TextBox();
            this.AdditionalParametersTextbox = new System.Windows.Forms.TextBox();
            this.AdditionalParametersCheckbox = new System.Windows.Forms.CheckBox();
            this.HoverTip = new System.Windows.Forms.ToolTip(this.components);
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FlowPanel
            // 
            this.FlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FlowPanel.AutoScroll = true;
            this.FlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FlowPanel.Location = new System.Drawing.Point(3, 21);
            this.FlowPanel.Name = "FlowPanel";
            this.FlowPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.FlowPanel.Size = new System.Drawing.Size(294, 171);
            this.FlowPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 201);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Generated Parameters";
            // 
            // GeneratedParametersTextbox
            // 
            this.GeneratedParametersTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneratedParametersTextbox.Location = new System.Drawing.Point(137, 198);
            this.GeneratedParametersTextbox.Name = "GeneratedParametersTextbox";
            this.GeneratedParametersTextbox.ReadOnly = true;
            this.GeneratedParametersTextbox.Size = new System.Drawing.Size(160, 20);
            this.GeneratedParametersTextbox.TabIndex = 2;
            // 
            // AdditionalParametersTextbox
            // 
            this.AdditionalParametersTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AdditionalParametersTextbox.Enabled = false;
            this.AdditionalParametersTextbox.Location = new System.Drawing.Point(137, 224);
            this.AdditionalParametersTextbox.Name = "AdditionalParametersTextbox";
            this.AdditionalParametersTextbox.Size = new System.Drawing.Size(160, 20);
            this.AdditionalParametersTextbox.TabIndex = 2;
            this.AdditionalParametersTextbox.TextChanged += new System.EventHandler(this.AdditionalParametersTextboxChanged);
            // 
            // AdditionalParametersCheckbox
            // 
            this.AdditionalParametersCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AdditionalParametersCheckbox.AutoSize = true;
            this.AdditionalParametersCheckbox.Location = new System.Drawing.Point(3, 226);
            this.AdditionalParametersCheckbox.Name = "AdditionalParametersCheckbox";
            this.AdditionalParametersCheckbox.Size = new System.Drawing.Size(128, 17);
            this.AdditionalParametersCheckbox.TabIndex = 3;
            this.AdditionalParametersCheckbox.Text = "Additional Parameters";
            this.AdditionalParametersCheckbox.UseVisualStyleBackColor = true;
            this.AdditionalParametersCheckbox.CheckedChanged += new System.EventHandler(this.AdditionalParametersChanged);
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionLabel.Location = new System.Drawing.Point(4, 5);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(100, 13);
            this.DescriptionLabel.TabIndex = 1;
            this.DescriptionLabel.Text = "Description here";
            // 
            // CompileParameterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AdditionalParametersCheckbox);
            this.Controls.Add(this.AdditionalParametersTextbox);
            this.Controls.Add(this.GeneratedParametersTextbox);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FlowPanel);
            this.MinimumSize = new System.Drawing.Size(300, 250);
            this.Name = "CompileParameterPanel";
            this.Size = new System.Drawing.Size(300, 250);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FlowPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox GeneratedParametersTextbox;
        private System.Windows.Forms.TextBox AdditionalParametersTextbox;
        private System.Windows.Forms.CheckBox AdditionalParametersCheckbox;
        private System.Windows.Forms.ToolTip HoverTip;
        private System.Windows.Forms.Label DescriptionLabel;
    }
}
