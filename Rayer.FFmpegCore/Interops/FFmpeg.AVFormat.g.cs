using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore.Interops;

internal unsafe partial struct AVBuffer
{
}

internal unsafe partial struct AVBufferPool
{
}

internal unsafe partial struct AVBPrint
{
}

internal unsafe partial struct AVDictionary
{
    internal int count;
    internal AVDictionaryEntry* elems;

    public AVDictionaryEntry[] Elements => GetElements();

    private AVDictionaryEntry[] GetElements()
    {
        var cpy = elems;
        AVDictionaryEntry[] elements = new AVDictionaryEntry[count];

        for (int i = 0; i < count; i++)
        {
            elements[i] = cpy[i];
        }

        return elements;
    }
}

internal unsafe partial struct AVCodecInternal
{
}

internal unsafe partial struct AVCodecDefault
{
}

internal unsafe partial struct MpegEncContext
{
}

internal unsafe partial struct ReSampleContext
{
}

internal unsafe partial struct AVResampleContext
{
}

internal unsafe partial struct AVBSFInternal
{
}

internal unsafe partial struct AVBSFList
{
}

internal unsafe partial struct AVIOInterruptCB
{
    internal IntPtr @callback;
    internal void* @opaque;
}

internal unsafe partial struct AVIOContext
{
    internal AVClass* @av_class;
    internal sbyte* @buffer;
    internal int @buffer_size;
    internal sbyte* @buf_ptr;
    internal sbyte* @buf_end;
    internal void* @opaque;
    internal IntPtr @read_packet;
    internal IntPtr @write_packet;
    internal IntPtr @seek;
    internal long @pos;
    internal int @must_flush;
    internal int @eof_reached;
    internal int @write_flag;
    internal int @max_packet_size;
    internal int @checksum;
    internal sbyte* @checksum_ptr;
    internal IntPtr @update_checksum;
    internal int @error;
    internal IntPtr @read_pause;
    internal IntPtr @read_seek;
    internal int @seekable;
    internal long @maxsize;
    internal int @direct;
    internal long @bytes_read;
    internal int @seek_count;
    internal int @writeout_count;
    internal int @orig_buffer_size;
    internal int @short_seek_threshold;
    internal sbyte* @protocol_whitelist;
    internal sbyte* @protocol_blacklist;
    internal IntPtr @write_data_type;
    internal int @ignore_boundary_point;
    internal AVIODataMarkerType @current_type;
    internal long @last_time;
}

internal unsafe partial struct AVBPrint
{
}

internal unsafe partial struct AVFormatContext
{
}

internal unsafe partial struct AVFrac
{
    internal long @val;
    internal long @num;
    internal long @den;
}

internal unsafe partial struct AVCodecTag
{
}

internal unsafe partial struct AVProbeData
{
    internal sbyte* @filename;
    internal sbyte* @buf;
    internal int @buf_size;
    internal sbyte* @mime_type;
}

internal unsafe partial struct AVOutputFormat
{
    internal sbyte* @name;
    internal sbyte* @long_name;
    internal sbyte* @mime_type;
    internal sbyte* @extensions;
    internal AvCodecId @audio_codec;
    internal AvCodecId @video_codec;
    internal AvCodecId @subtitle_codec;
    internal int @flags;
    internal AVCodecTag** @codec_tag;
    internal AVClass* @priv_class;
    internal AVOutputFormat* @next;
    internal int @priv_data_size;
    internal IntPtr @write_header;
    internal IntPtr @write_packet;
    internal IntPtr @write_trailer;
    internal IntPtr @interleave_packet;
    internal IntPtr @query_codec;
    internal IntPtr @get_output_timestamp;
    internal IntPtr @control_message;
    internal IntPtr @write_uncoded_frame;
    internal IntPtr @get_device_list;
    internal IntPtr @create_device_capabilities;
    internal IntPtr @free_device_capabilities;
    internal AvCodecId @data_codec;
    internal IntPtr @init;
    internal IntPtr @deinit;
    internal IntPtr @check_bitstream;
}

internal unsafe partial struct AVInputFormat
{
    internal sbyte* @name;
    internal sbyte* @long_name;
    internal int @flags;
    internal sbyte* @extensions;
    internal AVCodecTag** @codec_tag;
    internal AVClass* @priv_class;
    internal sbyte* @mime_type;
    internal AVInputFormat* @next;
    internal int @raw_codec_id;
    internal int @priv_data_size;
    internal IntPtr @read_probe;
    internal IntPtr @read_header;
    internal IntPtr @read_packet;
    internal IntPtr @read_close;
    internal IntPtr @read_seek;
    internal IntPtr @read_timestamp;
    internal IntPtr @read_play;
    internal IntPtr @read_pause;
    internal IntPtr @read_seek2;
    internal IntPtr @get_device_list;
    internal IntPtr @create_device_capabilities;
    internal IntPtr @free_device_capabilities;
}

