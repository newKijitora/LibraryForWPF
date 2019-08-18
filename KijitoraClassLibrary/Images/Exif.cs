using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.Images
{
    /// <summary>
    /// 画像ファイルのEXIF情報を表します。
    /// </summary>
    public class Exif
    {
        /// <summary>
        /// 画像ファイルの作成日です。
        /// </summary>
        public DateTime PhotographingDate { get; }

        /// <summary>
        /// <see cref="Exif"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="filePath"></param>
        public Exif(string filePath)
        {
            using (var bitmap = new Bitmap(filePath))
            {
                foreach (var item in bitmap.PropertyItems)
                {
                    if (item.Id == 0x9003 && item.Type == 2)
                    {
                        var dateStr = Encoding.ASCII.GetString(item.Value).Trim(new char[] { '\0' });
                        PhotographingDate = DateTime.ParseExact(dateStr, "yyyy:MM:dd HH:mm:ss", null);
                    }
                }
            }
        }
    }
}
