using System;
using System.Numerics;
using System.Windows.Forms;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components
{
    public partial class PasteSpecialDialog : Form, IManualTranslate
    {
        public enum PasteSpecialStartPoint
        {
            Origin,
            CenterOriginal,
            CenterSelection
        }

        public enum PasteSpecialGrouping
        {
            None,
            Individual,
            All
        }

        private readonly Box _source;

        public int NumberOfCopies
        {
            get => (int) NumCopies.Value;
            set => NumCopies.Value = value;
        }

        public PasteSpecialStartPoint StartPoint
        {
            get
            {
                if (StartOrigin.Checked) return PasteSpecialStartPoint.Origin;
                if (StartOriginal.Checked) return PasteSpecialStartPoint.CenterOriginal;
                return PasteSpecialStartPoint.CenterSelection;
            }
            set
            {
                switch (value)
                {
                    case PasteSpecialStartPoint.Origin:
                        StartOrigin.Checked = true;
                        break;
                    case PasteSpecialStartPoint.CenterOriginal:
                        StartOriginal.Checked = true;
                        break;
                    case PasteSpecialStartPoint.CenterSelection:
                        StartSelection.Checked = true;
                        break;
                }
            }
        }

        public PasteSpecialGrouping Grouping
        {
            get
            {
                if (GroupNone.Checked) return PasteSpecialGrouping.None;
                if (GroupIndividual.Checked) return PasteSpecialGrouping.Individual;
                return PasteSpecialGrouping.All;
            }
            set
            {
                switch (value)
                {
                    case PasteSpecialGrouping.None:
                        GroupNone.Checked = true;
                        break;
                    case PasteSpecialGrouping.Individual:
                        GroupIndividual.Checked = true;
                        break;
                    case PasteSpecialGrouping.All:
                        GroupAll.Checked = true;
                        break;
                }
            }
        }

        public Vector3 AccumulativeOffset
        {
            get => new Vector3((float) OffsetX.Value, (float) OffsetY.Value, (float) OffsetZ.Value);
            set
            {
                OffsetX.Value = (decimal) value.X;
                OffsetY.Value = (decimal) value.Y;
                OffsetZ.Value = (decimal) value.Z;
            }
        }

        public Vector3 AccumulativeRotation
        {
            get => new Vector3((float) RotationX.Value, (float) RotationY.Value, (float) RotationZ.Value);
            set
            {
                RotationX.Value = (decimal) value.X;
                RotationY.Value = (decimal) value.Y;
                RotationZ.Value = (decimal) value.Z;
            }
        }

        public bool MakeEntitiesUnique
        {
            get => UniqueEntityNames.Checked;
            set => UniqueEntityNames.Checked = value;
        }

        public bool PrefixEntityNames
        {
            get => PrefixEntityNamesCheckbox.Checked;
            set
            {
                PrefixEntityNamesCheckbox.Checked = value;
                EntityPrefix.Enabled = PrefixEntityNamesCheckbox.Checked;
            }
        }

        public string EntityNamePrefix
        {
            get => EntityPrefix.Text;
            set => EntityPrefix.Text = value;
        }

        private static decimal _lastNumCopies = 1;
        private static decimal _lastXOffset = 0;
        private static decimal _lastYOffset = 0;
        private static decimal _lastZOffset = 0;
        private static decimal _lastXRotation = 0;
        private static decimal _lastYRotation = 0;
        private static decimal _lastZRotation = 0;

        public PasteSpecialDialog(Box source)
        {
            _source = source;
            InitializeComponent();
            EntityPrefix.Enabled = PrefixEntityNamesCheckbox.Checked;

            ZeroOffsetXButton.Click += (sender, e) => OffsetX.Value = 0;
            ZeroOffsetYButton.Click += (sender, e) => OffsetY.Value = 0;
            ZeroOffsetZButton.Click += (sender, e) => OffsetZ.Value = 0;

            SourceOffsetXButton.Click += (sender, e) => OffsetX.Value = (decimal) _source.Width;
            SourceOffsetYButton.Click += (sender, e) => OffsetY.Value = (decimal) _source.Length;
            SourceOffsetZButton.Click += (sender, e) => OffsetZ.Value = (decimal) _source.Height;

            ZeroRotationXButton.Click += (sender, e) => RotationX.Value = 0;
            ZeroRotationYButton.Click += (sender, e) => RotationY.Value = 0;
            ZeroRotationZButton.Click += (sender, e) => RotationZ.Value = 0;

            PrefixEntityNamesCheckbox.CheckedChanged += (sender, e) => EntityPrefix.Enabled = PrefixEntityNamesCheckbox.Checked;

            NumCopies.Value = _lastNumCopies;
            OffsetX.Value = _lastXOffset;
            OffsetY.Value = _lastYOffset;
            OffsetZ.Value = _lastZOffset;
            RotationX.Value = _lastXRotation;
            RotationY.Value = _lastYRotation;
            RotationZ.Value = _lastZRotation;
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");
                lblCopies.Text = strings.GetString(prefix, "NumberOfCopies");
                grpStartPoint.Text = strings.GetString(prefix, "StartPoint");
                StartOrigin.Text = strings.GetString(prefix, "StartAtOrigin");
                StartOriginal.Text = strings.GetString(prefix, "StartAtCenterOfOriginal");
                StartSelection.Text = strings.GetString(prefix, "StartAtCenterOfSelection");
                grpGrouping.Text = strings.GetString(prefix, "Grouping");
                GroupNone.Text = strings.GetString(prefix, "NoGrouping");
                GroupIndividual.Text = strings.GetString(prefix, "GroupIndividual");
                GroupAll.Text = strings.GetString(prefix, "GroupAll");
                grpOffset.Text = strings.GetString(prefix, "Offset");
                grpRotation.Text = strings.GetString(prefix, "Rotation");
                UniqueEntityNames.Text = strings.GetString(prefix, "MakeEntityNamesUnique");
                PrefixEntityNamesCheckbox.Text = strings.GetString(prefix, "PrevixEntityNames");
                OkButton.Text = strings.GetString(prefix, "OK");
                CancelButton.Text = strings.GetString(prefix, "Cancel");
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            NumCopies.Focus();
            NumCopies.Select(0, NumCopies.Text.Length);

            base.OnLoad(e);
        }

        private void OkButtonClicked(object sender, EventArgs e)
        {
            _lastNumCopies = NumCopies.Value;
            _lastXOffset = OffsetX.Value;
            _lastYOffset = OffsetY.Value;
            _lastZOffset = OffsetZ.Value;
            _lastXRotation = RotationX.Value;
            _lastYRotation = RotationY.Value;
            _lastZRotation = RotationZ.Value;
        }
    }
}
