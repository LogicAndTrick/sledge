using System;
using System.Numerics;
using System.Windows.Forms;
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

        public Vector3 TransformValue
        {
            get => new Vector3((float) ValueX.Value, (float) ValueY.Value, (float) ValueZ.Value);
            set
            {
                ValueX.Value = (decimal) value.X;
                ValueY.Value = (decimal) value.Y;
                ValueZ.Value = (decimal) value.Z;
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

            SourceValueXButton.Click += (sender, e) => ValueX.Value = (decimal) _source.Width;
            SourceValueYButton.Click += (sender, e) => ValueY.Value = (decimal) _source.Length;
            SourceValueZButton.Click += (sender, e) => ValueZ.Value = (decimal) _source.Height;

            TypeChanged(null, null);
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
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

        public Matrix4x4 GetTransformation(Box selectionBox)
        {
            var value = TransformValue;
            switch (Type)
            {
                case TransformType.Rotate:
                    var rads = value * (float) Math.PI / 180;
                    var rMov = Matrix4x4.CreateTranslation(selectionBox.Center);
                    var rRot = Matrix4x4.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(rads.Y, rads.X, rads.Z));
                    var rFin = Matrix4x4.CreateTranslation(-selectionBox.Center);
                    return rFin * rRot * rMov;
                case TransformType.Translate:
                    return Matrix4x4.CreateTranslation(value);
                case TransformType.Scale:
                    if (Math.Abs(value.X) < 0.001 || Math.Abs(value.Y) < 0.001 || Math.Abs(value.Z) < 0.001) throw new CannotScaleByZeroException();
                    var sMov = Matrix4x4.CreateTranslation(-selectionBox.Center);
                    var sScl = Matrix4x4.CreateScale(value);
                    var sFin = Matrix4x4.CreateTranslation(selectionBox.Center);
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
