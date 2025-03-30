using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Utils
{
    public static class WindowHelper
    {
        public static Microsoft.UI.Xaml.Window GetWindowForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                // Get the window through the XamlRoot's HostWindow property
                var window = GetWindowFromXamlRoot(element.XamlRoot);
                return window;
            }
            return null;
        }

        private static Microsoft.UI.Xaml.Window GetWindowFromXamlRoot(XamlRoot xamlRoot)
        {
            // In WinUI 3, we can access the Window through reflection
            // since there's no direct API to get the Window from XamlRoot
            var windowProperty = xamlRoot.GetType().GetProperty("HostWindow",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (windowProperty != null)
            {
                return windowProperty.GetValue(xamlRoot) as Microsoft.UI.Xaml.Window;
            }

            return null;
        }
    }
}