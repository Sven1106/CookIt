using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RandomObjects
{
    public static class RandomTools
    {
        // The Random object this method uses.
        private static Random Rand = null;

        // Return num_items random values.
        public static List<T> PickRandom<T>(
            this T[] values, int num_values)
        {
            // Create the Random object if it doesn't exist.
            if (Rand == null) Rand = new Random();

            // Don't exceed the array's length.
            if (num_values >= values.Length)
                num_values = values.Length - 1;

            // Make an array of indexes 0 through values.Length - 1.
            int[] indexes =
                Enumerable.Range(0, values.Length).ToArray();

            // Build the return list.
            List<T> results = new List<T>();

            // Randomize the first num_values indexes.
            for (int i = 0; i < num_values; i++)
            {
                // Pick a random entry between i and values.Length - 1.
                int j = Rand.Next(i, values.Length);

                // Swap the values.
                int temp = indexes[i];
                indexes[i] = indexes[j];
                indexes[j] = temp;

                // Save the ith value.
                results.Add(values[indexes[i]]);
            }

            // Return the selected items.
            return results;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int randomElementCount = 100;
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            string solutionRootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\"));
            foreach (string fullJsonFileName in Directory.EnumerateFiles(Path.Combine(solutionRootPath, "Hosts")))
            {
                var json = File.ReadAllText(fullJsonFileName);
                var createRecipeDto = JsonConvert.DeserializeObject<CreateRecipeDto>(json);
                var randomItems = RandomTools.PickRandom(createRecipeDto.Task.AllRecipes.ToArray(), randomElementCount).OrderBy(x => x.Recipe.Heading).ToList();
                createRecipeDto.Task.AllRecipes = randomItems;
                string fileName = Path.GetFileName(fullJsonFileName);
                string newFileName = randomElementCount + "_random_" + fileName;
                string outputFolder = Path.Combine(fullJsonFileName.Replace(fileName, ""), "output");
                Directory.CreateDirectory(outputFolder);
                File.WriteAllText(Path.Combine(outputFolder, newFileName), JsonConvert.SerializeObject(createRecipeDto, settings));

            }

            Console.WriteLine("Hello World!");
        }



        public partial class CreateRecipeDto
        {
            public string ProjectName { get; set; }
            public Uri StartUrl { get; set; }
            public TaskDTO Task { get; set; }
        }
        public partial class TaskDTO
        {
            public List<AllRecipesDTO> AllRecipes { get; set; }
        }
        public partial class AllRecipesDTO
        {
            public RecipeDTO Recipe { get; set; }
            public MetadataDTO Metadata { get; set; }
        }
        public partial class RecipeDTO
        {
            public string Heading { get; set; }
            public List<string> Ingredients { get; set; }
            public ImageDTO Image { get; set; }
        }
        public partial class ImageDTO
        {
            public Uri Src { get; set; }
            public string Alt { get; set; }
        }
        public partial class MetadataDTO
        {
            public Uri FoundAtUrl { get; set; }
            public DateTimeOffset DateFound { get; set; }
        }
    }
}
