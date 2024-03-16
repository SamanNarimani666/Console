string test = @"


man 

saman 
narimani



hkjk

kk




asdklkdl;sk;dk


1666";


Console.WriteLine(test);


Console.Clear();

FileStream fs = new FileStream("Test05.txt", FileMode.Create);
// First, save the standard output.
StreamWriter sw = new StreamWriter(fs);
Console.SetOut(sw);
Console.SetCursorPosition(60,44);
test += "kk";

Console.Write(test);



TextWriter tmp = Console.Out;

Console.SetOut(tmp);
sw.Close();

