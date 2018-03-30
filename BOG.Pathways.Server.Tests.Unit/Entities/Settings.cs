using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOG.Pathways.Server.Tests.Unit.Entities
{
    [TestClass]
    public class Settings
    {
        [TestMethod]
        public void Test_ValidSettings()
        {
            // Defaults are valid values.
            var s = new BOG.Pathways.Server.Entity.Settings();
            string errors = null;
            var isValid = s.TryValidate(out errors);
            Assert.IsTrue(isValid, errors);
        }

        [TestMethod]
        public void Test_FailureChecks()
        {
            var details = string.Empty;
            var s = new BOG.Pathways.Server.Entity.Settings();
            s.SuperAccessToken = string.Empty;
            s.AdminAccessToken = string.Empty;
            s.UserAccessToken = string.Empty;
            s.HttpPortNumber = -1;
            s.HttpsPortNumber = 65536;
            bool isValid = s.TryValidate(out details);
            Assert.IsFalse(isValid, "Valid is true; expected false (1)");
            Assert.IsTrue(details.Contains("UserAccessToken can not be empty"), "(2)");
            Assert.IsTrue(details.Contains("SuperAccessToken can not be empty"), "(3)");
            Assert.IsTrue(details.Contains("AdminAccessToken can not be empty"), "(4)");
            Assert.IsTrue(details.Contains("Super, Admin and User AccessTokens can not have duplicate values"), "(5)");
            Assert.IsTrue(details.Contains("HttpPortNumber is out of allowed range (1, 65534)"), "(6)");
            Assert.IsTrue(details.Contains("HttpsPortNumber is out of allowed range (1, 65534)"), "(7)");

            s.HttpPortNumber = 2400;
            s.HttpsPortNumber = 2400;
            s.PayloadCountLimitPathway = 1001;
            s.PayloadSizeLimitPathway = 500 * 1024 * 1024 + 1;
            s.PayloadCountLimitTotal = 2001;
            s.PayloadSizeLimitTotal = 2147483649;
            isValid = s.TryValidate(out details);
            Assert.IsFalse(isValid, "Valid is true; expected false (8)");
            Assert.IsTrue(details.Contains("HttpPortNumber and HttpsPortNumber can not contain the same port value"), "(9)");
            Assert.IsTrue(details.Contains("PayloadCountLimitPathway is out of range"), "(10)");
            Assert.IsTrue(details.Contains("PayloadCountLimitTotal is out of range"), "(11)");
            Assert.IsTrue(details.Contains("PayloadSizeLimitPathway is out of range"), "(12)");
            Assert.IsTrue(details.Contains("PayloadSizeLimitTotal is out of range"), "(13)");

            s.PayloadCountLimitPathway = 0;
            s.PayloadSizeLimitPathway = 50 * 1024 - 1;
            s.PayloadCountLimitTotal = 0;
            s.PayloadSizeLimitTotal = 500 * 1024 - 1; ;
            isValid = s.TryValidate(out details);
            Assert.IsTrue(details.Contains("PayloadCountLimitPathway is out of range"), "(10)");
            Assert.IsTrue(details.Contains("PayloadCountLimitTotal is out of range"), "(11)");
            Assert.IsTrue(details.Contains("PayloadSizeLimitPathway is out of range"), "(12)");
            Assert.IsTrue(details.Contains("PayloadSizeLimitTotal is out of range"), "(13)");
        }
    }
}
