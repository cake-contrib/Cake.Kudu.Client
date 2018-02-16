
Task("Cake.Kudu.Client.Extensions.KuduClientSettingsExtensions.SettingsGet")
    .Test( ()=>
{
    // Given
    var expect = new Dictionary<string, string> {
        { "deployment_branch",  "master"        },
        { "SCM_GIT_USERNAME",   "windowsazure"  },
        { "SCM_GIT_EMAIL",      "windowsazure"  },
        { "WEBSITE_SITE_NAME",  "KuduClient"    }
    };

    // When
    var result = kuduClient.SettingsGet();

    // Then
    Assert.Equal(expect, result);
});


Task("Cake.Kudu.Client.Extensions.KuduClientSettingsExtensions.SettingsSet")
    .Test( ()=>
{
    // Given
    var settings = new Dictionary<string, string> {
        { "FOO",   "bar"  }
    };

    // When
    kuduClient.SettingsSet(settings);
});


Task("Cake.Kudu.Client.Extensions.KuduClientSettingsExtensions.SettingsDelete")
    .Test( ()=>
{
    // Given
    var setting = "FOO";

    // When
    kuduClient.SettingsDelete(setting);
});