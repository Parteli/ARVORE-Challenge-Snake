
public class DeathBorderAction : BorderLineAction
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.isBorder) return;

        for (int i = 0; i < slot.users.Count; i++)
            slot.users[i].Contact();
        
    }

}