internal unsafe partial struct AVIndexEntry
{
    internal long @pos;
    internal long @timestamp;
    internal int @flags;
    internal int @size;
    internal int @min_distance;
}

internal unsafe partial struct AVStreamInternal
{
}

internal unsafe partial struct AVStream
{
    internal int @index;
    internal int @id;
    internal AVCodecContext* @codec;
    internal void* @priv_data;
    internal AVFrac @pts;
    internal AVRational @time_base;
    internal long @start_time;
    internal long @duration;
    internal long @nb_frames;
    internal int @disposition;
    internal AVDiscard @discard;
    internal AVRational @sample_aspect_ratio;
    internal AVDictionary* @metadata;
    internal AVRational @avg_frame_rate;
    internal AVPacket @attached_pic;
    internal AVPacketSideData* @side_data;
    internal int @nb_side_data;
    internal int @event_flags;
    internal Info* @info;
    internal int @pts_wrap_bits;
    internal long @first_dts;
    internal long @cur_dts;
    internal long @last_IP_pts;
    internal int @last_IP_duration;
    internal int @probe_packets;
    internal int @codec_info_nb_frames;
    internal AVStreamParseType @need_parsing;
    internal AVCodecParserContext* @parser;
    internal AVPacketList* @last_in_packet_buffer;
    internal AVProbeData @probe_data;
    internal fixed long @pts_buffer[17]; 
    internal AVIndexEntry* @index_entries;
    internal int @nb_index_entries;
    internal uint @index_entries_allocated_size;
    internal AVRational @r_frame_rate;
    internal int @stream_identifier;
    internal long @interleaver_chunk_size;
    internal long @interleaver_chunk_duration;
    internal int @request_probe;
    internal int @skip_to_keyframe;
    internal int @skip_samples;
    internal long @start_skip_samples;
    internal long @first_discard_sample;
    internal long @last_discard_sample;
    internal int @nb_decoded_frames;
    internal long @mux_ts_offset;
    internal long @pts_wrap_reference;
    internal int @pts_wrap_behavior;
    internal int @update_initial_durations_done;
    internal fixed long @pts_reorder_error[17]; 
    internal fixed sbyte @pts_reorder_error_count[17]; 
    internal long @last_dts_for_order_check;
    internal sbyte @dts_ordered;
    internal sbyte @dts_misordered;
    internal int @inject_global_side_data;
    internal sbyte* @recommended_encoder_configuration;
    internal AVRational @display_aspect_ratio;
    internal FFFrac* @priv_pts;
    internal AVStreamInternal* @internal;
    internal AVCodecParameters* @codecpar;
}

internal unsafe partial struct FFFrac
{
}

internal unsafe partial struct AVPacketList
{
}

internal unsafe partial struct Info
{
    internal long @last_dts;
    internal long @duration_gcd;
    internal int @duration_count;
    internal long @rfps_duration_sum;
    internal IntPtr @duration_error;
    internal long @codec_info_duration;
    internal long @codec_info_duration_fields;
    internal int @found_decoder;
    internal long @last_duration;
    internal long @fps_first_dts;
    internal int @fps_first_dts_idx;
    internal long @fps_last_dts;
    internal int @fps_last_dts_idx;
}

internal unsafe partial struct AVProgram
{
    internal int @id;
    internal int @flags;
    internal AVDiscard @discard;
    internal uint* @stream_index;
    internal uint @nb_stream_indexes;
    internal AVDictionary* @metadata;
    internal int @program_num;
    internal int @pmt_pid;
    internal int @pcr_pid;
    internal long @start_time;
    internal long @end_time;
    internal long @pts_wrap_reference;
    internal int @pts_wrap_behavior;
}

internal unsafe partial struct AVChapter
{
    internal int @id;
    internal AVRational @time_base;
    internal long @start;
    internal long @end;
    internal AVDictionary* @metadata;
}

internal unsafe partial struct AVFormatInternal
{
}

