

string[] lines = new string[] { "saman", "", "narimani" };

Console.Write("enter the index :");
int index = Convert.ToInt32(Console.ReadLine());


if(index>=0 && index<lines.Length)
{
	for (int i = index; i < lines.Length-1; i++)
	{
		lines[i] = lines[i + 1];
	}

	Array.Resize(ref lines, lines.Length-1);
}

var result = lines;

foreach (var item in lines)
{
    Console.WriteLine(item);
}