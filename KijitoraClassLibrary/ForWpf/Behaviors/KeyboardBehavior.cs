using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace KijitoraClassLibrary.ForWpf.Behaviors
{
    /// <summary>
    /// キーボード操作を再現します。
    /// </summary>
    public class KeyboardBehavior : Behavior<FrameworkElement>
    {
        //-------------------------------------------------------------
        // キーボードの各キー
        //-------------------------------------------------------------

        /// <summary>
        /// キーボードの各キーの値を取得する添付プロパティです。
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(object), typeof(KeyboardBehavior));

        /// <summary>
        /// キーボードの各キーの値を取得します。
        /// </summary>
        public static object GetKey(DependencyObject obj)
        {
            return obj.GetValue(KeyProperty);
        }

        /// <summary>
        /// キーボードの各キーの値を設定します。
        /// </summary>
        public static void SetKey(DependencyObject obj, object value)
        {
            obj.SetValue(KeyProperty, value);
        }

        //-------------------------------------------------------------
        // キーボードの選択されたキー
        //-------------------------------------------------------------

        /// <summary>
        /// キーボードの選択されたキーを表します。
        /// </summary>
        public static readonly DependencyProperty SelectedKeyProperty =
            DependencyProperty.Register(nameof(SelectedKey), typeof(object), typeof(KeyboardBehavior),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                });

        /// <summary>
        /// キーボードの選択されたキーを表します。
        /// </summary>
        public object SelectedKey
        {
            get => GetValue(SelectedKeyProperty);
            set => SetValue(SelectedKeyProperty, value);
        }

        //-------------------------------------------------------------
        // キーボードのキーが選択されたときのイベントハンドラ
        //-------------------------------------------------------------

        /// <summary>
        /// キーボードのキーが選択されたときのイベントハンドラ
        /// </summary>
        private void Keyboard_KeySelected(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement source)
            {
                if (GetKey(source) != null)
                {
                    SelectedKey = GetKey(source);
                    return;
                }

                if (source is ContentControl contentControl)
                {
                    SelectedKey = contentControl.Content;
                    return;
                }
            }
        }

        //---------------------------------------------------
        // アタッチ時、デタッチ時の動作
        //---------------------------------------------------

        /// <summary>
        /// アタッチ時の動作
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Keyboard_KeySelected));
        }

        /// <summary>
        /// デタッチ時の動作
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Keyboard_KeySelected));
        }
    }
}

