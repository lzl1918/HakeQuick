using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HakeQuick.Implementation.Base;

namespace UnitTest
{
    [TestClass]
    public class CommandTest
    {
        [TestMethod]
        public void TestCommandParse()
        {
            string input = "";
            Command command;
            command = new Command(input);
            Assert.AreEqual("", command.Identity);
            Assert.AreEqual("", command.Action);
            Assert.AreEqual("", command.ActionPost);
            Assert.AreEqual(0, command.NamedArguments.Count);
            Assert.AreEqual(0, command.UnnamedArguments.Count);

            input = "test";
            command = new Command(input);
            Assert.AreEqual("test", command.Identity);
            Assert.AreEqual("", command.Action);
            Assert.AreEqual("", command.ActionPost);
            Assert.AreEqual(0, command.NamedArguments.Count);
            Assert.AreEqual(0, command.UnnamedArguments.Count);

            input = "test action";
            command = new Command(input);
            Assert.AreEqual("test", command.Identity);
            Assert.AreEqual("action", command.Action);
            Assert.AreEqual("action", command.ActionPost);
            Assert.AreEqual(1, command.NamedArguments.Count);
            Assert.AreEqual("action", command.NamedArguments["action"]);
            Assert.AreEqual(0, command.UnnamedArguments.Count);

            input = "test \"action act\"";
            command = new Command(input);
            Assert.AreEqual("test", command.Identity);
            Assert.AreEqual("", command.Action);
            Assert.AreEqual("\"action act\"", command.ActionPost);
            Assert.AreEqual(0, command.NamedArguments.Count);
            Assert.AreEqual(1, command.UnnamedArguments.Count);
            Assert.AreEqual("action act", command.UnnamedArguments[0]);

            input = "test \"action act\";act";
            command = new Command(input);
            string[] comp = new string[] { "action act", "act" };
            Assert.AreEqual("test", command.Identity);
            Assert.AreEqual("", command.Action);
            Assert.AreEqual(0, command.NamedArguments.Count);
            Assert.AreEqual(1, command.UnnamedArguments.Count);
            Assert.AreEqual(comp.Length, (command.UnnamedArguments[0] as string[]).Length);
            for (int i = 0; i < comp.Length; i++)
            {
                Assert.AreEqual(comp[i], (command.UnnamedArguments[0] as string[])[i]);
            }

            input = "test \"action act\"; act";
            command = new Command(input);
            Assert.AreEqual("test", command.Identity);
            Assert.AreEqual("", command.Action);
            Assert.AreEqual(0, command.NamedArguments.Count);
            Assert.AreEqual(2, command.UnnamedArguments.Count);
            Assert.AreEqual("action act", command.UnnamedArguments[0]);
            Assert.AreEqual("act", command.UnnamedArguments[1]);

            input = "test \"action act\"; act -sub";
            command = new Command(input);
            Assert.AreEqual("test", command.Identity);
            Assert.AreEqual("", command.Action);
            Assert.AreEqual(1, command.NamedArguments.Count);
            Assert.AreEqual(true, command.NamedArguments["sub"]);
            Assert.AreEqual(2, command.UnnamedArguments.Count);
            Assert.AreEqual("action act", command.UnnamedArguments[0]);
            Assert.AreEqual("act", command.UnnamedArguments[1]);

            input = "test \"action act\"; act -sub text;test";
            command = new Command(input);
            comp = new string[] { "text", "test" };
            Assert.AreEqual("test", command.Identity);
            Assert.AreEqual("", command.Action);
            Assert.AreEqual(1, command.NamedArguments.Count);
            Assert.AreEqual(comp.Length, (command.NamedArguments["sub"] as string[]).Length);
            for (int i = 0; i < comp.Length; i++)
            {
                Assert.AreEqual(comp[i], (command.NamedArguments["sub"] as string[])[i]);
            }
            Assert.AreEqual(2, command.UnnamedArguments.Count);
            Assert.AreEqual("action act", command.UnnamedArguments[0]);
            Assert.AreEqual("act", command.UnnamedArguments[1]);
        }
    }
}
