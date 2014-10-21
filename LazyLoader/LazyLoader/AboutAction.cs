using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace LazyLoader
{
    [ActionHandler("LazyLoader.About")]
    public class AboutAction : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // return true or false to enable/disable this action
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            MessageBox.Show(
              "Lazy<T> Converter\nImad Hashmi\n\nConvert your component types to Lazy<T>",
              "About Lazy<T> Converter",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information);
        }
    }
}