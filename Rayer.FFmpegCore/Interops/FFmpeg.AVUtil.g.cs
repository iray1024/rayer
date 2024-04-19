using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore.Interops;

#pragma warning disable CS0649

internal unsafe partial struct AVRational
{
    internal int @num;
    internal int @den;
}

internal unsafe partial struct AVOptionRanges
{
}

internal unsafe partial struct AVClass
{
    internal sbyte* @class_name;
    internal IntPtr @item_name;
    internal AVOption* @option;
    internal int @version;
    internal int @log_level_offset_offset;
    internal int @parent_log_context_offset;
    internal IntPtr @child_next;
    internal IntPtr @child_class_next;
    internal AVClassCategory @category;
    internal IntPtr @get_category;
    internal IntPtr @query_ranges;
}

internal unsafe partial struct AVOption
{
}

internal unsafe partial struct AVOptionRanges
{
}

internal unsafe partial struct AVBPrint
{
}

internal unsafe partial struct AVOptionRanges
{
}

internal unsafe partial struct AVBuffer
{
}

internal unsafe partial struct AVBufferRef
{
    internal AVBuffer* @buffer;
    internal sbyte* @data;
    internal int @size;
}

internal unsafe partial struct AVBufferPool
{
}

internal unsafe partial struct AVDictionaryEntry
{
    internal sbyte* @key;
    internal sbyte* @value;

    public string Key
    {
        get { return Marshal.PtrToStringAnsi((IntPtr) key); }
    }

    public string Value
    {
        get { return Marshal.PtrToStringAnsi((IntPtr) value); }
    }
}

internal unsafe partial struct AVDictionary
{
}

internal unsafe partial struct AVFrameSideData
{
    internal AVFrameSideDataType @type;
    internal sbyte* @data;
    internal int @size;
    internal AVDictionary* @metadata;
    internal AVBufferRef* @buf;
}

internal unsafe partial struct AVFrame
{
    internal sbyte* @data0; internal sbyte* @data1; internal sbyte* @data2; internal sbyte* @data3; internal sbyte* @data4; internal sbyte* @data5; internal sbyte* @data6; internal sbyte* @data7; 
    internal fixed int @linesize[8]; 
    internal byte** @extended_data;
    internal int @width;
    internal int @height;
    internal int @nb_samples;
    internal int @format;
    internal int @key_frame;
    internal AVPictureType @pict_type;
    internal AVRational @sample_aspect_ratio;
    internal long @pts;
    internal long @pkt_pts;
    internal long @pkt_dts;
    internal int @coded_picture_number;
    internal int @display_picture_number;
    internal int @quality;
    internal void* @opaque;
    internal fixed ulong @error[8]; 
    internal int @repeat_pict;
    internal int @interlaced_frame;
    internal int @top_field_first;
    internal int @palette_has_changed;
    internal long @reordered_opaque;
    internal int @sample_rate;
    internal ulong @channel_layout;
    internal AVBufferRef* @buf0; internal AVBufferRef* @buf1; internal AVBufferRef* @buf2; internal AVBufferRef* @buf3; internal AVBufferRef* @buf4; internal AVBufferRef* @buf5; internal AVBufferRef* @buf6; internal AVBufferRef* @buf7; 
    internal AVBufferRef** @extended_buf;
    internal int @nb_extended_buf;
    internal AVFrameSideData** @side_data;
    internal int @nb_side_data;
    internal int @flags;
    internal AVColorRange @color_range;
    internal AVColorPrimaries @color_primaries;
    internal AVColorTransferCharacteristic @color_trc;
    internal AVColorSpace @colorspace;
    internal AVChromaLocation @chroma_location;
    internal long @best_effort_timestamp;
    internal long @pkt_pos;
    internal long @pkt_duration;
    internal AVDictionary* @metadata;
    internal int @decode_error_flags;
    internal int @channels;
    internal int @pkt_size;
    internal sbyte* @qscale_table;
    internal int @qstride;
    internal int @qscale_type;
    internal AVBufferRef* @qp_table_buf;
    internal AVBufferRef* @hw_frames_ctx;
}

internal unsafe partial struct AVOptionRanges
{
}

internal unsafe partial struct AVDictionary
{
}

internal unsafe partial struct AVOption
{
    internal sbyte* @name;
    internal sbyte* @help;
    internal int @offset;
    internal AVOptionType @type;
    internal default_val @default_val;
    internal double @min;
    internal double @max;
    internal int @flags;
    internal sbyte* @unit;
}

internal unsafe partial struct default_val
{
    internal long @i64;
    internal double @dbl;
    internal sbyte* @str;
    internal AVRational @q;
}

internal unsafe partial struct AVOptionRange
{
    internal sbyte* @str;
    internal double @value_min;
    internal double @value_max;
    internal double @component_min;
    internal double @component_max;
    internal int @is_range;
}

internal unsafe partial struct AVOptionRanges
{
    internal AVOptionRange** @range;
    internal int @nb_ranges;
    internal int @nb_components;
}

#pragma warning restore CS0649

internal enum AVMediaType : int
{
    @AVMEDIA_TYPE_UNKNOWN = -1,
    @AVMEDIA_TYPE_VIDEO = 0,
    @AVMEDIA_TYPE_AUDIO = 1,
    @AVMEDIA_TYPE_DATA = 2,
    @AVMEDIA_TYPE_SUBTITLE = 3,
    @AVMEDIA_TYPE_ATTACHMENT = 4,
    @AVMEDIA_TYPE_NB = 5,
}

