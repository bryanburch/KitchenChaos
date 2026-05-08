using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There isn't a kitchen object on this counter
            if (player.HasKitchenObject())
            {
                // Player's carrying a kitchen object
                if (HasCuttingRecipe(player.GetKitchenObject().GetKitchenObjectSO()))
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

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()))
        {
            var outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private bool HasCuttingRecipe(KitchenObjectSO kitchenObjectSO)
    {
        return GetOutputForInput(kitchenObjectSO) != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }
}
