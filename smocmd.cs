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

    //what is playing
	private int curSongId = 0;
	private	int curPlayId = 0;
    
	IEventSubscription ytTimer;

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
			Tuple.Create("Spocktopus", "qc57-IcwnB0", 196),
			Tuple.Create("Warriors", "qc57-IcwnB0", 260),
			Tuple.Create("Miami Nights 1984", "rDBbaGCCIhk", 234),
			Tuple.Create("Guy On A Buffalo - Episode 1", "iJ4T9CQA0UM", 122),
			Tuple.Create("Guy On A Buffalo - Episode 2", "v5Lmkm5EF5E", 135),
			Tuple.Create("Guy On A Buffalo - Episode 3", "L55dKrjxcCY", 122),
			Tuple.Create("Guy On A Buffalo - Episode 4", "WXtpNm_a4Us", 183),
			Tuple.Create("Oh Fuck Yeah Bud", "sjdhnmQ-wmk", 216),
            Tuple.Create("Out For A Rip", "F-glHAzXi_M", 210)
		};

        var playlists = new List<Tuple<string, string, int>>
        {
            Tuple.Create("Pogo", "PLupdJjxWWYR55JN3UlaQdS2LPe5-fjlx7", 28),
            Tuple.Create("Jaboody Dubs", "PLSZtrpAn6e8eP_U7Fbf-jA2ob-76IqY2u", 24)
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
                                  "\n " + cmdChar + "reset    Reset experience" +
                                  "\n " + cmdChar + "about    About experience" +
                                  "\n " + cmdChar + "commands This command" +
                                  "\n "
                                  );
                            break;
                            
                        case "about":
                            info  = ScenePrivate.SceneInfo;
							msgId(data.SourceId,
                                  "About: " + info.ExperienceName +
                                  "\n- AvatarId: " + info.AvatarId
                                  );
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
								playRandomSong(songlist);
                                ScenePrivate.OverrideMediaSource(getYtEmbedUrl(songlist[curSongId].Item2));
                            }
                            break;
                        
                        case "ytpl":
                            if(parts.Length > 1)
                            {
                                string ytPlUrl = getYtPlEmbedUrl(parts[1]);
                                msgId(data.SourceId, "ytplurl: " + ytPlUrl);
                                ScenePrivate.OverrideMediaSource(ytPlUrl);
                            } else {
								Random r = new Random();
								int rInt = r.Next(0, playlists.Count);
                                msgAll(
                                  "[" + (rInt+1) + "/" +
                                  playlists.Count + "]" +
                                  playlists[rInt].Item1
                                  );
                                curPlayId = rInt;
                                ScenePrivate.OverrideMediaSource(getYtPlEmbedUrl(playlists[rInt].Item2));
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
		//Play some random youtubes
		playRandomSong(songlist);
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

	private void playRandomSong(List<Tuple<string, string, int>> slist) {
		Random r = new Random();
		int rInt = 0;
		int nextId = curSongId;
		//Make sure we don't get the same ID
		while (nextId == curSongId) {
			rInt = r.Next(0, slist.Count);
			nextId = rInt;
		}
		curSongId = nextId;
		msgAll(
			"Random Play\n" +
			"[" + (rInt+1) + "/" +
			slist.Count + "] " +
			slist[rInt].Item1 +
			"\nPlaying Next in " +
			slist[curSongId].Item3 + " Seconds"
			);
		ScenePrivate.OverrideMediaSource(getYtEmbedUrl(slist[curSongId].Item2));
		//TODO: destroy this timer before creating a new one
		ytTimer = Timer.Create(TimeSpan.FromSeconds(slist[curSongId].Item3), () => { 
			playRandomSong(slist);
		});
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
