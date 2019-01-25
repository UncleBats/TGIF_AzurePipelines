using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace SelfDiagnosis
{
    public abstract class SmokeBase
    {
        private List<UnitTestResultType> results = new List<UnitTestResultType>();
        private List<UnitTestType> definitions = new List<UnitTestType>();
        private bool failed;

        public void Execute()
        {
            foreach (var item in ConfigurationManager.AppSettings)
            {
                try
                {
                    ValidateAppSetting(item);

                    AddSucces(nameof(ConfigurationManager.AppSettings) + ":" + item.ToString());
                }
                catch (Exception ex)
                {
                    AddExceptionFailure(nameof(ConfigurationManager.AppSettings) + ":" + item.ToString(), ex);
                }
            }

            foreach (var item in ConfigurationManager.ConnectionStrings)
            {
                try
                {
                    ValidateConfigSetting(item);
                    AddSucces(nameof(ConfigurationManager.ConnectionStrings) + ":" + ((ConnectionStringSettings)item).Name);
                }
                catch (Exception ex)
                {
                    AddExceptionFailure(nameof(ConfigurationManager.ConnectionStrings) + ":" + ((ConnectionStringSettings)item).Name, ex);
                }
            }

            ValidateOther();
            Testresult();
            if (failed)
                throw new SmokeTestException("A one or more smoketest(s) failed to compleet successfull");
        }

        protected void AddSucces(string name)
        {
            Add("Passed", name, null, null);
        }

        protected void AddFailure(string name, string message)
        {
            Add("Failed", name, message, null);
        }

        protected void AddExceptionFailure(string name, Exception exception)
        {
            Add("Failed", name, exception.Message, exception.StackTrace);
        }

        private void Add(string result, string name, string message, string stackTrace)
        {
            var testid = Guid.NewGuid().ToString();

            name = AppDomain.CurrentDomain.FriendlyName + ":" + name;

            var resultType = new UnitTestResultType
            {
                outcome = result,
                testName = name,
                testId = testid
            };

            if (result == "Failed")
            {
                failed = true;
                resultType.Items = new OutputType[] { new OutputType { ErrorInfo = new OutputTypeErrorInfo { Message = message, StackTrace = stackTrace } } };
            }

            results.Add(resultType);

            definitions.Add(new UnitTestType
            {
                name = name,
                id = testid
            });
        }
        public abstract bool ValidateAppSetting(object item);
        public abstract bool ValidateConfigSetting(object item);
        public abstract void ValidateOther();

        private void Testresult()
        {
            var testRun = new TestRunType();
            var resultContainer = new ResultsType();
            var definitionContainer = new TestDefinitionType();

            resultContainer.Items = results.ToArray();
            definitionContainer.Items = definitions.ToArray();

            resultContainer.ItemsElementName = new ItemsChoiceType3[results.Count];
            definitionContainer.ItemsElementName = new ItemsChoiceType4[results.Count];
            for (int i = 0; i < results.Count; i++)
            {
                resultContainer.ItemsElementName[i] = ItemsChoiceType3.UnitTestResult;
                definitionContainer.ItemsElementName[i] = ItemsChoiceType4.UnitTest;
            }
            testRun.Items = new List<object> { resultContainer, definitionContainer }.ToArray();

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "smoketestresult.trx");

            var x = new XmlSerializer(testRun.GetType());
            var xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("x", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            xmlnsEmpty.Add("x1", "http://www.w3.org/2001/XMLSchema-instance");
            xmlnsEmpty.Add("x2", "http://www.w3.org/2001/XMLSchema");
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                x.Serialize(fs, testRun, xmlnsEmpty);
                fs.Flush();
            }

            //Replace namespaces because on the publish test results to TFS, TFS doesnt accept the namespaces and we couldn't suppress them during serialization
            string text = File.ReadAllText(filePath);
            text = text.Replace("x:", "");
            text = text.Replace("x1:", "");
            text = text.Replace("x2:", "");
            text = text.Replace("xmlns:x=\"http://microsoft.com/schemas/VisualStudio/TeamTest/2010\"", "xmlns=\"http://microsoft.com/schemas/VisualStudio/TeamTest/2010\"");
            text = text.Replace("xmlns:x1=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            text = text.Replace("xmlns:x2=\"http://www.w3.org/2001/XMLSchema\"", "");
            File.WriteAllText(filePath, text);
        }
    }


    [Serializable]
    public class SmokeTestException : Exception
    {
        public SmokeTestException() { }
        public SmokeTestException(string message) : base(message) { }
        public SmokeTestException(string message, Exception inner) : base(message, inner) { }
        protected SmokeTestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
