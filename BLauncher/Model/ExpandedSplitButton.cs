using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BLauncher.Model
{
    internal class ExpandedSplitButton : SplitButton
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RemoveCloseOnClick(GetTemplateChild("PART_Button") as Button);
        }

        private void RemoveCloseOnClick(Button button)
        {
            // Get the EventHandlersStore instance which holds event handlers for the specified element.
            // The EventHandlersStore class is declared as internal.
            var buttonClickMethodInfo = typeof(SplitButton).GetMethod(
                "ButtonClick", BindingFlags.Instance | BindingFlags.NonPublic);
            RoutedEventHandler handler = buttonClickMethodInfo.CreateDelegate<RoutedEventHandler>(this);
            button.Click -= handler;

            button.Click += (_, e) =>
            {
                Command?.Execute(null);
                e.RoutedEvent = ClickEvent;
                RaiseEvent(e);
            };
        }
    }
}
