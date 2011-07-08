using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ImpromptuInterface.MVVM
{
    public class DialogBox
    {

#if !SILVERLIGHT

        public MessageBoxOptions Options { get; set; }

        public MessageBoxResult DefaultResult { get; set; }

        public MessageBoxImage Icon { get; set; }
#endif

        public MessageBoxButton Button { get; set; }

        public string Caption { get; set; }

        public string MessageBoxText { get; set; }

        public bool? ShowDialog(Window owner =null)
        {
            MessageBoxResult tResult;
            if(owner ==null)
            {
                tResult = MessageBox.Show(MessageBoxText, Caption, Button
#if !SILVERLIGHT
                                          , Icon, DefaultResult, Options
#endif
                    );
            }else
            {
                tResult = MessageBox.Show(
#if !SILVERLIGHT
                    owner,
#endif
                    MessageBoxText, Caption, Button
#if !SILVERLIGHT
                    , Icon, DefaultResult, Options
#endif
                    );
            }
            return tResult == MessageBoxResult.Yes || tResult == MessageBoxResult.OK;
        }
    }
}
