using SelfDiagnosis;
using System;
using System.Configuration;

namespace EchoConsole
{
    class Smoke : SmokeBase
    {
        public override bool ValidateAppSetting(object item)
        {
            switch (item.ToString())
            {
                case "ApplicationEnvironment":
                    {
                        if (ConfigurationManager.AppSettings.Get(item.ToString().ToLower()).Contains("local"))
                        {
                            return true;
                        }
                        return false;
                    }
                default:
                    return false;
            }
       }

        public override bool ValidateConfigSetting(object item)
        {
            switch (item.ToString())
            {
                default:
                    //Just return true, doesnt matter for the workshop
                    return true;
            }
        }

        public override void ValidateOther()
        {
            if (ConfigurationManager.AppSettings.Count >= 1)
                AddSucces(nameof(ConfigurationManager.AppSettings) + ":Number of");
            else
                AddFailure(nameof(ConfigurationManager.AppSettings) + ":Number of", "Incorrect number of configurations applied");
        }
    }
}
