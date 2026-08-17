using System;
using System.Collections.Generic;
using System.Drawing;
using _4RTools.Utils;

namespace _4RTools.Model
{
    internal class Buff
    {
        public String name { get; set; }
        public EffectStatusIDs effectStatusID { get; set; }
        public Bitmap icon { get; set; }

        public Buff(string name, EffectStatusIDs effectStatus, Bitmap icon)
        {
            this.name = name;
            this.effectStatusID = effectStatus;
            this.icon = icon;
        }

        //--------------------- SKILLS ------------------------------

        //Archer Skills
        public static List<Buff> GetArcherSkills()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Concentration", EffectStatusIDs.CONCENTRATION, Resources._4RTools.Icons.ac_concentration),
                new Buff("Wind Walk", EffectStatusIDs.WINDWALK, Resources._4RTools.Icons.sn_windwalk),
                new Buff("True Sight", EffectStatusIDs.TRUESIGHT, Resources._4RTools.Icons.sn_sight),            
            };

            return skills;
        }

        //Swordsman Skills
        public static List<Buff> GetSwordmanSkill()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Endure", EffectStatusIDs.ENDURE, Resources._4RTools.Icons.sm_endure),
                new Buff("Auto Berserk", EffectStatusIDs.AUTOBERSERK, Resources._4RTools.Icons.sm_autoberserk),
                new Buff("Auto Guard", EffectStatusIDs.AUTOGUARD, Resources._4RTools.Icons.cr_autoguard),
                new Buff("Reflect Shield", EffectStatusIDs.REFLECTSHIELD, Resources._4RTools.Icons.cr_reflectshield),
                new Buff("Spear Quicken", EffectStatusIDs.SPEARQUICKEN, Resources._4RTools.Icons.cr_spearquicken),
                new Buff("Defender", EffectStatusIDs.DEFENDER, Resources._4RTools.Icons.cr_defender),
                new Buff("Concentration", EffectStatusIDs.LKCONCENTRATION, Resources._4RTools.Icons.lk_concentration),
                new Buff("Berserk", EffectStatusIDs.BERSERK, Resources._4RTools.Icons.lk_berserk),
                new Buff("Two-Hand Quicken", EffectStatusIDs.TWOHANDQUICKEN, Resources._4RTools.Icons.mer_quicken),
                new Buff("Parry", EffectStatusIDs.PARRYING, Resources._4RTools.Icons.ms_parrying),
                new Buff("Aura Blade", EffectStatusIDs.AURABLADE, Resources._4RTools.Icons.lk_aurablade),
                new Buff("Shrink", EffectStatusIDs.CR_SHRINK, Resources._4RTools.Icons.cr_shrink),
                new Buff("One-Hand Quicken", EffectStatusIDs.EFST_ONEHANDQUICKEN, Resources._4RTools.Icons.one_hand_quicken),
            };

            return skills;
        }

        //Mage Skills
        public static List<Buff> GetMageSkills()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Energy Coat", EffectStatusIDs.ENERGYCOAT, Resources._4RTools.Icons.mg_energycoat),
                new Buff("Sight Blaster", EffectStatusIDs.SIGHTBLASTER, Resources._4RTools.Icons.wz_sightblaster),
                new Buff("Autospell", EffectStatusIDs.AUTOSPELL, Resources._4RTools.Icons.sa_autospell),
                new Buff("Double Casting", EffectStatusIDs.DOUBLECASTING, Resources._4RTools.Icons.pf_doublecasting),
                new Buff("Memorize", EffectStatusIDs.MEMORIZE, Resources._4RTools.Icons.pf_memorize),
                new Buff("Amplification", EffectStatusIDs.MYST_AMPLIFY, Resources._4RTools.Icons.amplify),
            };

            return skills;
        }

        //Merchant Skills
        public static List<Buff> GetMerchantSkills()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Crazy Uproar", EffectStatusIDs.CRAZY_UPROAR, Resources._4RTools.Icons.mc_loud),
                new Buff("Power-Thrust", EffectStatusIDs.OVERTHRUST, Resources._4RTools.Icons.bs_overthrust),
                new Buff("Adrenaline Rush", EffectStatusIDs.ADRENALINE, Resources._4RTools.Icons.bs_adrenaline),
                new Buff("Advanced Adrenaline Rush", EffectStatusIDs.ADRENALINE2, Resources._4RTools.Icons.bs_adrenaline2),
                new Buff("Maximum Power-Thrust", EffectStatusIDs.OVERTHRUSTMAX, Resources._4RTools.Icons.ws_overthrustmax),
                new Buff("Weapon Perfection", EffectStatusIDs.WEAPONPERFECT, Resources._4RTools.Icons.bs_weaponperfect),
                new Buff("Power Maximize", EffectStatusIDs.MAXIMIZE, Resources._4RTools.Icons.bs_maximize),
                new Buff("Cart Boost", EffectStatusIDs.CARTBOOST, Resources._4RTools.Icons.ws_cartboost),
                new Buff("Meltdown", EffectStatusIDs.MELTDOWN, Resources._4RTools.Icons.ws_meltdown),
            };

            return skills;
        }

        //Thief Skills
        public static List<Buff> GetThiefSkills()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Poison React", EffectStatusIDs.POISONREACT, Resources._4RTools.Icons.as_poisonreact),
                new Buff("Reject Sword", EffectStatusIDs.SWORDREJECT, Resources._4RTools.Icons.st_rejectsword),
                new Buff("Preserve", EffectStatusIDs.PRESERVE, Resources._4RTools.Icons.st_preserve),
                new Buff("Enchant Deadly Poison", EffectStatusIDs.EDP, Resources._4RTools.Icons.asc_edp),
            };

            return skills;
        }

        //Acolyte Skills
        public static List<Buff> GetAcolyteSkills()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Blessing", EffectStatusIDs.BLESSING, Resources._4RTools.Icons.al_blessing1),
                new Buff("Increase Agility", EffectStatusIDs.INC_AGI, Resources._4RTools.Icons.al_incagi1),
                new Buff("Gloria", EffectStatusIDs.GLORIA, Resources._4RTools.Icons.pr_gloria),
                new Buff("Magnificat", EffectStatusIDs.MAGNIFICAT, Resources._4RTools.Icons.pr_magnificat),
                new Buff("Angelus", EffectStatusIDs.ANGELUS, Resources._4RTools.Icons.al_angelus),
                new Buff("Impositio Manus",  EffectStatusIDs.IMPOSITIO, Resources._4RTools.Icons.impositio_manus),
                new Buff("Basilica", EffectStatusIDs.BASILICA, Resources._4RTools.Icons.pr_magnificat),
                new Buff("Fury", EffectStatusIDs.FURY, Resources._4RTools.Icons.fury),
                new Buff("Steel Body", EffectStatusIDs.STEELBODY, Resources._4RTools.Icons.fury),
            };

            return skills;
        }

        //Padawan Skills
        public static List<Buff> GetPadawanSkills()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Force Element (Earth)", EffectStatusIDs.PD_ELEMENT_EARTH, Resources._4RTools.Icons.forceelement_earth),
                new Buff("Force Element (Wind)", EffectStatusIDs.PD_ELEMENT_WIND, Resources._4RTools.Icons.forceelement_wind),
                new Buff("Force Element (Water)", EffectStatusIDs.PD_ELEMENT_WATER, Resources._4RTools.Icons.forceelement_water),
                new Buff("Force Element (Fire)", EffectStatusIDs.PD_ELEMENT_FIRE, Resources._4RTools.Icons.forceelement_fire),
                new Buff("Force Element (Ghost)", EffectStatusIDs.PD_ELEMENT_GHOST, Resources._4RTools.Icons.forceelement_ghost),
                new Buff("Force Element (Shadow)", EffectStatusIDs.PD_ELEMENT_SHADOW, Resources._4RTools.Icons.forceelement_shadow),
                new Buff("Force Element (Holy)", EffectStatusIDs.PD_ELEMENT_HOLY, Resources._4RTools.Icons.forceelement_holy),
                new Buff("Force Projection", EffectStatusIDs.SI_PROJECTION, Resources._4RTools.Icons.forceprojection),
                new Buff("Cold Skin", EffectStatusIDs.SI_COLDSKIN, Resources._4RTools.Icons.coldskin),
                new Buff("Saber Parry", EffectStatusIDs.JS_SABERPARRY, Resources._4RTools.Icons.saberparry),
                new Buff("Force Concentration", EffectStatusIDs.JS_CONCENTRATE, Resources._4RTools.Icons.forceconcentrate),
                new Buff("Saber Thrust", EffectStatusIDs.SI_SABERTHRUST, Resources._4RTools.Icons.saberthrust),
                new Buff("Force Persuasion", EffectStatusIDs.JS_PERSUADE, Resources._4RTools.Icons.forcepersuasion),
                new Buff("Jedi Stealth", EffectStatusIDs.JE_STEALTH, Resources._4RTools.Icons.jedistealth),
                new Buff("Force Levitate", EffectStatusIDs.JE_LEVITATE, Resources._4RTools.Icons.forcelevitate),
                new Buff("Jedi Frenzy", EffectStatusIDs.JE_FRENZY, Resources._4RTools.Icons.jedifrenzy),
                new Buff("Force Sacrifice", EffectStatusIDs.JE_SACRIFICE, Resources._4RTools.Icons.forcesacrifice),
            };

            return skills;
        }

        //Ninja Skills
        public static List<Buff> GetNinjaSkills()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Cicada Skin Shed", EffectStatusIDs.PEEL_CHANGE, Resources._4RTools.Icons.nj_utsusemi),
                new Buff("Ninja Aura", EffectStatusIDs.AURA_NINJA, Resources._4RTools.Icons.nj_nen),
                new Buff("Izayoi", EffectStatusIDs.IZAYOI, Resources._4RTools.Icons.izayoi)
            };

            return skills;
        }

        //Taekwon Skills
        public static List<Buff> GetTaekwonSkills()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Mild Wind (Earth)", EffectStatusIDs.PROPERTYGROUND, Resources._4RTools.Icons.tk_mild_earth),
                new Buff("Mild Wind (Fire)", EffectStatusIDs.PROPERTYFIRE, Resources._4RTools.Icons.tk_mild_fire),
                new Buff("Mild Wind (Water)", EffectStatusIDs.PROPERTYWATER, Resources._4RTools.Icons.tk_mild_water),
                new Buff("Mild Wind (Wind)", EffectStatusIDs.PROPERTYWIND, Resources._4RTools.Icons.tk_mild_wind),
                new Buff("Mild Wind (Ghost)", EffectStatusIDs.PROPERTYTELEKINESIS, Resources._4RTools.Icons.tk_mild_ghost),
                new Buff("Mild Wind (Holy)", EffectStatusIDs.ASPERSIO, Resources._4RTools.Icons.tk_mild_holy),
                new Buff("Mild Wind (Shadow)", EffectStatusIDs.PROPERTYDARK, Resources._4RTools.Icons.tk_mild_shadow),
                new Buff("Solar Warmth", EffectStatusIDs.EFST_SG_SUN_WARM, Resources._4RTools.Icons.SG_SUN_WARM),
                new Buff("Comfort of the Sun", EffectStatusIDs.EFST_SUN_COMFORT, Resources._4RTools.Icons.SG_SUN_COMFORT),
                new Buff("Lunar Warmth", EffectStatusIDs.EFST_SG_MOON_WARM, Resources._4RTools.Icons.SG_MOON_WARM),
                new Buff("Comfort of the Moon", EffectStatusIDs.EFST_MOON_COMFORT, Resources._4RTools.Icons.SG_MOON_COMFORT),
                new Buff("Stellar Warmth", EffectStatusIDs.EFST_SG_STAR_WARM, Resources._4RTools.Icons.SG_STAR_WARM),
                new Buff("Comfort of the Stars", EffectStatusIDs.EFST_STAR_COMFORT, Resources._4RTools.Icons.SG_STAR_COMFORT),
                new Buff("Tumbling", EffectStatusIDs.DODGE_ON, Resources._4RTools.Icons.tumbling),
                new Buff("Enchanting Sky", EffectStatusIDs.EFST_SKY_ENCHANT, Resources._4RTools.Icons.enchanting_sky),
                new Buff("Universal Stance", EffectStatusIDs.EFST_UNIVERSESTANCE, Resources._4RTools.Icons.universal_stance),
            };

            return skills;
        }


        public static List<Buff> GetGunsSkills()
        {
            List<Buff> skills = new List<Buff>();

            skills.Add(new Buff("Gatling Fever", EffectStatusIDs.GATLINGFEVER, Resources._4RTools.Icons.gatling_fever));
            skills.Add(new Buff("Madness Canceller", EffectStatusIDs.MADNESSCANCEL, Resources._4RTools.Icons.madnesscancel));
            skills.Add(new Buff("Adjustment", EffectStatusIDs.ADJUSTMENT, Resources._4RTools.Icons.adjustment));
            skills.Add(new Buff("Increase Accuracy", EffectStatusIDs.ACCURACY, Resources._4RTools.Icons.increase_accuracy));

            return skills;
        }

        //--------------------- STUFFS ------------------------------
        //--------------------- Potions ------------------------------
        public static List<Buff> GetPotionsBuffs()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Concentration Potion", EffectStatusIDs.CONCENTRATION_POTION, Resources._4RTools.Icons.concentration_potiongif),
                new Buff("Awakening Potion", EffectStatusIDs.AWAKENING_POTION, Resources._4RTools.Icons.awakening_potion),
                new Buff("Berserk Potion", EffectStatusIDs.BERSERK_POTION, Resources._4RTools.Icons.berserk_potion),
            };

            return skills;
        }

        public static List<Buff> GetElementalsBuffs()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Elemental Converter (Fire)", EffectStatusIDs.EFST_ATTACK_PROPERTY_FIRE, Resources._4RTools.Icons.PROPERTY_FIRE),
                new Buff("Elemental Converter (Wind)", EffectStatusIDs.EFST_ATTACK_PROPERTY_WIND, Resources._4RTools.Icons.PROPERTY_WIND),
                new Buff("Elemental Converter (Earth)", EffectStatusIDs.EFST_ATTACK_PROPERTY_GROUND, Resources._4RTools.Icons.PROPERTY_GROUND),
                new Buff("Elemental Converter (Water)", EffectStatusIDs.EFST_ATTACK_PROPERTY_WATER, Resources._4RTools.Icons.PROPERTY_WATER),
                new Buff("Cursed Water", EffectStatusIDs.EFST_ATTACK_PROPERTY_DARKNESS, Resources._4RTools.Icons.cursed_water),
                new Buff("Fireproof Potion", EffectStatusIDs.RESIST_PROPERTY_FIRE, Resources._4RTools.Icons.fireproof),
                new Buff("Waterproof Potion", EffectStatusIDs.RESIST_PROPERTY_WATER, Resources._4RTools.Icons.coldproof),
                new Buff("Windproof Potion", EffectStatusIDs.RESIST_PROPERTY_WIND, Resources._4RTools.Icons.thunderproof),
                new Buff("Earthproof Potion", EffectStatusIDs.RESIST_PROPERTY_GROUND, Resources._4RTools.Icons.earhproof)
            };

            return skills;
        }

        public static List<Buff> GetFoodBuffs()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("STR Food", EffectStatusIDs.FOOD_STR, Resources._4RTools.Icons.strfood),
                new Buff("AGI Food", EffectStatusIDs.FOOD_AGI, Resources._4RTools.Icons.agi_food),
                new Buff("VIT Food", EffectStatusIDs.FOOD_VIT, Resources._4RTools.Icons.vit_food),
                new Buff("INT Food", EffectStatusIDs.FOOD_INT, Resources._4RTools.Icons.int_food),
                new Buff("DEX Food", EffectStatusIDs.FOOD_DEX, Resources._4RTools.Icons.dex_food),
                new Buff("LUK Food", EffectStatusIDs.FOOD_LUK, Resources._4RTools.Icons.luk_food),
            };

            return skills;
        }

        public static List<Buff> GetBoxesBuffs()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Drowsiness Box", EffectStatusIDs.DROWSINESS_BOX, Resources._4RTools.Icons.drowsiness),
                new Buff("Resentment Box", EffectStatusIDs.RESENTMENT_BOX, Resources._4RTools.Icons.resentment),
                new Buff("Sunlight Box", EffectStatusIDs.SUNLIGHT_BOX, Resources._4RTools.Icons.sunbox),
                new Buff("Box of Gloom", EffectStatusIDs.CONCENTRATION, Resources._4RTools.Icons.gloom),
                new Buff("Box of Thunder", EffectStatusIDs.BOX_OF_THUNDER, Resources._4RTools.Icons.speed),
                new Buff("Anodyne", EffectStatusIDs.ENDURE, Resources._4RTools.Icons.anodyne),
                new Buff("Aloevera", EffectStatusIDs.PROVOKE, Resources._4RTools.Icons.aloevera),

            };

            return skills;
        }

        public static List<Buff> GetScrollBuffs()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Éden Scroll", EffectStatusIDs.EFST_EDEN, Resources._4RTools.Icons.eden_scroll),
                new Buff("Increase Agility Scroll", EffectStatusIDs.INC_AGI, Resources._4RTools.Icons.al_incagi1),
                new Buff("Bless Scroll", EffectStatusIDs.BLESSING, Resources._4RTools.Icons.al_blessing1),
                new Buff("Full Chemical Protection (Scroll)", EffectStatusIDs.PROTECTARMOR, Resources._4RTools.Icons.cr_fullprotection),
                new Buff("Burn Incense",  EffectStatusIDs.EFST_BURNT_INCENSE, Resources._4RTools.Icons.burnt_incense),
                new Buff("Link Scroll", EffectStatusIDs.SOULLINK, Resources._4RTools.Icons.sl_soullinker),
                new Buff("Monster Transform",  EffectStatusIDs.MONSTER_TRANSFORM, Resources._4RTools.Icons.mob_transform),
                new Buff("Assumptio",  EffectStatusIDs.ASSUMPTIO, Resources._4RTools.Icons.assumptio),
                new Buff("Holy Armor Scroll",  EffectStatusIDs.EFST_ARMOR_PROPERTY, Resources._4RTools.Icons.holy_armor),
                new Buff("Shadow Armor Scroll",  EffectStatusIDs.EFST_ARMOR_PROPERTY, Resources._4RTools.Icons.shadow_armor_scroll),
                new Buff("Soul Scroll",  EffectStatusIDs.EFST_SOULSCROLL, Resources._4RTools.Icons.soul_scroll),
                new Buff("Undead Element Scroll",  EffectStatusIDs.EFST_RESIST_PROPERTY_UNDEAD, Resources._4RTools.Icons.undead_element_scroll),
            };

            return skills;
        }


        public static List<Buff> GetEXPBuffs()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Bubble Gum", EffectStatusIDs.CASH_RECEIVEITEM, Resources._4RTools.Icons.he_bubble_gum),
                new Buff("Base Combat Manual", EffectStatusIDs.CASH_PLUSEXP, Resources._4RTools.Icons.combat_manual_base),
                new Buff("Job Combat Manual", EffectStatusIDs.CASH_PLUSECLASSXP, Resources._4RTools.Icons.combat_manual_class),
            };

            return skills;
        }


        //--------------------- DEBUFFS ------------------------------
        public static List<Buff> GetDebuffs()
        {
            List<Buff> skills = new List<Buff>
            {
                new Buff("Critical Wounds", EffectStatusIDs.CRITICALWOUND, Resources._4RTools.Icons.critical_wound),
                new Buff("FREEZING", EffectStatusIDs.EFST_FREEZING, Resources._4RTools.Icons.freezing),
                new Buff("Curse", EffectStatusIDs.CURSE, Resources._4RTools.Icons.curse),
                new Buff("Bleeding", EffectStatusIDs.EFST_BLEEDING, Resources._4RTools.Icons.bleeding),
                new Buff("Silence", EffectStatusIDs.SILENCE, Resources._4RTools.Icons.silence),
                new Buff("Decrease Agi", EffectStatusIDs.EFST_DECREASE_AGI, Resources._4RTools.Icons.decrease_agi),
                new Buff("Confusion / chaos", EffectStatusIDs.CONFUSION, Resources._4RTools.Icons.chaos),
                new Buff("STUN", EffectStatusIDs.EFST_STUN, Resources._4RTools.Icons.stun),
                new Buff("Deep Sleep", EffectStatusIDs.EFST_DEEP_SLEEP, Resources._4RTools.Icons.deep_sleep),
                new Buff("Posion", EffectStatusIDs.POISON, Resources._4RTools.Icons.poison_status),
            };

            return skills;
        }
    }
}
