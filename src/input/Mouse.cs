using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace VoxelSpace.Input {


    // mouse util with support for raw input and clipping
    // currently windows only
    public static class Mouse {

        static MouseWindow _mouseWindow;

        public static bool IsRawInputAvailable => _mouseWindow?.IsRawInputAvailable ?? false;

        public static void Initialize(Game game) {
            _mouseWindow = new MouseWindow(game.Window);
        }

        // note: does not change cursor position. only resets the internally stored mouse position
        // this can be used to calculate deltas over a period of events
        public static void SetRawPositionState(Vector2 p) => SetRawPositionState((int) p.X, (int) p.Y);
        public static void SetRawPositionState(int x = 0, int y = 0) {
            if (_mouseWindow != null) {
                lock (_mouseWindow) {
                    _mouseWindow.RawXValue = x;
                    _mouseWindow.RawYValue = y;
                }
            }
        }

        public static Vector2 GetRawPositionState() {
            if (_mouseWindow != null) {
                return new Vector2(_mouseWindow.RawXValue, _mouseWindow.RawYValue);
            }
            return Vector2.Zero;
        }

        public static void ClipCursor(Rectangle? rect) {
            _mouseWindow.SetClip(rect);
        }


        class MouseWindow : NativeWindow {

            const int WM_INPUT = 0x00FF;
            const int WM_ACTIVATEAPP = 0x001C;

            public bool IsRawInputAvailable { get; private set; }
            public bool HasFocus { get; private set; }

            Rectangle? _cursorClip;

            public MouseWindow(GameWindow window) {
                AssignHandle(window.Handle);
            }

            protected unsafe override void WndProc(ref Message m) {
                switch (m.Msg) {
                    case WM_INPUT:
                        HandleRawInput(ref m);
                        break;
                    case WM_ACTIVATEAPP:
                        HasFocus = m.WParam.ToPointer() != null;
                        UpdateClip();
                        break;
                }
                base.WndProc(ref m);
            }

            protected override void OnHandleChange() {                
                base.OnHandleChange();
                if (Handle != IntPtr.Zero)
                    RegisterMouseRawInput(Handle);
            }

            public override void ReleaseHandle() {
                UnRegisterMouseRawInput();
                base.ReleaseHandle();
            }

            public void SetClip(Rectangle? rect) {
                _cursorClip = rect;
                UpdateClip();
            }

            unsafe void UpdateClip() {
                if (HasFocus) {
                    if (_cursorClip is Rectangle r) {
                        ClipCursor(&r);
                    }
                    else {
                        ClipCursor(null);
                    }
                }
                else {
                    ClipCursor(null);
                }
            }

            [DllImport("user32.dll")]
            static extern unsafe void ClipCursor(Rectangle *rect);


            internal struct RAWINPUTDEVICE {
                internal ushort UsagePage;
                internal ushort Usage;
                internal int Flags;
                internal IntPtr hwndTarget;
            }

            [DllImport("user32.dll", SetLastError = true)]
            static extern bool RegisterRawInputDevices(ref RAWINPUTDEVICE pRawInputDevice, uint numberDevices, uint size);

            private const int RIDEV_REMOVE    = 0x00000001;
            private const int RIDEV_NOLEGACY  = 0x00000030;
            private const int RIDEV_INPUTSINK = 0x00000100;

            internal void RegisterMouseRawInput(IntPtr hwnd) {
                RAWINPUTDEVICE rid;
                rid.UsagePage = 0x01;
                rid.Usage = 0x02;
                rid.Flags = RIDEV_INPUTSINK;
                rid.hwndTarget = hwnd;
                var hr = RegisterRawInputDevices(ref rid, 1, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
                IsRawInputAvailable = hr;
            }

            internal void UnRegisterMouseRawInput() {
                RAWINPUTDEVICE rid;
                rid.UsagePage = 0x01;
                rid.Usage = 0x02;
                rid.Flags = RIDEV_REMOVE;
                rid.hwndTarget = IntPtr.Zero;
                var hr = RegisterRawInputDevices(ref rid, 1, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
                IsRawInputAvailable = false;
            }


            internal struct RAWMOUSE {
                public ushort Flags;
                public ushort ButtonFlags;
                public ushort ButtonData;

                public uint RawButtons;
                public int LastX;
                public int LastY;
                public uint ExtraInformation;
            }
            
            [StructLayout(LayoutKind.Explicit)]
            public struct RAWINPUTDATA {
                [FieldOffset(0)]
                public RAWMOUSE mouse;
                //[FieldOffset(0)]
                //internal Rawkeyboard keyboard;
                //[FieldOffset(0)]
                //internal Rawhid hid;
            }

            public struct RAWINPUTHEADER {
                public int Type;                     // Type of raw input (RIM_TYPEHID 2, RIM_TYPEKEYBOARD 1, RIM_TYPEMOUSE 0)
                public int Size;                     // Size in bytes of the entire input packet of data. This includes RAWINPUT plus possible extra input reports in the RAWHID variable length array. 
                public IntPtr hDevice;               // A handle to the device generating the raw input data. 
                public IntPtr wParam;                // RIM_INPUT 0 if input occurred while application was in the foreground else RIM_INPUTSINK 1 if it was not.
            }

            public struct RAWINPUT {
                public RAWINPUTHEADER header;           // 64 bit header size is 24  32 bit the header size is 16
                public RAWINPUTDATA data;               // Creating the rest in a struct allows the header size to align correctly for 32 or 64 bit
            }

            [DllImport("User32.dll")]
            internal static extern int GetRawInputData( IntPtr hRawInput, uint command, 
                                                        IntPtr pData, ref uint size, int sizeHeader);
            
            private const uint RID_INPUT  = 0x10000003;
            private const uint RID_HEADER = 0x10000005;

            private const uint RIM_TYPEMOUSE    = 0x00;
            private const uint RIM_TYPEKEYBOARD = 0x01;
            private const uint RIM_TYPEHID      = 0x02;            

            private const uint MOUSE_MOVE_RELATIVE      = 0x00;
            private const uint MOUSE_MOVE_ABSOLUTE      = 0x01;
            private const uint MOUSE_VIRTUAL_DESKTOP    = 0x02;
            private const uint MOUSE_ATTRIBUTES_CHANGED = 0x04;

            private const uint RI_MOUSE_WHEEL = 0x0400;

            public int RawXValue = 0;
            public int RawYValue = 0;

            private unsafe void HandleRawInput(ref Message message) {
                int hr;
                uint dataSize = 0;
                hr = GetRawInputData(message.LParam, RID_INPUT, IntPtr.Zero, ref dataSize, sizeof(RAWINPUTHEADER));

                // the RAWINPUT struct we pass in as &ri should be the same size as the available data size
                if (dataSize != sizeof(RAWINPUT))
                    return;

                RAWINPUT ri;
                hr = GetRawInputData(message.LParam, RID_INPUT, new IntPtr(&ri), ref dataSize, sizeof(RAWINPUTHEADER));

                if (ri.header.Type == RIM_TYPEMOUSE) {
                    RAWMOUSE* mi = &ri.data.mouse;

                    // if ((mi->Flags & MOUSE_MOVE_ABSOLUTE) != 0) {
                    // }
                    if ((mi->Flags & 0b111) == 0) {
                        lock (this) {
                            RawXValue += mi->LastX;
                            RawYValue += mi->LastY;
                        }
                    }
                }
            }

        }

    }

}
