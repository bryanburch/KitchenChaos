using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There isn't a kitchen object on this counter
            if (player.HasKitchenObject())
            {
                // Player's carrying a kitchen object
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // Player's not carrying anything
            }

        }
        else
        {
            // There's already a kitchen object on this counter
            if (player.HasKitchenObject())
            {
                // Player's carrying something
            }
            else
            {
                // Player's not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
