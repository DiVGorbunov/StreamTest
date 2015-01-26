using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

public class FindHwnd : MonoBehaviour {

	internal class Window
	{
		public string Title;
		public IntPtr Handle;
		
		public override string ToString()
		{
			return Title;
		}
	}
	private List<Window> windows;

	delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

	private bool Callback(IntPtr hwnd, int lParam)
	{
        StringBuilder sb = new StringBuilder(200);
		User32.GetWindowText(hwnd, sb, sb.Capacity);

		if (sb.ToString()!="" && User32.IsWindowVisible(hwnd)!=0)
		{
			//StringBuilder sb = new StringBuilder(200);
			//User32.GetWindowText(hwnd, sb, sb.Capacity);
			Window t = new Window();
			t.Handle = hwnd;
			t.Title = sb.ToString();
			windows.Add(t);
		}
		
		return true; //continue enumeration
	}

	Bitmap bmp;
	System.Drawing.Graphics g;

	IntPtr hdcSrc;
	IntPtr hdcDest;
	int width,height;
	int windowDC;
	GameObject plane;
	//GameObject cube;
	//System.IO.MemoryStream ms;
	Texture2D tex;
	// Use this for initialization
	void Start () 
	{
		//IntPtr a = NativeMethods.Init ();
		//int b = a.ToInt32();
        IntPtr ptr = GameObject.Find ("Plane").renderer.material.mainTexture.GetNativeTexturePtr();
        NativeMethods.RetrieveDeviceFromTexPtr(ptr);

		//IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
		//g = System.Drawing.Graphics.FromHdc (hdcDest);
		windows = new List<Window>();
		
		User32.EnumWindows(Callback, 0);

		foreach (Window w in windows)
			print(w.Title);
		print (windows [windows.Count - 1].Handle);

		hdcSrc = User32.GetWindowDC(windows[0].Handle);
		User32.RECT windowRect = new User32.RECT();
		User32.GetWindowRect(windows[0].Handle,ref windowRect);
		width = windowRect.right - windowRect.left;
		height = windowRect.bottom - windowRect.top;
		
		//bmp = new Bitmap (width, height);
		//g = System.Drawing.Graphics.FromImage (bmp);
		//hdcDest = g.GetHdc();
		print (width+"x"+height);
		//ms = new System.IO.MemoryStream ();
		IntPtr ShaderResourceView = NativeMethods.getSRVbyHandle (windows [0].Handle);
		//tex = new Texture2D(bmp.Width, bmp.Height);
		tex = Texture2D.CreateExternalTexture (width, height, TextureFormat.RGBA32, false, false, ShaderResourceView);

		//bmp.Save("C:/Users/DiV/Desktop/text.bmp");
		plane = GameObject.Find ("Plane_1");
        plane.renderer.material.mainTexture = tex;
	}



	public UnityEngine.Color ToUeColor(System.Drawing.Color c)
	{
		return new UnityEngine.Color32(c.R, c.G , c.B , c.A );
	}

	// Update is called once per frame
	void Update () 
	{
		//g.CopyFromScreen(500, 200, 0, 0, new System.Drawing.Size(100, 100)); 
		/*
		GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);


		ms = new System.IO.MemoryStream ();
		bmp.Save (ms, ImageFormat.Png);
		ms.Seek(0, System.IO.SeekOrigin.Begin);
		
		tex = new Texture2D(bmp.Width, bmp.Height);
		
		tex.LoadImage(ms.ToArray());
		print(tex.GetPixel (0, 0).ToString());
		print(bmp.GetPixel (0, 0).ToString());
		//Close the stream.
		*/
        IntPtr ShaderResourceView = NativeMethods.getSRVbyHandle(windows[3].Handle);
        //tex = new Texture2D(bmp.Width, bmp.Height);
        tex = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, ShaderResourceView);
	}

	void OnDestroy()
	{
		//ms.Close(); 
		//ms = null;
		//bmp.Dispose ();
		//bmp = null;
		//g.ReleaseHdc ();
		//g.Dispose ();
		//g = null;
		NativeMethods.Release ();
	}

	private class GDI32
	{
		
		public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hObject,int nXDest,int nYDest,
		                                 int nWidth,int nHeight,IntPtr hObjectSource,
		                                 int nXSrc,int nYSrc,int dwRop);
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC,int nWidth, 
		                                                   int nHeight);
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
		[DllImport("gdi32.dll")]
		public static extern bool DeleteDC(IntPtr hDC);
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hDC,IntPtr hObject);
	}
	
	/// <summary>
	/// Helper class containing User32 API functions
	/// </summary>
	private class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd,IntPtr hDC);
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hWnd,ref RECT rect);

		//[DllImport("user32.dll")]
		//public static extern bool DwmGetDxSharedSurface(IntPtr hWnd, IntPtr);

		
		[DllImport("user32.dll")]
		public static extern int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        public static extern int EnumDesktopWindows(EnumWindowsCallback lpEnumFunc, int lParam);
		
		[DllImport("user32.dll")]
		public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		
		[DllImport("user32.dll")]
		public static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

	    [DllImport("user32.dll")]
	    public static extern int IsWindowVisible(IntPtr hwnd);

		
		public static readonly int GWL_STYLE = -16;
		
		public static readonly ulong WS_VISIBLE = 0x10000000L;
		public static readonly ulong WS_BORDER = 0x00800000L;
		public static readonly ulong TARGETWINDOW = WS_BORDER | WS_VISIBLE;
	}
	private class NativeMethods
	{
		[DllImport("WinDxRender")]
		public static extern IntPtr Init();
		[DllImport("WinDxRender")]
		public static extern IntPtr getSRVbyHandle(IntPtr hWnd);

		[DllImport("WinDxRender")]
		public static extern void Release();

	    [DllImport("WinDxRender")]
        public static extern void RetrieveDeviceFromTexPtr(IntPtr ptr);
	}
    /*
	[DllImport("WinDxRender")]
	public static extern void Init();

    [DllImport("WinDxRender")]
    public static extern void Release();

    [DllImport("WinDxRender")]
    public static extern IntPtr getSRVbyHandle(IntPtr hwnd);*/
}