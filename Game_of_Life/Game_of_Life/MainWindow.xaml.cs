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
using System.Windows.Threading;


/*DONE:
 * Playing the Game
 * From time to time documentation and Tagebuch
 * Z 133, change from Field 2 and 3 to anzahlNeighbors condition!
 *  * Start / Stop button (Program the stop button and change it to stop when start is clicked and vice versa)
 * */
//TODO:
/*
 *
*/
namespace Game_of_Life
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //This shows which of the squares in the whole field are filled
        //Coordinates of the field:
        //Up-Left: 90, 50;
        //Up-Right: 1090, 50;
        //Down-Left: 90, 500;
        //Down-Right: 1090, 500;

        //States of the Field:
        //true -> alive     false -> dead
        Boolean[,] Field = new Boolean[50, 27];
        // Determines for each cell if it will live on the next turn or not
        Boolean[,] cellWillLive = new Boolean[50, 27];
        DispatcherTimer timer = new DispatcherTimer();
        //converting from x and y coordinates into columns and rows
        int col = 0, row = 0;
        //buttonState 1: the game will be played; -1: the game will be stopped
        short buttonState = -1;
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(400);
            timer.Tick += gameTick;
        }

        private void gameTick(object sender, EventArgs e)
        {
            //Loop after the game ticked
            playGame();
        }

        public void playGame()
        {
            int anzahlNeighbors = 0;
            for (int x = 0; x < 49; x++)
            {
                for(int y = 0; y < 26; y++)
                {
                    //}
                    /*
                     * A live cell dies if it has fewer than two live neighbors.
                     * A live cell with two or three live neighbors lives on to the next generation.
                     * A live cell with more than three live neighbors dies.
                     * A dead cell will be brought back to live if it has exactly three live neighbors.
                    */

                    if (x != 0)
                    {
                        if(y!= 0)
                        {
                            if (Field[x - 1, y - 1])
                            {
                                anzahlNeighbors++;
                            }
                        }
                        if (Field[x - 1, y])
                        {
                            anzahlNeighbors++;
                        }
                        if (Field[x - 1, y + 1])
                        {
                            anzahlNeighbors++;
                        }
                    }
                    if (y != 0)
                    {
                        if (Field[x + 1, y - 1])
                        {
                            anzahlNeighbors++;
                        }
                        if (Field[x, y - 1])
                        {
                            anzahlNeighbors++;
                        }
                    }
                        if (Field[x , y +1])
                        {
                            anzahlNeighbors++;
                        }
                        if (Field[x + 1, y + 1])
                        {
                            anzahlNeighbors++;
                        }
                        if (Field[x + 1, y])
                        {
                            anzahlNeighbors++;
                        }
                        
                if (anzahlNeighbors < 2 || anzahlNeighbors > 3)
                    {
                        cellWillLive[x, y] = false;
                    }
                else if(anzahlNeighbors == 3)
                    {
                        cellWillLive[x, y] = true;
                    }
                else if (Field[x,y] && anzahlNeighbors == 2)
                    {
                        cellWillLive[x, y] = true;
                    }
                //Resetting for the next cell
                    anzahlNeighbors = 0;
                }

            }
            //Preparing for the next phase
            for(int x = 0; x < 49; x++)
            {
                for (int y = 0; y < 25; y++)
                {
                    //Cell will live in the next step
                    if (cellWillLive[x,y] == true)
                    {
                        Field[x, y] = true;
                        createCell(x, y);
                    }

                    //Cell will die in the next step
                    else if (Field[x,y] && cellWillLive[x,y] == false)
                    {
                        Field[x, y] = false;
                        destroyCell(x, y);
                    }
                }
            }
            RemakeBorder();
        }
        public void createCell(int posx, int posy)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#57994e");
            SolidColorBrush myBrush = new SolidColorBrush(color);
            Rectangle newRec = new Rectangle();
            newRec.Fill = myBrush;
            newRec.Stroke = Brushes.Pink;
            newRec.Height = 20;
            newRec.Width = 20;
            mainCanvas.Children.Add(newRec);
            //10 pixel abstand zum oberen Rand
            Canvas.SetTop(newRec, posy * 20 + 70);
            //10 pixel abstand zum linkeren Rand
            Canvas.SetLeft(newRec, posx * 20 + 90);
        }

        public void destroyCell(int posx, int posy)
        {
            Rectangle newRec = new Rectangle();
            newRec.Fill = Brushes.Black;
            newRec.Stroke = Brushes.Black;
            newRec.Height = 20;
            newRec.Width = 20;
            mainCanvas.Children.Add(newRec);
            //10 pixel abstand zum oberen Rand
            Canvas.SetTop(newRec, posy * 20 + 70);
            //10 pixel abstand zum linkeren Rand
            Canvas.SetLeft(newRec, posx * 20 + 90);
        }

        public void RemakeBorder()
        {
            Border newBorder = new Border();
            newBorder.Height = 500;
            newBorder.Width = 980;
            newBorder.BorderThickness = new Thickness(1);
            newBorder.BorderBrush = Brushes.White;
            Canvas.SetLeft(newBorder, 90);
            Canvas.SetTop(newBorder, 70);
            mainCanvas.Children.Add(newBorder);
        }
        private void Start_Stop_Click(object sender, RoutedEventArgs e)
        {
            buttonState *= -1;
            if(buttonState == 1)
            {
                Start_Stop.Content = "Stop";
                playGame();
                timer.Start();
            }
            if (buttonState == -1)
            {
                Start_Stop.Content = "Start";
                timer.Stop();
            }
        }

        //When the left mouse button was clicked inside the field, a cell will be created
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*
            Debugging: MessageBox.Show(text2 + " " + text3);
            string text2 = e.GetPosition(mainCanvas).X.ToString();
            string text3 = e.GetPosition(mainCanvas).Y.ToString();
            */

            //Max 49 columns, 26 rows
            col = (int)(e.GetPosition(mainCanvas).X - 90) / 20;
            row = (int)(e.GetPosition(mainCanvas).Y - 70) / 20;

            if(col > 48 || col < 0 || row > 24 || row < 0)
            {
                //EMPTY!
            }
            else
            {
                createCell(col, row);
                Field[col, row] = true;
                //Debugging: MessageBox.Show("col: " + col.ToString() + " row: " + row.ToString());
            }

        }
    }
}
