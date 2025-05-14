using Rayer.Core.Models;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Foundation.Metadata;
using Windows.Media;
using Windows.Storage;
using Windows.System;

namespace Rayer.Core.Playing;

public class SystemMediaTransportControlsManager
{
    private SystemMediaTransportControls _smtc = default!;
    private Action? _playAction;
    private Action? _pauseAction;
    private Action? _nextAction;
    private Action? _previousAction;

    public void Initialize(
        IntPtr windowHandle,
        Action playAction,
        Action pauseAction,
        Action? nextAction = null,
        Action? previousAction = null)
    {
        if (!ApiInformation.IsTypePresent("Windows.Media.SystemMediaTransportControls"))
        {
            return;
        }

        _playAction = playAction;
        _pauseAction = pauseAction;
        _nextAction = nextAction;
        _previousAction = previousAction;

        _smtc = SystemMediaTransportControlsInterop.GetForWindow(windowHandle);

        ConfigureMediaControls();
    }

    private void ConfigureMediaControls()
    {
        _smtc.IsPlayEnabled = _playAction is not null;
        _smtc.IsPauseEnabled = _pauseAction is not null;
        _smtc.IsNextEnabled = _nextAction is not null;
        _smtc.IsPreviousEnabled = _previousAction is not null;

        _smtc.ButtonPressed += OnButtonPressed;
    }

    private void OnButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
    {
        switch (args.Button)
        {
            case SystemMediaTransportControlsButton.Play:
                _playAction?.Invoke();
                break;
            case SystemMediaTransportControlsButton.Pause:
                _pauseAction?.Invoke();
                break;
            case SystemMediaTransportControlsButton.Next:
                _nextAction?.Invoke();
                break;
            case SystemMediaTransportControlsButton.Previous:
                _previousAction?.Invoke();
                break;
        }
    }

    // 修正后的播放状态更新方法
    public void UpdatePlaybackStatus(MediaPlaybackStatus status)
    {
        if (_smtc is not null)
        {
            _smtc.PlaybackStatus = status;
        }
    }

    public async Task UpdateMetadata(Audio audio)
    {
        if (_smtc is null)
        {
            return;
        }

        if (audio.Cover is not null)
        {
            await SetAlbumArtFromStreamAsync(audio);
        }
    }

    private async Task SetAlbumArtFromStreamAsync(Audio audio)
    {
        try
        {
            var bitmapImage = (BitmapImage)audio.Cover!;

            // 1. 创建与 BitmapImage 相同尺寸的 WriteableBitmap
            var writableBitmap = new WriteableBitmap(bitmapImage);

            // 2. 将 BitmapImage 的像素数据复制到 WriteableBitmap
            var stride = bitmapImage.PixelWidth * (bitmapImage.Format.BitsPerPixel / 8);
            var pixels = new byte[bitmapImage.PixelHeight * stride];

            bitmapImage.CopyPixels(pixels, stride, 0);

            writableBitmap.WritePixels(
                new Int32Rect(0, 0, bitmapImage.PixelWidth, bitmapImage.PixelHeight),
                pixels,
                stride,
                0);

            // 3. 编码为可寻的 MemoryStream
            using var memoryStream = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(writableBitmap));
            encoder.Save(memoryStream);

            // 4. 创建临时文件
            var tempFilePath = Path.Combine(Path.GetTempPath(), "cover_temp.png");
            using (var fileStream = File.Create(tempFilePath))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(fileStream);
            }

            // 5. 设置封面
            var cover = await StorageFile.GetFileFromPathForUserAsync(User.GetDefault(), tempFilePath);
            await _smtc.DisplayUpdater.CopyFromFileAsync(MediaPlaybackType.Music, cover);

            _smtc.DisplayUpdater.MusicProperties.Title = audio.Title;
            _smtc.DisplayUpdater.MusicProperties.Artist = string.Join('/', audio.Artists);
            _smtc.DisplayUpdater.MusicProperties.AlbumTitle = audio.Album;
            _smtc.DisplayUpdater.Update();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"设置专辑封面失败: {ex.Message}");
            // 失败时清除封面
            _smtc.DisplayUpdater.Thumbnail = null;
            _smtc.DisplayUpdater.Update();
        }
    }
}