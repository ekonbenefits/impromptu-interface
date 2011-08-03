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


        Win<T> Window<T>(params object[] args);
    }

    public interface IWindowBuilder<out TInterface> where TInterface : class,IDialogFactory
    {
        TInterface New { get; }
        TInterface SingleInstance { get; }
    }

  
    /// <summary>
    /// Dynamic Window Builder with limited static interface can help decouple talking with new windows.
    /// Experimental as of 3.5, may change a lot in future.
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    public class ImpromptuWindowBuilder<TInterface> : IWindowBuilder<TInterface> where TInterface : class,IDialogFactory
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


        public class WinObscure:ImpromptuForwarder
        {
           internal WinObscure(object target):base(target)
           {
               
           }

           public override bool TryInvoke(System.Dynamic.InvokeBinder binder, object[] args, out object result)
           {
               if (base.TryInvoke(binder, args, out result))
               {
                   result = new Win<T>(result);
                   return true;
               }
               return false;
           }
        }

        public dynamic SetProperties
        {
            get { return new WinObscure(Impromptu.Curry(Impromptu.InvokeSetAll)(_target)); }
        }

        public dynamic Get
        {
            get{
                return new ImpromptuGet(_target);
            }
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
