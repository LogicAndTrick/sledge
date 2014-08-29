//
// GtkUtil.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
//
// Copyright (c) 2014 Xamarin, Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using System.Collections.Generic;
using Gtk;
using System.Runtime.InteropServices;

namespace Mono.TextEditor
{
	static partial class GtkUtil
	{
		public static Cairo.Color ToCairoColor (this Gdk.Color color)
		{
			return new Cairo.Color ((double)color.Red / ushort.MaxValue,
				(double)color.Green / ushort.MaxValue,
				(double)color.Blue / ushort.MaxValue);
		}

		public static Xwt.Drawing.Color ToXwtColor (this Gdk.Color color)
		{
			return new Xwt.Drawing.Color ((double)color.Red / ushort.MaxValue,
				(double)color.Green / ushort.MaxValue,
				(double)color.Blue / ushort.MaxValue);
		}

		public static Gdk.Color ToGdkColor (this Cairo.Color color)
		{
			return new Gdk.Color ((byte)(color.R * 255d), (byte)(color.G * 255d), (byte)(color.B * 255d));
		}

		public static Xwt.Drawing.Context CreateXwtContext (this Gtk.Widget w)
		{
			var c = Gdk.CairoHelper.Create (w.GdkWindow);
			return Xwt.Toolkit.CurrentEngine.WrapContext (w, c);
		}
	}
}

// GtkUtil.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

namespace MonoDevelop.Components
{
	public static partial class GtkUtil
	{
		static readonly Xwt.Toolkit gtkToolkit = Xwt.Toolkit.Load (Xwt.ToolkitType.Gtk);

		public static Cairo.Color ToCairoColor (this Gdk.Color color)
		{
			return new Cairo.Color ((double)color.Red / ushort.MaxValue,
				(double)color.Green / ushort.MaxValue,
				(double)color.Blue / ushort.MaxValue);
		}

		public static Xwt.Drawing.Color ToXwtColor (this Gdk.Color color)
		{
			return new Xwt.Drawing.Color ((double)color.Red / ushort.MaxValue,
				(double)color.Green / ushort.MaxValue,
				(double)color.Blue / ushort.MaxValue);
		}

		public static Gdk.Color ToGdkColor (this Cairo.Color color)
		{
			return new Gdk.Color ((byte)(color.R * 255d), (byte)(color.G * 255d), (byte)(color.B * 255d));
		}

		public static Gdk.Color ToGdkColor (this Xwt.Drawing.Color color)
		{
			return new Gdk.Color ((byte)(color.Red * 255d), (byte)(color.Green * 255d), (byte)(color.Blue * 255d));
		}

