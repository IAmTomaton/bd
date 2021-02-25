using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fridge
{
    class FridgeProgram
    {
        private TextWriter _writer;

        public FridgeProgram(TextWriter writer)
        {
            _writer = writer;
        }

        public void RunAlgo(string input)
        {
            var lines = input.Split('\n');
            var shelvesCount = int.Parse(lines[0]);
            var shelves = new List<Shelve>();
            var productCounts = new Dictionary<int, int>();
            var index = 1;
            for (var i = 0; i < shelvesCount; i++)
            {
                index++;
                var size = lines[index].Split(' ');
                var shelve = new Shelve
                {
                    Height = int.Parse(size[0]),
                    Width = int.Parse(size[1])
                };
                shelves.Add(shelve);
                for (var j = 0; j < shelve.Height; j++)
                {
                    index++;
                    foreach (var product in lines[index].Split(' '))
                    {
                        if (product == "-")
                            continue;
                        var productNumber = int.Parse(product);
                        if (!productCounts.ContainsKey(productNumber))
                            productCounts.Add(productNumber, 0);
                        productCounts[productNumber]++;
                    }
                }
                index++;
            }

            var output = new StringBuilder();

            var sortedShelvesBySize = shelves.OrderBy(shelve => shelve.Size);
            foreach (var product in productCounts.OrderBy(pair => pair.Value))
            {
                var shelve = sortedShelvesBySize.First(shelve => shelve.Size >= product.Value && !shelve.Used);
                shelve.Used = true;
                shelve.Product = product.Key;
            }

            var sortedShelvesByProduct = shelves.OrderBy(shelve => shelve.Product);
            var first = true;
            foreach (var shelve in sortedShelvesByProduct)
            {
                if (!first)
                    output.Append('\n');
                first = false;
                for (var y = 0; y < shelve.Height; y++)
                {
                    for (var x = 0; x < shelve.Width; x++)
                    {
                        if (x != 0) output.Append(' ');
                        if (productCounts[shelve.Product] > 0)
                        {
                            output.Append(shelve.Product);
                            productCounts[shelve.Product]--;
                        }
                        else
                            output.Append('-');
                    }
                    output.Append('\n');
                }
            }

            _writer.Write(output.ToString());
        }

        private class Shelve
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public bool Used { get; set; }
            public int Product { get; set; }

            public int Size => Height * Width;
        }
    }
}