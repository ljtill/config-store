using ConfigStore.Clients;
using ConfigStore.Files;
using ConfigStore.Items;

namespace ConfigStore.Commands;

public static class ImportCommand
{
    public static Command Create()
    {
        // Command
        var command = new Command("import", "");

        // Options
        var accountNameOption = new Option<string>("--account-name");
        accountNameOption.AddAlias("-n");
        command.AddOption(accountNameOption);

        var accountKeyOption = new Option<string>("--account-key");
        accountKeyOption.AddAlias("-k");
        command.AddOption(accountKeyOption);

        var filePathOption = new Option<string>("--file-path")
        {
            IsRequired = true
        };
        filePathOption.AddAlias("-f");
        command.AddOption(filePathOption);

        // Handler
        command.SetHandler(async (string? accountName, string? accountKey, string filePath) =>
        {
            try
            {
                // Setup the database client
                var client = await DatabaseClient.CreateAsync(accountName, accountKey);

                // Parse the user provided file
                var items = ImportFile.Invoke(filePath);

                // Push items to cosmos db
                await ImportItems.InvokeAsync(client, items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application terminated");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception message - {0}", ex.Message);
            }
        }, accountNameOption, accountKeyOption, filePathOption);

        return command;
    }
}