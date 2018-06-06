using System;
using System.Collections.Generic;
using Sansar;
using Sansar.Script;
using Sansar.Simulation;

public class SmoTeleSafe : SceneObjectScript
{
    [DefaultValue(true)]
    [DisplayName("Enabled")]
    public Boolean isEnabled = true;

    [DefaultValue(true)]
    [DisplayName("Safe Teleport")]
    public Boolean isSafe = true;

    [DefaultValue("<0,0,0>")]
    [DisplayName("Teleport To")]
    public Sansar.Vector tpVec;

    [DefaultValue("Do you wish to teleport?")]
    [DisplayName("Question to ask")]
    public string thequestion;

    public override void Init()
    {
        RigidBodyComponent rb;
        if (ObjectPrivate.TryGetFirstComponent(out rb) && rb.IsTriggerVolume())
        {
            rb.Subscribe(CollisionEventType.Trigger, onCol);
        }
        else
        {
            Log.Write(LogLevel.Warning, GetType().Name, $"something went wrong");
        }
    }
    private void onCol(CollisionData data)
    {
        if (data.HitControlPoint != ControlPointType.Invalid) return;
        
        if (isEnabled && (data.Phase == CollisionEventPhase.TriggerEnter))
        {
            Log.Write(LogLevel.Info, GetType().Name, $"teleport enabled");
            if (isSafe)
            {
                AgentPrivate agent = ScenePrivate.FindAgent(data.HitComponentId.ObjectId);
                
                if (agent == null)
                {
                    Log.Write(LogLevel.Warning, GetType().Name, $"no agent");
                    return;
                }
                
                ModalDialog dialog = agent.Client.UI.ModalDialog;
                OperationCompleteEvent result = (OperationCompleteEvent)WaitFor(dialog.Show, thequestion, "Yes", "No");
                if (result.Success)
                {
                    if (dialog.Response != "Yes")
                    {
                        return;
                    }
                }
            }

            AnimationComponent comp;
            if(ScenePrivate.FindObject(data.HitComponentId.ObjectId).TryGetFirstComponent<AnimationComponent>(out comp))
            {
                comp.SetPosition(tpVec);
            } 
        }
    }
}