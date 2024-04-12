using Rayer.FFmpegCore.Interops;

namespace Rayer.FFmpegCore;

internal sealed class AvFrame(AvFormatContext formatContext) : IDisposable
{
    private unsafe AVFrame* _frame = FFmpegCalls.AvFrameAlloc();

    public unsafe int ReadNextFrame(out double seconds, ref byte[] buffer)
    {
        var stream = formatContext.SelectedStream.Stream;
        var packet = new AVPacket();

        FFmpegCalls.InitPacket(&packet);
        var offset = 0;
        do
        {
            try
            {
                packet.data = null;
                packet.size = 0;

                seconds = 0;

                if (!FFmpegCalls.AvReadFrame(formatContext, &packet))
                {
                    break;
                }

                if (packet.stream_index != formatContext.SelectedStream.Stream.index)
                {
                    continue;
                }

                seconds = packet.pts * stream.time_base.num / (double)stream.time_base.den;
                do
                {
                    int bytesConsumed;
                    try
                    {
                        var bufferLength = DecodePacket(ref buffer, offset, &packet, out bytesConsumed);
                        if (bufferLength == 0)
                        {
                            break;
                        }

                        offset += bufferLength;
                    }
                    catch (FFmpegException)
                    {
                        break;
                    }

                    packet.data += bytesConsumed;
                    packet.size -= bytesConsumed;
                } while (packet.size > 0);
            }
            finally
            {
                FFmpegCalls.FreePacket(&packet);
            }
        } while (offset <= 0);

        return offset;
    }

    private unsafe int DecodePacket(ref byte[] buffer, int offset, AVPacket* packet, out int bytesConsumed)
    {
        var stream = formatContext.SelectedStream;
        var decoderContext = stream.Stream.codec;
        var decodingSuccess = FFmpegCalls.AvCodecDecodeAudio4(decoderContext, _frame, packet, out bytesConsumed);
        bytesConsumed = Math.Min(bytesConsumed, packet->size);

        if (decodingSuccess)
        {
            var dataSize = FFmpegCalls.AvGetBytesPerSample((AVSampleFormat)_frame->format);
            var size = FFmpegCalls.AvSamplesGetBufferSize(_frame);
            if (buffer == null || buffer.Length < offset + size)
            {
                var bufferTemp = new byte[offset + size];
                if (buffer != null)
                {
                    Buffer.BlockCopy(buffer, 0, bufferTemp, 0, buffer.Length);
                }

                buffer = bufferTemp;
            }

            if (IsPlanar((AVSampleFormat)_frame->format))
            {
                for (var c = 0; c < _frame->channels; c++)
                {
                    for (var i = 0; i < _frame->nb_samples; i++)
                    {
                        if (dataSize == 1)
                        {
                            buffer[offset + (i * _frame->channels) + c] = _frame->extended_data[c][i];
                        }
                        else if (dataSize == 2)
                        {
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize)] =
                                _frame->extended_data[c][i * dataSize];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 1] =
                                _frame->extended_data[c][(i * dataSize) + 1];
                        }
                        else if (dataSize == 4)
                        {
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize)] =
                                _frame->extended_data[c][i * dataSize];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 1] =
                                _frame->extended_data[c][(i * dataSize) + 1];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 2] =
                                _frame->extended_data[c][(i * dataSize) + 2];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 3] =
                                _frame->extended_data[c][(i * dataSize) + 3];
                        }
                        else if (dataSize == 8)
                        {
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize)] =
                                _frame->extended_data[c][i * dataSize];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 1] =
                                _frame->extended_data[c][(i * dataSize) + 1];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 2] =
                                _frame->extended_data[c][(i * dataSize) + 2];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 3] =
                                _frame->extended_data[c][(i * dataSize) + 3];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 4] =
                                _frame->extended_data[c][(i * dataSize) + 4];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 5] =
                                _frame->extended_data[c][(i * dataSize) + 5];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 6] =
                                _frame->extended_data[c][(i * dataSize) + 6];
                            buffer[offset + (i * dataSize * _frame->channels) + (c * dataSize) + 7] =
                                _frame->extended_data[c][(i * dataSize) + 7];
                        }
                    }
                }

                size = dataSize * _frame->channels * _frame->nb_samples;
            }
            else
            {
                for (var i = 0; i < size; i++)
                {
                    buffer[i + offset] = _frame->extended_data[0][i];
                }
            }

            if (dataSize == 8)
            {
                size = ConvertDblToFloat(buffer, offset, size);
            }

            return size;
        }

        return 0;
    }

    private unsafe int ConvertDblToFloat(byte[] buffer, int offset, int count)
    {
        var fltBuffer = new byte[count / 2];
        fixed (void* pbuf = &buffer[offset])
        fixed (void* pout = &fltBuffer[0])
        {
            var pflt = (float*)pout;
            var pdbl = (double*)pbuf;
            for (var i = 0; i < count / 8; i++)
            {
                pflt[i] = (float)pdbl[i];
            }
        }
        Array.Clear(buffer, offset, count);
        Buffer.BlockCopy(fltBuffer, 0, buffer, offset, fltBuffer.Length);
        return fltBuffer.Length;
    }

    private static bool IsPlanar(AVSampleFormat sampleFormat)
    {
        return sampleFormat is AVSampleFormat.AV_SAMPLE_FMT_U8P or
               AVSampleFormat.AV_SAMPLE_FMT_S16P or
               AVSampleFormat.AV_SAMPLE_FMT_S32P or
               AVSampleFormat.AV_SAMPLE_FMT_FLTP or
               AVSampleFormat.AV_SAMPLE_FMT_DBLP;
    }

    public unsafe void Dispose()
    {
        GC.SuppressFinalize(this);
        if (_frame != null)
        {
            FFmpegCalls.AvFrameFree(_frame);
            _frame = null;
        }
    }

    ~AvFrame()
    {
        Dispose();
    }
}