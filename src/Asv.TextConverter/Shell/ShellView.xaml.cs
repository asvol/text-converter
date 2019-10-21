using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Asv.TextConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView 
    {
        private ScrollViewer _viewerSource;
        private ScrollViewer _viewerResult;

        public ShellView()
        {
            InitializeComponent();
            Loaded += ShellView_Loaded;
        }

        private void ShellView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewerSource = SourceList.FindVisualDescendant<ScrollViewer>();
            _viewerResult = ResultList.FindVisualDescendant<ScrollViewer>();
            _viewerSource.ScrollChanged += (o, args) =>
            {
                _viewerResult.ScrollToVerticalOffset(args.VerticalOffset);
                _viewerResult.ScrollToHorizontalOffset(args.HorizontalOffset);
            };
            _viewerResult.ScrollChanged += (o, args) =>
            {
                _viewerSource.ScrollToVerticalOffset(args.VerticalOffset);
                _viewerSource.ScrollToHorizontalOffset(args.HorizontalOffset);
            };
        }

        private void ViewerSource_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }

                
            }
        }
    }
}
