using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BCIT615.Assessment1.GamePlayer.Model;

namespace Wpf;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    GamePlayer game = new GamePlayer();
    public MainWindow()
    {
        InitializeComponent();
        for (int row = 0; row < ReferenceBoardData.Rows; row++)
        {
            for (int column = 0; column < ReferenceBoardData.Columns; column++)
            {
                var button = new Button
                {
                    Content = "test"
                };
                Board.Children.Add(button);
            }
        }

    }
}