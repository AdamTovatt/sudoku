using System;
using System.IO;

public class Int3DArrayLoader
{
   public static int[,,] LoadArray(string fileName)
   {
      string path = "/home/jonataneckeskog/lth/ups/sudoku/Sudoku/Solvers/associated.bin";

      if (!File.Exists(path))
      {
         throw new FileNotFoundException($"The file '{fileName}' was not found in the application directory.");
      }

      using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
      {
         // Read dimensions
         int x = reader.ReadInt32();
         int y = reader.ReadInt32();
         int z = reader.ReadInt32();

         int[,,] array = new int[x, y, z];

         // Read the array data
         for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
               for (int k = 0; k < z; k++)
                  array[i, j, k] = reader.ReadInt32();

         return array;
      }
   }
}