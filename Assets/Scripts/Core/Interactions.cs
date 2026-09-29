using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    public sealed partial class GameSession
    {
        private readonly WorldCatalog world;
        private readonly PlayerProfile profile;
        private readonly SkillBook skillBook;
        private readonly Dictionary<CellPosition, Shop> shops = new Dictionary<CellPosition, Shop>();
        private readonly Dictionary<CellPosition, List<string>> offers = new Dictionary<CellPosition, List<string>>();
        private CellState interaction;
        public Battle LastCompletedBattle { get; private set; }
        public CellDefinition ActiveInteraction => interaction?.Definition;
        public Shop ActiveShop => interaction != null && shops.TryGetValue(interaction.Definition.Position, out var shop) ? shop : null;
        public IReadOnlyList<string> ActiveOffers => interaction != null && offers.TryGetValue(interaction.Definition.Position, out var choices) ? choices.AsReadOnly() : Array.Empty<string>();
        public IReadOnlyList<EventOptionDefinition> EventOptions => interaction?.Definition.Kind == CellKind.Event ? world.Events[interaction.Definition.ContentId].Options : Array.Empty<EventOptionDefinition>();
        private static bool IsInteraction(CellKind kind) => kind == CellKind.Shop || kind == CellKind.SkillChoice || kind == CellKind.RelicChoice || kind == CellKind.Event;
        private void ValidateContent(CellDefinition cell)
        {
            if (!IsInteraction(cell.Kind)) return;
            bool valid = world != null && (cell.Kind == CellKind.Shop ? world.Shops.ContainsKey(cell.ContentId) :
                cell.Kind == CellKind.SkillChoice ? skillBook != null && world.Skills.ContainsKey(cell.ContentId) :
                cell.Kind == CellKind.RelicChoice ? world.Relics.ContainsKey(cell.ContentId) : world.Events.ContainsKey(cell.ContentId));
            if (!valid) throw new ArgumentException("Missing cell content: " + cell.ContentId);
        }
        private void OpenInteraction(CellState cell)
        {
            interaction = cell; cell.IsContentKnown = true;
            var definition = cell.Definition;
            if (definition.Kind == CellKind.Shop && !shops.ContainsKey(definition.Position))
            {
                var offer = world.Shops[definition.ContentId];
                shops.Add(definition.Position, new Shop(catalog, offer.Pool.Draw(random, offer.Count), offer.RefreshBase, offer.RefreshGrowth, difficulty?.ShopMultiplier ?? 1));
            }
            if ((definition.Kind == CellKind.SkillChoice || definition.Kind == CellKind.RelicChoice) && !offers.ContainsKey(definition.Position))
            {
                var offer = definition.Kind == CellKind.SkillChoice ? world.Skills[definition.ContentId] : world.Relics[definition.ContentId];
                offers.Add(definition.Position, offer.Pool.Draw(random, offer.Count, predicate: id => definition.Kind == CellKind.SkillChoice ?
                    catalog.Skills[id].Kind != SkillKind.Weapon && Player.FindSkill(id) == null : !Player.Relics.Any(x => x.Id == id)).ToList());
            }
        }
        public bool Buy(int slot) => CanExplore && ActiveShop != null && ActiveShop.TryBuy(slot, Inventory);
        public bool RefreshShop()
        {
            if (!CanExplore || ActiveShop == null || Inventory.Coins < ActiveShop.RefreshPrice) return false;
            var offer = world.Shops[interaction.Definition.ContentId]; return ActiveShop.TryRefresh(Inventory, offer.Pool.Draw(random, offer.Count));
        }
        public bool ChooseOffer(string id)
        {
            if (!CanExplore || interaction == null || !ActiveOffers.Contains(id)) return false;
            bool isSkill = interaction.Definition.Kind == CellKind.SkillChoice;
            var definition = isSkill ? world.Skills[interaction.Definition.ContentId] : world.Relics[interaction.Definition.ContentId];
            int price = Data.Int(definition.Price * (difficulty?.ShopMultiplier ?? 1));
            if (Inventory.Coins < price || (isSkill ? Player.Skills.Count >= skillBook.OpenSlots || Player.FindSkill(id) != null : Player.Relics.Any(x => x.Id == id))) return false;
            var complete = PrepareCompletion(interaction);
            bool success = ExplorationTransaction.Run(Player, Inventory, () =>
            {
                if (!(isSkill ? skillBook.Learn(id) : Progression.EquipRelic(Player, id))) return false;
                return Inventory.TrySpend(price);
            });
            if (!success) return false;
            complete(); interaction = null; CheckPlayer(); return true;
        }
        public bool ChooseEvent(string optionId)
        {
            if (!CanExplore || interaction?.Definition.Kind != CellKind.Event) return false;
            var option = EventOptions.FirstOrDefault(x => x.Id == optionId);
            if (option == null || Player.Health <= option.HealthCost || (option.RequiredUnlock != null && (profile == null || !profile.IsUnlocked(option.RequiredUnlock))) ||
                (option.GrantUnlock != null && profile == null)) return false;
            if (!WorldActions.CanApply(catalog, option.Effects)) return false;
            var complete = PrepareCompletion(interaction);
            bool success = ExplorationTransaction.Run(Player, Inventory, () =>
            {
                if (!Inventory.TryExchange(option.Reward, option.Cost.Items.ToDictionary(x => x.Key, x => x.Value), option.Cost.Coins)) return false;
                Player.Health -= option.HealthCost; WorldActions.Apply(Player, option.Effects); return true;
            });
            if (!success) return false;
            if (option.GrantUnlock != null) profile.Unlock(option.GrantUnlock);
            complete(); interaction = null; CheckPlayer(); return true;
        }
        public bool LeaveInteraction(bool discard = false)
        {
            if (!CanExplore || interaction == null) return false;
            // Shops remain available until explicitly closed permanently; other choices may also be revisited.
            if (discard) CompleteInteraction(); else interaction = null;
            return true;
        }
        private void CompleteInteraction() { Complete(interaction); interaction = null; }
    }
}
