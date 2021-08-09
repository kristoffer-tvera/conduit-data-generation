using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConduitData.Models
{
    class ItemResponse
    {
        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("quality")]
        public Quality Quality { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("required_level")]
        public int RequiredLevel { get; set; }

        [JsonPropertyName("media")]
        public Media Media { get; set; }

        [JsonPropertyName("item_class")]
        public ItemClass ItemClass { get; set; }

        [JsonPropertyName("item_subclass")]
        public ItemSubclass ItemSubclass { get; set; }

        [JsonPropertyName("inventory_type")]
        public InventoryType InventoryType { get; set; }

        [JsonPropertyName("purchase_price")]
        public int PurchasePrice { get; set; }

        [JsonPropertyName("sell_price")]
        public int SellPrice { get; set; }

        [JsonPropertyName("max_count")]
        public int MaxCount { get; set; }

        [JsonPropertyName("is_equippable")]
        public bool IsEquippable { get; set; }

        [JsonPropertyName("is_stackable")]
        public bool IsStackable { get; set; }

        [JsonPropertyName("preview_item")]
        public PreviewItem PreviewItem { get; set; }

        [JsonPropertyName("purchase_quantity")]
        public int PurchaseQuantity { get; set; }
    }

    public class Quality
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Media
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class ItemClass
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class ItemSubclass
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class InventoryType
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Binding
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Spell
    {
        [JsonPropertyName("spell")]
        public Spell This { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class Level
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("display_string")]
        public string DisplayString { get; set; }
    }

    public class PreviewItem
    {
        [JsonPropertyName("item")]
        public Item Item { get; set; }

        [JsonPropertyName("quality")]
        public Quality Quality { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("media")]
        public Media Media { get; set; }

        [JsonPropertyName("item_class")]
        public ItemClass ItemClass { get; set; }

        [JsonPropertyName("item_subclass")]
        public ItemSubclass ItemSubclass { get; set; }

        [JsonPropertyName("inventory_type")]
        public InventoryType InventoryType { get; set; }

        [JsonPropertyName("binding")]
        public Binding Binding { get; set; }

        [JsonPropertyName("spells")]
        public List<Spell> Spells { get; set; }

        [JsonPropertyName("level")]
        public Level Level { get; set; }

        [JsonPropertyName("is_subclass_hidden")]
        public bool IsSubclassHidden { get; set; }
    }
}
