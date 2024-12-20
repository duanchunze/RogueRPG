﻿namespace Hsenl {
    public enum InputCode {
        None = 0,
        Backspace = 8,
        Tab = 9,
        Clear = 12, // 0x0000000C
        Return = 13, // 0x0000000D
        Pause = 19, // 0x00000013
        Escape = 27, // 0x0000001B
        Space = 32, // 0x00000020
        Exclaim = 33, // 0x00000021
        DoubleQuote = 34, // 0x00000022
        Hash = 35, // 0x00000023
        Dollar = 36, // 0x00000024
        Percent = 37, // 0x00000025
        Ampersand = 38, // 0x00000026
        Quote = 39, // 0x00000027
        LeftParen = 40, // 0x00000028
        RightParen = 41, // 0x00000029
        Asterisk = 42, // 0x0000002A
        Plus = 43, // 0x0000002B
        Comma = 44, // 0x0000002C
        Minus = 45, // 0x0000002D
        Period = 46, // 0x0000002E
        Slash = 47, // 0x0000002F
        Alpha0 = 48, // 0x00000030
        Alpha1 = 49, // 0x00000031
        Alpha2 = 50, // 0x00000032
        Alpha3 = 51, // 0x00000033
        Alpha4 = 52, // 0x00000034
        Alpha5 = 53, // 0x00000035
        Alpha6 = 54, // 0x00000036
        Alpha7 = 55, // 0x00000037
        Alpha8 = 56, // 0x00000038
        Alpha9 = 57, // 0x00000039
        Colon = 58, // 0x0000003A
        Semicolon = 59, // 0x0000003B
        Less = 60, // 0x0000003C
        Equals = 61, // 0x0000003D
        Greater = 62, // 0x0000003E
        Question = 63, // 0x0000003F
        At = 64, // 0x00000040
        LeftBracket = 91, // 0x0000005B
        Backslash = 92, // 0x0000005C
        RightBracket = 93, // 0x0000005D
        Caret = 94, // 0x0000005E
        Underscore = 95, // 0x0000005F
        BackQuote = 96, // 0x00000060
        A = 97, // 0x00000061
        B = 98, // 0x00000062
        C = 99, // 0x00000063
        D = 100, // 0x00000064
        E = 101, // 0x00000065
        F = 102, // 0x00000066
        G = 103, // 0x00000067
        H = 104, // 0x00000068
        I = 105, // 0x00000069
        J = 106, // 0x0000006A
        K = 107, // 0x0000006B
        L = 108, // 0x0000006C
        M = 109, // 0x0000006D
        N = 110, // 0x0000006E
        O = 111, // 0x0000006F
        P = 112, // 0x00000070
        Q = 113, // 0x00000071
        R = 114, // 0x00000072
        S = 115, // 0x00000073
        T = 116, // 0x00000074
        U = 117, // 0x00000075
        V = 118, // 0x00000076
        W = 119, // 0x00000077
        X = 120, // 0x00000078
        Y = 121, // 0x00000079
        Z = 122, // 0x0000007A
        LeftCurlyBracket = 123, // 0x0000007B
        Pipe = 124, // 0x0000007C
        RightCurlyBracket = 125, // 0x0000007D
        Tilde = 126, // 0x0000007E
        Delete = 127, // 0x0000007F
        Keypad0 = 256, // 0x00000100
        Keypad1 = 257, // 0x00000101
        Keypad2 = 258, // 0x00000102
        Keypad3 = 259, // 0x00000103
        Keypad4 = 260, // 0x00000104
        Keypad5 = 261, // 0x00000105
        Keypad6 = 262, // 0x00000106
        Keypad7 = 263, // 0x00000107
        Keypad8 = 264, // 0x00000108
        Keypad9 = 265, // 0x00000109
        KeypadPeriod = 266, // 0x0000010A
        KeypadDivide = 267, // 0x0000010B
        KeypadMultiply = 268, // 0x0000010C
        KeypadMinus = 269, // 0x0000010D
        KeypadPlus = 270, // 0x0000010E
        KeypadEnter = 271, // 0x0000010F
        KeypadEquals = 272, // 0x00000110
        UpArrow = 273, // 0x00000111
        DownArrow = 274, // 0x00000112
        RightArrow = 275, // 0x00000113
        LeftArrow = 276, // 0x00000114
        Insert = 277, // 0x00000115
        Home = 278, // 0x00000116
        End = 279, // 0x00000117
        PageUp = 280, // 0x00000118
        PageDown = 281, // 0x00000119
        F1 = 282, // 0x0000011A
        F2 = 283, // 0x0000011B
        F3 = 284, // 0x0000011C
        F4 = 285, // 0x0000011D
        F5 = 286, // 0x0000011E
        F6 = 287, // 0x0000011F
        F7 = 288, // 0x00000120
        F8 = 289, // 0x00000121
        F9 = 290, // 0x00000122
        F10 = 291, // 0x00000123
        F11 = 292, // 0x00000124
        F12 = 293, // 0x00000125
        F13 = 294, // 0x00000126
        F14 = 295, // 0x00000127
        F15 = 296, // 0x00000128
        Numlock = 300, // 0x0000012C
        CapsLock = 301, // 0x0000012D
        ScrollLock = 302, // 0x0000012E
        RightShift = 303, // 0x0000012F
        LeftShift = 304, // 0x00000130
        RightCtrl = 305, // 0x00000131
        LeftCtrl = 306, // 0x00000132
        RightAlt = 307, // 0x00000133
        LeftAlt = 308, // 0x00000134
        RightApple = 309, // 0x00000135
        RightCommand = 309, // 0x00000135
        RightMeta = 309, // 0x00000135
        LeftApple = 310, // 0x00000136
        LeftCommand = 310, // 0x00000136
        LeftMeta = 310, // 0x00000136
        LeftWindows = 311, // 0x00000137
        RightWindows = 312, // 0x00000138
        AltGr = 313, // 0x00000139
        Help = 315, // 0x0000013B
        Print = 316, // 0x0000013C
        SysReq = 317, // 0x0000013D
        Break = 318, // 0x0000013E
        Menu = 319, // 0x0000013F
        WASD = 400,
        MouseLeft = 401,
        MouseRight = 402,
        MouseMiddle = 403,
        MousePosition = 404,

        JoystickNone = 1000,
        LeftStick = 1001,
        RightStick = 1002,
        LeftBumper = 1003,
        RightBumper = 1004,
        LeftTrigger = 1005,
        RightTrigger = 1006,
        LeftStickPress = 1007,
        RightStickPress = 1008,
        ButtonSouth = 1009,
        ButtonEast = 1010,
        ButtonWest = 1011,
        ButtonNorth = 1012,
        Start = 1013,
        Select = 1014,
        Dpad_Up = 1015,
        Dpad_Down = 1016,
        Dpad_Left = 1017,
        Dpad_Right = 1018,
    }
}