internal enum AVPictureType : int
{
    @AV_PICTURE_TYPE_NONE = 0,
    @AV_PICTURE_TYPE_I = 1,
    @AV_PICTURE_TYPE_P = 2,
    @AV_PICTURE_TYPE_B = 3,
    @AV_PICTURE_TYPE_S = 4,
    @AV_PICTURE_TYPE_SI = 5,
    @AV_PICTURE_TYPE_SP = 6,
    @AV_PICTURE_TYPE_BI = 7,
}

internal enum AVClassCategory : int
{
    @AV_CLASS_CATEGORY_NA = 0,
    @AV_CLASS_CATEGORY_INPUT = 1,
    @AV_CLASS_CATEGORY_OUTPUT = 2,
    @AV_CLASS_CATEGORY_MUXER = 3,
    @AV_CLASS_CATEGORY_DEMUXER = 4,
    @AV_CLASS_CATEGORY_ENCODER = 5,
    @AV_CLASS_CATEGORY_DECODER = 6,
    @AV_CLASS_CATEGORY_FILTER = 7,
    @AV_CLASS_CATEGORY_BITSTREAM_FILTER = 8,
    @AV_CLASS_CATEGORY_SWSCALER = 9,
    @AV_CLASS_CATEGORY_SWRESAMPLER = 10,
    @AV_CLASS_CATEGORY_DEVICE_VIDEO_OUTPUT = 40,
    @AV_CLASS_CATEGORY_DEVICE_VIDEO_INPUT = 41,
    @AV_CLASS_CATEGORY_DEVICE_AUDIO_OUTPUT = 42,
    @AV_CLASS_CATEGORY_DEVICE_AUDIO_INPUT = 43,
    @AV_CLASS_CATEGORY_DEVICE_OUTPUT = 44,
    @AV_CLASS_CATEGORY_DEVICE_INPUT = 45,
    @AV_CLASS_CATEGORY_NB = 46,
}

