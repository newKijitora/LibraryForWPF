using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ForWpf
{
    /// <summary>
    /// ビューモデルの機能を定義する抽象クラスです。
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        //-----------------------------------------------------------------------------------------
        // コンストラクタ
        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="ViewModel"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public ViewModel() { }

        //-----------------------------------------------------------------------------------------
        // ビューモデルの開始と終了
        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// ビューモデルの開始を通知します。
        /// </summary>
        public static event EventHandler ViewModelEntried;

        /// <summary>
        /// 開始イベントを発行します。
        /// </summary>
        protected virtual void Entry()
        {
            ViewModelEntried?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// ビューモデルの終了を通知します。
        /// </summary>
        public static event EventHandler ViewModelExited;

        /// <summary>
        /// 終了イベントを発行します。
        /// </summary>
        protected virtual void Exit()
        {
            ViewModelExited?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 終了イベントを発行します。
        /// </summary>
        protected virtual void Exit(bool done)
        {
            IsDone = done;
            IsCanceled = !done;

            ViewModelExited?.Invoke(this, new EventArgs());
        }

        //----------------------------------------------------------------------
        // 実行中のビューモデルをキャンセルするコマンド
        //----------------------------------------------------------------------

        /// <summary>
        /// 実行中のビューモデルをキャンセルします。
        /// </summary>
        public DelegateCommand CancelCommand
        {
            get => _cancelCommand ?? (_cancelCommand =
                new DelegateCommand(CancelExecute, CanCancelExecute));
        }

        private DelegateCommand _cancelCommand;

        // 実行内容
        protected virtual void CancelExecute()
        {
            Exit(false);
        }

        // 実行条件
        protected virtual bool CanCancelExecute()
        {
            return true;
        }

        //----------------------------------------------------------------------
        // フラグ
        //----------------------------------------------------------------------

        /// <summary>
        /// ビューモデルが正常に終了したどうかを表します。
        /// </summary>
        public bool IsDone
        {
            get => _isDone;
            set => SetProperty(ref _isDone, value);
        }

        private bool _isDone = false;

        /// <summary>
        /// ビューモデルがキャンセルされたかどうかを表します。
        /// </summary>
        public bool IsCanceled
        {
            get => _isCanceled;
            set => SetProperty(ref _isCanceled, value);
        }

        private bool _isCanceled;

        /// <summary>
        /// プロパティの変更を通知するイベントです。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
