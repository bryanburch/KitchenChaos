using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() && HasCuttingRecipe(player.GetKitchenObject().GetKitchenObjectSO()))
                player.GetKitchenObject().SetKitchenObjectParent(this);
        }
        else
        {
            if (!player.HasKitchenObject())
                GetKitchenObject().SetKitchenObjectParent(player);
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()))
        {
            var outputKitchenObjectSO = GetCuttingRecipeOutput(GetKitchenObject().GetKitchenObjectSO());
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private bool HasCuttingRecipe(KitchenObjectSO kitchenObjectSO)
    {
        return GetCuttingRecipeOutput(kitchenObjectSO) != null;
    }

    private KitchenObjectSO GetCuttingRecipeOutput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var cuttingRecipeSO in cuttingRecipes)
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
                return cuttingRecipeSO.output;
        return null;
    }
}