		public static Cairo.Color ToCairoColor (this Xwt.Drawing.Color color)
		{
			return new Cairo.Color (color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static Xwt.Drawing.Color ToXwtColor (this Cairo.Color color)
		{
			return new Xwt.Drawing.Color (color.R, color.G, color.B, color.A);
		}

		/// <summary>
		/// Makes a color lighter or darker
		/// </summary>
		/// <param name='lightAmount'>
		/// Amount of lightness to add. If the value is positive, the color will be lighter,
		/// if negative it will be darker. Value must be between 0 and 1.
		/// </param>
		public static Gdk.Color AddLight (this Gdk.Color color, double lightAmount)
		{
			var c = color.ToXwtColor ();
			c.Light += lightAmount;
			return c.ToGdkColor ();
		}

		public static Cairo.Color AddLight (this Cairo.Color color, double lightAmount)
		{
			var c = color.ToXwtColor ();
			c.Light += lightAmount;
			return c.ToCairoColor ();
		}

		public static Xwt.Drawing.Context CreateXwtContext (this Gtk.Widget w)
		{
			var c = Gdk.CairoHelper.Create (w.GdkWindow);
			return gtkToolkit.WrapContext (w, c);
		}

		public static Gtk.Widget ToGtkWidget (this Xwt.Widget widget)
		{
			return (Gtk.Widget) gtkToolkit.GetNativeWidget (widget);
		}

		public static void DrawImage (this Cairo.Context s, Gtk.Widget widget, Xwt.Drawing.Image image, double x, double y)
		{
			gtkToolkit.RenderImage (widget, s, image, x, y);
		}

		public static Xwt.Drawing.Image ToXwtImage (this Gdk.Pixbuf pix)
		{
			return gtkToolkit.WrapImage (pix);
		}

		public static Gdk.Pixbuf ToPixbuf (this Xwt.Drawing.Image image)
		{
			return (Gdk.Pixbuf)gtkToolkit.GetNativeImage (image);
		}

		public static Gdk.Pixbuf ToPixbuf (this Xwt.Drawing.Image image, Gtk.IconSize size)
		{
			return (Gdk.Pixbuf)gtkToolkit.GetNativeImage (image.WithSize (size));
		}

		public static Xwt.Drawing.Image WithSize (this Xwt.Drawing.Image image, Gtk.IconSize size)
		{
			int w, h;
			if (!Gtk.Icon.SizeLookup (size, out w, out h))
				return image;
			if (size == IconSize.Menu)
				w = h = 16;
			return image.WithSize (w, h);
		}

		public static Gdk.Rectangle ToScreenCoordinates (Gtk.Widget widget, Gdk.Window w, Gdk.Rectangle rect)
		{
			return new Gdk.Rectangle (ToScreenCoordinates (widget, w, rect.X, rect.Y), rect.Size);
		}

		public static Gdk.Point ToScreenCoordinates (Gtk.Widget widget, Gdk.Window w, int x, int y)
		{
			int ox, oy;
			w.GetOrigin (out ox, out oy);
			ox += widget.Allocation.X;
			oy += widget.Allocation.Y;
			return new Gdk.Point (ox + x, oy + y);
		}

		public static Gdk.Rectangle ToWindowCoordinates (Gtk.Widget widget, Gdk.Window w, Gdk.Rectangle rect)
		{
			return new Gdk.Rectangle (ToWindowCoordinates (widget, w, rect.X, rect.Y), rect.Size);
		}

		public static Gdk.Point ToWindowCoordinates (Gtk.Widget widget, Gdk.Window w, int x, int y)
		{
			int ox, oy;
			w.GetOrigin (out ox, out oy);
			ox += widget.Allocation.X;
			oy += widget.Allocation.Y;
			return new Gdk.Point (x - ox, y - oy);
		}

		public static T ReplaceWithWidget<T> (this Gtk.Widget oldWidget, T newWidget, bool transferChildren = false) where T:Gtk.Widget
		{
			Gtk.Container parent = (Gtk.Container) oldWidget.Parent;
			if (parent == null)
				throw new InvalidOperationException ();

			if (parent is Box) {
				var box = (Box) parent;
				var bc = (Gtk.Box.BoxChild) parent [oldWidget];
				box.Add (newWidget);
				var nc = (Gtk.Box.BoxChild) parent [newWidget];
				nc.Expand = bc.Expand;
				nc.Fill = bc.Fill;
				nc.PackType = bc.PackType;
				nc.Padding = bc.Padding;
				nc.Position = bc.Position;
				box.Remove (oldWidget);
			}
			else if (parent is Table) {
				var table = (Table) parent;
				var bc = (Gtk.Table.TableChild) parent [oldWidget];
				table.Add (newWidget);
				var nc = (Gtk.Table.TableChild) parent [newWidget];
				nc.BottomAttach = bc.BottomAttach;
				nc.LeftAttach = bc.LeftAttach;
				nc.RightAttach = bc.RightAttach;
				nc.TopAttach = bc.TopAttach;
				nc.XOptions = bc.XOptions;
				nc.XPadding = bc.XPadding;
				nc.YOptions = bc.YOptions;
				nc.YPadding = bc.YPadding;
				table.Remove (oldWidget);
			}
			else if (parent is Paned) {
				var paned = (Paned) parent;
				var bc = (Gtk.Paned.PanedChild) parent [oldWidget];
				var resize = bc.Resize;
				var shrink = bc.Shrink;
				if (oldWidget == paned.Child1) {
					paned.Remove (oldWidget);
					paned.Add1 (newWidget);
				} else {
					paned.Remove (oldWidget);
					paned.Add2 (newWidget);
				}
				var nc = (Gtk.Paned.PanedChild) parent [newWidget];
				nc.Resize = resize;
				nc.Shrink = shrink;
			}
			else
				throw new NotSupportedException ();

			if (transferChildren) {
				if (newWidget is Paned && oldWidget is Paned) {
					var panedOld = (Paned) oldWidget;
					var panedNew = (Paned) (object) newWidget;
					if (panedOld.Child1 != null) {
						var c = panedOld.Child1;
						var bc = (Gtk.Paned.PanedChild) panedOld [c];
						var resize = bc.Resize;
						var shrink = bc.Shrink;
						panedOld.Remove (c);
						panedNew.Add1 (c);
						var nc = (Gtk.Paned.PanedChild) panedNew [c];
						nc.Resize = resize;
						nc.Shrink = shrink;
					}
					if (panedOld.Child2 != null) {
						var c = panedOld.Child2;
						var bc = (Gtk.Paned.PanedChild) panedOld [c];
						var resize = bc.Resize;
						var shrink = bc.Shrink;
						panedOld.Remove (c);
						panedNew.Add2 (c);
						var nc = (Gtk.Paned.PanedChild) panedNew [c];
						nc.Resize = resize;
						nc.Shrink = shrink;
					}
				}
				else
					throw new NotSupportedException ();
			}

			newWidget.Visible = oldWidget.Visible;
			return newWidget;
		}

		public static bool ScreenSupportsARGB ()
		{
			return Gdk.Screen.Default.IsComposited;
		}

		/// <summary>
		/// This method can be used to get a reliave Leave event for a widget, which
		/// is not fired if the pointer leaves the widget to enter a child widget.
		/// To ubsubscribe the event, dispose the object returned by the method.
		/// </summary>
		public static IDisposable SubscribeLeaveEvent (this Gtk.Widget w, System.Action leaveHandler)
		{
			return new LeaveEventData (w, leaveHandler);
		}
	}

	class LeaveEventData: IDisposable
	{
		public System.Action LeaveHandler;
		public Gtk.Widget RootWidget;
		public bool Inside;

		public LeaveEventData (Gtk.Widget w, System.Action leaveHandler)
		{
			RootWidget = w;
			LeaveHandler = leaveHandler;
			if (w.IsRealized) {
				RootWidget.Unrealized += HandleUnrealized;
				TrackLeaveEvent (w);
			}
			else
				w.Realized += HandleRealized;
		}

		void HandleRealized (object sender, EventArgs e)
		{
			RootWidget.Realized -= HandleRealized;
			RootWidget.Unrealized += HandleUnrealized;
			TrackLeaveEvent (RootWidget);
		}

		void HandleUnrealized (object sender, EventArgs e)
		{
			RootWidget.Unrealized -= HandleUnrealized;
			UntrackLeaveEvent (RootWidget);
			RootWidget.Realized += HandleRealized;
			if (Inside) {
				Inside = false;
				LeaveHandler ();
			}
		}

		public void Dispose ()
		{
			if (RootWidget.IsRealized) {
				UntrackLeaveEvent (RootWidget);
				RootWidget.Unrealized -= HandleUnrealized;
			} else {
				RootWidget.Realized -= HandleRealized;
			}
		}

		public void TrackLeaveEvent (Gtk.Widget w)
		{
			w.LeaveNotifyEvent += HandleLeaveNotifyEvent;
			w.EnterNotifyEvent += HandleEnterNotifyEvent;
			if (w is Gtk.Container) {
				((Gtk.Container)w).Added += HandleAdded;
				((Gtk.Container)w).Removed += HandleRemoved;
				foreach (var c in ((Gtk.Container)w).Children)
					TrackLeaveEvent (c);
			}
		}

		void UntrackLeaveEvent (Gtk.Widget w)
		{
			w.LeaveNotifyEvent -= HandleLeaveNotifyEvent;
			w.EnterNotifyEvent -= HandleEnterNotifyEvent;
			if (w is Gtk.Container) {
				((Gtk.Container)w).Added -= HandleAdded;
				((Gtk.Container)w).Removed -= HandleRemoved;
				foreach (var c in ((Gtk.Container)w).Children)
					UntrackLeaveEvent (c);
			}
		}

		void HandleRemoved (object o, RemovedArgs args)
		{
			UntrackLeaveEvent (args.Widget);
		}

		void HandleAdded (object o, AddedArgs args)
		{
			TrackLeaveEvent (args.Widget);
		}

		void HandleEnterNotifyEvent (object o, EnterNotifyEventArgs args)
		{
			Inside = true;
		}

		void HandleLeaveNotifyEvent (object o, LeaveNotifyEventArgs args)
		{
			Inside = false;

			// Delay the call to the leave handler since the pointer may be
			// entering a child widget, in which case the event doesn't have to be fired
			Gtk.Application.Invoke (delegate {
				if (!Inside)
					LeaveHandler ();
			});
		}
	}

	[StructLayout (LayoutKind.Sequential)] 
	struct NativeEventButtonStruct { 
		public Gdk.EventType type; 
		public IntPtr window; 
		public sbyte send_event; 
		public uint time; 
		public double x; 
		public double y; 
		public IntPtr axes; 
		public uint state; 
		public uint button; 
		public IntPtr device; 
		public double x_root; 
		public double y_root; 
	} 

	[StructLayout (LayoutKind.Sequential)] 
	struct NativeEventScrollStruct { 
		public Gdk.EventType type; 
		public IntPtr window; 
		public sbyte send_event; 
		public uint time; 
		public double x; 
		public double y; 
		public uint state; 
		public Gdk.ScrollDirection direction;
		public IntPtr device; 
		public double x_root; 
		public double y_root;
		//FIXME: scroll deltas
	} 
}
