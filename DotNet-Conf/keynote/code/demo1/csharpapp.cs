
// i want to read off args
if (args.Length > 0)
{
    Console.WriteLine("Arguments passed:");
    foreach (var arg in args)
    {
        Console.WriteLine(arg);
    }
}
else
{
    Console.WriteLine("No arguments passed."); 
}
