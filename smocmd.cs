using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sansar;
using Sansar.Script;
using Sansar.Simulation;

public class SmoCmd : SceneObjectScript
{
	/*
	my public vars go here
    */
	
    [DefaultValue("!")]
    [DisplayName("Character used to start commands")]
	public string cmdChar = "!";
	
	//END PUBLIC
	
	public override void Init()
    {
		Script.UnhandledException += UnhandledException;
		ScenePrivate.Chat.Subscribe(0, (ChatData data) => {

			if(isSmoCmd(data))
			{
				//remove cmdChar
				string cmdstr = data.Message.Substring(1,data.Message.Length-1);
				
				//split on sep max 3 parts
				string[] parts = cmdstr.Split(new string[] { " " }, 3, StringSplitOptions.RemoveEmptyEntries);

				//find out operation
				if(parts.Length > 0) 
				{
				string oper = parts[0].ToLower();				
				switch (oper) {
						case "commands":
							msgId(data.SourceId, "\nCommand List: " +
												 "\n " + cmdChar + "yt       Plays youtube urls" +
												 "\n " + cmdChar + "chan     Change youtube channel" +
												 "\n " + cmdChar + "ping     Ping" +
												 "\n " + cmdChar + "about    About the experience" +
												 "\n " + cmdChar + "commands This command" +
												 "\n "
												 );
							break;
							
						case "about":
							msgId(data.SourceId, "About: " + ScenePrivate.SceneInfo.ExperienceName +
										 "\n- AvatarId: " + ScenePrivate.SceneInfo.AvatarId
										 );
							break;

						case "ping":
							msgId(data.SourceId, "\nPong");
							break;

						case "chan":
							msgId(data.SourceId, "\nChanging to channel " + parts[1]);
							break;

						case "yt":
							//Make sure there is parameter
							if(parts.Length > 1)
							{
								//string ytUrl = parts[2];
								string ytUrl = getYtEmbedUrl(parts[1]);
								msgId(data.SourceId, "yturl: " + ytUrl);
								ScenePrivate.OverrideMediaSource(ytUrl);
							} else {	
								msgId(data.SourceId, "Copy and paste a url from youtube for this to work");
							}
							break;

						default:
							//msgId(data.SourceId, "Invalid Command");
							break;
					}
				}
			}
		});
	}

    private void msgAll(string Text) {
		ScenePrivate.Chat.MessageAllUsers($"{Text}");
	}
	private void msgId(SessionId sourceId, string Text) {
		AgentPrivate agent = ScenePrivate.FindAgent(sourceId);
		agent.SendChat($"{Text}");
		//agent.SendChat($"{ScenePrivate.SceneInfo.ExperienceName} scene!");
	}
	private bool isSmoCmd(ChatData data) {
		return data.Message.Trim().ToLower().StartsWith($"{cmdChar.ToLower()}");
	}
	
	private string getYtEmbedUrl(string url) {
		string youtube_id = "nQN9CITV-T4";
		
		if (url.IndexOf("youtube.com/") != -1)
		{
			youtube_id = url.Split(new string[] { "v=" }, StringSplitOptions.None)[1];
			if (url.IndexOf("&") != -1)
			{
			youtube_id = youtube_id.Split(new string[] { "&" }, StringSplitOptions.None)[0];
			}
		} else if (url.IndexOf("youtu.be/") != -1) {
			youtube_id = url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];
			if (url.IndexOf("&") != -1)
			{
			youtube_id = youtube_id.Split(new string[] { "&" }, StringSplitOptions.None)[0];
			}
		}
		
		return "https://www.youtube.com/embed/" + youtube_id + "?autoplay=1";
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
