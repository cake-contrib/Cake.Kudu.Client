# Cake Kudu Client

Cake addin that provides aliases for remotely communicating with the Azure App Service Kudu engine.

## Fetures

The addin lets you

* Execute remote shell commands
* Enumerate remote files and directories on
* Upload files and directories to AppService
* Download files and directories from AppService
* Deploy to AppService from local folder or zip file

## Usage

A Kudu client is obtained by using the `KuduClient` alias.

```cake
#addin nuget:?package=Cake.Kudu.Client

 string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");

 IKuduClient kuduClient = KuduClient(
     baseUri,
     userName,
     password);
```

Example deploying local folder
```cake
DirectoryPath sourceDirectoryPath = "./Documentation/";
DirectoryPath remoteDirectoryPath = "/site/wwwroot/docs/";

kuduClient.ZipUploadDirectory(
    sourceDirectoryPath,
    remoteDirectoryPath);
```

Full documentation available at [cakebuild.net/dsl/kudu/](https://cakebuild.net/dsl/kudu/).
