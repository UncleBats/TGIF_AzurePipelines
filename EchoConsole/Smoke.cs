using SelfDiagnosis;
using System;

namespace EchoConsole
{
    class Smoke : SmokeBase
    {
        public override bool ValidateAppSetting(object item)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateConfigSetting(object item)
        {
            throw new NotImplementedException();
        }

        public override void ValidateOther()
        {
            throw new NotImplementedException();
        }
    }
}
