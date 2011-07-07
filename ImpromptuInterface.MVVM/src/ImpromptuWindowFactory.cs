using System;
using ImpromptuInterface.Dynamic;

#if !SILVERLIGHT
using Microsoft.Win32;
#endif

namespace ImpromptuInterface.MVVM
{

    public interface IDialogFactory
    {
#if !SILVERLIGHT

        Win<OpenFileDialog> OpenDialog { get; }
        Win<SaveFileDialog> SaveDialog { get; }
#endif
        Win<DialogBox> DialogBox { get; }
    }

    public interface IWindowFactory<out TInterface> where TInterface : class,IDialogFactory
    {
        TInterface New { get; }
        TInterface SingleInstance { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    public class ImpromptuWindowFactory<TInterface> : IWindowFactory<TInterface> where TInterface : class,IDialogFactory
    {
        internal class ImpromptuWinFactory : ImpromptuFactory
        {
            protected override object CreateType(Type type, params object[] args)
            {
                var tObj= base.CreateType(type.GetGenericArguments()[0], args);
                return base.CreateType(type, tObj); 
            }
        }

        internal class ImpromptuWinSingleInstancesFactory : ImpromptuSingleInstancesFactory
        {
            protected override object CreateType(Type type, params object[] args)
            {
                var tObj = base.CreateType(type.GetGenericArguments()[0], args);
                return base.CreateType(type, tObj);
            }
        }

        private readonly TInterface _factory = new ImpromptuWinFactory().ActLike<TInterface>();
        private readonly TInterface _singletonFactory = new ImpromptuWinSingleInstancesFactory().ActLike<TInterface>();

        public TInterface New
        {
            get
            {
                return _factory;
            }
        }

        public TInterface SingleInstance
        {
            get { return _singletonFactory; }
        }
    }

    public class Win<T>
    {
        private dynamic _target;

        public Win(dynamic target)
        {
            _target = target;
        }

        public Type RepresentedType
        {
            get { return typeof (T); }
        }

        public dynamic SetProperties
        {
            get { return Impromptu.Curry(Impromptu.InvokeSetAll)(_target); }
        }

        public void Show()
        {
            _target.Show();
        }

        public void Hide()
        {
            _target.Hide();
        }
        public void Close()
        {
            _target.Close();
        }
        public bool? ShowDialog()
        {
            return _target.ShowDialog();
        }
    }
}
