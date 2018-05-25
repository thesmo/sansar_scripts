using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sansar;
using Sansar.Script;
using Sansar.Simulation;

//Some examples taken from https://help.sansar.com/hc/en-us/articles/115003373967-Sansar-Script-API

public class SmoJoin : SceneObjectScript
{
    public override void Init() {
        Script.UnhandledException += UnhandledException;
        // Subscribe to Add User events. Use SessionId.Invalid to track all users.
        ScenePrivate.User.Subscribe(User.AddUser, SessionId.Invalid, AddUser);
    }

    void AddUser(UserData data) {
        // Lookup the name of the agent.
        string name = ScenePrivate.FindAgent(data.User).AgentInfo.Name;
        ScenePrivate.Chat.MessageAllUsers(string.Format("{0} Has Entered the Smotel!", name));
        AgentPrivate agent = ScenePrivate.FindAgent(data.User);
        agent.SendChat($"type !commands for a list");
    }
    
    private void UnhandledException(object sender, Exception e) {
        if(!Script.UnhandledExceptionRecoverable)
        {
            Log.Write(LogLevel.Error, GetType().Name, $"Error: {e.Message}\n\n{e.StackTrace}");
        } else
        {
            Log.Write(LogLevel.Warning, GetType().Name, $"Warning: {e.Message}\n\n{e.StackTrace}");
        }
    }
}