internal enum AVPixelFormat : int
{
    @AV_PIX_FMT_NONE = -1,
    @AV_PIX_FMT_YUV420P = 0,
    @AV_PIX_FMT_YUYV422 = 1,
    @AV_PIX_FMT_RGB24 = 2,
    @AV_PIX_FMT_BGR24 = 3,
    @AV_PIX_FMT_YUV422P = 4,
    @AV_PIX_FMT_YUV444P = 5,
    @AV_PIX_FMT_YUV410P = 6,
    @AV_PIX_FMT_YUV411P = 7,
    @AV_PIX_FMT_GRAY8 = 8,
    @AV_PIX_FMT_MONOWHITE = 9,
    @AV_PIX_FMT_MONOBLACK = 10,
    @AV_PIX_FMT_PAL8 = 11,
    @AV_PIX_FMT_YUVJ420P = 12,
    @AV_PIX_FMT_YUVJ422P = 13,
    @AV_PIX_FMT_YUVJ444P = 14,
    @AV_PIX_FMT_XVMC_MPEG2_MC = 15,
    @AV_PIX_FMT_XVMC_MPEG2_IDCT = 16,
    @AV_PIX_FMT_XVMC = 16,
    @AV_PIX_FMT_UYVY422 = 17,
    @AV_PIX_FMT_UYYVYY411 = 18,
    @AV_PIX_FMT_BGR8 = 19,
    @AV_PIX_FMT_BGR4 = 20,
    @AV_PIX_FMT_BGR4_BYTE = 21,
    @AV_PIX_FMT_RGB8 = 22,
    @AV_PIX_FMT_RGB4 = 23,
    @AV_PIX_FMT_RGB4_BYTE = 24,
    @AV_PIX_FMT_NV12 = 25,
    @AV_PIX_FMT_NV21 = 26,
    @AV_PIX_FMT_ARGB = 27,
    @AV_PIX_FMT_RGBA = 28,
    @AV_PIX_FMT_ABGR = 29,
    @AV_PIX_FMT_BGRA = 30,
    @AV_PIX_FMT_GRAY16BE = 31,
    @AV_PIX_FMT_GRAY16LE = 32,
    @AV_PIX_FMT_YUV440P = 33,
    @AV_PIX_FMT_YUVJ440P = 34,
    @AV_PIX_FMT_YUVA420P = 35,
    @AV_PIX_FMT_VDPAU_H264 = 36,
    @AV_PIX_FMT_VDPAU_MPEG1 = 37,
    @AV_PIX_FMT_VDPAU_MPEG2 = 38,
    @AV_PIX_FMT_VDPAU_WMV3 = 39,
    @AV_PIX_FMT_VDPAU_VC1 = 40,
    @AV_PIX_FMT_RGB48BE = 41,
    @AV_PIX_FMT_RGB48LE = 42,
    @AV_PIX_FMT_RGB565BE = 43,
    @AV_PIX_FMT_RGB565LE = 44,
    @AV_PIX_FMT_RGB555BE = 45,
    @AV_PIX_FMT_RGB555LE = 46,
    @AV_PIX_FMT_BGR565BE = 47,
    @AV_PIX_FMT_BGR565LE = 48,
    @AV_PIX_FMT_BGR555BE = 49,
    @AV_PIX_FMT_BGR555LE = 50,
    @AV_PIX_FMT_VAAPI_MOCO = 51,
    @AV_PIX_FMT_VAAPI_IDCT = 52,
    @AV_PIX_FMT_VAAPI_VLD = 53,
    @AV_PIX_FMT_VAAPI = 53,
    @AV_PIX_FMT_YUV420P16LE = 54,
    @AV_PIX_FMT_YUV420P16BE = 55,
    @AV_PIX_FMT_YUV422P16LE = 56,
    @AV_PIX_FMT_YUV422P16BE = 57,
    @AV_PIX_FMT_YUV444P16LE = 58,
    @AV_PIX_FMT_YUV444P16BE = 59,
    @AV_PIX_FMT_VDPAU_MPEG4 = 60,
    @AV_PIX_FMT_DXVA2_VLD = 61,
    @AV_PIX_FMT_RGB444LE = 62,
    @AV_PIX_FMT_RGB444BE = 63,
    @AV_PIX_FMT_BGR444LE = 64,
    @AV_PIX_FMT_BGR444BE = 65,
    @AV_PIX_FMT_YA8 = 66,
    @AV_PIX_FMT_Y400A = 66,
    @AV_PIX_FMT_GRAY8A = 66,
    @AV_PIX_FMT_BGR48BE = 67,
    @AV_PIX_FMT_BGR48LE = 68,
    @AV_PIX_FMT_YUV420P9BE = 69,
    @AV_PIX_FMT_YUV420P9LE = 70,
    @AV_PIX_FMT_YUV420P10BE = 71,
    @AV_PIX_FMT_YUV420P10LE = 72,
    @AV_PIX_FMT_YUV422P10BE = 73,
    @AV_PIX_FMT_YUV422P10LE = 74,
    @AV_PIX_FMT_YUV444P9BE = 75,
    @AV_PIX_FMT_YUV444P9LE = 76,
    @AV_PIX_FMT_YUV444P10BE = 77,
    @AV_PIX_FMT_YUV444P10LE = 78,
    @AV_PIX_FMT_YUV422P9BE = 79,
    @AV_PIX_FMT_YUV422P9LE = 80,
    @AV_PIX_FMT_VDA_VLD = 81,
    @AV_PIX_FMT_GBRP = 82,
    @AV_PIX_FMT_GBR24P = 82,
    @AV_PIX_FMT_GBRP9BE = 83,
    @AV_PIX_FMT_GBRP9LE = 84,
    @AV_PIX_FMT_GBRP10BE = 85,
    @AV_PIX_FMT_GBRP10LE = 86,
    @AV_PIX_FMT_GBRP16BE = 87,
    @AV_PIX_FMT_GBRP16LE = 88,
    @AV_PIX_FMT_YUVA422P = 89,
    @AV_PIX_FMT_YUVA444P = 90,
    @AV_PIX_FMT_YUVA420P9BE = 91,
    @AV_PIX_FMT_YUVA420P9LE = 92,
    @AV_PIX_FMT_YUVA422P9BE = 93,
    @AV_PIX_FMT_YUVA422P9LE = 94,
    @AV_PIX_FMT_YUVA444P9BE = 95,
    @AV_PIX_FMT_YUVA444P9LE = 96,
    @AV_PIX_FMT_YUVA420P10BE = 97,
    @AV_PIX_FMT_YUVA420P10LE = 98,
    @AV_PIX_FMT_YUVA422P10BE = 99,
    @AV_PIX_FMT_YUVA422P10LE = 100,
    @AV_PIX_FMT_YUVA444P10BE = 101,
    @AV_PIX_FMT_YUVA444P10LE = 102,
    @AV_PIX_FMT_YUVA420P16BE = 103,
    @AV_PIX_FMT_YUVA420P16LE = 104,
    @AV_PIX_FMT_YUVA422P16BE = 105,
    @AV_PIX_FMT_YUVA422P16LE = 106,
    @AV_PIX_FMT_YUVA444P16BE = 107,
    @AV_PIX_FMT_YUVA444P16LE = 108,
    @AV_PIX_FMT_VDPAU = 109,
    @AV_PIX_FMT_XYZ12LE = 110,
    @AV_PIX_FMT_XYZ12BE = 111,
    @AV_PIX_FMT_NV16 = 112,
    @AV_PIX_FMT_NV20LE = 113,
    @AV_PIX_FMT_NV20BE = 114,
    @AV_PIX_FMT_RGBA64BE = 115,
    @AV_PIX_FMT_RGBA64LE = 116,
    @AV_PIX_FMT_BGRA64BE = 117,
    @AV_PIX_FMT_BGRA64LE = 118,
    @AV_PIX_FMT_YVYU422 = 119,
    @AV_PIX_FMT_VDA = 120,
    @AV_PIX_FMT_YA16BE = 121,
    @AV_PIX_FMT_YA16LE = 122,
    @AV_PIX_FMT_GBRAP = 123,
    @AV_PIX_FMT_GBRAP16BE = 124,
    @AV_PIX_FMT_GBRAP16LE = 125,
    @AV_PIX_FMT_QSV = 126,
    @AV_PIX_FMT_MMAL = 127,
    @AV_PIX_FMT_D3D11VA_VLD = 128,
    @AV_PIX_FMT_CUDA = 129,
    @AV_PIX_FMT_0RGB = 295,
    @AV_PIX_FMT_RGB0 = 296,
    @AV_PIX_FMT_0BGR = 297,
    @AV_PIX_FMT_BGR0 = 298,
    @AV_PIX_FMT_YUV420P12BE = 299,
    @AV_PIX_FMT_YUV420P12LE = 300,
    @AV_PIX_FMT_YUV420P14BE = 301,
    @AV_PIX_FMT_YUV420P14LE = 302,
    @AV_PIX_FMT_YUV422P12BE = 303,
    @AV_PIX_FMT_YUV422P12LE = 304,
    @AV_PIX_FMT_YUV422P14BE = 305,
    @AV_PIX_FMT_YUV422P14LE = 306,
    @AV_PIX_FMT_YUV444P12BE = 307,
    @AV_PIX_FMT_YUV444P12LE = 308,
    @AV_PIX_FMT_YUV444P14BE = 309,
    @AV_PIX_FMT_YUV444P14LE = 310,
    @AV_PIX_FMT_GBRP12BE = 311,
    @AV_PIX_FMT_GBRP12LE = 312,
    @AV_PIX_FMT_GBRP14BE = 313,
    @AV_PIX_FMT_GBRP14LE = 314,
    @AV_PIX_FMT_YUVJ411P = 315,
    @AV_PIX_FMT_BAYER_BGGR8 = 316,
    @AV_PIX_FMT_BAYER_RGGB8 = 317,
    @AV_PIX_FMT_BAYER_GBRG8 = 318,
    @AV_PIX_FMT_BAYER_GRBG8 = 319,
    @AV_PIX_FMT_BAYER_BGGR16LE = 320,
    @AV_PIX_FMT_BAYER_BGGR16BE = 321,
    @AV_PIX_FMT_BAYER_RGGB16LE = 322,
    @AV_PIX_FMT_BAYER_RGGB16BE = 323,
    @AV_PIX_FMT_BAYER_GBRG16LE = 324,
    @AV_PIX_FMT_BAYER_GBRG16BE = 325,
    @AV_PIX_FMT_BAYER_GRBG16LE = 326,
    @AV_PIX_FMT_BAYER_GRBG16BE = 327,
    @AV_PIX_FMT_YUV440P10LE = 328,
    @AV_PIX_FMT_YUV440P10BE = 329,
    @AV_PIX_FMT_YUV440P12LE = 330,
    @AV_PIX_FMT_YUV440P12BE = 331,
    @AV_PIX_FMT_AYUV64LE = 332,
    @AV_PIX_FMT_AYUV64BE = 333,
    @AV_PIX_FMT_VIDEOTOOLBOX = 334,
    @AV_PIX_FMT_P010LE = 335,
    @AV_PIX_FMT_P010BE = 336,
    @AV_PIX_FMT_GBRAP12BE = 337,
    @AV_PIX_FMT_GBRAP12LE = 338,
    @AV_PIX_FMT_GBRAP10BE = 339,
    @AV_PIX_FMT_GBRAP10LE = 340,
    @AV_PIX_FMT_MEDIACODEC = 341,
    @AV_PIX_FMT_NB = 342,
}

