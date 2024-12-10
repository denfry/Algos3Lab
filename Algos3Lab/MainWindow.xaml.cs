using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Algos3Lab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int vertexCount = 15;
        protected internal ObservableCollection<MatrixRow> AdjacencyMatrix { get; set; }

        public int edgeCount { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeAdjacencyMatrix();
            InitializeIncidenceMatrix(edgeCount);
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
        private void InitializeIncidenceMatrix(int edgeCount)
        {
            DataTable incidenceTable = new DataTable();
            incidenceTable.Columns.Add("Vertex", typeof(string));

            for (int edgeIndex = 0; edgeIndex < edgeCount; edgeIndex++)
            {
                incidenceTable.Columns.Add($"Edge {edgeIndex + 1}", typeof(int));
            }

            for (int vertex = 0; vertex < vertexCount; vertex++)
            {
                var row = new object[edgeCount + 1];
                row[0] = $"Vertex {vertex + 1}";
                for (int col = 1; col <= edgeCount; col++)
                {
                    row[col] = 0;
                }
                incidenceTable.Rows.Add(row);
            }

            IncidenceMatrixGrid.ItemsSource = incidenceTable.DefaultView;
            IncidenceMatrixGrid.Columns.Clear();

            foreach (DataColumn column in incidenceTable.Columns)
            {
                var gridColumn = new DataGridTextColumn
                {
                    Header = column.ColumnName,
                    Binding = new Binding($"[{column.ColumnName}]"),
                    Width = 80
                };
                IncidenceMatrixGrid.Columns.Add(gridColumn);
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

        private bool ValidateIncidenceMatrix(int[,] adjacencyMatrix, int[,] incidenceMatrix)
        {
            int vertexCount = adjacencyMatrix.GetLength(0);
            int edgeCount = incidenceMatrix.GetLength(1);

            for (int edgeIndex = 0; edgeIndex < edgeCount; edgeIndex++)
            {
                int onesCount = 0;
                for (int vertex = 0; vertex < vertexCount; vertex++)
                {
                    if (incidenceMatrix[vertex, edgeIndex] == 1)
                        onesCount++;
                }
                if (onesCount != 2)
                {
                    MessageBox.Show($"Edge {edgeIndex + 1} must connect exactly 2 vertices.", "Error");
                    return false;
                }
            }

            int[,] reconstructedAdjacency = new int[vertexCount, vertexCount];
            for (int edgeIndex = 0; edgeIndex < edgeCount; edgeIndex++)
            {
                int[] connectedVertices = new int[2];
                int vertexIndex = 0;

                for (int vertex = 0; vertex < vertexCount; vertex++)
                {
                    if (incidenceMatrix[vertex, edgeIndex] == 1)
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
                        MessageBox.Show($"Adjacency matrix mismatch at [{i},{j}].", "Error");
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
                        edges.Add((i, j));
                    }
                }
            }

            if (edges.Count == 0)
            {
                MessageBox.Show("No edges found in the graph.", "Error");
                return;
            }

            var incidenceTable = new DataTable();
            incidenceTable.Columns.Add("Vertex", typeof(string));
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                incidenceTable.Columns.Add($"Edge {edgeIndex + 1}", typeof(int));
            }

            for (int i = 0; i < vertexCount; i++)
            {
                var row = new object[edges.Count + 1];
                row[0] = $"Vertex {i + 1}";
                for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
                {
                    var (u, v) = edges[edgeIndex];
                    row[edgeIndex + 1] = (i == u || i == v) ? 1 : 0;
                }
                incidenceTable.Rows.Add(row);
            }

            IncidenceMatrixGrid.ItemsSource = incidenceTable.DefaultView;
            IncidenceMatrixGrid.Columns.Clear();

            foreach (DataColumn column in incidenceTable.Columns)
            {
                var gridColumn = new DataGridTextColumn
                {
                    Header = column.ColumnName,
                    Binding = new Binding($"[{column.ColumnName}]"),
                    Width = 80
                };
                IncidenceMatrixGrid.Columns.Add(gridColumn);
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

            DataTable incidenceTable = ((DataView)IncidenceMatrixGrid.ItemsSource).Table;
            int edgeCount = incidenceTable.Columns.Count - 1;
            int[,] incidenceMatrix = new int[vertexCount, edgeCount];

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < edgeCount; j++)
                {
                    incidenceMatrix[i, j] = Convert.ToInt32(incidenceTable.Rows[i][j + 1]);
                }
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
