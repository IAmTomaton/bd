using System;
using DB.Shell;

var (sender, inputBuffer, cancellationToken) = DbShellFactory.Create(args);

await sender.PingAsync().ConfigureAwait(false);

while (!cancellationToken.IsCancellationRequested)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (inputBuffer.TryGetCommands(line, out var commands))
    {
        foreach (var command in commands)
        {
            var result = await sender.SendAsync(command).ConfigureAwait(false);
            Console.WriteLine(result);
        }
    }
}