internal enum AVColorPrimaries : int
{
    @AVCOL_PRI_RESERVED0 = 0,
    @AVCOL_PRI_BT709 = 1,
    @AVCOL_PRI_UNSPECIFIED = 2,
    @AVCOL_PRI_RESERVED = 3,
    @AVCOL_PRI_BT470M = 4,
    @AVCOL_PRI_BT470BG = 5,
    @AVCOL_PRI_SMPTE170M = 6,
    @AVCOL_PRI_SMPTE240M = 7,
    @AVCOL_PRI_FILM = 8,
    @AVCOL_PRI_BT2020 = 9,
    @AVCOL_PRI_SMPTEST428_1 = 10,
    @AVCOL_PRI_SMPTE431 = 11,
    @AVCOL_PRI_SMPTE432 = 12,
    @AVCOL_PRI_NB = 13,
}

internal enum AVColorTransferCharacteristic : int
{
    @AVCOL_TRC_RESERVED0 = 0,
    @AVCOL_TRC_BT709 = 1,
    @AVCOL_TRC_UNSPECIFIED = 2,
    @AVCOL_TRC_RESERVED = 3,
    @AVCOL_TRC_GAMMA22 = 4,
    @AVCOL_TRC_GAMMA28 = 5,
    @AVCOL_TRC_SMPTE170M = 6,
    @AVCOL_TRC_SMPTE240M = 7,
    @AVCOL_TRC_LINEAR = 8,
    @AVCOL_TRC_LOG = 9,
    @AVCOL_TRC_LOG_SQRT = 10,
    @AVCOL_TRC_IEC61966_2_4 = 11,
    @AVCOL_TRC_BT1361_ECG = 12,
    @AVCOL_TRC_IEC61966_2_1 = 13,
    @AVCOL_TRC_BT2020_10 = 14,
    @AVCOL_TRC_BT2020_12 = 15,
    @AVCOL_TRC_SMPTEST2084 = 16,
    @AVCOL_TRC_SMPTEST428_1 = 17,
    @AVCOL_TRC_ARIB_STD_B67 = 18,
    @AVCOL_TRC_NB = 19,
}

internal enum AVColorSpace : int
{
    @AVCOL_SPC_RGB = 0,
    @AVCOL_SPC_BT709 = 1,
    @AVCOL_SPC_UNSPECIFIED = 2,
    @AVCOL_SPC_RESERVED = 3,
    @AVCOL_SPC_FCC = 4,
    @AVCOL_SPC_BT470BG = 5,
    @AVCOL_SPC_SMPTE170M = 6,
    @AVCOL_SPC_SMPTE240M = 7,
    @AVCOL_SPC_YCOCG = 8,
    @AVCOL_SPC_BT2020_NCL = 9,
    @AVCOL_SPC_BT2020_CL = 10,
    @AVCOL_SPC_SMPTE2085 = 11,
    @AVCOL_SPC_NB = 12,
}

internal enum AVColorRange : int
{
    @AVCOL_RANGE_UNSPECIFIED = 0,
    @AVCOL_RANGE_MPEG = 1,
    @AVCOL_RANGE_JPEG = 2,
    @AVCOL_RANGE_NB = 3,
}

