using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace WpfApp
{
  [StructLayout(LayoutKind.Sequential)]
  public struct MARGINS
  {
    public int cxLeftWidth;
    public int cxRightWidth;
    public int cyTopHeight;
    public int cyBottomHeight;
  };

  public class GlassEffect
  {
    [DllImport("DwmApi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

    [DllImport("dwmapi.dll", PreserveSig = false)]
    static extern bool DwmIsCompositionEnabled();

    protected static Dictionary<Window, HwndSource> WindowDic = new Dictionary<Window, HwndSource>();

    #region Color DependencyProperty
    public static readonly DependencyProperty ColorProperty =
      DependencyProperty.RegisterAttached("Color", typeof(SolidColorBrush), typeof(GlassEffect), new FrameworkPropertyMetadata(OnColorChanged));

    public static void SetColor(DependencyObject element, SolidColorBrush value)
    {
      element.SetValue(ColorProperty, value);
    }
    public static SolidColorBrush GetColor(DependencyObject element)
    {
      return (SolidColorBrush)element.GetValue(ColorProperty);
    } 
    #endregion

    public static void OnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (!DwmIsCompositionEnabled()) return;
      var colorBrush = args.NewValue as SolidColorBrush;
      if (colorBrush == null) return;

      var wnd = (Window)obj;
      if (!wnd.IsLoaded) wnd.Loaded += delegate { SetGlassEffect(wnd, colorBrush.Color); };
      else SetGlassEffect(wnd, colorBrush.Color);
    }

    static void SetGlassEffect(Window wnd,Color color)
    {
      try
      {
        if (!WindowDic.ContainsKey(wnd))
        {
          var ptr = new WindowInteropHelper(wnd).Handle;
          var mainWindowSrc = HwndSource.FromHwnd(ptr);
          WindowDic.Add(wnd, mainWindowSrc);
        }
        if (WindowDic[wnd].CompositionTarget == null) return;

        wnd.Background = Brushes.Transparent;
        WindowDic[wnd].CompositionTarget.BackgroundColor = color;
        var margins = new MARGINS { cxLeftWidth = -1, cxRightWidth = -1, cyTopHeight = -1, cyBottomHeight = -1 };
        DwmExtendFrameIntoClientArea(WindowDic[wnd].Handle, ref margins);
      }
      catch (Exception) { }
    }

  }

}
