namespace Sledge.Shell.Forms
{
    partial class TranslationForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranslationForm));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.cmbFile = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNewLanguage = new System.Windows.Forms.Button();
            this.colFullID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFriendlyID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnglish = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTranslation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFullID,
            this.Type,
            this.colFriendlyID,
            this.colEnglish,
            this.colTranslation});
            this.dataGridView.Location = new System.Drawing.Point(11, 62);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView.RowTemplate.Height = 24;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(785, 463);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EditCell);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Target language";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "File to translate";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Location = new System.Drawing.Point(104, 9);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(318, 21);
            this.cmbLanguage.TabIndex = 3;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.LanguageChanged);
            // 
            // cmbFile
            // 
            this.cmbFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFile.FormattingEnabled = true;
            this.cmbFile.Location = new System.Drawing.Point(104, 36);
            this.cmbFile.Name = "cmbFile";
            this.cmbFile.Size = new System.Drawing.Size(318, 21);
            this.cmbFile.TabIndex = 3;
            this.cmbFile.SelectedIndexChanged += new System.EventHandler(this.FileChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(429, 36);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(126, 21);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save changes";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.SaveClicked);
            // 
            // btnNewLanguage
            // 
            this.btnNewLanguage.Location = new System.Drawing.Point(428, 9);
            this.btnNewLanguage.Name = "btnNewLanguage";
            this.btnNewLanguage.Size = new System.Drawing.Size(127, 21);
            this.btnNewLanguage.TabIndex = 4;
            this.btnNewLanguage.Text = "Add a new language";
            this.btnNewLanguage.UseVisualStyleBackColor = true;
            this.btnNewLanguage.Click += new System.EventHandler(this.AddLanguageClicked);
            // 
            // colFullID
            // 
            this.colFullID.DataPropertyName = "ID";
            this.colFullID.HeaderText = "Full ID";
            this.colFullID.Name = "colFullID";
            this.colFullID.ReadOnly = true;
            this.colFullID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colFullID.Visible = false;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Visible = false;
            // 
            // colFriendlyID
            // 
            this.colFriendlyID.DataPropertyName = "FriendlyID";
            this.colFriendlyID.HeaderText = "ID";
            this.colFriendlyID.Name = "colFriendlyID";
            this.colFriendlyID.ReadOnly = true;
            this.colFriendlyID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colEnglish
            // 
            this.colEnglish.DataPropertyName = "English";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colEnglish.DefaultCellStyle = dataGridViewCellStyle1;
            this.colEnglish.HeaderText = "English";
            this.colEnglish.Name = "colEnglish";
            this.colEnglish.ReadOnly = true;
            this.colEnglish.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colTranslation
            // 
            this.colTranslation.DataPropertyName = "Translation";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colTranslation.DefaultCellStyle = dataGridViewCellStyle2;
            this.colTranslation.HeaderText = "Translation";
            this.colTranslation.Name = "colTranslation";
            this.colTranslation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TranslationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 536);
            this.Controls.Add(this.btnNewLanguage);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbFile);
            this.Controls.Add(this.cmbLanguage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(484, 251);
            this.Name = "TranslationForm";
            this.Text = "Sledge Translator";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.ComboBox cmbFile;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNewLanguage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFullID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFriendlyID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEnglish;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTranslation;
    }
}