internal enum AVChromaLocation : int
{
    @AVCHROMA_LOC_UNSPECIFIED = 0,
    @AVCHROMA_LOC_LEFT = 1,
    @AVCHROMA_LOC_CENTER = 2,
    @AVCHROMA_LOC_TOPLEFT = 3,
    @AVCHROMA_LOC_TOP = 4,
    @AVCHROMA_LOC_BOTTOMLEFT = 5,
    @AVCHROMA_LOC_BOTTOM = 6,
    @AVCHROMA_LOC_NB = 7,
}

internal enum AVSampleFormat : int
{
    @AV_SAMPLE_FMT_NONE = -1,
    @AV_SAMPLE_FMT_U8 = 0,
    @AV_SAMPLE_FMT_S16 = 1,
    @AV_SAMPLE_FMT_S32 = 2,
    @AV_SAMPLE_FMT_FLT = 3,
    @AV_SAMPLE_FMT_DBL = 4,
    @AV_SAMPLE_FMT_U8P = 5,
    @AV_SAMPLE_FMT_S16P = 6,
    @AV_SAMPLE_FMT_S32P = 7,
    @AV_SAMPLE_FMT_FLTP = 8,
    @AV_SAMPLE_FMT_DBLP = 9,
    @AV_SAMPLE_FMT_S64 = 10,
    @AV_SAMPLE_FMT_S64P = 11,
    @AV_SAMPLE_FMT_NB = 12,
}

internal enum AVFrameSideDataType : int
{
    @AV_FRAME_DATA_PANSCAN = 0,
    @AV_FRAME_DATA_A53_CC = 1,
    @AV_FRAME_DATA_STEREO3D = 2,
    @AV_FRAME_DATA_MATRIXENCODING = 3,
    @AV_FRAME_DATA_DOWNMIX_INFO = 4,
    @AV_FRAME_DATA_REPLAYGAIN = 5,
    @AV_FRAME_DATA_DISPLAYMATRIX = 6,
    @AV_FRAME_DATA_AFD = 7,
    @AV_FRAME_DATA_MOTION_VECTORS = 8,
    @AV_FRAME_DATA_SKIP_SAMPLES = 9,
    @AV_FRAME_DATA_AUDIO_SERVICE_TYPE = 10,
    @AV_FRAME_DATA_MASTERING_DISPLAY_METADATA = 11,
    @AV_FRAME_DATA_GOP_TIMECODE = 12,
}

internal enum AVOptionType : int
{
    @AV_OPT_TYPE_FLAGS = 0,
    @AV_OPT_TYPE_INT = 1,
    @AV_OPT_TYPE_INT64 = 2,
    @AV_OPT_TYPE_DOUBLE = 3,
    @AV_OPT_TYPE_FLOAT = 4,
    @AV_OPT_TYPE_STRING = 5,
    @AV_OPT_TYPE_RATIONAL = 6,
    @AV_OPT_TYPE_BINARY = 7,
    @AV_OPT_TYPE_DICT = 8,
    @AV_OPT_TYPE_CONST = 128,
    @AV_OPT_TYPE_IMAGE_SIZE = 1397316165,
    @AV_OPT_TYPE_PIXEL_FMT = 1346784596,
    @AV_OPT_TYPE_SAMPLE_FMT = 1397116244,
    @AV_OPT_TYPE_VIDEO_RATE = 1448231252,
    @AV_OPT_TYPE_DURATION = 1146442272,
    @AV_OPT_TYPE_COLOR = 1129270354,
    @AV_OPT_TYPE_CHANNEL_LAYOUT = 1128811585,
    @AV_OPT_TYPE_BOOL = 1112493900,
}

