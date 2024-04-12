using System.Configuration;

namespace Rayer.FFmpegCore;

public class FFmpegConfigurationSection : ConfigurationSection
{
    [ConfigurationProperty("httpProxy", DefaultValue = "", IsRequired = false)]
    public string HttpProxy
    {
        get { return (string)this["httpProxy"]; }
        set { this["httpProxy"] = value; }
    }

    [ConfigurationProperty("proxyWhitelist", DefaultValue = "*", IsRequired = false)]
    public string ProxyWhitelist
    {
        get { return (string)this["proxyWhitelist"]; }
        set { this["proxyWhitelist"] = value; }
    }

    [ConfigurationProperty("loglevel", DefaultValue = null, IsRequired = false)]
    public LogLevel? LogLevel
    {
        get
        {
            var obj = this["loglevel"];
            return obj is LogLevel && Enum.IsDefined(typeof(LogLevel), obj) ? (LogLevel?)obj : null;
        }
        set { this["loglevel"] = value; }
    }
}