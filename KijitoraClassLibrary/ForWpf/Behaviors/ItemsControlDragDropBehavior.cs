using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Prototype.Views.Behaviors
{
    /// <summary>
    /// <see cref="ItemsControl"/>のドラッグアンドドロップをサポートするビヘイビアです。
    /// </summary>
    public class ItemsControlDragDropBehavior : Behavior<ItemsControl>
    {
        // ドラッグされるデータの識別文字列
        private const string DragDataIdentifier = "dragData";

        /// <summary>
        /// ドロップ先のコントロールを指定します。
        /// </summary>
        public static DependencyProperty DropTargetProperty = DependencyProperty.Register(
            nameof(DropTarget), typeof(FrameworkElement), typeof(ItemsControlDragDropBehavior), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ドロップ先のコントロールを指定します。
        /// </summary>
        public FrameworkElement DropTarget
        {
            get => (FrameworkElement)GetValue(DropTargetProperty);
            set => SetValue(DropTargetProperty, value);
        }

        /// <summary>
        /// ドロップされたデータを受け入れるオブジェクトを指定します。
        /// </summary>
        public static DependencyProperty DroppedDataProperty = DependencyProperty.Register(
            nameof(DroppedData), typeof(object), typeof(ItemsControlDragDropBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// ドロップされたデータを受け入れるオブジェクトを指定します。
        /// </summary>
        public object DroppedData
        {
            get => GetValue(DroppedDataProperty);
            set => SetValue(DroppedDataProperty, value);
        }

        // プライベートフィールド
        private Window _window;
        private Point? _position;
        private object _dragData;
        private int? _index;

        /// <summary>
        /// コントロールにビヘイビアがアタッチされた時の動作です。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
            _window = Window.GetWindow(AssociatedObject);
        }

        /// <summary>
        /// コントロールからビヘイビアがデタッチされた時の動作です。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseLeftButtonDown -= ItemsControl_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp -= ItemsControl_PreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove -= ItemsControl_PreviewMouseMove;

            DropTarget.PreviewDragEnter -= DropTarget_PreviewDragEnter;
            DropTarget.PreviewDragOver -= DropTarget_PreviewDragOver;
            DropTarget.PreviewDrop -= DropTarget_PreviewDrop;
        }

        // ウィンドウがロードされた時
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.PreviewMouseLeftButtonDown += ItemsControl_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += ItemsControl_PreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove += ItemsControl_PreviewMouseMove;

            DropTarget.PreviewDragEnter += DropTarget_PreviewDragEnter;
            DropTarget.PreviewDragOver += DropTarget_PreviewDragOver;
            DropTarget.PreviewDrop += DropTarget_PreviewDrop;
        }

        // マウス左ボタンが押された時
        private void ItemsControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            if (itemsControl == null)
                return;

            var item = e.OriginalSource as FrameworkElement;
            if (item == null)
                return;

            var container = itemsControl.ContainerFromElement(item) as FrameworkElement;
            if (container == null)
                return;

            var itemData = container.DataContext;
            _dragData = new DataObject(DragDataIdentifier, itemData);

            var list = new List<object>();
            foreach (var itemSource in itemsControl.ItemsSource)
            {
                list.Add(itemSource);
            }

            _index = list.IndexOf(itemData);
            _position = _window.PointToScreen(e.GetPosition(_window));
        }

        // マウス左ボタンが離された時のキャンセル判定
        private void ItemsControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CleanUp();
        }

        // マウスの動きからドラッグ開始を判定
        private void ItemsControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragData == null || _position == null)
                return;

            var itemsControl = sender as ItemsControl;
            if (itemsControl == null)
                return;

            var multiSelector = itemsControl as MultiSelector;

            var position = _window.PointToScreen(e.GetPosition(_window));
            var point = (position - _position);

            var moveEnough =
                point.Value.X > SystemParameters.MinimumHorizontalDragDistance ||
                point.Value.Y > SystemParameters.MinimumVerticalDragDistance;

            if (!moveEnough)
            {
                if (multiSelector != null)
                    trimSelectedItems(multiSelector);

                return;
            }

            if (multiSelector != null)
                trimSelectedItems(multiSelector);

            DragDrop.DoDragDrop(itemsControl, _dragData, DragDropEffects.Move);

            CleanUp();

            // ローカル関数
            void trimSelectedItems(MultiSelector mSelector)
            {
                if (multiSelector.SelectedItems.Count > 1)
                {
                    var tmp = multiSelector.SelectedItem;
                    multiSelector.SelectedItems.Clear();
                    multiSelector.SelectedItem = tmp;
                }
            }
        }

        // ドロップ可能領域にマウスポインタが入った時
        private void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            var accept = e.Data.GetDataPresent(DragDataIdentifier);
            if (accept)
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        // ドロップ可能領域内でマウスがドラッグされている間
        private void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            var accept = e.Data.GetDataPresent(DragDataIdentifier);
            if (accept)
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        // ドロップ可能領域でドロップされた時
        private void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            DroppedData = e.Data.GetData(DragDataIdentifier);
            DropTarget.Tag = DroppedData;

            CleanUp();
        }

        // フィールドのクリーンアップ処理
        private void CleanUp()
        {
            _index = null;
            _position = null;
        }
    }
}
