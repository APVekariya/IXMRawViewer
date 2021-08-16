using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IXMRawViewer
{
    public static class MyExtension
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        public static string ToSize(this Int64 value, SizeUnits unit)
        {
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
        }
    }
    public static class MyWpfExtensions
    {
       
        public static System.Windows.Forms.IWin32Window GetIWin32Window(this System.Windows.Media.Visual visual)
        {
            var source = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }

     


        public class BackAndForthList<T> : List<T>
        {
            private int _current = 0;

            public T Current
            {
                get { return this[_current]; }
            }

            public void MoveNext()
            {
                _current++;
                if (_current >= Count)

                {
                    _current = 0;
                }
            }

            public void MovePrevious()
            {
                _current--;

                if (_current < 0)
                {
                    _current = Count - 1;
                }
            }
        }
    }
}
