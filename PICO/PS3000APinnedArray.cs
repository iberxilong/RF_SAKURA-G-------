/**************************************************************************
 *
 *     ooooooooo.    o8o                       
 *     `888   `Y88.  `''                       
 *      888   .d88' oooo   .ooooo.   .ooooo.   
 *      888ooo88P'  `888  d88' `'Y8 d88' `88b  
 *      888          888  888       888   888  
 *      888          888  888   .o8 888   888  
 *     o888o        o888o `Y8bod8P' `Y8bod8P'  
 *
 *
 *    Copyright Pico Technology Ltd 1995-2015
 *
 * 
 *    For sales and general information visit
 *    https://www.picotech.com   https://www.picoauto.com
 *    
 *    For help and support visit
 *    https://www.picotech.com/tech-support
 * 
 *    If you have what it takes to join us visit
 *    http://pico.jobs/
 *
 *
 **************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace PS3000ACSConsole
{
    public class PinnedArray<T> : IDisposable
    {
        GCHandle _pinnedHandle;
        private bool _disposed;

        public PinnedArray(int arraySize) : this(new T[arraySize]) { }

        public PinnedArray(T[] array)
        {
            /*
             * GCHandle.Alloc函数的第二个形参，除了有GCHandleType.Normal 外，
             * 还有Pinned。但Normal不会固定其地址，只是保证内存不被GC回收。而
             * Pinned可以将地址固定住，Pinned后这将防止垃圾回收器移动内存地址。
             */
            _pinnedHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
        }

        ~PinnedArray()  //销毁类时，会调用析构函数
        {
            Dispose(false);
        }

        public T[] Target
        {
            
            get { return (T[])_pinnedHandle.Target; }
            
        }

        public static implicit operator T[](PinnedArray<T> a)
        {
            if (a == null)
                return null;

            return (T[])a._pinnedHandle.Target;
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                // Dispose of any IDisposable members
            }

            _pinnedHandle.Free();
        }
    }
}
