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

        //Init Songlist
        var songlist = new List<Tuple<string, string, int>>
        {
            Tuple.Create("Fmaa", "vtfKVoUYzyE", 56),
            Tuple.Create("Cirrus", "WF34N4gJAKE", 202),
            Tuple.Create("Boy & Bear", "sy4IhE-KAEg", 205),
            Tuple.Create("We Are Number One", "DUzBtXi-9Bs", 163),
            Tuple.Create("Miami Nights 1984", "rDBbaGCCIhk", 234),
            Tuple.Create("Out For A Rip", "F-glHAzXi_M", 210)
        };

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
                            msgId(data.SourceId, 
                                  "\nCommand List: " +
                                  "\n " + cmdChar + "yt       Plays youtube urls" +
                                  "\n " + cmdChar + "ytpl     Plays youtube playlist" +
                                  "\n " + cmdChar + "chan     Change youtube channel" +
                                  "\n " + cmdChar + "ping     Ping" +
                                  "\n " + cmdChar + "reset    Reset experience" +
                                  "\n " + cmdChar + "about    About experience" +
                                  "\n " + cmdChar + "commands This command" +
                                  "\n "
                                  );
                            break;
                            
                        case "about":
                            msgId(data.SourceId,
                                  "About: " + ScenePrivate.SceneInfo.ExperienceName +
                                  "\n- AvatarId: " + ScenePrivate.SceneInfo.AvatarId
                                  );
                            break;

                        case "ping":
                            msgId(data.SourceId, "\nPong");
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
                                //msgId(data.SourceId, "Copy and paste a url from youtube");
                                Random r = new Random();
                                int rInt = r.Next(0, songlist.Count-1);
                            msgAll(
                                  "[" + (rInt+1) + "/" +
                                  songlist.Count + "]" +
                                  songlist[rInt].Item1
                                  );

                                ScenePrivate.OverrideMediaSource(getYtEmbedUrl(songlist[rInt].Item2));
                            }
                            break;
                        
                        case "ytpl":
                            if(parts.Length > 1)
                            {
                                string ytPlUrl = getYtPlEmbedUrl(parts[1]);
                                msgId(data.SourceId, "ytplurl: " + ytPlUrl);
                                ScenePrivate.OverrideMediaSource(ytPlUrl);
                            } else {
                                msgId(data.SourceId, "Copy and paste playlist url or use playlist ID");
                            }
                            break;

                        case "chan":
                            if(parts.Length > 1)
                            {
                                int reqchan = Int32.Parse(parts[1]);
                                msgId(data.SourceId, "channel: " + reqchan);
                            } else {
                                msgId(data.SourceId,
                                      "\nChannel List:" +
                                      "\n1 Pogo" +
                                      "\n2 Jaboody Dubs" +
                                      "\n1 Stuff"
                                );
                            }
                            break;

                        case "reset":
                            msgAll("Reset in 5 seconds");
                            Wait(TimeSpan.FromSeconds(5));
                            ScenePrivate.ResetScene();
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
        } else if (url.Length == 11) {
            //This might be an actual ID so lets try it
            youtube_id = url;
        }
        return "https://www.youtube.com/embed/" + youtube_id + "?autoplay=1";
    }
    
    private string getYtPlEmbedUrl(string url) {
        string playlist_id = "PL2B009153AC977F90";

        //Is this a playlist ID?
        if (url.Length == 34 && url.Substring(0,2) == "PL") 
        {
            playlist_id = url;
        } else if (url.IndexOf("youtube.com/") != -1) {
            playlist_id = url.Split(new string[] { "list=" }, StringSplitOptions.None)[1];
            if (url.IndexOf("&") != -1)
            {
            playlist_id = playlist_id.Split(new string[] { "&" }, StringSplitOptions.None)[0];
            }
        }
        return "https://www.youtube.com/embed/videoseries?list=" + playlist_id + "&autoplay=1&loop=1";
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
