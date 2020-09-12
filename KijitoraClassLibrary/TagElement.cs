using System.Text.RegularExpressions;

namespace KijitoraClassLibrary
{
    public class TagElement
    {
        public TagElement(string elementName)
        {
            ElementName = elementName;

            OpenTag = "<" + elementName + ">";
            CloseTag = "</" + elementName + ">";

            _pattern = new Regex(@"\<" + elementName + @"\>[\s\S]*?\</" + elementName + @"\>");
        }

        // 要素名
        public string ElementName { get; }

        // 開きタグ
        public string OpenTag { get; }

        // 閉じタグ
        public string CloseTag { get; }

        // 要素照合用の正規表現文字列
        private readonly Regex _pattern;

        // マッチオブジェクトを返す
        public Match Match(string input)
        {
            return _pattern.Match(input);
        }

        // 渡された文字列をマークアップして返す
        public string Markup(string rawStr)
        {
            return OpenTag + rawStr + CloseTag;
        }

        // マークアップされた文字列からマークアップを外して返す
        public string Release(string markup)
        {
            return markup.Replace(OpenTag, "").Replace(CloseTag, "");
        }

        // 文字列がマークアップされているかどうかを調べる
        public bool IsMarkup(string markup)
        {
            var matches1 = Regex.Matches(markup, OpenTag);

            if (matches1.Count != 1)
            {
                return false;
            }

            var matches2 = Regex.Matches(markup, CloseTag);

            if (matches2.Count != 1)
            {
                return false;
            }

            return true;
        }
    }
}