internal unsafe partial struct AVFormatContext
{
    internal AVClass* @av_class;
    internal AVInputFormat* @iformat;
    internal AVOutputFormat* @oformat;
    internal void* @priv_data;
    internal AVIOContext* @pb;
    internal int @ctx_flags;
    internal uint @nb_streams;
    internal AVStream** @streams;
    internal fixed sbyte @filename[1024]; 
    internal long @start_time;
    internal long @duration;
    internal long @bit_rate;
    internal uint @packet_size;
    internal int @max_delay;
    internal int @flags;
    internal long @probesize;
    internal long @max_analyze_duration;
    internal sbyte* @key;
    internal int @keylen;
    internal uint @nb_programs;
    internal AVProgram** @programs;
    internal AvCodecId @video_codec_id;
    internal AvCodecId @audio_codec_id;
    internal AvCodecId @subtitle_codec_id;
    internal uint @max_index_size;
    internal uint @max_picture_buffer;
    internal uint @nb_chapters;
    internal AVChapter** @chapters;
    internal AVDictionary* @metadata;
    internal long @start_time_realtime;
    internal int @fps_probe_size;
    internal int @error_recognition;
    internal AVIOInterruptCB @interrupt_callback;
    internal int @debug;
    internal long @max_interleave_delta;
    internal int @strict_std_compliance;
    internal int @event_flags;
    internal int @max_ts_probe;
    internal int @avoid_negative_ts;
    internal int @ts_id;
    internal int @audio_preload;
    internal int @max_chunk_duration;
    internal int @max_chunk_size;
    internal int @use_wallclock_as_timestamps;
    internal int @avio_flags;
    internal AVDurationEstimationMethod @duration_estimation_method;
    internal long @skip_initial_bytes;
    internal uint @correct_ts_overflow;
    internal int @seek2any;
    internal int @flush_packets;
    internal int @probe_score;
    internal int @format_probesize;
    internal sbyte* @codec_whitelist;
    internal sbyte* @format_whitelist;
    internal AVFormatInternal* @internal;
    internal int @io_repositioned;
    internal AVCodec* @video_codec;
    internal AVCodec* @audio_codec;
    internal AVCodec* @subtitle_codec;
    internal AVCodec* @data_codec;
    internal int @metadata_header_padding;
    internal void* @opaque;
    internal IntPtr @control_message_cb;
    internal long @output_ts_offset;
    internal sbyte* @dump_separator;
    internal AvCodecId @data_codec_id;
    internal IntPtr @open_cb;
    internal sbyte* @protocol_whitelist;
    internal IntPtr @io_open;
    internal IntPtr @io_close;
    internal sbyte* @protocol_blacklist;
}

