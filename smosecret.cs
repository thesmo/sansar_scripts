using System;
using System.Collections.Generic;
using Sansar;
using Sansar.Script;
using Sansar.Simulation;

public class SmoSecret : SceneObjectScript
{
	private string tehsecret = "wwssadad21 ";
	private Dictionary<string, string> players = new Dictionary<string, string>();
	private Vector secretloc = new Vector(-82, 28, 2);

	public override void Init()
    {
		ScenePrivate.User.Subscribe(User.AddUser, UserJoin);
    }

	void UserJoin(UserData data) {
		AgentPrivate agent = ScenePrivate.FindAgent(data.User);
		ObjectPrivate aobj = ScenePrivate.FindObject(agent.AgentInfo.ObjectId);
		if (aobj == null)
		{
			Log.Write(LogLevel.Error, GetType().Name, $"no obj for user {data.User}");
			return;
		}
		AnimationComponent anicomp = null;
		if (!aobj.TryGetFirstComponent(out anicomp))
		{
			Log.Write(LogLevel.Error, GetType().Name, $"no ani comp for user {data.User}");
			return;
		}
		SubscribeKeys(anicomp);
	}
	
	void SubscribeKeys(AnimationComponent ani) {
		ani.Subscribe("Key_W", w_Down);
		ani.Subscribe("Key_S", s_Down);
		ani.Subscribe("Key_A", a_Down);
		ani.Subscribe("Key_D", d_Down);
		ani.Subscribe("Key_1", one_Down);
		ani.Subscribe("Key_2", two_Down);
		ani.Subscribe("Key_Space", space_Down);
	}

	private void w_Down(AnimationData data) {
		PressedSpecial(data, "w");
	}

	private void s_Down(AnimationData data) {
		PressedSpecial(data, "s");
	}

	private void a_Down(AnimationData data) {
		PressedSpecial(data, "a");
	}

	private void d_Down(AnimationData data) {
		PressedSpecial(data, "d");
	}

	private void one_Down(AnimationData data) {
		PressedSpecial(data, "1");
	}

	private void two_Down(AnimationData data) {
		PressedSpecial(data, "2");
	}

	private void space_Down(AnimationData data) {
		PressedSpecial(data, " ");
	}

	private string do_dialog(AgentPrivate agent, string text) {
		ModalDialog dialog = agent.Client.UI.ModalDialog;
		OperationCompleteEvent result = (OperationCompleteEvent)WaitFor(dialog.Show, text, "Yes", "No");
		if (result.Success)
		{
			return dialog.Response;
		}
		return "";
	}

	private void PressedSpecial(AnimationData data, string key) {
		AgentPrivate agent = ScenePrivate.FindAgent(data.ComponentId.ObjectId);
        string res = "";
		if (agent == null)
        {
            Log.Write(LogLevel.Warning, GetType().Name, $"no agent");
            return;
        }
		string name = agent.AgentInfo.Name;
		//Log.Write(LogLevel.Warning, GetType().Name, $"{name} Pressed {key}");
		addKeys(name, key);

		if (isSecret(name))
		{
			players[name] = "";
			res = do_dialog(agent, "You found the secret key combination!\nWould you like to teleport to the secret spot?");
			if (res == "Yes")
			{
				AnimationComponent comp;
				if(ScenePrivate.FindObject(data.ComponentId.ObjectId).TryGetFirstComponent<AnimationComponent>(out comp)) 
				{
					comp.SetPosition(secretloc);
				}
			}
		}
	}
	
	private void addKeys(string name, string key) {
		string keystr = "";
		if (players.TryGetValue(name, out keystr))
		{
			if (keystr.Length == tehsecret.Length)
			{
				keystr = keystr.Substring(1,tehsecret.Length-1);
			}	
		} 
		players[name] = keystr + key;		
	}

	private bool isSecret(string name) {
		string keystr = "";
		if (players.TryGetValue(name, out keystr))
		{
			if (keystr == tehsecret)
			{
				Log.Write(LogLevel.Info, GetType().Name, $"{name} got tehsecret!");
				return true;
			}
		}
		return false;
	}
}