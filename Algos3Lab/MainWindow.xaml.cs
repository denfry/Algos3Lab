using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Algos3Lab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int vertexCount = 15;
        private ObservableCollection<MatrixRow> AdjacencyMatrix { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            InitializeAdjacencyMatrix();
            InitializeIncidenceMatrix();
        }

        private void InitializeAdjacencyMatrix()
        {
            AdjacencyMatrix = new ObservableCollection<MatrixRow>();

            for (int i = 0; i < vertexCount; i++)
            {
                AdjacencyMatrix.Add(new MatrixRow(vertexCount));
            }

            AdjacencyMatrixGrid.ItemsSource = AdjacencyMatrix;

            // Добавляем столбец для номеров вершин
            var headerColumn = new DataGridTextColumn
            {
                Header = "V",
                Binding = new Binding("VertexName"),
                Width = 40
            };
            AdjacencyMatrixGrid.Columns.Add(headerColumn);

            for (int i = 0; i < vertexCount; i++)
            {
                var column = new DataGridTextColumn
                {
                    Header = $"V{i + 1}",
                    Binding = new Binding($"Values[{i}]")
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidatesOnDataErrors = true,
                        ValidatesOnExceptions = true,
                        ValidationRules = { new BinaryValueValidationRule() }
                    },
                    Width = 40
                };
                AdjacencyMatrixGrid.Columns.Add(column);
            }
        }

        private void InitializeIncidenceMatrix()
        {
            IncidenceMatrixGrid.Columns.Clear();

            // Добавляем столбец для номеров рёбер
            var edgeColumn = new DataGridTextColumn
            {
                Header = "Ребро",
                Binding = new Binding("EdgeName"),
                Width = 40
            };
            IncidenceMatrixGrid.Columns.Add(edgeColumn);

            // Добавляем столбцы для вершин
            for (int i = 0; i < vertexCount; i++)
            {
                var column = new DataGridTextColumn
                {
                    Header = $"V{i + 1}",
                    Binding = new Binding($"Values[{i}]"),
                    Width = 40
                };
                IncidenceMatrixGrid.Columns.Add(column);
            }
        }



        private bool ValidateAdjacencyMatrix()
        {
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    if (AdjacencyMatrix[i].Values[j] < 0 || AdjacencyMatrix[i].Values[j] > 1)
                    {
                        MessageBox.Show($"Элемент [{i + 1}, {j + 1}] должен быть 0 или 1.", "Ошибка");
                        return false;
                    }

                    if (AdjacencyMatrix[i].Values[j] != AdjacencyMatrix[j].Values[i])
                    {
                        MessageBox.Show($"Матрица должна быть симметричной. Ошибка в элементах [{i + 1}, {j + 1}] и [{j + 1}, {i + 1}].", "Ошибка");
                        return false;
                    }
                }
            }

            MessageBox.Show("Матрица корректна!", "Успех");
            return true;
        }

        private bool ValidateIncidenceMatrix(int[,] adjacencyMatrix, ObservableCollection<MatrixRow> incidenceMatrix)
        {
            int vertexCount = adjacencyMatrix.GetLength(0);

            int edgeCount = 0;
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = i + 1; j < vertexCount; j++)
                {
                    if (adjacencyMatrix[i, j] == 1)
                        edgeCount++;
                }
            }

            if (incidenceMatrix.Count != edgeCount)
            {
                MessageBox.Show("Количество рёбер в матрице инцидентности не совпадает с количеством рёбер в матрице смежности.", "Ошибка");
                return false;
            }

            for (int i = 0; i < incidenceMatrix.Count; i++)
            {
                var row = incidenceMatrix[i];
                int count = row.Values.Count(v => v == 1);

                if (count != 2) 
                {
                    MessageBox.Show($"Строка {i + 1} в матрице инцидентности содержит неверное количество единиц. Ожидается 2.", "Ошибка");
                    return false;
                }
            }

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    if (adjacencyMatrix[i, j] == 1)
                    {
                        bool found = false;
                        for (int row = 0; row < incidenceMatrix.Count; row++)
                        {
                            var rowData = incidenceMatrix[row].Values;
                            if ((rowData[i] == 1 && rowData[j] == 1)) 
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            MessageBox.Show($"Ошибка инцидентности: между вершинами {i + 1} и {j + 1} должно быть ребро.", "Ошибка");
                            return false;
                        }
                    }
                }
            }

            MessageBox.Show("Матрица инцидентности корректна!", "Успех");
            return true;
        }





        private void CalculateIncidenceMatrix()
        {
            if (!ValidateAdjacencyMatrix())
                return;

            var edges = new List<(int, int)>();
            // Collect edges from adjacency matrix
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = i + 1; j < vertexCount; j++)
                {
                    if (AdjacencyMatrix[i].Values[j] == 1)
                    {
                        edges.Add((i, j)); // Edge between vertex i and j
                    }
                }
            }

            // Check for zero edges
            if (edges.Count == 0)
            {
                MessageBox.Show("No edges in the graph.", "Error");
                return;
            }

            // Generate columns for edges in the DataGrid
            IncidenceMatrixGrid.Columns.Clear();
            IncidenceMatrixGrid.ItemsSource = null;

            for (int i = 0; i < edges.Count; i++)
            {
                IncidenceMatrixGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"E{i + 1}",
                    Binding = new Binding($"Values[{i}]"),
                    Width = 40
                });
            }

            // Generate rows for vertices (vertex-edge relationships)
            var incidenceMatrix = new ObservableCollection<MatrixRow>();
            for (int i = 0; i < vertexCount; i++)
            {
                var row = new MatrixRow(edges.Count);
                for (int j = 0; j < edges.Count; j++)
                {
                    if (edges[j].Item1 == i || edges[j].Item2 == i)
                        row.Values[j] = 1;
                }
                incidenceMatrix.Add(row);
            }

            IncidenceMatrixGrid.ItemsSource = incidenceMatrix;

            // Debug output
            Console.WriteLine("Edges:");
            foreach (var edge in edges)
            {
                Console.WriteLine($"E{(edges.IndexOf(edge) + 1)}: ({edge.Item1}, {edge.Item2})");
            }

            Console.WriteLine("Incidence Matrix:");
            for (int i = 0; i < incidenceMatrix.Count; i++)
            {
                Console.Write($"Vertex {i}: ");
                foreach (var value in incidenceMatrix[i].Values)
                {
                    Console.Write($"{value} ");
                }
                Console.WriteLine();
            }
        }





        private void DrawGraph()
        {
            GraphCanvas.Children.Clear();
            double centerX = GraphCanvas.ActualWidth / 2;
            double centerY = GraphCanvas.ActualHeight / 2;
            double radius = Math.Min(centerX, centerY) - 20;

            var vertices = new Point[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                double angle = 2 * Math.PI * i / vertexCount;
                vertices[i] = new Point(centerX + radius * Math.Cos(angle), centerY + radius * Math.Sin(angle));

                GraphCanvas.Children.Add(new Ellipse
                {
                    Fill = Brushes.Black,
                    Width = 10,
                    Height = 10,
                    Margin = new Thickness(vertices[i].X - 5, vertices[i].Y - 5, 0, 0)
                });

                GraphCanvas.Children.Add(new TextBlock
                {
                    Text = $"V{i + 1}",
                    Foreground = Brushes.Red,
                    Margin = new Thickness(vertices[i].X + 5, vertices[i].Y + 5, 0, 0)
                });
            }

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = i + 1; j < vertexCount; j++)
                {
                    if (AdjacencyMatrix[i].Values[j] == 1)
                    {
                        var line = new Line
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = vertices[i].X,
                            Y1 = vertices[i].Y,
                            X2 = vertices[j].X,
                            Y2 = vertices[j].Y
                        };
                        GraphCanvas.Children.Add(line);
                    }
                }
            }
        }


        private void CheckMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateAdjacencyMatrix();
        }

        private void CalculateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateIncidenceMatrix();
        }

        private void DrawGraphButton_Click(object sender, RoutedEventArgs e)
        {
            DrawGraph();
        }



        private void GenerateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = i + 1; j < vertexCount; j++) 
                {
                    int value = (i == j) ? 0 : random.Next(0, 2); 
                    AdjacencyMatrix[i].Values[j] = value;
                    AdjacencyMatrix[j].Values[i] = value; 
                }
            }

            StringBuilder matrixString = new StringBuilder();
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    matrixString.Append(AdjacencyMatrix[i].Values[j] + " ");
                }
                matrixString.AppendLine();
            }
            Console.WriteLine(matrixString.ToString());

            AdjacencyMatrixGrid.Items.Refresh();
            MessageBox.Show("Матрица успешно сгенерирована!", "Успех");
        }



        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    AdjacencyMatrix[i].Values[j] = 0;
                }
            }

            AdjacencyMatrixGrid.Items.Refresh();

            IncidenceMatrixGrid.ItemsSource = null;

            GraphCanvas.Children.Clear();

            MessageBox.Show("Всё было сброшено!", "Сброс");
        }

        private void CheckMatrixIncidenceButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] adjacencyMatrix = new int[vertexCount, vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    adjacencyMatrix[i, j] = AdjacencyMatrix[i].Values[j];
                }
            }

            ObservableCollection<MatrixRow> incidenceMatrix = new ObservableCollection<MatrixRow>();
            foreach (MatrixRow row in IncidenceMatrixGrid.Items)
            {
                incidenceMatrix.Add(row);
            }

            bool isValid = ValidateIncidenceMatrix(adjacencyMatrix, incidenceMatrix);

            if (isValid)
            {
                MessageBox.Show("Матрица инцидентности корректна!", "Успех");
            }
            else
            {
                MessageBox.Show("Матрица инцидентности некорректна.", "Ошибка");
            }
        }


    }

}
