namespace Hsenl.Network {
    public static class Constant {
        public const int MessageHeadSize = 8; // 整个消息头那块字节大小
        public const int MessageHeadFirstHalfSize = 4; // 存有消息体大小信息的那块字节大小
        public const int MessageHeadSecondHalfSize = 4; // 存有消息id信息的那块字节大小
    }
}