internal unsafe partial struct AVPacketList
{
    internal AVPacket @pkt;
    internal AVPacketList* @next;
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate int av_format_control_message(AVFormatContext* @s, int @type, void* @data, ulong @data_size);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate int AVOpenCallback(AVFormatContext* @s, AVIOContext** @pb, [MarshalAs(UnmanagedType.LPStr)] string @url, int @flags, AVIOInterruptCB* @int_cb, AVDictionary** @options);

internal enum AVIODataMarkerType : int
{
    @AVIO_DATA_MARKER_HEADER = 0,
    @AVIO_DATA_MARKER_SYNC_POINT = 1,
    @AVIO_DATA_MARKER_BOUNDARY_POINT = 2,
    @AVIO_DATA_MARKER_UNKNOWN = 3,
    @AVIO_DATA_MARKER_TRAILER = 4,
}

internal enum AVStreamParseType : int
{
    @AVSTREAM_PARSE_NONE = 0,
    @AVSTREAM_PARSE_FULL = 1,
    @AVSTREAM_PARSE_HEADERS = 2,
    @AVSTREAM_PARSE_TIMESTAMPS = 3,
    @AVSTREAM_PARSE_FULL_ONCE = 4,
    @AVSTREAM_PARSE_FULL_RAW = 1463898624,
}

internal enum AVDurationEstimationMethod : int
{
    @AVFMT_DURATION_FROM_PTS = 0,
    @AVFMT_DURATION_FROM_STREAM = 1,
    @AVFMT_DURATION_FROM_BITRATE = 2,
}

internal unsafe static partial class FFmpeg
{
    internal const int LIBAVFORMAT_VERSION_MAJOR = 57;
    internal const int LIBAVFORMAT_VERSION_MINOR = 56;
    internal const int LIBAVFORMAT_VERSION_MICRO = 100;
    internal const bool FF_API_LAVF_BITEXACT = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_LAVF_FRAC = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_LAVF_CODEC_TB = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_URL_FEOF = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_LAVF_FMT_RAWPICTURE = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_COMPUTE_PKT_FIELDS2 = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_OLD_OPEN_CALLBACKS = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_LAVF_AVCTX = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_NOCONST_GET_SIDE_DATA = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const bool FF_API_HTTP_USER_AGENT = (LIBAVFORMAT_VERSION_MAJOR<58);
    internal const int FF_API_R_FRAME_RATE = 1;
    internal const int AVIO_SEEKABLE_NORMAL = 0x0001;
    internal const int AVSEEK_SIZE = 0x10000;
    internal const int AVSEEK_FORCE = 0x20000;
    internal const int AVIO_FLAG_READ = 1;
    internal const int AVIO_FLAG_WRITE = 2;
    internal const int AVIO_FLAG_READ_WRITE = (AVIO_FLAG_READ|AVIO_FLAG_WRITE);
    internal const int AVIO_FLAG_NONBLOCK = 8;
    internal const int AVIO_FLAG_DIRECT = 0x8000;
    internal const int AVPROBE_SCORE_RETRY = (AVPROBE_SCORE_MAX/4);
    internal const int AVPROBE_SCORE_STREAM_RETRY = (AVPROBE_SCORE_MAX/4-1);
    internal const int AVPROBE_SCORE_EXTENSION = 50;
    internal const int AVPROBE_SCORE_MIME = 75;
    internal const int AVPROBE_SCORE_MAX = 100;
    internal const int AVPROBE_PADDING_SIZE = 32;
    internal const int AVFMT_NOFILE = 0x0001;
    internal const int AVFMT_NEEDNUMBER = 0x0002;
    internal const int AVFMT_SHOW_IDS = 0x0008;
    internal const int AVFMT_RAWPICTURE = 0x0020;
    internal const int AVFMT_GLOBALHEADER = 0x0040;
    internal const int AVFMT_NOTIMESTAMPS = 0x0080;
    internal const int AVFMT_GENERIC_INDEX = 0x0100;
    internal const int AVFMT_TS_DISCONT = 0x0200;
    internal const int AVFMT_VARIABLE_FPS = 0x0400;
    internal const int AVFMT_NODIMENSIONS = 0x0800;
    internal const int AVFMT_NOSTREAMS = 0x1000;
    internal const int AVFMT_NOBINSEARCH = 0x2000;
    internal const int AVFMT_NOGENSEARCH = 0x4000;
    internal const int AVFMT_NO_BYTE_SEEK = 0x8000;
    internal const int AVFMT_ALLOW_FLUSH = 0x10000;
    internal const int AVFMT_TS_NONSTRICT = 0x20000;
    internal const int AVFMT_TS_NEGATIVE = 0x40000;
    internal const int AVFMT_SEEK_TO_PTS = 0x4000000;
    internal const int AVINDEX_KEYFRAME = 0x0001;
    internal const int AVINDEX_DISCARD_FRAME = 0x0002;
    internal const int AV_DISPOSITION_DEFAULT = 0x0001;
    internal const int AV_DISPOSITION_DUB = 0x0002;
    internal const int AV_DISPOSITION_ORIGINAL = 0x0004;
    internal const int AV_DISPOSITION_COMMENT = 0x0008;
    internal const int AV_DISPOSITION_LYRICS = 0x0010;
    internal const int AV_DISPOSITION_KARAOKE = 0x0020;
    internal const int AV_DISPOSITION_FORCED = 0x0040;
    internal const int AV_DISPOSITION_HEARING_IMPAIRED = 0x0080;
    internal const int AV_DISPOSITION_VISUAL_IMPAIRED = 0x0100;
    internal const int AV_DISPOSITION_CLEAN_EFFECTS = 0x0200;
    internal const int AV_DISPOSITION_ATTACHED_PIC = 0x0400;
    internal const int AV_DISPOSITION_TIMED_THUMBNAILS = 0x0800;
    internal const int AV_DISPOSITION_CAPTIONS = 0x10000;
    internal const int AV_DISPOSITION_DESCRIPTIONS = 0x20000;
    internal const int AV_DISPOSITION_METADATA = 0x40000;
    internal const int AV_PTS_WRAP_IGNORE = 0;
    internal const int AV_PTS_WRAP_ADD_OFFSET = 1;
    internal const int AV_PTS_WRAP_SUB_OFFSET = -1;
    internal const int AVSTREAM_EVENT_FLAG_METADATA_UPDATED = 0x0001;
    internal const int MAX_STD_TIMEBASES = (30*12+30+3+6);
    internal const int MAX_REORDER_DELAY = 16;
    internal const int AV_PROGRAM_RUNNING = 1;
    internal const int AVFMTCTX_NOHEADER = 0x0001;
    internal const int AVFMT_FLAG_GENPTS = 0x0001;
    internal const int AVFMT_FLAG_IGNIDX = 0x0002;
    internal const int AVFMT_FLAG_NONBLOCK = 0x0004;
    internal const int AVFMT_FLAG_IGNDTS = 0x0008;
    internal const int AVFMT_FLAG_NOFILLIN = 0x0010;
    internal const int AVFMT_FLAG_NOPARSE = 0x0020;
    internal const int AVFMT_FLAG_NOBUFFER = 0x0040;
    internal const int AVFMT_FLAG_CUSTOM_IO = 0x0080;
    internal const int AVFMT_FLAG_DISCARD_CORRUPT = 0x0100;
    internal const int AVFMT_FLAG_FLUSH_PACKETS = 0x0200;
    internal const int AVFMT_FLAG_BITEXACT = 0x0400;
    internal const int AVFMT_FLAG_MP4A_LATM = 0x8000;
    internal const int AVFMT_FLAG_SORT_DTS = 0x10000;
    internal const int AVFMT_FLAG_PRIV_OPT = 0x20000;
    internal const int AVFMT_FLAG_KEEP_SIDE_DATA = 0x40000;
    internal const int AVFMT_FLAG_FAST_SEEK = 0x80000;
    internal const int AVFMT_FLAG_SHORTEST = 0x100000;
    internal const int AVFMT_FLAG_AUTO_BSF = 0x200000;
    internal const int FF_FDEBUG_TS = 0x0001;
    internal const int AVFMT_EVENT_FLAG_METADATA_UPDATED = 0x0001;
    internal const int AVFMT_AVOID_NEG_TS_AUTO = -1;
    internal const int AVFMT_AVOID_NEG_TS_MAKE_NON_NEGATIVE = 1;
    internal const int AVFMT_AVOID_NEG_TS_MAKE_ZERO = 2;
    internal const int AVSEEK_FLAG_BACKWARD = 1;
    internal const int AVSEEK_FLAG_BYTE = 2;
    internal const int AVSEEK_FLAG_ANY = 4;
    internal const int AVSEEK_FLAG_FRAME = 8;
    internal const int AVSTREAM_INIT_IN_WRITE_HEADER = 0;
    internal const int AVSTREAM_INIT_IN_INIT_OUTPUT = 1;
    internal const int AV_FRAME_FILENAME_FLAGS_MULTIPLE = 1;