internal unsafe static partial class FFmpeg
{
    internal const int __STDC_CONSTANT_MACROS = 1;
    internal const int AVCODEC_D3D11VA_H = 1;
    internal const int AVCODEC_DXVA2_H = 1;
    internal const int AVCODEC_QSV_H = 1;
    internal const int AVCODEC_VDA_H = 1;
    internal const int AVCODEC_VDPAU_H = 1;
    internal const int AVCODEC_VIDEOTOOLBOX_H = 1;
    internal const int AVCODEC_XVMC_H = 1;
    internal const int FF_LAMBDA_SHIFT = 7;
    internal const int FF_LAMBDA_SCALE = (1<<FF_LAMBDA_SHIFT);
    internal const int FF_QP2LAMBDA = 118;
    internal const int FF_LAMBDA_MAX = (256*128-1);
    internal const int FF_QUALITY_SCALE = FF_LAMBDA_SCALE;
    internal const ulong AV_NOPTS_VALUE = 0x8000000000000000;
    internal const int AV_TIME_BASE = 1000000;
    internal const int LIBAVUTIL_VERSION_MAJOR = 55;
    internal const int LIBAVUTIL_VERSION_MINOR = 34;
    internal const int LIBAVUTIL_VERSION_MICRO = 100;
    internal const bool FF_API_VDPAU = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_XVMC = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_OPT_TYPE_METADATA = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_DLOG = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_VAAPI = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_FRAME_QP = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_PLUS1_MINUS1 = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_ERROR_FRAME = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_CRC_BIG_TABLE = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const bool FF_API_PKT_PTS = (LIBAVUTIL_VERSION_MAJOR<56);
    internal const int AV_HAVE_BIGENDIAN = 0;
    internal const int AV_HAVE_FAST_UNALIGNED = 1;
    internal const int AV_ERROR_MAX_STRING_SIZE = 64;
    internal const double M_E = 2.7182818284590452354;
    internal const double M_LN2 = 0.69314718055994530942;
    internal const double M_LN10 = 2.30258509299404568402;
    internal const double M_LOG2_10 = 3.32192809488736234787;
    internal const double M_PHI = 1.61803398874989484820;
    internal const double M_PI = 3.14159265358979323846;
    internal const double M_PI_2 = 1.57079632679489661923;
    internal const double M_SQRT1_2 = 0.70710678118654752440;
    internal const double M_SQRT2 = 1.41421356237309504880;
    internal const int AV_LOG_QUIET = -8;
    internal const int AV_LOG_PANIC = 0;
    internal const int AV_LOG_FATAL = 8;
    internal const int AV_LOG_ERROR = 16;
    internal const int AV_LOG_WARNING = 24;
    internal const int AV_LOG_INFO = 32;
    internal const int AV_LOG_VERBOSE = 40;
    internal const int AV_LOG_DEBUG = 48;
    internal const int AV_LOG_TRACE = 56;
    internal const int AV_LOG_MAX_OFFSET = (AV_LOG_TRACE-AV_LOG_QUIET);
    internal const int AV_LOG_SKIP_REPEATED = 1;
    internal const int AV_LOG_PRINT_LEVEL = 2;
    internal const int AVPALETTE_SIZE = 1024;
    internal const int AVPALETTE_COUNT = 256;
    internal const int AV_CH_FRONT_LEFT = 0x00000001;
    internal const int AV_CH_FRONT_RIGHT = 0x00000002;
    internal const int AV_CH_FRONT_CENTER = 0x00000004;
    internal const int AV_CH_LOW_FREQUENCY = 0x00000008;
    internal const int AV_CH_BACK_LEFT = 0x00000010;
    internal const int AV_CH_BACK_RIGHT = 0x00000020;
    internal const int AV_CH_FRONT_LEFT_OF_CENTER = 0x00000040;
    internal const int AV_CH_FRONT_RIGHT_OF_CENTER = 0x00000080;
    internal const int AV_CH_BACK_CENTER = 0x00000100;
    internal const int AV_CH_SIDE_LEFT = 0x00000200;
    internal const int AV_CH_SIDE_RIGHT = 0x00000400;
    internal const int AV_CH_TOP_CENTER = 0x00000800;
    internal const int AV_CH_TOP_FRONT_LEFT = 0x00001000;
    internal const int AV_CH_TOP_FRONT_CENTER = 0x00002000;
    internal const int AV_CH_TOP_FRONT_RIGHT = 0x00004000;
    internal const int AV_CH_TOP_BACK_LEFT = 0x00008000;
    internal const int AV_CH_TOP_BACK_CENTER = 0x00010000;
    internal const int AV_CH_TOP_BACK_RIGHT = 0x00020000;
    internal const int AV_CH_STEREO_LEFT = 0x20000000;
    internal const int AV_CH_STEREO_RIGHT = 0x40000000;
    internal const ulong AV_CH_WIDE_LEFT = 0x0000000080000000UL;
    internal const ulong AV_CH_WIDE_RIGHT = 0x0000000100000000UL;
    internal const ulong AV_CH_SURROUND_DIRECT_LEFT = 0x0000000200000000UL;
    internal const ulong AV_CH_SURROUND_DIRECT_RIGHT = 0x0000000400000000UL;
    internal const ulong AV_CH_LOW_FREQUENCY_2 = 0x0000000800000000UL;
    internal const ulong AV_CH_LAYOUT_NATIVE = 0x8000000000000000UL;
    internal const int AV_CH_LAYOUT_MONO = (AV_CH_FRONT_CENTER);
    internal const int AV_CH_LAYOUT_STEREO = (AV_CH_FRONT_LEFT|AV_CH_FRONT_RIGHT);
    internal const int AV_CH_LAYOUT_2POINT1 = (AV_CH_LAYOUT_STEREO|AV_CH_LOW_FREQUENCY);
    internal const int AV_CH_LAYOUT_2_1 = (AV_CH_LAYOUT_STEREO|AV_CH_BACK_CENTER);
    internal const int AV_CH_LAYOUT_SURROUND = (AV_CH_LAYOUT_STEREO|AV_CH_FRONT_CENTER);
    internal const int AV_CH_LAYOUT_3POINT1 = (AV_CH_LAYOUT_SURROUND|AV_CH_LOW_FREQUENCY);
    internal const int AV_CH_LAYOUT_4POINT0 = (AV_CH_LAYOUT_SURROUND|AV_CH_BACK_CENTER);
    internal const int AV_CH_LAYOUT_4POINT1 = (AV_CH_LAYOUT_4POINT0|AV_CH_LOW_FREQUENCY);
    internal const int AV_CH_LAYOUT_2_2 = (AV_CH_LAYOUT_STEREO|AV_CH_SIDE_LEFT|AV_CH_SIDE_RIGHT);
    internal const int AV_CH_LAYOUT_QUAD = (AV_CH_LAYOUT_STEREO|AV_CH_BACK_LEFT|AV_CH_BACK_RIGHT);
    internal const int AV_CH_LAYOUT_5POINT0 = (AV_CH_LAYOUT_SURROUND|AV_CH_SIDE_LEFT|AV_CH_SIDE_RIGHT);
    internal const int AV_CH_LAYOUT_5POINT1 = (AV_CH_LAYOUT_5POINT0|AV_CH_LOW_FREQUENCY);
    internal const int AV_CH_LAYOUT_5POINT0_BACK = (AV_CH_LAYOUT_SURROUND|AV_CH_BACK_LEFT|AV_CH_BACK_RIGHT);
    internal const int AV_CH_LAYOUT_5POINT1_BACK = (AV_CH_LAYOUT_5POINT0_BACK|AV_CH_LOW_FREQUENCY);
    internal const int AV_CH_LAYOUT_6POINT0 = (AV_CH_LAYOUT_5POINT0|AV_CH_BACK_CENTER);
    internal const int AV_CH_LAYOUT_6POINT0_FRONT = (AV_CH_LAYOUT_2_2|AV_CH_FRONT_LEFT_OF_CENTER|AV_CH_FRONT_RIGHT_OF_CENTER);
    internal const int AV_CH_LAYOUT_HEXAGONAL = (AV_CH_LAYOUT_5POINT0_BACK|AV_CH_BACK_CENTER);
    internal const int AV_CH_LAYOUT_6POINT1 = (AV_CH_LAYOUT_5POINT1|AV_CH_BACK_CENTER);
    internal const int AV_CH_LAYOUT_6POINT1_BACK = (AV_CH_LAYOUT_5POINT1_BACK|AV_CH_BACK_CENTER);
    internal const int AV_CH_LAYOUT_6POINT1_FRONT = (AV_CH_LAYOUT_6POINT0_FRONT|AV_CH_LOW_FREQUENCY);
    internal const int AV_CH_LAYOUT_7POINT0 = (AV_CH_LAYOUT_5POINT0|AV_CH_BACK_LEFT|AV_CH_BACK_RIGHT);
    internal const int AV_CH_LAYOUT_7POINT0_FRONT = (AV_CH_LAYOUT_5POINT0|AV_CH_FRONT_LEFT_OF_CENTER|AV_CH_FRONT_RIGHT_OF_CENTER);
    internal const int AV_CH_LAYOUT_7POINT1 = (AV_CH_LAYOUT_5POINT1|AV_CH_BACK_LEFT|AV_CH_BACK_RIGHT);
    internal const int AV_CH_LAYOUT_7POINT1_WIDE = (AV_CH_LAYOUT_5POINT1|AV_CH_FRONT_LEFT_OF_CENTER|AV_CH_FRONT_RIGHT_OF_CENTER);
    internal const int AV_CH_LAYOUT_7POINT1_WIDE_BACK = (AV_CH_LAYOUT_5POINT1_BACK|AV_CH_FRONT_LEFT_OF_CENTER|AV_CH_FRONT_RIGHT_OF_CENTER);
    internal const int AV_CH_LAYOUT_OCTAGONAL = (AV_CH_LAYOUT_5POINT0|AV_CH_BACK_LEFT|AV_CH_BACK_CENTER|AV_CH_BACK_RIGHT);
    internal const ulong AV_CH_LAYOUT_HEXADECAGONAL = (AV_CH_LAYOUT_OCTAGONAL|AV_CH_WIDE_LEFT|AV_CH_WIDE_RIGHT|AV_CH_TOP_BACK_LEFT|AV_CH_TOP_BACK_RIGHT|AV_CH_TOP_BACK_CENTER|AV_CH_TOP_FRONT_CENTER|AV_CH_TOP_FRONT_LEFT|AV_CH_TOP_FRONT_RIGHT);
    internal const int AV_CH_LAYOUT_STEREO_DOWNMIX = (AV_CH_STEREO_LEFT|AV_CH_STEREO_RIGHT);
    internal const uint AV_CPU_FLAG_FORCE = 0x80000000;
    internal const int AV_CPU_FLAG_MMX = 0x0001;
    internal const int AV_CPU_FLAG_MMXEXT = 0x0002;
    internal const int AV_CPU_FLAG_MMX2 = 0x0002;
    internal const int AV_CPU_FLAG_3DNOW = 0x0004;
    internal const int AV_CPU_FLAG_SSE = 0x0008;
    internal const int AV_CPU_FLAG_SSE2 = 0x0010;
    internal const int AV_CPU_FLAG_SSE2SLOW = 0x40000000;
    internal const int AV_CPU_FLAG_3DNOWEXT = 0x0020;
    internal const int AV_CPU_FLAG_SSE3 = 0x0040;
    internal const int AV_CPU_FLAG_SSE3SLOW = 0x20000000;
    internal const int AV_CPU_FLAG_SSSE3 = 0x0080;
    internal const int AV_CPU_FLAG_ATOM = 0x10000000;
    internal const int AV_CPU_FLAG_SSE4 = 0x0100;
    internal const int AV_CPU_FLAG_SSE42 = 0x0200;
    internal const int AV_CPU_FLAG_AESNI = 0x80000;
    internal const int AV_CPU_FLAG_AVX = 0x4000;
    internal const int AV_CPU_FLAG_AVXSLOW = 0x8000000;
    internal const int AV_CPU_FLAG_XOP = 0x0400;
    internal const int AV_CPU_FLAG_FMA4 = 0x0800;
    internal const int AV_CPU_FLAG_CMOV = 0x1000;
    internal const int AV_CPU_FLAG_AVX2 = 0x8000;
    internal const int AV_CPU_FLAG_FMA3 = 0x10000;
    internal const int AV_CPU_FLAG_BMI1 = 0x20000;
    internal const int AV_CPU_FLAG_BMI2 = 0x40000;
    internal const int AV_CPU_FLAG_ALTIVEC = 0x0001;
    internal const int AV_CPU_FLAG_VSX = 0x0002;
    internal const int AV_CPU_FLAG_POWER8 = 0x0004;
    internal const int AV_CPU_FLAG_ARMV5TE = (1<<0);
    internal const int AV_CPU_FLAG_ARMV6 = (1<<1);
    internal const int AV_CPU_FLAG_ARMV6T2 = (1<<2);
    internal const int AV_CPU_FLAG_VFP = (1<<3);
    internal const int AV_CPU_FLAG_VFPV3 = (1<<4);
    internal const int AV_CPU_FLAG_NEON = (1<<5);
    internal const int AV_CPU_FLAG_ARMV8 = (1<<6);
    internal const int AV_CPU_FLAG_VFP_VM = (1<<7);
    internal const int AV_CPU_FLAG_SETEND = (1<<16);
    internal const int AV_BUFFER_FLAG_READONLY = (1<<0);
    internal const int AV_DICT_MATCH_CASE = 1;
    internal const int AV_DICT_IGNORE_SUFFIX = 2;
    internal const int AV_DICT_DONT_STRDUP_KEY = 4;
    internal const int AV_DICT_DONT_STRDUP_VAL = 8;
    internal const int AV_DICT_DONT_OVERWRITE = 16;
    internal const int AV_DICT_APPEND = 32;
    internal const int AV_DICT_MULTIKEY = 64;
    internal const int AV_NUM_DATA_POINTERS = 8;
    internal const int AV_FRAME_FLAG_CORRUPT = (1<<0);
    internal const int AV_FRAME_FLAG_DISCARD = (1<<2);
    internal const int FF_DECODE_ERROR_INVALID_BITSTREAM = 1;
    internal const int FF_DECODE_ERROR_MISSING_REFERENCE = 2;
    internal const int AV_OPT_FLAG_ENCODING_PARAM = 1;
    internal const int AV_OPT_FLAG_DECODING_PARAM = 2;
    internal const int AV_OPT_FLAG_METADATA = 4;
    internal const int AV_OPT_FLAG_AUDIO_PARAM = 8;
    internal const int AV_OPT_FLAG_VIDEO_PARAM = 16;
    internal const int AV_OPT_FLAG_SUBTITLE_PARAM = 32;
    internal const int AV_OPT_FLAG_EXPORT = 64;
    internal const int AV_OPT_FLAG_READONLY = 128;
    internal const int AV_OPT_FLAG_FILTERING_PARAM = (1<<16);
    internal const int AV_OPT_SEARCH_CHILDREN = (1<<0);
    internal const int AV_OPT_SEARCH_FAKE_OBJ = (1<<1);
    internal const int AV_OPT_ALLOW_NULL = (1<<2);
    internal const int AV_OPT_MULTI_COMPONENT_RANGE = (1<<12);
    internal const int AV_OPT_SERIALIZE_SKIP_DEFAULTS = 0x00000001;
    internal const int AV_OPT_SERIALIZE_OPT_FLAGS_EXACT = 0x00000002;
    internal const int AV_PIX_FMT_FLAG_BE = (1<<0);
    internal const int AV_PIX_FMT_FLAG_PAL = (1<<1);
    internal const int AV_PIX_FMT_FLAG_BITSTREAM = (1<<2);
    internal const int AV_PIX_FMT_FLAG_HWACCEL = (1<<3);
    internal const int AV_PIX_FMT_FLAG_PLANAR = (1<<4);
    internal const int AV_PIX_FMT_FLAG_RGB = (1<<5);
    internal const int AV_PIX_FMT_FLAG_PSEUDOPAL = (1<<6);
    internal const int AV_PIX_FMT_FLAG_ALPHA = (1<<7);
    internal const int FF_LOSS_RESOLUTION = 0x0001;
    internal const int FF_LOSS_DEPTH = 0x0002;
    internal const int FF_LOSS_COLORSPACE = 0x0004;
    internal const int FF_LOSS_ALPHA = 0x0008;
    internal const int FF_LOSS_COLORQUANT = 0x0010;
    internal const int FF_LOSS_CHROMA = 0x0020;

