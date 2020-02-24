using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace KijitoraClassLibrary.ForWpf
{
    /// <summary>
    /// ビューモデルの開始と対応するウィンドウをスタックで管理します。
    /// </summary>
    public class WindowStack : Stack<Window>
    {
        // アプリケーション
        private Application _app;

        /// <summary>
        /// <see cref="WindowStack" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        public WindowStack(Application app)
        {
            // アプリケーションのバインド
            _app = app;

            // イベントハンドラの登録
            ViewModel.ViewModelEntried += OpenWindowWithViewModelEntried;
            ViewModel.ViewModelExited += CloseWindowWithViewModelExited;
        }

        // ビューモデル開始時
        private void OpenWindowWithViewModelEntried(object sender, EventArgs e)
        {
            var type = _app.GetType();
            var assembly = Assembly.GetAssembly(type);
            var namespaceName = $"{assembly.GetName().Name}.{"Views"}";
            var viewModelName = sender.GetType().Name;
            var windowName = viewModelName.Replace(nameof(ViewModel), string.Empty);

            var window = assembly.CreateInstance($"{namespaceName}.{windowName}") as Window;
            if (window is null)
            {
                return;
            }

            var viewModel = sender as ViewModel;
            if (viewModel is null)
            {
                return;
            }

            window.Closing += Window_Closing;
            window.DataContext = viewModel;

            if (this.Any())
                window.Owner = Peek();

            Push(window);
            window.ShowDialog();

            // ウィンドウが閉じられたあと
            Pop();
        }

        //----------------------------------------------------------------------
        // ウィンドウを閉じるときの処理
        //----------------------------------------------------------------------

        // ビューモデル終了時
        private void CloseWindowWithViewModelExited(object sender, EventArgs e)
        {
            if (!this.Any())
            {
                return;
            }

            var window = Peek();
            window.Close();
        }

        // 画面の閉じるボタンが押されたとき、画面の×印やAltキー＋F4を押して画面を閉じた時
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var window = sender as Window;
            if (window is null) return;

            var viewModel = window.DataContext as ViewModel;
            if (viewModel is null) return;

            if (viewModel.CanClose)
            {
                window.Closing -= Window_Closing;
                ClosingCheck(window);
            }
            else
            {
                e.Cancel = true;
            }
        }

        // アプリケーションを閉じる必要があるかどうかをチェックする
        protected virtual void ClosingCheck(Window window)
        {
            var viewModel = window.DataContext as ViewModel;
            if (viewModel is null) return;

            var viewModelIsCanceled = viewModel != null ? viewModel.IsCanceled : true;

            // メインウィンドウを閉じた場合
            if (window == _app.MainWindow)
            {
                // イベントハンドラの解除
                ViewModel.ViewModelEntried -= OpenWindowWithViewModelEntried;
                ViewModel.ViewModelExited -= CloseWindowWithViewModelExited;
            }
        }
    }
}
