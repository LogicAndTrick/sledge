////////////////////////////////////////////////////////////////////////////////
// Gtk GLWidget Sharp - Gtk OpenGL Widget for CSharp using OpenTK
////////////////////////////////////////////////////////////////////////////////
/*
Usage:
	To render either override OnRenderFrame() or hook to the RenderFrame event.

	When GraphicsContext.ShareContexts == True (Default)
	To setup OpenGL state hook to the following events:
		GLWidget.GraphicsContextInitialized
		GLWidget.GraphicsContextShuttingDown

	When GraphicsContext.ShareContexts == False
	To setup OpenGL state hook to the following events:
		GLWidget.Initialized
		GLWidget.ShuttingDown 
*/
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Gtk;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES10;
using OpenTK.Platform;
using OpenTK.Platform.MacOS;

namespace Sledge.Gui.Gtk.Controls.Implementations
{
	[ToolboxItem(true)]
	public class GLWidget : EventBox, IDisposable
	{
		IGraphicsContext graphicsContext;
		static int graphicsContextCount;

		/// <summary>Use a single buffer versus a double buffer.</summary>
		[Browsable(true)]
		public bool SingleBuffer { get; set; }

		/// <summary>Color Buffer Bits-Per-Pixel</summary>
		public int ColorBPP { get; set; }
		
		/// <summary>Accumulation Buffer Bits-Per-Pixel</summary>
		public int AccumulatorBPP { get; set; }

		/// <summary>Depth Buffer Bits-Per-Pixel</summary>
		public int DepthBPP { get; set; }

		/// <summary>Stencil Buffer Bits-Per-Pixel</summary>
		public int StencilBPP { get; set; }

		/// <summary>Number of samples</summary>
		public int Samples { get; set; }

		/// <summary>Indicates if steropic renderering is enabled</summary>
		public bool Stereo { get; set; }

		IWindowInfo windowInfo;

		/// <summary>The major version of OpenGL to use.</summary>
		public int GlVersionMajor { get; set; }

		/// <summary>The minor version of OpenGL to use.</summary>
		public int GlVersionMinor { get; set; }

		public GraphicsContextFlags GraphicsContextFlags
		{
			get { return graphicsContextFlags; }
			set { graphicsContextFlags = value; }
		}
		GraphicsContextFlags graphicsContextFlags;

		/// <summary>Constructs a new GLWidget.</summary>
		public GLWidget() : this(GraphicsMode.Default) { }

		/// <summary>Constructs a new GLWidget using a given GraphicsMode</summary>
		public GLWidget(GraphicsMode graphicsMode) : this(graphicsMode, 1, 0, GraphicsContextFlags.Default) { }

		/// <summary>Constructs a new GLWidget</summary>
		public GLWidget(GraphicsMode graphicsMode, int glVersionMajor, int glVersionMinor, GraphicsContextFlags graphicsContextFlags)
		{
			this.DoubleBuffered = false;
			
			SingleBuffer = graphicsMode.Buffers == 1;
			ColorBPP = graphicsMode.ColorFormat.BitsPerPixel;
			AccumulatorBPP = graphicsMode.AccumulatorFormat.BitsPerPixel;
			DepthBPP = graphicsMode.Depth;
			StencilBPP = graphicsMode.Stencil;
			Samples = graphicsMode.Samples;
			Stereo = graphicsMode.Stereo;
			
			GlVersionMajor = glVersionMajor;
			GlVersionMinor = glVersionMinor;
			GraphicsContextFlags = graphicsContextFlags;
		}

		~GLWidget() { Dispose(false); }

		public override void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
			base.Dispose();
		}

