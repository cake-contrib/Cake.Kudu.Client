Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipDownloadFile")
    .Test( ()=>
{
    // Given
    DirectoryPath remotePath = "/site/wwwroot/";
    FilePath localPath = "/ZipDownloadFile.zip";

    // When
    kuduClient.ZipDownloadFile(remotePath, localPath);
    var result = fakeFileSystem.GetFile(localPath).OpenRead().ReadAllBytes();

    // Then
    Assert.Equal(expectZipFile, result);
});

Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipDownloadStream")
    .Test( ()=>
{
    // Given
    DirectoryPath remotePath = "/site/wwwroot/";

    // When
    var result = kuduClient.ZipDownloadStream(remotePath).ReadAllBytes();

    // Then
    Assert.Equal(expectZipFile, result);
});

Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipUploadStream")
    .Test( ()=>
{
    // Given
    Stream stream = kuduClient.FileSystem.GetFile("/testfile.zip").OpenRead();
    DirectoryPath remotePath = "/site/wwwroot/ZipUploadStream/";

    // When
    kuduClient.ZipUploadStream(stream, remotePath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipUploadFile")
    .Test( ()=>
{
    // Given
    FilePath localPath = "/testfile.zip";
    DirectoryPath remotePath = "/site/wwwroot/ZipUploadFile/";

    // When
    kuduClient.ZipUploadFile(localPath, remotePath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipUploadDirectory")
    .Test( ()=>
{
    // Given
    DirectoryPath localPath = "/testfolder/";
    DirectoryPath remotePath = "/site/wwwroot/ZipUploadDirectory/";

    // When
    kuduClient.ZipUploadDirectory(localPath, remotePath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipDeployStream")
    .Test( ()=>
{
    // Given
    Stream stream = kuduClient.FileSystem.GetFile("/testfile.zip").OpenRead();

    // When
    kuduClient.ZipDeployStream(stream);
});

Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipDeployFile")
    .Test( ()=>
{
    // Given
    FilePath localPath = "/testfile.zip";

    // When
    kuduClient.ZipDeployFile(localPath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipDeployDirectory")
    .Test( ()=>
{
    // Given
    DirectoryPath localPath = "/testfolder/";

    // When
    kuduClient.ZipDeployDirectory(localPath);
});

Task("Cake.Kudu.Client.Extensions.KuduClientZipExtensions.ZipRunFromDirectory")
    .Test( ()=>
{
    // Given
    DirectoryPath localPath = "/testfolder/";
    bool skipPostDeploymentValidation = false;
    string  relativeValidateUrl = "/CustomVersion.txt",
            expectedValidateValue = @"deployments
locks
wwwroot";

    // When
    kuduClient.ZipRunFromDirectory(
        localPath,
        skipPostDeploymentValidation,
        relativeValidateUrl,
        expectedValidateValue);
});