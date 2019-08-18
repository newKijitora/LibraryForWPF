using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Prototype.Models
{
    /// <summary>
    /// 画像を一元的に扱います。
    /// </summary>
    public static class ImageOperation
    {
        /// <summary>
        /// 画像ファイルのファイルサイズを取得します。
        /// </summary>
        public static long GetFileSize(this BitmapSource bitmapSource)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var wrappingStream = new WrappingStream(new MemoryStream()))
            {
                encoder.Save(wrappingStream);
                return wrappingStream.Length;
            }
        }

        /// <summary>
        /// ビットマップ画像を生成します。
        /// </summary>
        public static BitmapImage CreateBitmapImage(string filePath, bool freezing)
        {
            using (var wrappingStream = new WrappingStream(new MemoryStream(ConvertFileToBytes(filePath))))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.Initialize(wrappingStream);

                if (freezing)
                    bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        /// <summary>
        /// ビットマップ画像を生成します。
        /// </summary>
        public static BitmapImage CreateBitmapImage(string filePath, bool freezing, BitmapScalingMode scalingMode)
        {
            var bitmapImage = CreateBitmapImage(filePath, freezing);

            // 品質指定つき
            RenderOptions.SetBitmapScalingMode(bitmapImage, scalingMode);

            return bitmapImage;
        }

        /// <summary>
        /// ビットマップ画像を保存します。
        /// </summary>
        public static void Save(this BitmapImage bitmapImage, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(fileStream);
            }
        }

        // BitmapImageを初期化する
        private static void Initialize(this BitmapImage bitmapImage, Stream source)
        {
            bitmapImage.BeginInit();

            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.CreateOptions = BitmapCreateOptions.None;
            bitmapImage.StreamSource = source;

            bitmapImage.EndInit();
        }

        // ファイルをバイト型の配列に変換する
        private static byte[] ConvertFileToBytes(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }

        // メモリストリームを解放するためのラッパー
        // http://faithlife.codes/blog/2009/05/wrappingstream_implementation/
        private class WrappingStream : Stream
        {
            private Stream _streamBase;

            public WrappingStream(Stream streamBase)
            {
                _streamBase = streamBase ?? throw new ArgumentNullException(nameof(streamBase));
            }

            public override bool CanRead
            {
                get => _streamBase != null ? _streamBase.CanRead : false;
            }

            public override bool CanSeek
            {
                get => _streamBase != null ? _streamBase.CanSeek : false;
            }

            public override bool CanWrite
            {
                get => _streamBase != null ? _streamBase.CanWrite : false;
            }

            public override long Length
            {
                get
                {
                    ThrowIfDisposed();
                    return _streamBase.Length;
                }
            }

            public override long Position
            {
                get
                {
                    ThrowIfDisposed();
                    return _streamBase.Position;
                }
                set => _streamBase.Position = value;
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                ThrowIfDisposed();
                return _streamBase.BeginRead(buffer, offset, count, callback, state);
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                ThrowIfDisposed();
                return _streamBase.BeginWrite(buffer, offset, count, callback, state);
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                ThrowIfDisposed();
                return _streamBase.EndRead(asyncResult);
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                ThrowIfDisposed();
                _streamBase.EndWrite(asyncResult);
            }

            public override void Flush()
            {
                ThrowIfDisposed();
                _streamBase.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                ThrowIfDisposed();
                return _streamBase.Read(buffer, offset, count);
            }

            public override int ReadByte()
            {
                ThrowIfDisposed();
                return base.ReadByte();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                ThrowIfDisposed();
                return _streamBase.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                ThrowIfDisposed();
                _streamBase.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                ThrowIfDisposed();
                _streamBase.Write(buffer, offset, count);
            }

            public override void WriteByte(byte value)
            {
                ThrowIfDisposed();
                _streamBase.WriteByte(value);
            }

            protected Stream WrappedStream
            {
                get => _streamBase;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _streamBase = null;

                base.Dispose(disposing);
            }

            private void ThrowIfDisposed()
            {
                if (_streamBase == null)
                    throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}
