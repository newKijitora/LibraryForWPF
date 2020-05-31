using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KijitoraClassLibrary.ForWpf
{
    /// <summary>
    /// ビューモデルの基底クラスです。
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
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
        protected void Entry()
        {
            ViewModelEntried?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// ビューモデルの終了を通知します。
        /// </summary>
        public static event EventHandler ViewModelExited;

        /// <summary>
        /// 終了イベントを発行します。
        /// </summary>
        protected void Exit(bool done)
        {
            IsDone = done;
            IsCanceled = !done;

            ViewModelExited?.Invoke(this, EventArgs.Empty);
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

        private bool _isCanceled = true;

        //----------------------------------------------------------------------
        // 認証が必要かどうかを表す
        //----------------------------------------------------------------------

        public bool IsAuthenticationRequired { get; }

        public virtual bool Authenticate()
        {
            throw new NotImplementedException();
        }

        //----------------------------------------------------------------------
        // オーバーライド用
        //----------------------------------------------------------------------

        /// <summary>
        /// ビューモデルで必要な更新を実行します。
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// ビューモデルの終了を許可するかどうかを表します。
        /// </summary>
        public virtual bool CanClose
        {
            get => true;
        }

        //----------------------------------------------------------------------
        // プロパティの変更通知 -- INotifyPropertyChanged の実装
        //----------------------------------------------------------------------

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
