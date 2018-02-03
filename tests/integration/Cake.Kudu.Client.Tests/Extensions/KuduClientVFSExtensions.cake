Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSList")
    .Test( ()=>
{
    // Given
    DirectoryPath remotePath = "SystemDrive/Program%20Files%20(x86)/SiteExtensions/Kudu/";

    // When
    var result = kuduClient.VFSList(remotePath);

    // Then
    Assert.Equal(remotePath, result?.Path);
    Assert.Equal(6, result?.Entries.Count);
    Assert.Equal(3, result?.Directories.Count);
    Assert.Equal(3, result?.Files.Count);
    Assert.Equal("69.61204.3166", result.Entries.First().Name);
    Assert.Equal("69.61204.3166", result.Directories.First().Name);
    Assert.Equal("69.61204.3166.installed", result.Files.First().Name);
    Assert.Equal(
        new []{
            636506687150363902L,
            636506687259776283L,
            636507781264604385L,
            636507781368511372L,
            636523253281083638L,
            636523253385937422L
        },
        result.Entries.Select(entry => entry.Created.UtcTicks).ToArray()
    );
    Assert.Equal(
        new []{
            636506687259113729L,
            636506687259894894L,
            636507781367886346L,
            636507781368511372L,
            636523253384991289L,
            636523253385937422L
        },
        result.Entries.Select(entry => entry.Modified.UtcTicks).ToArray()
    );
    Assert.Equal(
        new []{
            0L,
            6L,
            0L,
            6L,
            0L,
            6L
        },
        result.Entries.Select(entry => entry.Size).ToArray()
    );
});

Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSDownloadString")
    .Test( ()=>
{
    // Given
    FilePath remotePath = "/SystemDrive/Program Files (x86)/SiteExtensions/Kudu/69.61204.3166.installed";

    // When
    var result = kuduClient.VFSDownloadString(remotePath);

    // Then
    Assert.Equal("done", result);
});

Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSDownloadFile")
    .Test( ()=>
{
    // Given
    FilePath remotePath = "/SystemDrive/Program Files (x86)/SiteExtensions/Kudu/69.61204.3166.installed";
    FilePath localPath = "/69.61204.3166.installed";

    // When
    kuduClient.VFSDownloadFile(remotePath, localPath);
    var result = fakeFileSystem.GetFile(localPath).GetTextContent();

    // Then
    Assert.Equal("done", result);
});

Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSDownloadStream")
    .Test( ()=>
{
    // Given
    FilePath remotePath = "/SystemDrive/Program Files (x86)/SiteExtensions/Kudu/69.61204.3166.installed";

    // When
    string result;
    using(var resultStream = kuduClient.VFSDownloadStream(remotePath))
    {
        using(var reader = new StreamReader(resultStream, Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }
    }

    // Then
    Assert.Equal("done", result);
});

Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSUploadStream")
    .Test( ()=>
{
    // Given
    Stream stream = kuduClient.FileSystem.GetFile("/testfile.txt").OpenRead();
    FilePath remotePath = "/site/wwwroot/VFSUploadStream.txt";

    // When
    kuduClient.VFSUploadStream(stream, remotePath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSUploadFile")
    .Test( ()=>
{
    // Given
    FilePath localPath = "/testfile.txt";
    FilePath remotePath = "/site/wwwroot/VFSUploadFile.txt";

    // When
    kuduClient.VFSUploadFile(localPath, remotePath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSUploadString")
    .Test( ()=>
{
    // Given
    string sourceString = "The MIT License (MIT)";
    FilePath remotePath = "/site/wwwroot/VFSUploadString.txt";

    // When
    kuduClient.VFSUploadString(sourceString, remotePath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSCreateDirectory")
    .Test( ()=>
{
    // Given
    DirectoryPath remotePath = "/site/wwwroot/images/";

    // When
    kuduClient.VFSCreateDirectory(remotePath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientVFSExtensions.VFSDelete")
    .Test( ()=>
{
    // Given
    FilePath remotePath = "/site/wwwroot/LICENSE";

    // When
    kuduClient.VFSDelete(remotePath);
});