		public virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				graphicsContext.MakeCurrent(windowInfo);
				OnShuttingDown();
				if (GraphicsContext.ShareContexts && (Interlocked.Decrement(ref graphicsContextCount) == 0))
				{
					OnGraphicsContextShuttingDown();
					sharedContextInitialized = false;
				}
				graphicsContext.Dispose();
			}
		}

		// Called when the first GraphicsContext is created in the case of GraphicsContext.ShareContexts == True;
		public static event EventHandler GraphicsContextInitialized;
		static void OnGraphicsContextInitialized() { if (GraphicsContextInitialized != null) GraphicsContextInitialized(null, EventArgs.Empty); }

		// Called when the first GraphicsContext is being destroyed in the case of GraphicsContext.ShareContexts == True;
		public static event EventHandler GraphicsContextShuttingDown;
		static void OnGraphicsContextShuttingDown() { if (GraphicsContextShuttingDown != null) GraphicsContextShuttingDown(null, EventArgs.Empty); }

		// Called when this GLWidget has a valid GraphicsContext
		public event EventHandler Initialized;
		protected virtual void OnInitialized() { if (Initialized != null) Initialized(this, EventArgs.Empty); }

		// Called when this GLWidget needs to render a frame
		public event EventHandler RenderFrame;
		protected virtual void OnRenderFrame() { if (RenderFrame != null) RenderFrame(this, EventArgs.Empty); }

		// Called when this GLWidget is being Disposed
		public event EventHandler ShuttingDown;
		protected virtual void OnShuttingDown() { if (ShuttingDown != null) ShuttingDown(this, EventArgs.Empty); }

		// Called when a widget is realized. (window handles and such are valid)
		// protected override void OnRealized() { base.OnRealized(); }

		static bool sharedContextInitialized = false;
		bool initialized = false;

		// Called when the widget needs to be (fully or partially) redrawn.
		protected override bool OnExposeEvent(Gdk.EventExpose eventExpose)
		{
			if (!initialized)
			{
				initialized = true;

				// If this looks uninitialized...  initialize.
				if( ColorBPP == 0 )
				{
					ColorBPP = 32;
					
					if( DepthBPP == 0 ) DepthBPP = 16;
				}
				
				ColorFormat colorBufferColorFormat = new ColorFormat(ColorBPP);
				
				ColorFormat accumulationColorFormat = new ColorFormat(AccumulatorBPP);
				
				int buffers = 2;
				if( SingleBuffer ) buffers--;
				
				GraphicsMode graphicsMode = new GraphicsMode(colorBufferColorFormat, DepthBPP, StencilBPP, Samples, accumulationColorFormat, buffers, Stereo);

				Toolkit.Init ();

				// IWindowInfo
				if (Configuration.RunningOnWindows)
				{
					IntPtr windowHandle = gdk_win32_drawable_get_handle(GdkWindow.Handle);
					windowInfo = OpenTK.Platform.Utilities.CreateWindowsWindowInfo(windowHandle);
				}
				else if (Configuration.RunningOnMacOS)
				{
                    throw new PlatformNotSupportedException("OpenGL on GTK/OSX is too hard :(");
                    // this.GdkWindow.Handle
                    // IntPtr windowHandle = gdk_x11_drawable_get_xid(GdkWindow.Handle);
                    // IntPtr windowHandle = gdk_quartz_window_get_nswindow(GdkWindow.Handle);

                    //var nsBox = Class.Get("NSBox");
				    //var box = Cocoa.SendIntPtr(Cocoa.SendIntPtr(nsBox, Selector.Alloc), Selector.Get("initWithFrame:"), new RectangleF(Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height));
				    // var frame = Cocoa.SendIntPtr(box, Selector.Get("frame"));
				    //var wrap = gtk_ns_view_new(box);
				    //var widget = new Widget(wrap);
                    //this.Add(widget);
                    // uhh? dunno.


                    //IntPtr num2 = Cocoa.SendIntPtr(Cocoa.SendIntPtr(CocoaContext.NSOpenGLContext, Selector.Alloc), Selector.Get("initWithFormat:shareContext:"), num1, shareContextRef);

                    //IntPtr viewHandle = gdk_quartz_window_get_nsview(GdkWindow.Handle);
					//windowInfo = OpenTK.Platform.Utilities.CreateMacOSWindowInfo(windowHandle, box);
                    // windowInfo = OpenTK.Platform.Utilities.CreateMacOSCarbonWindowInfo(windowHandle, false, true);
                    // graphicsContext = new AglContext(graphicsMode, windowInfo, GraphicsContext.CurrentContext, () => Allocation.X, () => Allocation.Y); 
				}
				else if (Configuration.RunningOnX11)
				{
					IntPtr display = gdk_x11_display_get_xdisplay(Display.Handle);
					int screen = Screen.Number;
					IntPtr windowHandle = gdk_x11_drawable_get_xid(GdkWindow.Handle);
					IntPtr rootWindow = gdk_x11_drawable_get_xid(RootWindow.Handle);

					IntPtr visualInfo;
					if (graphicsMode.Index.HasValue)
					{
						XVisualInfo info = new XVisualInfo();
						info.VisualID = graphicsMode.Index.Value;
						int dummy;
						visualInfo = XGetVisualInfo(display, XVisualInfoMask.ID, ref info, out dummy);
					}
					else
					{
						visualInfo = GetVisualInfo(display);
					}

					windowInfo = OpenTK.Platform.Utilities.CreateX11WindowInfo(display, screen, windowHandle, rootWindow, visualInfo);
					XFree(visualInfo);
				}
                else throw new PlatformNotSupportedException();

				// GraphicsContext
				if (graphicsContext == null) graphicsContext = new GraphicsContext(graphicsMode, windowInfo, GlVersionMajor, GlVersionMinor, graphicsContextFlags);

				graphicsContext.MakeCurrent(windowInfo);

				if (GraphicsContext.ShareContexts)
				{
					Interlocked.Increment(ref graphicsContextCount);

					if (!sharedContextInitialized)
					{
						sharedContextInitialized = true;
						((IGraphicsContextInternal)graphicsContext).LoadAll();
						OnGraphicsContextInitialized();
					}
				}
				else
				{
					((IGraphicsContextInternal)graphicsContext).LoadAll();
					OnGraphicsContextInitialized();
				}

				OnInitialized();
                graphicsContext.MakeCurrent(null);
			}
			else
			{
				//graphicsContext.MakeCurrent(windowInfo);
			}

			bool result = base.OnExposeEvent(eventExpose);
			//OnRenderFrame();
			//eventExpose.Window.Display.Sync(); // Add Sync call to fix resize rendering problem (Jay L. T. Cornwall) - How does this affect VSync?
			//graphicsContext.SwapBuffers();
            //graphicsContext.MakeCurrent(null);
			return result;
		}

		// Called on Resize
		protected override bool OnConfigureEvent(Gdk.EventConfigure evnt)
		{
			bool result = base.OnConfigureEvent(evnt);
			if (graphicsContext != null) graphicsContext.Update(windowInfo);
			return result;
		}

	    protected void MakeCurrent()
	    {
            if (!graphicsContext.IsCurrent) graphicsContext.MakeCurrent(windowInfo);
	    }

	    protected void UnmakeCurrent()
	    {
	        if (graphicsContext.IsCurrent) graphicsContext.MakeCurrent(null);
	    }

	    protected void SwapBuffers()
	    {
	        graphicsContext.SwapBuffers();
	    }

        protected bool IsInitialised { get { return graphicsContext != null; } }
	    protected bool IsDisposed { get { return graphicsContext != null && graphicsContext.IsDisposed; } }

        [SuppressUnmanagedCodeSecurity, DllImport("libgdk-win32-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr gdk_win32_drawable_get_handle(IntPtr d);

		public enum XVisualClass : int
		{
			StaticGray = 0,
			GrayScale = 1,
			StaticColor = 2,
			PseudoColor = 3,
			TrueColor = 4,
			DirectColor = 5,
		}

		[StructLayout(LayoutKind.Sequential)]
		struct XVisualInfo
		{
			public IntPtr Visual;
			public IntPtr VisualID;
			public int Screen;
			public int Depth;
			public XVisualClass Class;
			public long RedMask;
			public long GreenMask;
			public long blueMask;
			public int ColormapSize;
			public int BitsPerRgb;

			public override string ToString()
			{
				return String.Format("id ({0}), screen ({1}), depth ({2}), class ({3})",
					VisualID, Screen, Depth, Class);
			}
		}

		[Flags]
		internal enum XVisualInfoMask
		{
			No = 0x0,
			ID = 0x1,
			Screen = 0x2,
			Depth = 0x4,
			Class = 0x8,
			Red = 0x10,
			Green = 0x20,
			Blue = 0x40,
			ColormapSize = 0x80,
			BitsPerRGB = 0x100,
			All = 0x1FF,
		}

        [DllImport("libX11", EntryPoint = "XGetVisualInfo", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr XGetVisualInfoInternal(IntPtr display, IntPtr vinfo_mask, ref XVisualInfo template, out int nitems);
		static IntPtr XGetVisualInfo(IntPtr display, XVisualInfoMask vinfo_mask, ref XVisualInfo template, out int nitems)
		{
			return XGetVisualInfoInternal(display, (IntPtr)(int)vinfo_mask, ref template, out nitems);
		}

		const string linux_libx11_name = "libX11.so.6";

        [SuppressUnmanagedCodeSecurity, DllImport(linux_libx11_name, CallingConvention = CallingConvention.Cdecl)]
		static extern void XFree(IntPtr handle);

		const string linux_libgdk_x11_name = "libgdk-x11-2.0.so.0";

		/// <summary> Returns the X resource (window or pixmap) belonging to a GdkDrawable. </summary>
		/// <remarks> XID gdk_x11_drawable_get_xid(GdkDrawable *drawable); </remarks>
		/// <param name="gdkDisplay"> The GdkDrawable. </param>
		/// <returns> The ID of drawable's X resource. </returns>
        [SuppressUnmanagedCodeSecurity, DllImport(linux_libgdk_x11_name, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr gdk_x11_drawable_get_xid(IntPtr gdkDisplay);

		/// <summary> Returns the X display of a GdkDisplay. </summary>
		/// <remarks> Display* gdk_x11_display_get_xdisplay(GdkDisplay *display); </remarks>
		/// <param name="gdkDisplay"> The GdkDrawable. </param>
		/// <returns> The X Display of the GdkDisplay. </returns>
        [SuppressUnmanagedCodeSecurity, DllImport(linux_libgdk_x11_name, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr gdk_x11_display_get_xdisplay(IntPtr gdkDisplay);


        [SuppressUnmanagedCodeSecurity, DllImport("libgdk-quartz-2.0.dylib", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gdk_quartz_window_get_nswindow(IntPtr gdkDisplay);
        [SuppressUnmanagedCodeSecurity, DllImport("libgdk-quartz-2.0.dylib", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gdk_quartz_window_get_nsview(IntPtr gdkDisplay);
        [SuppressUnmanagedCodeSecurity, DllImport("libgtk-quartz-2.0", CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr gtk_ns_view_new(IntPtr nsview);

		IntPtr GetVisualInfo(IntPtr display)
		{
			try
			{
				int[] attributes = AttributeList.ToArray();
				return glXChooseVisual(display, Screen.Number, attributes);
			}
			catch (DllNotFoundException e)
			{
				throw new DllNotFoundException("OpenGL dll not found!", e);
			}
			catch (EntryPointNotFoundException enf)
			{
				throw new EntryPointNotFoundException("Glx entry point not found!", enf);
			}
		}

		const int GLX_NONE = 0;
		const int GLX_USE_GL = 1;
		const int GLX_BUFFER_SIZE = 2;
		const int GLX_LEVEL = 3;
		const int GLX_RGBA = 4;
		const int GLX_DOUBLEBUFFER = 5;
		const int GLX_STEREO = 6;
		const int GLX_AUX_BUFFERS = 7;
		const int GLX_RED_SIZE = 8;
		const int GLX_GREEN_SIZE = 9;
		const int GLX_BLUE_SIZE = 10;
		const int GLX_ALPHA_SIZE = 11;
		const int GLX_DEPTH_SIZE = 12;
		const int GLX_STENCIL_SIZE = 13;
		const int GLX_ACCUM_RED_SIZE = 14;
		const int GLX_ACCUM_GREEN_SIZE = 15;
		const int GLX_ACCUM_BLUE_SIZE = 16;
		const int GLX_ACCUM_ALPHA_SIZE = 17;

		List<int> AttributeList
		{
			get
			{
				List<int> attributeList = new List<int>(24);

				attributeList.Add(GLX_RGBA);

				if (!SingleBuffer) attributeList.Add(GLX_DOUBLEBUFFER);

				if (Stereo) attributeList.Add(GLX_STEREO);

				attributeList.Add(GLX_RED_SIZE);
				attributeList.Add(ColorBPP/4); // TODO support 16-bit

				attributeList.Add(GLX_GREEN_SIZE);
				attributeList.Add(ColorBPP/4); // TODO support 16-bit

				attributeList.Add(GLX_BLUE_SIZE);
				attributeList.Add(ColorBPP/4); // TODO support 16-bit

				attributeList.Add(GLX_ALPHA_SIZE);
				attributeList.Add(ColorBPP/4); // TODO support 16-bit

				attributeList.Add(GLX_DEPTH_SIZE);
				attributeList.Add(DepthBPP);

				attributeList.Add(GLX_STENCIL_SIZE);
				attributeList.Add(StencilBPP);

				//attributeList.Add(GLX_AUX_BUFFERS);
				//attributeList.Add(Buffers);

				attributeList.Add(GLX_ACCUM_RED_SIZE);
				attributeList.Add(AccumulatorBPP/4);// TODO support 16-bit

				attributeList.Add(GLX_ACCUM_GREEN_SIZE);
				attributeList.Add(AccumulatorBPP/4);// TODO support 16-bit

				attributeList.Add(GLX_ACCUM_BLUE_SIZE);
				attributeList.Add(AccumulatorBPP/4);// TODO support 16-bit

				attributeList.Add(GLX_ACCUM_ALPHA_SIZE);
				attributeList.Add(AccumulatorBPP/4);// TODO support 16-bit

				attributeList.Add(GLX_NONE);

				return attributeList;
			}
		}

		const string linux_libgl_name = "libGL.so.1";

        [SuppressUnmanagedCodeSecurity, DllImport(linux_libgl_name, CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr glXChooseVisual(IntPtr display, int screen, int[] attr);
	}
}
