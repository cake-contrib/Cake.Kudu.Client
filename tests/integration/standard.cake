#addin nuget:?package=Microsoft.AspNetCore.TestHost&version=2.0.0&loaddependencies=true
#addin nuget:?package=Microsoft.AspNetCore.Http.Extensions&version=2.0.0&loaddependencies=true

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

Information("Hello Mark");

new ConfigurationBuilder().Build();