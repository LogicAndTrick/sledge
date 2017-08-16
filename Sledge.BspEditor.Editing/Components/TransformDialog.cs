using System;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components
{
    public partial class TransformDialog : Form, IManualTranslate
    {
        public enum TransformType
        {
            Rotate,
            Translate,
            Scale
        }

        public class CannotScaleByZeroException : Exception { }

        private readonly Box _source;
        private decimal _zeroValue = 0;

        public Coordinate TransformValue
        {
            get => new Coordinate(ValueX.Value, ValueY.Value, ValueZ.Value);
            set
            {
                ValueX.Value = value.X;
                ValueY.Value = value.Y;
                ValueZ.Value = value.Z;
            }
        }

        public TransformType Type
        {
            get
            {
                if (lblRotate.Checked) return TransformType.Rotate;
                if (lblScale.Checked) return TransformType.Scale;
                return TransformType.Translate;
            }
            set
            {
                switch (value)
                {
                    case TransformType.Rotate:
                        lblRotate.Checked = true;
                        break;
                    case TransformType.Scale:
                        lblScale.Checked = true;
                        break;
                    default:
                        lblTranslate.Checked = true;
                        break;
                }
            }
        }

        public TransformDialog(Box source)
        {
            _source = source;
            InitializeComponent();

            ZeroValueXButton.Click += (sender, e) => ValueX.Value = _zeroValue;
            ZeroValueYButton.Click += (sender, e) => ValueY.Value = _zeroValue;
            ZeroValueZButton.Click += (sender, e) => ValueZ.Value = _zeroValue;

            SourceValueXButton.Click += (sender, e) => ValueX.Value = _source.Width;
            SourceValueYButton.Click += (sender, e) => ValueY.Value = _source.Length;
            SourceValueZButton.Click += (sender, e) => ValueZ.Value = _source.Height;

            TypeChanged(null, null);
        }

        public void Translate(TranslationStringsCollection strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.Invoke(() =>
            {
                Text = strings.GetString(prefix, "Title");

                var src = strings.GetString(prefix, "Source");
                SourceValueXButton.Text = src;
                SourceValueYButton.Text = src;
                SourceValueZButton.Text = src;

                lblRotate.Text = strings.GetString(prefix, "Rotate");
                lblTranslate.Text = strings.GetString(prefix, "Translate");
                lblScale.Text = strings.GetString(prefix, "Scale");

                OkButton.Text = strings.GetString(prefix, "OK");
                CancelButton.Text = strings.GetString(prefix, "Cancel");
            });
        }

        public Matrix GetTransformation(Box selectionBox)
        {
            var value = TransformValue;
            switch (Type)
            {
                case TransformType.Rotate:
                    var rMov = Matrix.Translation(-selectionBox.Center);
                    var rRot = Matrix.Rotation(Quaternion.EulerAngles(value * DMath.PI / 180));
                    var rFin = Matrix.Translation(selectionBox.Center);
                    return rFin * rRot * rMov;
                case TransformType.Translate:
                    return Matrix.Translation(value);
                case TransformType.Scale:
                    if (value.X == 0 || value.Y == 0 || value.Z == 0) throw new CannotScaleByZeroException();
                    var sMov = Matrix.Translation(-selectionBox.Center);
                    var sScl = Matrix.Scale(value);
                    var sFin = Matrix.Translation(selectionBox.Center);
                    return sFin * sScl * sMov;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TypeChanged(object sender, EventArgs e)
        {
            SourceValueXButton.Visible
                = SourceValueYButton.Visible
                  = SourceValueZButton.Visible
                    = lblTranslate.Checked;
            ZeroValueXButton.Text
                = ZeroValueYButton.Text
                  = ZeroValueZButton.Text
                    = lblScale.Checked ? "1" : "0";
            if (lblScale.Checked)
            {
                if (ValueX.Value == 0) ValueX.Value = 1;
                if (ValueY.Value == 0) ValueY.Value = 1;
                if (ValueZ.Value == 0) ValueZ.Value = 1;
                _zeroValue = 1;
            }
            else
            {
                if (ValueX.Value == 1) ValueX.Value = 0;
                if (ValueY.Value == 1) ValueY.Value = 0;
                if (ValueZ.Value == 1) ValueZ.Value = 0;
                _zeroValue = 0;
            }
        }
    }
}
