using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        private ObservableCollection<MatrixRow> IncidenceMatrix;

        public int edgeCount { get; set; }



        public MainWindow()
        {
            InitializeComponent();
            InitializeAdjacencyMatrix();
            IncidenceMatrix = new ObservableCollection<MatrixRow>();
        }

        private void InitializeAdjacencyMatrix()
        {
            AdjacencyMatrix = new ObservableCollection<MatrixRow>();

            for (int i = 0; i < vertexCount; i++)
            {
                AdjacencyMatrix.Add(new MatrixRow(vertexCount, $"Ребро {i + 1}"));
            }

            AdjacencyMatrixGrid.ItemsSource = AdjacencyMatrix;

            AdjacencyMatrixGrid.Columns.Clear();

            var edgeColumn = new DataGridTextColumn
            {
                Binding = new Binding("EdgeName"),
                Width = 100
            };
            AdjacencyMatrixGrid.Columns.Add(edgeColumn);

            for (int i = 0; i < vertexCount; i++)
            {
                var column = new DataGridTextColumn
                {
                    Header = $"Vertex {i + 1}",
                    Binding = new Binding($"Values[{i}].Value")
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidationRules = { new BinaryValueValidationRule() }
                    },
                    Width = 80
                };
                AdjacencyMatrixGrid.Columns.Add(column);
            }
        }




        private bool ValidateAdjacencyMatrix()
        {
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    if (AdjacencyMatrix[i].Values[j].Value < 0 || AdjacencyMatrix[i].Values[j].Value > 1)
                    {
                        MessageBox.Show($"Элемент [{i + 1}, {j + 1}] должен быть 0 или 1.", "Ошибка");
                        return false;
                    }

                    if (AdjacencyMatrix[i].Values[j].Value != AdjacencyMatrix[j].Values[i].Value)
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
            int edgeCount = incidenceMatrix[0].Values.Count;

            if (incidenceMatrix.Count != vertexCount)
            {
                MessageBox.Show("Number of rows in incidence matrix should equal vertex count.", "Error");
                return false;
            }

            for (int edgeIndex = 0; edgeIndex < edgeCount; edgeIndex++)
            {
                int onesCount = 0;
                for (int vertex = 0; vertex < vertexCount; vertex++)
                {
                    if (incidenceMatrix[vertex].Values[edgeIndex].Value == 1)
                        onesCount++;
                }
                if (onesCount != 2)
                {
                    MessageBox.Show($"Edge {edgeIndex + 1} is connected to {onesCount} vertices, should be 2.", "Error");
                    return false;
                }
            }

            int[,] reconstructedAdjacency = new int[vertexCount, vertexCount];
            for (int edgeIndex = 0; edgeCount > edgeIndex; edgeIndex++)
            {
                int[] connectedVertices = new int[2];
                int vertexIndex = 0;
                for (int vertex = 0; vertex < vertexCount; vertex++)
                {
                    if (incidenceMatrix[vertex].Values[edgeIndex].Value == 1)
                    {
                        connectedVertices[vertexIndex] = vertex;
                        vertexIndex++;
                        if (vertexIndex == 2)
                            break;
                    }
                }
                reconstructedAdjacency[connectedVertices[0], connectedVertices[1]] = 1;
                reconstructedAdjacency[connectedVertices[1], connectedVertices[0]] = 1;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    if (adjacencyMatrix[i, j] != reconstructedAdjacency[i, j])
                    {
                        MessageBox.Show($"Adjacency matrix does not match incidence matrix at [{i},{j}].", "Error");
                        return false;
                    }
                }
            }

            MessageBox.Show("Incidence matrix is valid!", "Success");
            return true;
        }

        private void CalculateIncidenceMatrix()
        {
            if (!ValidateAdjacencyMatrix())
                return;

            var edges = new List<(int, int)>();
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = i + 1; j < vertexCount; j++)
                {
                    if (AdjacencyMatrix[i].Values[j].Value == 1)
                    {
                        edges.Add((i + 1, j + 1));
                    }
                }
            }

            if (edges.Count == 0)
            {
                MessageBox.Show("No edges in the graph.", "Error");
                return;
            }

            DataTable incidenceTable = new DataTable();
            incidenceTable.Columns.Add("Вершины", typeof(string));
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                incidenceTable.Columns.Add($"Ребро {edgeIndex + 1}", typeof(int));
            }

            for (int vertex = 1; vertex <= vertexCount; vertex++)
            {
                object[] rowValues = new object[edges.Count + 1];
                rowValues[0] = $"Вершина {vertex}";
                for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
                {
                    rowValues[edgeIndex + 1] = (edges[edgeIndex].Item1 == vertex || edges[edgeIndex].Item2 == vertex) ? 1 : 0;
                }
                incidenceTable.Rows.Add(rowValues);
            }

            IncidenceMatrixGrid.ItemsSource = incidenceTable.DefaultView;

            IncidenceMatrixGrid.Columns.Clear();

            foreach (DataColumn column in incidenceTable.Columns)
            {
                var dataGridTextColumn = new DataGridTextColumn
                {
                    Header = column.ColumnName,
                    Binding = new Binding(column.ColumnName),
                    Width = 80
                };
                IncidenceMatrixGrid.Columns.Add(dataGridTextColumn);
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
                    if (AdjacencyMatrix[i].Values[j].Value == 1)
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
                    AdjacencyMatrix[i].Values[j].Value = value;
                    AdjacencyMatrix[j].Values[i].Value = value; 
                }
            }

            StringBuilder matrixString = new StringBuilder();
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    matrixString.Append(AdjacencyMatrix[i].Values[j].Value + " ");
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
                    AdjacencyMatrix[i].Values[j].Value = 0;
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
                    adjacencyMatrix[i, j] = AdjacencyMatrix[i].Values[j].Value;
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
