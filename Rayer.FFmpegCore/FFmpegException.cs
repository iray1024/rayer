namespace Rayer.FFmpegCore;

public class FFmpegException : Exception
{
    public static void Try(int errorCode, string function)
    {
        if (errorCode < 0)
        {
            throw new FFmpegException(errorCode, function);
        }
    }

    public FFmpegException(int errorCode, string function)
        : base(string.Format("{0} ·µ»Ø 0x{1:x8}: {2}", function, errorCode, FFmpegCalls.AvStrError(errorCode)))
    {
        ErrorCode = errorCode;
        Function = function;
    }

    public FFmpegException(string message, string function)
        : base(string.Format("{0} Ê§°Ü: {1}", message, function))
    {
        Function = function;
    }

    public FFmpegException(string message)
        : base(message)
    {
    }

    public int ErrorCode { get; private set; }

    public string Function { get; private set; } = string.Empty;
}