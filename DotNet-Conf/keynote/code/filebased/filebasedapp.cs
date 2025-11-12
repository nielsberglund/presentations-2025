#!/usr/local/share/dotnet/dotnet run

if (args.Length > 0)
{
    string message = string.Join(" ", args);
    Console.WriteLine(message);
}
else
{
    Console.WriteLine("Hello from Filebased apps.");
}