    private const string libavutil = "avutil-55";
    
    [DllImport(libavutil, EntryPoint = "av_strerror", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int av_strerror(int @errnum, IntPtr @errbuf, ulong @errbuf_size);
    
    [DllImport(libavutil, EntryPoint = "av_malloc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern void* av_malloc(ulong @size);
   
    [DllImport(libavutil, EntryPoint = "av_free", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern void av_free(void* @ptr);
        
    [DllImport(libavutil, EntryPoint = "av_log_get_level", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int av_log_get_level();
    
    [DllImport(libavutil, EntryPoint = "av_log_set_level", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern void av_log_set_level(int @level);
    
    [DllImport(libavutil, EntryPoint = "av_log_set_callback", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern void av_log_set_callback(IntPtr @callback);
    
    [DllImport(libavutil, EntryPoint = "av_log_default_callback", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern void av_log_default_callback(void* @avcl, int @level, [MarshalAs(UnmanagedType.LPStr)] string @fmt, sbyte* @vl);
       
    [DllImport(libavutil, EntryPoint = "av_log_format_line2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int av_log_format_line2(void* @ptr, int @level, [MarshalAs(UnmanagedType.LPStr)] string @fmt, sbyte* @vl, IntPtr @line, int @line_size, int* @print_prefix);
    
    [DllImport(libavutil, EntryPoint = "av_get_bytes_per_sample", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int av_get_bytes_per_sample(AVSampleFormat @sample_fmt);
        
    [DllImport(libavutil, EntryPoint = "av_samples_get_buffer_size", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int av_samples_get_buffer_size(int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    
    [DllImport(libavutil, EntryPoint = "av_frame_alloc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern AVFrame* av_frame_alloc();
    
    [DllImport(libavutil, EntryPoint = "av_frame_free", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern void av_frame_free(AVFrame** @frame);            
}