using System.Text;

string filePath = @"C:\Users\Saman\Desktop\ConsoleApp3\ConsoleApp6\bin\Debug\net7.0\foo.txt";
Console.WriteLine("enter search experssion :");
string s = Console.ReadLine();
Search(s, filePath);

void Search(string q, string filePath)
{
    string line;

	using (StreamReader sr = new StreamReader(filePath))
	{
		StringBuilder stringBuilder = new StringBuilder();

		while ((line = sr.ReadLine()) != null)
		{
			string[] words = line.Split(new char[] { ' ', '\t' },StringSplitOptions.None);

			foreach (string word in words) 
			{
				if(word.StartsWith(q))
					stringBuilder.Append($"\x1b[32m{word}\x1b[0m"+" ");
				else
                    stringBuilder.Append(word+" ");
            }
			stringBuilder.AppendLine();
        }

        Console.WriteLine(stringBuilder.ToString());
    }
}