    private const string libavformat = "avformat-57";
    
    [DllImport(libavformat, EntryPoint = "av_register_all", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern void av_register_all();

    [DllImport(libavformat, EntryPoint = "avformat_network_init", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int avformat_network_init();
    
    [DllImport(libavformat, EntryPoint = "av_iformat_next", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern AVInputFormat* av_iformat_next(AVInputFormat* @f);
    
    [DllImport(libavformat, EntryPoint = "av_oformat_next", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern AVOutputFormat* av_oformat_next(AVOutputFormat* @f);
    
    [DllImport(libavformat, EntryPoint = "avformat_alloc_context", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern AVFormatContext* avformat_alloc_context();

    [DllImport(libavformat, EntryPoint = "avformat_open_input", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int avformat_open_input(AVFormatContext** @ps, [MarshalAs(UnmanagedType.LPStr)] string @url, AVInputFormat* @fmt, AVDictionary** @options);
    
    [DllImport(libavformat, EntryPoint = "avformat_find_stream_info", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int avformat_find_stream_info(AVFormatContext* @ic, AVDictionary** @options);

    [DllImport(libavformat, EntryPoint = "av_find_best_stream", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int av_find_best_stream(AVFormatContext* @ic, AVMediaType @type, int @wanted_stream_nb, int @related_stream, AVCodec** @decoder_ret, int @flags);
    
    [DllImport(libavformat, EntryPoint = "av_read_frame", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int av_read_frame(AVFormatContext* @s, AVPacket* @pkt);

    [DllImport(libavformat, EntryPoint = "avformat_seek_file", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int avformat_seek_file(AVFormatContext* @s, int @stream_index, long @min_ts, long @ts, long @max_ts, int @flags);

    [DllImport(libavformat, EntryPoint = "avformat_close_input", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern void avformat_close_input(AVFormatContext** @s);
        
    [DllImport(libavformat, EntryPoint = "av_codec_get_id", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern AvCodecId av_codec_get_id(AVCodecTag** @tags, uint @tag);
}