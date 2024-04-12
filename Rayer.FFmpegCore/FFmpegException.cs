namespace Rayer.FFmpegCore;

public class FFmpegException : Exception
{
    /// <summary>
    /// Throws an <see cref="FFmpegException"/> if the <paramref name="errorCode"/> is less than zero.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="function">The name of the function that returned the <paramref name="errorCode"/>.</param>
    /// <exception cref="FFmpegException"><see cref="FFmpegException"/> with some details (including the <paramref name="errorCode"/> and the <paramref name="function"/>).</exception>
    public static void Try(int errorCode, string function)
    {
        if (errorCode < 0)
        {
            throw new FFmpegException(errorCode, function);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FFmpegException"/> class with an <paramref name="errorCode"/> that got returned by any FFmpeg <paramref name="function"/>.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="function">The name of the function that returned the <paramref name="errorCode"/>.</param>
    public FFmpegException(int errorCode, string function)
        : base(string.Format("{0} returned 0x{1:x8}: {2}", function, errorCode, FFmpegCalls.AvStrError(errorCode)))
    {
        ErrorCode = errorCode;
        Function = function;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FFmpegException"/> class with a <paramref name="message"/> describing an error that occurred by calling any FFmpeg <paramref name="function"/>.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="function">The name of the function that caused the error.</param>
    public FFmpegException(string message, string function)
        : base(string.Format("{0} failed: {1}", message, function))
    {
        Function = function;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FFmpegException"/> class with a message that describes the error.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FFmpegException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Gets the error code.
    /// </summary>
    public int ErrorCode { get; private set; }

    /// <summary>
    /// Gets the FFmpeg function which caused the error.
    /// </summary>
    public string Function { get; private set; } = string.Empty;
}