using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace KijitoraClassLibrary.ForWpf.Behaviors
{
    /// <summary>
    /// <see cref="Selector"/>のドラッグアンドドロップ（複数ドラッグ・並び替え）をサポートするビヘイビアです。
    /// </summary>
    public class SelectorMultiSelectAndDragDropBehavior : Behavior<Selector>
    {
        //-------------------------------------------------------------
        // コンストラクタ
        //-------------------------------------------------------------

        /// <summary>
        /// <see cref="SelectorMultiSelectAndDragDropBehavior"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public SelectorMultiSelectAndDragDropBehavior() : base()
        {
            SetValue(SelectedItemsProperty, new List<object>());
        }

        //-------------------------------------------------------------
        // フィールドとプロパティ
        //-------------------------------------------------------------

        private readonly List<DataGridRow> _dataGridRows = new List<DataGridRow>();
        private readonly string DragDataFormat = DataFormats.UnicodeText;
        private bool _canThrough = false;
        private IList _itemsCache;
        private Window _window;
        private Point? _mousePosition;
        private object _dragData;
        private int? _index;
        private object _removeItem;

        /// <summary>
        /// ドロップ先のコントロールを指定します。
        /// </summary>
        public static readonly DependencyProperty DropTargetProperty = DependencyProperty.Register(
            nameof(DropTarget), typeof(FrameworkElement),
            typeof(SelectorMultiSelectAndDragDropBehavior),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ドロップ先のコントロールを指定します。
        /// </summary>
        public FrameworkElement DropTarget
        {
            get => (FrameworkElement)GetValue(DropTargetProperty);
            set => SetValue(DropTargetProperty, value);
        }

        /// <summary>
        /// 選択されたアイテムを表します。
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems), typeof(IList),
            typeof(SelectorMultiSelectAndDragDropBehavior),
            new FrameworkPropertyMetadata(new List<object>()));

        /// <summary>
        /// 選択されたアイテムを表します。
        /// </summary>
        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// ドロップされたデータを受け入れるオブジェクトを指定します。
        /// </summary>
        public static readonly DependencyProperty DroppedDataProperty = DependencyProperty.Register(
            nameof(DroppedData), typeof(object),
            typeof(SelectorMultiSelectAndDragDropBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// ドロップされたデータを受け入れるオブジェクトを指定します。
        /// </summary>
        public object DroppedData
        {
            get => GetValue(DroppedDataProperty);
            set => SetValue(DroppedDataProperty, value);
        }

        /// <summary>
        /// コレクション内でドロップされたアイテムのインデックスです。
        /// </summary>
        public static readonly DependencyProperty DroppedItemIndexProperty = DependencyProperty.Register(
            nameof(DroppedItemIndex), typeof(int),
            typeof(SelectorMultiSelectAndDragDropBehavior),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// コレクション内でドロップされたアイテムのインデックスです。
        /// </summary>
        public int DroppedItemIndex
        {
            get => (int)GetValue(DroppedItemIndexProperty);
            set => SetValue(DroppedItemIndexProperty, value);
        }

        /// <summary>
        /// コレクション内でドラッグされるアイテムのインデックスです。
        /// </summary>
        public static readonly DependencyProperty DragItemIndexProperty = DependencyProperty.Register(
            nameof(DragItemIndex), typeof(int),
            typeof(SelectorMultiSelectAndDragDropBehavior),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// コレクション内でドラッグされるアイテムのインデックスです。
        /// </summary>
        public int DragItemIndex
        {
            get => (int)GetValue(DragItemIndexProperty);
            set => SetValue(DragItemIndexProperty, value);
        }

        /// <summary>
        /// 全選択されているかどうかを表します。
        /// </summary>
        public static readonly DependencyProperty AllSelectProperty = DependencyProperty.Register(
            nameof(AllSelect), typeof(bool),
            typeof(SelectorMultiSelectAndDragDropBehavior),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(AllSelectCallback)));

        /// <summary>
        /// 全選択されているかどうかを表します。
        /// </summary>
        public bool AllSelect
        {
            get => (bool)GetValue(AllSelectProperty);
            set => SetValue(AllSelectProperty, value);
        }

        // 全選択されたときのコールバック
        private static void AllSelectCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var behavior = dependencyObject as SelectorMultiSelectAndDragDropBehavior;
            if (behavior is null)
            {
                return;
            }

            if (behavior.AssociatedObject is DataGrid dataGrid)
            {
                dataGrid.SelectedItems.Clear();

                var selectAll = (bool)e.NewValue;
                if (selectAll)
                {
                    dataGrid.SelectAll();
                }
            }
        }

        //-------------------------------------------------------------
        // アタッチ、デタッチ、ロード
        //-------------------------------------------------------------

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
            AssociatedObject.PreviewMouseDown -= ItemsControl_PreviewMouseDown;

            AssociatedObject.SelectionChanged -= Selector_SelectionChanged;

            DropTarget.PreviewDragEnter -= DropTarget_PreviewDragEnter;
            DropTarget.PreviewDragOver -= DropTarget_PreviewDragOver;
            DropTarget.PreviewDrop -= DropTarget_PreviewDrop;
        }

        // コントロールがロードされた時
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.PreviewMouseLeftButtonDown += ItemsControl_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += ItemsControl_PreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove += ItemsControl_PreviewMouseMove;
            AssociatedObject.PreviewMouseDown += ItemsControl_PreviewMouseDown;

            AssociatedObject.SelectionChanged += Selector_SelectionChanged;

            if (DropTarget is null)
            {
                return;
            }

            DropTarget.PreviewDragEnter += DropTarget_PreviewDragEnter;
            DropTarget.PreviewDragOver += DropTarget_PreviewDragOver;
            DropTarget.PreviewDrop += DropTarget_PreviewDrop;
        }

        //-------------------------------------------------------------
        // イベントハンドラ
        //-------------------------------------------------------------

        //-------------------------------------------------------------
        // 1. PreviewMouseLeftButtonDown イベント
        //-------------------------------------------------------------

        private void ItemsControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var isCtrlSelected = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            _mousePosition = _window.PointToScreen(e.GetPosition(_window));

            var selector = sender as Selector;
            if (selector is null)
            {
                return;
            }

            var reportingElement = e.OriginalSource as DependencyObject;
            if (reportingElement is null)
            {
                return;
            }

            var container = selector.ContainerFromElement(reportingElement);
            if (container is null)
            {
                return;
            }

            if (selector is DataGrid dataGrid)
            {
                if (_itemsCache is null)
                {
                    return;
                }

                var selectedRow = container as DataGridRow;
                if (selectedRow is null)
                {
                    return;
                }

                var selectedItem = selectedRow.Item;

                if (!_itemsCache.Contains(selectedItem))
                {
                    return;
                }

                if (SelectedItems.Contains(selectedItem))
                {
                    if (isCtrlSelected)
                    {
                        _removeItem = selectedItem;
                    }
                    _canThrough = true;
                }

                foreach (var item in SelectedItems)
                {
                    var row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (row is null)
                    {
                        continue;
                    }

                    foreach (var column in dataGrid.Columns)
                    {
                        var cell = column.GetCellContent(row).Parent;
                        PaintCell(cell);
                    }
                    _dataGridRows.Add(row);
                }
                _dragData = new DataObject(DragDataFormat, SelectedItems);
            }

            if (selector is ListBox listBox)
            {

            }
        }

        //-------------------------------------------------------------
        // 2. PreviewMouseDown イベント
        //-------------------------------------------------------------

        private DataGridCellInfo? _currentCell = null;

        private void ItemsControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            if (_currentCell is null && dataGrid.CurrentCell.Item != null)
            {
                _currentCell = dataGrid.CurrentCell;
            }
        }

        //-------------------------------------------------------------
        // 4. SelectionChanged イベント
        //-------------------------------------------------------------

        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                _itemsCache = CreateNewSelectedItems(dataGrid.SelectedItems);

                if (!_canThrough)
                {
                    SelectedItems = CreateNewSelectedItems(_itemsCache);
                }
            }
        }

        //-------------------------------------------------------------
        // 5. PreviewMouseLeftButtonUp イベント
        //-------------------------------------------------------------

        private void ItemsControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = _window.PointToScreen(e.GetPosition(_window));
            var point = position - _mousePosition;

            if (!point.HasValue)
            {
                return;
            }

            if (sender is DataGrid dataGrid)
            {
                foreach (var row in _dataGridRows)
                {
                    if (row.Item == _removeItem)
                    {
                        row.IsSelected = false;
                    }

                    if (dataGrid.SelectedItem != row.Item && _canThrough && _removeItem is null)
                    {
                        row.IsSelected = false;
                    }

                    foreach (var column in dataGrid.Columns)
                    {
                        var cell = column.GetCellContent(row).Parent;
                        ClearCell(cell);
                    }
                }

                _dataGridRows.Clear();

                if (dataGrid.SelectedItems.Count == 1 && _canThrough)
                {
                    SelectedItems = CreateNewSelectedItems(dataGrid.SelectedItems);
                    _canThrough = false;
                }
                else if (_removeItem != null)
                {
                    SelectedItems = CreateNewSelectedItems(dataGrid.SelectedItems);
                    _canThrough = false;
                    _removeItem = null;
                }

                _currentCell = null;
                CleanUp();
            }

            if (sender is ListBox listBox)
            {

            }
        }

        //-------------------------------------------------------------
        // 6. PreviewMouseMove イベント（ドラッグイベント）
        //-------------------------------------------------------------

        private void ItemsControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is Thumb)
            {
                return;
            }

            var selector = sender as Selector;
            if (selector is null)
            {
                return;
            }

            if (!_mousePosition.HasValue)
            {
                return;
            }

            var mousePosition = _window.PointToScreen(e.GetPosition(_window));
            var moveDistance = mousePosition - _mousePosition;

            var dragStart =
                Math.Abs(moveDistance.Value.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(moveDistance.Value.Y) > SystemParameters.MinimumVerticalDragDistance;

            if (!dragStart)
            {
                return;
            }

            DragDrop.DoDragDrop(selector, _dragData, DragDropEffects.Move);

            if (selector is DataGrid dataGrid)
            {
                AssociatedObject.SelectionChanged -= Selector_SelectionChanged;

                foreach (var row in _dataGridRows)
                {
                    row.IsSelected = true;

                    foreach (var column in dataGrid.Columns)
                    {
                        var cell = column.GetCellContent(row).Parent;
                        ClearCell(cell);
                    }
                }
                _dataGridRows.Clear();

                for (int i = 0; i < SelectedItems.Count; i++)
                {
                    if (!dataGrid.SelectedItems.Contains(SelectedItems[i]))
                    {
                        dataGrid.SelectedItems.Insert(i, SelectedItems[i]);
                    }
                }
                _itemsCache = CreateNewSelectedItems(dataGrid.SelectedItems);
                _canThrough = false;

                AssociatedObject.SelectionChanged += Selector_SelectionChanged;
            }
            CleanUp();
        }

        //-------------------------------------------------------------
        // 7. PreviewDragEnter イベント（ドラッグイベント）
        //-------------------------------------------------------------

        private void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            var dataIsExists = e.Data.GetDataPresent(DragDataFormat);
            if (dataIsExists)
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        //-------------------------------------------------------------
        // 8. PreviewDragOver イベント（ドラッグイベント）
        //-------------------------------------------------------------

        private void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            var dataIsExists = e.Data.GetDataPresent(DragDataFormat);
            if (dataIsExists)
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        //-------------------------------------------------------------
        // 9. PreviewDrop イベント（ドラッグイベント）
        //-------------------------------------------------------------

        private void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            DroppedData = e.Data.GetData(DragDataFormat);

            if (AssociatedObject == sender)
            {
                var itemsControl = sender as ItemsControl;
                if (itemsControl is null) return;

                var item = e.OriginalSource as FrameworkElement;
                if (item is null) return;

                var container = itemsControl.ContainerFromElement(item) as FrameworkElement;
                if (container is null) return;

                var itemData = container.DataContext;
                _dragData = new DataObject(DragDataFormat, itemData);

                var list = new List<object>();
                foreach (var itemSource in itemsControl.ItemsSource)
                {
                    list.Add(itemSource);
                }

                if (_index.HasValue)
                {
                    DragItemIndex = _index.Value;
                    DroppedItemIndex = list.IndexOf(itemData);
                }
            }
            else
            {
                DropTarget.Tag = DroppedData;
            }
            CleanUp();
        }

        //-------------------------------------------------------------
        // その他のメソッド
        //-------------------------------------------------------------

        // フィールドのクリーンアップ処理
        private void CleanUp()
        {
            _index = null;
            _mousePosition = null;
        }

        // 新しい選択項目のリストを生成する
        private static IList CreateNewSelectedItems(IList sourceItems)
        {
            var items = new List<object>();
            foreach (var item in sourceItems)
            {
                items.Add(item);
            }
            return items;
        }

        // セルのカラーリング
        private static void PaintCell(DependencyObject cell)
        {
            cell.SetValue(Control.BackgroundProperty, new SolidColorBrush(SystemColors.HighlightColor));
            cell.SetValue(Control.ForegroundProperty, new SolidColorBrush(SystemColors.HighlightTextColor));
            cell.SetValue(Control.BorderBrushProperty, new SolidColorBrush(SystemColors.HighlightColor));
        }

        // セルの色をクリア
        private static void ClearCell(DependencyObject cell)
        {
            cell.ClearValue(Control.BackgroundProperty);
            cell.ClearValue(Control.ForegroundProperty);
            cell.ClearValue(Control.BorderBrushProperty);
        }
    }
}
