
Task("Cake.Kudu.Client.Extensions.KuduClientCommandExtensions.ExecuteCommand")
    .Test( ()=>
{
    // Given
    string  command     = "ls",
            directory   = "%home%",
            argument    = "site";

    // When
    var result = kuduClient.ExecuteCommand(command, directory, argument);

    // Then
    Assert.Equal("deployments\r\nlocks\r\nwwwroot\r\n", result?.Output);
    Assert.Equal(string.Empty, result?.Error);
    Assert.Equal(0, result?.ExitCode);
});