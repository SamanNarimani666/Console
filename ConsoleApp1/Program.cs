FileStream fs = new FileStream("Test.txt", FileMode.Create);
// First, save the standard output.
StreamWriter sw = new StreamWriter(fs);
Console.SetOut(sw);
string input = Console.ReadLine();
Console.WriteLine(input);
TextWriter tmp = Console.Out;

Console.SetOut(tmp);
sw.Close();